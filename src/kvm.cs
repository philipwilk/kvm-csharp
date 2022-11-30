using System.Runtime.InteropServices;

namespace Main
{
  class kvm
  {
    public struct KvmUserspaceMemoryRegion
    {
      public KvmUserspaceMemoryRegion(uint _slot, uint _flags, ulong _guest_phys_address, ulong _memory_size, IntPtr _userspace_address)
      {
        slot = _slot;
        flags = _flags;
        guest_phys_address = _guest_phys_address;
        memory_size = _memory_size;
        userspace_address = _userspace_address;
      }
      public uint slot { get; }
      public uint flags { get; }
      public ulong guest_phys_address { get; }
      public ulong memory_size { get; }
      public IntPtr userspace_address { get; }
    }

    public static int get_kvm_fd()
    {
      return Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);
    }

    public static int get_basic_api_ver(int fd)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int fd);
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

    public static void create_vm(int fd, nuint capacity)
    {
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_CREATE_VM(int fd);
      int ret_createvm = KVM_CREATE_VM(fd);
      kvm.KvmUserspaceMemoryRegion ram_region = define_memory_region(capacity);
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_SET_USER_MEMORY_REGION(int fd, kvm.KvmUserspaceMemoryRegion region);
      int ret_region = KVM_SET_USER_MEMORY_REGION(fd, ram_region);
      Console.WriteLine(ret_region);
    }
  }
}