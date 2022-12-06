using System.Runtime.InteropServices;

namespace Main
{
  class virtual_machine : template_virtual_machine
  {
    int kvm_fd;
    int vm_fd;
    bool is_open = false;
    List<int> vcpus_list;
    protected kvm.KvmUserspaceMemoryRegion ram_region;

    /// <summary>
    /// Stores a user definition of a virtual machine and contains the methods to manipulate it.
    /// </summary>
    /// <param name="_memory">Size of the vm's memory in mebibytes</param>
    /// <param name="_vcpus">Number of vcpus for this vm. Can be any number, but will be validated on vm creation</param>
    public virtual_machine(ulong _memory, uint _vcpus) : base(_memory, _vcpus)
    {
      vcpus_list = new List<int> { };
    }

    public virtual_machine(ulong _memory, uint _vcpus, string _name) : base(_memory, _vcpus, _name)
    {
      vcpus_list = new List<int> { };
    }

    /*
    Order of operations:
      - create vm
      - define devices
      - assign memory
      - create vcpus
      - assign registers
    */

    /// <summary>
    /// Creates a vm, assigns it its devices, memory and vcpus. 
    /// </summary>
    /// <param name="kvm_fd">File descriptor for an open /dev/kvm</param>
    public void create_vm_devices()
    {
      create_vm_fd();
      initialise_devices();
      initialise_memory();
      create_vcpus();
      intialise_registers();
    }

    public void start_vm(string image_path)
    {
      if (is_open)
      {
        Console.WriteLine("Already running!");
        return;
      }
      kvm_fd = kvm.get_kvm_fd();
      create_vm_devices();
      load_os(image_path);
      is_open = true;
    }

    /// <summary>
    /// Retrieves a file descriptor for a new vm. 
    /// </summary>
    /// <param name="kvm_fd">File descriptor for an open /dev/kvm</param>
    /// <exception cref="vm_fd_not_created"></exception>
    private void create_vm_fd()
    {
      // create vm fd
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_VM(int kvm_fd);
      vm_fd = KVM_CREATE_VM(kvm_fd);
      if (vm_fd < 0)
      {
        throw new vm_fd_not_created(vm_fd);
      }
    }

    /// <summary>
    /// Maps a region of memory using mmap, locks it with mlock and tells kvm this vm will use this region. 
    /// </summary>
    /// <exception cref="user_memory_region_not_set">Throws when kvm fails to assign the region of memory to the vm.</exception>
    private void initialise_memory()
    {
      // map memory region + lock
      ram_region = kvm.define_memory_region(memory);
      // set memory region
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_USER_MEMORY_REGION(int vm_fd, kvm.KvmUserspaceMemoryRegion region);
      int region_ret = KVM_SET_USER_MEMORY_REGION(vm_fd, ram_region);
      if (region_ret == -1)
      {
        throw new user_memory_region_not_set(memory, vm_fd);
      }
    }

    /// <summary>
    /// Initialises necessary devices for creating a vm. May differ in implementation across platforms.
    /// </summary>
    /// <exception cref="failed_setting_tss_addr"></exception>
    /// <exception cref="failed_setting_identity_map_addr"></exception>
    /// <exception cref="failed_creating_irqchip"></exception>
    /// <exception cref="failed_creating_pit2"></exception>
    private void initialise_devices()
    {
      /*
        Assign addresses for devices for vm
        has to happen before vcpus are made
      */
      int res;
      // TSS
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_TSS_ADDR(int vm_fd);
      res = KVM_SET_TSS_ADDR(vm_fd);
      if (res == -1)
      {
        throw new failed_setting_tss_addr(vm_fd);
      }
      // identity map addr
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_IDENTITY_MAP_ADDR(int vm_fd);
      res = KVM_SET_IDENTITY_MAP_ADDR(vm_fd);
      if (res == -1)
      {
        throw new failed_setting_identity_map_addr(vm_fd);
      }
      // IRQ
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_IRQCHIP(int vm_fd);
      res = KVM_CREATE_IRQCHIP(vm_fd);
      if (res == -1)
      {
        throw new failed_creating_irqchip(vm_fd);
      }
      // create in-kernel i8254 PIT
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_PIT2(int vm_fd);
      res = KVM_CREATE_PIT2(vm_fd);
      if (res == -1)
      {
        throw new failed_creating_pit2(vm_fd);
      }
    }

