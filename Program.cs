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

    public static List<log> get_pre_flight_checks()
    {
      List<log> results = new List<log>();

      // open kvm fd
      int fd = Mono.Unix.Native.Syscall.open("/dev/kvm", Mono.Unix.Native.OpenFlags.O_RDWR);
      // Check for kvm basic api presence
      results.Add(pfcs.has_valid_kvm_extension(fd));

      return results;
    }

    public static List<log> parse_by_severity(List<log> pfcs, logging.Severity level, List<String> modifiers)
    {
      List<log> notices = new List<log>();
      foreach (log pfc in pfcs)
      {
        // Only add tuple to notice if level <= the desired level and isnt being manually filtered out
        if (pfc.get_level() <= level && !(modifiers.Contains("-" + pfc.get_id())) && !(modifiers.Contains("-all") && !(modifiers.Contains("+" + pfc.get_id()))))
        {
          notices.Add(pfc);
        }
      }
      return notices;
    }
  }

  // log class, each log is an instance of this class
  class log
  {
    private logging.Severity level;
    private String id, desc;

    public log(logging.Severity _level, String _id, String _desc)
    {
      level = _level;
      id = _id;
      desc = _desc;
    }

    public logging.Severity get_level()
    {
      return level;
    }

    public String get_id()
    {
      return id;
    }

    public String get_desc()
    {
      return desc;
    }
  }

  class pfcs
  {
  public static log has_valid_kvm_extension(int fd)
    {
      // import ioctl function
      // remember to add the build dir to the LD_LIBRARY_PATH envvar format LD_LIBRARY_PATH=(whereever)/ioctls/build
      // OR add lib to /usr/local/lib64
      [DllImport("KVM_IOCTLS.so", SetLastError = true)]
      static extern int KVM_GET_API_VERSION(int fd);
      if (KVM_GET_API_VERSION(fd) == 12)
      {
        return new log(logging.Severity.Info, "has_kvm_api", "Basic kvm api found");
      }
      else return new log(logging.Severity.Emerg, "has_kvm_api", "Failed to get basic kvm api");
    }
  }

  class kvm
  {
    public static void Main()
    {
      List<log> pfcs = logging.get_pre_flight_checks();
      List<log> notices = logging.parse_by_severity(pfcs, logging.Severity.Info, new List<String>());
      if (notices.Count == 0)
      {
        Console.WriteLine("No notices from pfcs to display");
      }
      else
      {
        foreach (log i in notices)
        {
          Console.WriteLine("{0}, {1}", i.get_desc(), i.get_id());
        }
      }
    }
  }
}