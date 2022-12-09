namespace Main
{
  class command_template : run
  {
    const string description = "Creates and deletes vm templates";
    public command_template(List<string> args) : base(description, args) { }
    public command_template() : base(description) { }

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
            create_template();
            return;
          }
        case "delete":
          {
            // delete template STUB
            return;
          }
        case "list":
          {
            // list templates
            return;
          }
      }
    }

    private void create_template()
    {
      ulong memory;
      uint vcpus;

    mem: bool res = false;
      string? input;
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

      Console.Write("Enter template name (or ENTER for default)");
      input = Console.ReadLine();
      template_virtual_machine template;
      if (input!.Trim() == "")
      {
        template = new template_virtual_machine(memory, vcpus);
      }
      else
      {
        template = new template_virtual_machine(memory, vcpus, input.Trim());
      }

      sql sql = new sql("localhost", parameters![param.parameters.sqlUser], parameters[param.parameters.sqlPassword]);
      int rows_written = sql.create_template(template);
      if (rows_written > 0)
      {
        Console.WriteLine("Wrote template '{0}' to database.", template.friendly_name);
      }
      else
      {
        Console.WriteLine("Failed to write template to database");
      }

    }
  }
}