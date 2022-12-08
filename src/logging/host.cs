using System.Text.RegularExpressions;

namespace Main
{
  class host
  {
    Guid uuid; // identifier of host in dbms
    ulong memory; // in bytes, of course
    ushort threads; // logical processors, not actual cores
    string friendly_name; // user facing name
    string? arch; // host cpu arch
    string? ip; // optional, unlocks networked functions
    bool is_manager = true; // whether it has elevated privileges 
    string? state; // what the host is doing. changes wont be visible to user on single host setup.
    public host(string _friendly_name)
    {
      init_host();
      friendly_name = _friendly_name;
    }

    public host()
    {
      init_host();
      // csharp cannot do maths apparently
      // double mem_mb = Math.Truncate((double)memory / (1024 ^ 2));
      friendly_name = String.Format("asn {0} host with {1} threads and {2} bytes of memory total.", arch, threads, memory);
    }

    private void init_host()
    {
      /*
        Getting total memory size from proc
        https://stackoverflow.com/a/24641501
      */
      string mem_info_total = File.ReadLines(@"/proc/meminfo").First();
      string mem_kb = Regex.Replace(mem_info_total, "[^0-9.]", ""); // remove non numbers
      memory = ulong.Parse(mem_kb) * 1000;

      threads = (ushort)Environment.ProcessorCount;
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
    }

    public void hello()
    {
      Console.WriteLine("Name: {0}\nUuid:{1}", friendly_name, uuid);
    }
  }
}
