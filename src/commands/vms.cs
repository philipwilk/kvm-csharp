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
            // create vm STUB 
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
            // start vm STUB
            return;
          }
      }
    }
  }
}