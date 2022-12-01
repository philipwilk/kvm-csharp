using System.Runtime.InteropServices;

namespace Main
{
  class kvm
  {
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
      public uint slot;
      public uint flags;
      public ulong guest_phys_addr;
      public ulong memory_size;
      public IntPtr userspace_addr;
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

    public static void create_vm(int kvm_fd, nuint capacity)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_VM(int kvm_fd);
      int vm_fd = KVM_CREATE_VM(kvm_fd);
      kvm.KvmUserspaceMemoryRegion ram_region = define_memory_region(capacity);
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_USER_MEMORY_REGION(int vm_fd, kvm.KvmUserspaceMemoryRegion region);
      int ret_region = KVM_SET_USER_MEMORY_REGION(vm_fd, ram_region);
      Console.WriteLine(ret_region);
    }
  }
}