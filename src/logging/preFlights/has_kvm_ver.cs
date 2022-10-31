using System.Runtime.InteropServices;

namespace Main
{
  partial class preFlights
  {
    public static log has_kvm_extension(int fd)
    {
      // import ioctl function
      // remember to add the build dir to the LD_LIBRARY_PATH envvar format LD_LIBRARY_PATH=(whereever)/ioctls/build
      // OR add lib to /usr/local/lib64
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int fd);
      if (KVM_GET_API_VERSION(fd) == 12)
      {
        return new log(log.Severity.Info, "has_kvm_api", "Basic kvm api found");
      }
      else return new log(log.Severity.Emerg, "has_kvm_api", "Failed to get basic kvm api");
    }
  }
}