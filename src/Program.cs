namespace Main
{

  class program
  {
    public static void Main(string[] args)
    {
      var parameters = param.get_parameters(args);
      foreach (var item in parameters)
      {
        Console.WriteLine(item);
      }
      List<String> modifiers = new List<String>();
      List<log> notices = logging.get_parsed_preflights(log.Severity.Info, modifiers);
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


      var vm1 = new virtual_machine(9, 1);
      vm1.start_vm("/home/philip/Documents/test-bzImage2");
    }
  }
}
