namespace Main
{

  class program
  {
    public static void Main(string[] args)
    {
      var parameters = param.get_parameters(args);
      List<String> modifiers = new List<String>();
      if (!parameters.ContainsKey(param.parameters.NoPreflights))
      {
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
      }

      if (!parameters.ContainsKey(param.parameters.sqlPassword) || !parameters.ContainsKey(param.parameters.sqlUser))
      {
        throw new no_db_details();
      }

      host self = new host();
      self.hello();

      var sql = new sql("localhost", parameters[param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);
      sql.init_db();

      List<virtual_machine> vms = new List<virtual_machine>();
      List<template_virtual_machine> templates = new List<template_virtual_machine>();

      /*
      while (true)
      {
        var vm1 = new virtual_machine(128, 1);
        vm1.start_vm("/home/philip/Documents/test-bzImage2");
      }*/
    }
  }
}
