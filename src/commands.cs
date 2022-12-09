namespace Main
{
  class commands
  {
    public enum command
    {
      vm,
      template,
      info,
      vdisks,
      vdevs,
      hosts,
      help
    }

    public static void execute(command action, string subaction, IDictionary<param.parameters, String> parameters, host self)
    {
      switch (action)
      {
        case command.info:
          {
            var x = new command_info();
            x.execute(subaction, parameters, self);
            return;
          }
        case command.hosts:
          {
            var x = new command_hosts();
            x.execute(subaction, parameters, self);
            return;
          }
        case command.template:
          {
            var x = new command_template();
            x.execute(subaction, parameters, self);
            return;
          }
        case command.vm:
          {
            var x = new command_vm();
            x.execute(subaction, parameters, self);
            return;
          }
        case command.vdevs:
          {
            var x = new command_vdev();
            x.execute(subaction, parameters, self);
            return;
          }
        case command.vdisks:
          {
            var x = new command_vdisk();
            x.execute(subaction, parameters, self);
            return;
          }
      }
    }

    public static string get_sub_command(command action, List<string> raw_args)
    {
      switch (action)
      {
        case command.info:
          {
            command_info x = new command_info(raw_args);
            return x.get_sub_action();
          }
        case command.hosts:
          {
            command_hosts x = new command_hosts(raw_args);
            return x.get_sub_action();
          }
        case command.template:
          {
            command_template x = new command_template(raw_args);
            return x.get_sub_action();
          }
        case command.vm:
          {
            command_vm x = new command_vm(raw_args);
            return x.get_sub_action();
          }
        case command.vdisks:
          {
            command_vdisk x = new command_vdisk(raw_args);
            return x.get_sub_action();
          }
        case command.vdevs:
          {
            command_vdev x = new command_vdev(raw_args);
            return x.get_sub_action();
          }
        default:
          {
            // https://stackoverflow.com/a/3442429
            Console.WriteLine("Invalid command OR help called. Valid commands:");
            string list_of_commands = "";
            foreach (string comm in Enum.GetNames(typeof(command)))
            {
              list_of_commands += String.Format("{0}, ", comm);
            }
            Console.WriteLine(list_of_commands);
            Console.WriteLine("For help with a command, run the command with the 'help' argument");
            return "";
          }
      }
    }
    public static command match_command(List<string> args)
    {
      switch (comm(args).ToLower())
      {
        case "vm":
          {
            return commands.command.vm;
          }
        case "template":
          {
            return commands.command.template;
          }
        case "info":
          {
            return commands.command.info;
          }
        case "vdisks":
          {
            return commands.command.vdisks;
          }
        case "vdevs":
          {
            return commands.command.vdevs;
          }
        case "hosts":
          {
            return commands.command.hosts;
          }
        default: return commands.command.help;
      }
    }
    public static string comm(List<string> args)
    {
      if (args.Count > 0)
      {
        string tmp = args[0];
        args.RemoveAt(0);
        return tmp;
      }
      else
      {
        return "";
      }
    }
  }
  abstract class run
  {
    protected virtual string command_info { get; }
    public List<String>? args { get; set; }
    protected IDictionary<param.parameters, String>? parameters;

    public run(string _command_info, List<String> _args)
    {
      command_info = _command_info;
      args = _args;
    }

    public run(string _command_info)
    {
      command_info = _command_info;
    }

    protected void help()
    {
      Console.WriteLine("Helpme: {0}", command_info);
    }

    public abstract void execute(string subaction, IDictionary<param.parameters, String> parameters, host self);

    public abstract string get_sub_action();
  }
}

