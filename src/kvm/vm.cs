using System.Runtime.InteropServices;

namespace Main
{
  class virtual_machine : template_virtual_machine
  {
    int vm_fd;
    List<int> vcpus_list;
    kvm.KvmUserspaceMemoryRegion ram_region;

    public virtual_machine(uint _memory, uint _vcpus) : base(_memory, _vcpus)
    {
      vcpus_list = new List<int> { };
    }

    public virtual_machine(uint _memory, uint _vcpus, string _name) : base(_memory, _vcpus, _name)
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

    public void start_vm(int kvm_fd)
    {
      create_vm(kvm_fd);
      initialise_devices();
      initialise_memory();
      create_vcpus(kvm_fd);
      intialise_registers();
    }

    private void create_vm(int kvm_fd)
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

    private void create_vcpus(int kvm_fd)
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
  }
}