using System.Runtime.InteropServices;
namespace Main
{
  class kvm
  {

    public static void Main()
    {
      // import ioctl function
      // remember to add the build dirs to the LD_LIBRARY_PATH envvar format LD_LIBRARY_PATH=():():()
      // OR add libs to /usr/local/lib64
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int fd);

      // get kvm fd
      int fileDescriptor = Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);

      // Get result of function
      int result = KVM_GET_API_VERSION(fileDescriptor);

      Console.WriteLine("Is kvm basic compliant: {0}", result == 12);
    }
  }
}