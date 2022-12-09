using MySql.Data.MySqlClient;

namespace Main
{
  class command_vm : run
  {
    const string description = "Creates, deletes and starts virtual machines";
    public command_vm(List<string> args) : base(description, args) { }
    public command_vm() : base(description) { }

    public override string get_sub_action()
    {
      switch (commands.comm(args!))
      {
        case "create":
          {
            return "create";
          }
        case "delete":
          {
            return "delete";
          }
        case "list":
          {
            return "list";
          }
        case "start":
          {
            return "start";
          }
        default:
          {
            return "help";
          }
      }
    }

    public override void execute(string subaction, IDictionary<param.parameters, String> _parameters)
    {
      parameters = _parameters;
      switch (subaction)
      {
        default:
          {
            this.help();
            return;
          }
        case "create":
          {
            create_vm();
            return;
          }
        case "delete":
          {
            // delete vm STUB
            return;
          }
        case "list":
          {
            // list vms STUB
            return;
          }
        case "start":
          {
            start_vm();
            return;
          }
      }
    }

    private void create_vm()
    {
      sql sql = new sql("localhost", parameters![param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);

      ulong memory;
      uint vcpus;
      string? input;
      virtual_machine vm;
      Console.WriteLine("Create vm using template - YES? (ENTER for default NO");
      input = Console.ReadLine();
      // create without template
      if (input == "")
      {
      mem: bool res = false;
        Console.Write("Enter memory size (Mb): ");
        input = Console.ReadLine();
        res = ulong.TryParse(input, out memory);
        if (res)
        {
          memory = ulong.Parse(input!);
        }
        else
        {
          Console.WriteLine("Invalid values given, try again.");
          goto mem;
        }
      vcpus: res = false;
        Console.Write("Enter number of vcpus: ");
        input = Console.ReadLine();
        res = uint.TryParse(input, out vcpus);
        if (res)
        {
          vcpus = uint.Parse(input!);
        }

        if (!res)
        {
          Console.WriteLine("Invalid value given, try again.");
          goto vcpus;
        }

        Console.Write("Enter vm name (or ENTER for default)");
        input = Console.ReadLine();
        if (input!.Trim() == "")
        {
          vm = new virtual_machine(memory, vcpus);
        }
        else
        {
          vm = new virtual_machine(memory, vcpus, input.Trim());
        }
      }
      // creates from template
      else
      {
      getuuid: Console.WriteLine("Enter name or uuid of template to use: ");
        input = Console.ReadLine();
        template_virtual_machine template;
        MySqlDataReader res;
        Guid id;
        bool is_uuid = Guid.TryParse(input!.Trim(), out id);
        if (is_uuid)
        {
          res = sql.get_template(sql.conn, id);
        }
        else
        {
          res = sql.get_template(sql.conn, input!.Trim());
        }
        if (!res.HasRows)
        {
          Console.WriteLine("No vm found with name or uuid {0}", input);
          goto getuuid;
        }
        res.Read();
        Guid _uuid = res.GetGuid("Uuid");
        string _FriendlyName = res.GetString("FriendlyName");
        ulong _memory = res.GetUInt64("Memory");
        uint _vcpus = res.GetUInt32("Vcpus");
        string _arch = res.GetString("Arch");
        res.Close();
        template = new template_virtual_machine(_uuid, _memory, _vcpus, _FriendlyName, _arch);
        Console.Write("Enter vm name (or ENTER for default)");
        input = Console.ReadLine();
        if (input!.Trim() == "")
        {
          vm = new virtual_machine(template);
        }
        else
        {
          vm = new virtual_machine(template, input.Trim());
        }
      }

      // write vm to db
      int rows_written = sql.create_vm(vm);
    }

    public void start_vm()
    {
      sql sql = new sql("localhost", parameters![param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);

      string? input;
      virtual_machine vm;
    getvm: Console.WriteLine("Enter name or uuid of vm to start");
      input = Console.ReadLine();
      Guid id;
      MySqlDataReader res;
      bool is_uuid = Guid.TryParse(input!.Trim(), out id);
      if (is_uuid)
      {
        res = sql.get_vm(sql.conn, id);
      }
      else
      {
        res = sql.get_vm(sql.conn, input);
      }
      if (!res.HasRows)
      {
        Console.WriteLine("Couldn't find a vm with name or uuid {0}", input);
        goto getvm;
      }
      res.Read();
      Guid _uuid = res.GetGuid("Uuid");
      string _FriendlyName = res.GetString("FriendlyName");
      ulong _memory = res.GetUInt64("Memory");
      uint _vcpus = res.GetUInt32("Vcpus");
      string _arch = res.GetString("Arch");
      Guid _template = res.GetGuid("Template");
      res.Close();

      vm = new virtual_machine(_uuid, _FriendlyName, _memory, _vcpus, _arch, _template);
      vm.start_vm("/home/philip/Documents/test-bzImage2");
    }
  }
}