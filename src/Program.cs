namespace Main
{
  public class settings
  {
    public Guid self_uuid { get; set; }

    public void update_file()
    {
      System.IO.File.WriteAllText(String.Format("{0}/settings.json", AppDomain.CurrentDomain.BaseDirectory), System.Text.Json.JsonSerializer.Serialize<settings>(this));
    }

    ~settings()
    {
      update_file();
    }
  }
  class program
  {
    public static void Main(string[] args)
    {
      List<string> args_list = new List<string>(args);
      commands.command action = commands.match_command(args_list);
      string subaction = commands.get_sub_command(action, args_list);

      var parameters = param.get_parameters(args_list);
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

      settings config;
      if (!System.IO.File.Exists(String.Format("{0}/settings.json", AppDomain.CurrentDomain.BaseDirectory)))
      {
        config = new settings { };
        System.IO.File.WriteAllText(String.Format("{0}/settings.json", AppDomain.CurrentDomain.BaseDirectory), System.Text.Json.JsonSerializer.Serialize<settings>(config));
      }
      else
      {
        config = System.Text.Json.JsonSerializer.Deserialize<settings>(System.IO.File.ReadAllText(String.Format("{0}/settings.json", AppDomain.CurrentDomain.BaseDirectory)))!;
      }

      sql sql = new sql("localhost", parameters[param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);
      sql.init_db();

      host self;
      if (config.self_uuid != Guid.Empty)
      {
        self = new host(config.self_uuid, sql.conn);
      }
      else
      {
        self = new host();
        config.self_uuid = self.uuid;
        config.update_file();
        sql.create_host(self);
      }

      sql.conn.Close();

      //List<virtual_machine> vms = new List<virtual_machine>();
      //List<template_virtual_machine> templates = new List<template_virtual_machine>();

      commands.execute(action, subaction, parameters);


      var vm1 = new virtual_machine(128, 1);
      vm1.start_vm("/home/philip/Documents/test-bzImage2");

    }
  }
}
