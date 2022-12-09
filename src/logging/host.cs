using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
namespace Main
{
  class host
  {
    public Guid uuid { get; } // identifier of host in dbms
    public ulong memory { get; set; } // in bytes, of course
    public ushort threads { get; set; } // logical processors, not actual cores
    public string friendly_name { get; } // user facing name
    public string? arch { get; } // host cpu arch
    public string? ip { get; set; } // optional, unlocks networked functions
    public bool is_manager { get; set; } // whether it has elevated privileges 
    public string? state { get; set; } // what the host is doing. changes wont be visible to user on single host setup.
    public host(string _friendly_name)
    {
      uuid = Guid.NewGuid();
      memory = get_bytes();
      friendly_name = _friendly_name;
      is_manager = true;
      state = "online";
      threads = (ushort)Environment.ProcessorCount;
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
    }

    public host()
    {
      uuid = Guid.NewGuid();
      memory = get_bytes();
      is_manager = true;
      state = "online";
      threads = (ushort)Environment.ProcessorCount;
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
      // csharp cannot do maths apparently
      // double mem_mb = Math.Truncate((double)memory / (1024 ^ 2));
      friendly_name = String.Format("an {0} host with {1} threads and {2} bytes of memory total.", arch, threads, memory);
    }

    public host(Guid _uuid, MySqlConnection conn)
    {
      uuid = _uuid;
      var res = sql.get_host(_uuid, conn);
      res.Read();
      friendly_name = res.GetString("FriendlyName");
      memory = get_bytes();
      is_manager = res.GetBoolean("IsManager");
      res.Close();
      state = "online";
      threads = (ushort)Environment.ProcessorCount;
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
    }

    private ulong get_bytes()
    {
      /*
        Getting total memory size from proc
        https://stackoverflow.com/a/24641501
      */
      string mem_info_total = File.ReadLines(@"/proc/meminfo").First();
      string mem_kb = Regex.Replace(mem_info_total, "[^0-9.]", ""); // remove non numbers
      return ulong.Parse(mem_kb) * 1000;
    }

    public void hello()
    {
      Console.WriteLine("Name: {0}\nUuid:{1}", friendly_name, uuid);
    }
  }
}
