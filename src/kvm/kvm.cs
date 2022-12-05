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

    /// <summary>
    /// Retrieves a file descriptor for /dev/kvm
    /// </summary>
    /// <returns></returns>
    public static int get_kvm_fd()
    {
      return Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);
    }

    /// <summary>
    /// Retrieves value for kvm api version from kvm.
    /// </summary>
    /// <param name="fd">File descriptor for /dev/kvm</param>
    /// <returns></returns>
    public static int get_basic_api_ver(int fd)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int kvm_file_descriptor);
      return KVM_GET_API_VERSION(fd);
    }

    /// <summary>
    /// Maps a region of memory using mmap and locks it using mlock.
    /// May require running ```ulimit -l (bytes)``` to increase the locked memory limit.
    /// </summary>
    /// <param name="capacity"></param>
    /// <returns></returns>
    public static kvm.KvmUserspaceMemoryRegion define_memory_region(ulong capacity)
    {
      Mono.Unix.Native.MmapProts prot_flags = Mono.Unix.Native.MmapProts.PROT_READ | Mono.Unix.Native.MmapProts.PROT_WRITE;
      Mono.Unix.Native.MmapFlags map_flags = Mono.Unix.Native.MmapFlags.MAP_PRIVATE | Mono.Unix.Native.MmapFlags.MAP_ANONYMOUS | Mono.Unix.Native.MmapFlags.MAP_NORESERVE;
      IntPtr mem = Mono.Unix.Native.Syscall.mmap(IntPtr.Zero, capacity, prot_flags, map_flags, -1, 0);
      Mono.Unix.Native.Syscall.mlock(mem, capacity);
      return new kvm.KvmUserspaceMemoryRegion(0, 0, 0, capacity, mem);
    }

    /// <summary>
    /// Get max number of vcpus that can be created by kvm per vm. limited by number of logical threads on host and kvm.
    /// </summary>
    /// <param name="kvm_fd"></param>
    /// <returns></returns>
    public static uint get_max_cpus(int kvm_fd)
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
