using System.Runtime.InteropServices;

namespace Main
{
  class kvm
  {
    private static int KVM_CAP_NR_VCPUS = 9; // recomended max vcpus per vm
    private static int KVM_CAP_MAX_VCPUS = 66; // max vcpus per vm
    private static int KVM_CAP_MAX_VCPU_ID = 128; // max number of vcpu ids
    public struct KvmUserspaceMemoryRegion
    {
      public KvmUserspaceMemoryRegion(uint _slot, uint _flags, ulong _guest_phys_addr, ulong _memory_size, IntPtr _userspace_addr)
      {
        slot = _slot;
        flags = _flags;
        guest_phys_addr = _guest_phys_addr;
        memory_size = _memory_size;
        userspace_addr = _userspace_addr;
      }
      uint slot;
      uint flags;
      ulong guest_phys_addr;
      ulong memory_size;
      IntPtr userspace_addr;
    }

    public static int get_kvm_fd()
    {
      return Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);
    }

    public static int get_basic_api_ver(int fd)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int kvm_file_descriptor);
      return KVM_GET_API_VERSION(fd);
    }
    private static kvm.KvmUserspaceMemoryRegion define_memory_region(nuint capacity)
    {
      Mono.Unix.Native.MmapProts prot_flags = Mono.Unix.Native.MmapProts.PROT_READ | Mono.Unix.Native.MmapProts.PROT_WRITE;
      Mono.Unix.Native.MmapFlags map_flags = Mono.Unix.Native.MmapFlags.MAP_PRIVATE | Mono.Unix.Native.MmapFlags.MAP_ANONYMOUS | Mono.Unix.Native.MmapFlags.MAP_NORESERVE;
      IntPtr mem = Mono.Unix.Native.Syscall.mmap(IntPtr.Zero, capacity, prot_flags, map_flags, -1, 0);
      Mono.Unix.Native.Syscall.mlock(mem, capacity);
      return new kvm.KvmUserspaceMemoryRegion(0, 0, 0, capacity, mem);
    }

    public static void create_vm(int kvm_fd, nuint capacity, uint cores)
    {
      // create vm fd
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_VM(int kvm_fd);
      int vm_fd = KVM_CREATE_VM(kvm_fd);
      if (vm_fd < 0)
      {
        throw new vm_fd_not_created_Exception(vm_fd);
      }
      // lock memory region
      kvm.KvmUserspaceMemoryRegion ram_region = define_memory_region(capacity);
      // assign memory region to vm with fd from earlier
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_USER_MEMORY_REGION(int vm_fd, kvm.KvmUserspaceMemoryRegion region);
      int region_ret = KVM_SET_USER_MEMORY_REGION(vm_fd, ram_region);
      if (region_ret == -1)
      {
        throw new user_memory_region_not_set_Exception(capacity, vm_fd);
      }
      // Create vcpus, store vcpu fds
      if (cores > get_max_cpus(kvm_fd))
      {
        throw new not_enough_logical_processors(cores, Environment.ProcessorCount);
      }
      var vcpus = new List<int> { };
      for (uint core = 0; core < cores; core++)
      {
        [DllImport("KVM_IOCTLS.so", SetLastError = true)]
        static extern int KVM_CREATE_VCPU(int vm_fd, uint vcpu_id);
        int current_vcpu = KVM_CREATE_VCPU(vm_fd, core);
        if (current_vcpu == -1)
        {
          throw new failed_creating_vcpu(core, vm_fd);
        }
        else
        {
          vcpus.Add(current_vcpu);
        }
      }
      // Get and set special registers for vcpus
      /*
        Cannot do this natively because csharp does not use separate types for arrays of different length, so the structs are diff from the c ones,
        and csharp cannot get an address for the c to point to
      */
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_and_SET_SREGS(int vcpu_fd, short is_arm64);
      for (uint i = 0; i < vcpus.Count; i++)
      {
        int res = KVM_GET_and_SET_SREGS(vcpus[0], 0);
        if (res == -1)
        {
          throw new failed_setting_sregs(i, vm_fd);
        }
      }
      // Set (normal) registers for vcpus
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_REGS(int vcpu_fd);
      for (uint i = 0; i < vcpus.Count; i++)
      {
        int res = KVM_SET_REGS(vcpus[0]);
        if (res == -1)
        {
          throw new failed_setting_sregs(i, vm_fd);
        }
      }
    }

    // Get max number of vcpus that can be created. limited by number of logical threads on host and kvm itself.
    private static uint get_max_cpus(int kvm_fd)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern uint KVM_CHECK_EXTENSION(int kvm_fd, int extension_identifier);
      uint recommended_max_vcpus = KVM_CHECK_EXTENSION(kvm_fd, KVM_CAP_NR_VCPUS); // get recommended vcpu count from kvm
      if (recommended_max_vcpus == 0)
      {
        recommended_max_vcpus = (uint)Environment.ProcessorCount; // technically we should assume 4 but no thanks
      }
      uint max_vcpus = KVM_CHECK_EXTENSION(kvm_fd, KVM_CAP_MAX_VCPUS); // get max vcpu count from kvm (doesnt appear sensible)
      if (max_vcpus != 0 && max_vcpus < recommended_max_vcpus)
      {
        recommended_max_vcpus = max_vcpus;
      }
      uint max_vcpu_id = KVM_CHECK_EXTENSION(kvm_fd, KVM_CAP_MAX_VCPU_ID); // get max vcpu id from kvm. not sure why this is different from the previous number.
      if (max_vcpu_id != 0 && max_vcpu_id < recommended_max_vcpus)
      {
        recommended_max_vcpus = max_vcpu_id;
      }
      return recommended_max_vcpus;
    }

  }
}
