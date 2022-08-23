using System.Runtime.InteropServices;
namespace Main
{
  class logging
  {
    public enum Severity
    {
      Emerg = 0,
      Alert = 1,
      Crit = 2,
      Err = 3,
      Warning = 4,
      Notice = 5,
      Info = 6,
      Debug = 7
    }

    public static List<(logging.Severity, String, String)> get_pre_flight_checks()
    {
      List<(logging.Severity level, String id, String desc)> results = new List<(logging.Severity level, string id, string desc)>();

      // open kvm fd
      int fd = Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);
      // Check for kvm basic api presence
      results.Add(pfcs.has_valid_kvm_extension(fd));

      return results;
    }

    public static List<(logging.Severity level, String id, String desc)> parse_pre_flight_checks(List<(logging.Severity level, String id, String desc)> pfcs, logging.Severity level, List<String> modifiers)
    {
      List<(logging.Severity level, String id, String desc)> notices = new List<(logging.Severity level, String id, String desc)>();
      foreach ((logging.Severity level, String id, String desc) pfc in pfcs)
      {
        // Only add tuple to notice if level <= the desired level and isnt being manually filtered out
        if (pfc.level <= level && !(modifiers.Contains("-" + pfc.id)) && !(modifiers.Contains("-all") && !(modifiers.Contains("+" + pfc.id))))
        {
          notices.Add(pfc);
        }
      }
      return notices;
    }
  }

  class pfcs
  {
    public static (logging.Severity level, String id, String desc) has_valid_kvm_extension(int fd)
    {
      // import ioctl function
      // remember to add the build dir to the LD_LIBRARY_PATH envvar format LD_LIBRARY_PATH=(whereever)/ioctls/build
      // OR add lib to /usr/local/lib64
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int fd);
      if (KVM_GET_API_VERSION(fd) == 12)
      {
        return (logging.Severity.Info, "has_kvm_api", "Basic kvm api found");
      }
      else return (logging.Severity.Emerg, "has_kvm_api", "Failed to get basic kvm api");
    }
  }

  class kvm
  {
    public static void Main()
    {
      List<(logging.Severity level, String id, String desc)> pfcs = logging.get_pre_flight_checks();
      List<(logging.Severity level, String id, String desc)> notices = logging.parse_pre_flight_checks(pfcs, logging.Severity.Info, new List<String>());
      if (notices.Count == 0)
      {
        Console.WriteLine("No notices from pfcs to display");
      }
      else
      {
        foreach ((logging.Severity level, String id, String desc) i in notices)
        {
          Console.WriteLine("{0}, {1}", i.desc, i.id);
        }
      }
    }
  }
}