using System.Runtime.InteropServices;
namespace Main
{
  class kvm
  {

    public static void Main()
    {
      // import ioctl function
      [DllImport("KVM_GET_API_VERSION.so", SetLastError = true)]
      extern static int KVM_GET_API_VERSION(int fd);
      //[DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
      //extern static int KernelIoCtrl(int fd, int request, IntPtr data);

      // get kvm fd
      int fileDescriptor = Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);

      // Get result of function
      int result = KVM_GET_API_VERSION(fileDescriptor);
    }
  }
}