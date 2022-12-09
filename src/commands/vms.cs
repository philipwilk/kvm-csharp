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
        case "stop":
          {
            return "stop";
          }
        default:
          {
            return "help";
          }
      }
    }

    public override void execute(string subaction, IDictionary<param.parameters, String> _parameters, host self)
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
            list_vms();
            return;
          }
        case "start":
          {
            start_vm(self);
            return;
          }
        case "stop":
          {
            stop_vm();
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

    private void start_vm(host self)
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

      if (self.memory < vm.memory)
      {
        throw new OutOfMemoryException("Not enough host memory to create this vm");
      }
      if (self.threads < vm.vcpus)
      {
        throw new Exception("Not enough cores to start this vm.");
      }

      // tell sql db is running
      int affected_rows = sql.start_vm(vm.id, self.uuid);

      vm.start_vm("/home/philip/Documents/test-bzImage2");
    }

    private void stop_vm()
    {
      // tell sql db is stopped
      //sql.stop_vm(vm.id);
    }

    private void list_vms()
    {
      sql sql = new sql("localhost", parameters![param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);
      var datareader = sql.get_vms(sql.conn);
      List<virtual_machine> vms = new List<virtual_machine> { };
      Guid _uuid;
      string _FriendlyName;
      ulong _memory;
      uint _vcpus;
      string _arch;
      Guid _template;

      while (datareader.Read())
      {
        _uuid = datareader.GetGuid("Uuid");
        _FriendlyName = datareader.GetString("FriendlyName");
        _memory = datareader.GetUInt64("Memory");
        _vcpus = datareader.GetUInt32("Vcpus");
        _arch = datareader.GetString("Arch");
        bool temp_is_null = Convert.IsDBNull(datareader["Template"]);
        if (temp_is_null)
        {
          _template = Guid.Empty;
        }
        else
        {
          _template = datareader.GetGuid("Template");
        }
        datareader.Close();

        vms.Add(new virtual_machine(_uuid, _FriendlyName, _memory, _vcpus, _arch, _template));
      }

      Console.WriteLine("List of all vms:");
      foreach (var vm in vms)
      {
        Console.WriteLine("Name: '{0}', Memory: {1}, Vcpus: {2}, uuid: {3}, template used: {4}", vm.friendly_name, vm.memory, vm.vcpus, vm.id, vm.template_id);
      }
    }
  }
}