    /// <summary>
    /// If enough logical processors available, creates n vcpus for the vm.
    /// </summary>
    /// <param name="kvm_fd"></param>
    /// <exception cref="not_enough_logical_processors"></exception>
    /// <exception cref="failed_creating_vcpu"></exception>
    private void create_vcpus()
    {
      /*
        Create and store vcpus
      */
      // Create vcpus, store vcpu fds
      if (vcpus > kvm.get_max_cpus(kvm_fd))
      {
        throw new not_enough_logical_processors(vcpus, Environment.ProcessorCount);
      }
      for (uint i = 0; i < vcpus; i++)
      {
        [DllImport("KVM_IOCTLS.so", SetLastError = true)]
        static extern int KVM_CREATE_VCPU(int vm_fd, uint vcpu_id);
        int current_vcpu = KVM_CREATE_VCPU(vm_fd, i);
        if (current_vcpu == -1)
        {
          throw new failed_creating_vcpu(i, vm_fd);
        }
        else
        {
          vcpus_list.Add(current_vcpu);
        }
      }
    }

    /// <summary>
    /// Assigns values for the vcpu registers. Cannot assign in csharp because csharp does not type its array lengths individually.
    /// </summary>
    /// <exception cref="failed_setting_sregs"></exception>
    private void intialise_registers()
    {
      // Get and set special registers for vcpus
      /*
        Cannot do this natively because csharp does not use separate types for arrays of different length, so the structs are diff from the c ones,
        and csharp cannot get an address for the c to point to
      */
      int res;
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_and_SET_SREGS(int vcpu_fd, short is_arm64);
      for (uint i = 0; i < vcpus_list.Count; i++)
      {
        res = KVM_GET_and_SET_SREGS(vcpus_list[0], 0);
        if (res == -1)
        {
          throw new failed_setting_sregs(i, vm_fd);
        }
      }
      // Set (normal) registers for vcpus
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_REGS(int vcpu_fd);
      for (uint i = 0; i < vcpus_list.Count; i++)
      {
        res = KVM_SET_REGS(vcpus_list[0]);
        if (res == -1)
        {
          throw new failed_setting_sregs(i, vm_fd);
        }
      }
    }

    /// <summary>
    /// Load a linux kernel into vm memory. Based off the theory of https://gist.github.com/zserge/ae9098a75b2b83a1299d19b79b5fe488
    /// </summary>
    /// <param name="image">Path to linux image.</param>
    private void load_os(string image)
    {
      int image_fd = Mono.Unix.Native.Syscall.open(image, Mono.Unix.Native.OpenFlags.O_RDONLY);
      ulong image_bytes = (ulong)(new FileInfo(image)).Length;
      Mono.Unix.Native.MmapProts prot_flags = Mono.Unix.Native.MmapProts.PROT_READ | Mono.Unix.Native.MmapProts.PROT_WRITE;
      Mono.Unix.Native.MmapFlags map_flags = Mono.Unix.Native.MmapFlags.MAP_PRIVATE;
      IntPtr image_data = Mono.Unix.Native.Syscall.mmap(IntPtr.Zero, image_bytes, prot_flags, map_flags, image_fd, 0);
      Mono.Unix.Native.Syscall.close(image_fd);

      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int load_guest(IntPtr memory_start, IntPtr image_data, ulong image_size);
      int res;
      res = load_guest(ram_region.userspace_addr, image_data, image_bytes);
      Console.WriteLine(res);
    }

    /// <summary>
    /// Close all file descriptors.
    /// </summary>
    public void stop_vm()
    {
      if (!is_open)
      {
        Console.WriteLine("Already stopped!");
      }
      Mono.Unix.Native.Syscall.close(kvm_fd);
      Mono.Unix.Native.Syscall.close(vm_fd);
      foreach (var vcpu in vcpus_list)
      {
        Mono.Unix.Native.Syscall.close(vcpu);
      }
      Mono.Unix.Native.Syscall.munmap(ram_region.userspace_addr, 1 << 30);

    }

    // TODO: run guest. will need to be async for user to be able to interact. will not be fun.
    private int guest_run()
    {
      
      return 0;
    }
  }
}