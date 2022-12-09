namespace Main
{
  class command_vdev : run
  {
    const string description = "Create, delete and list virtual devices";
    public command_vdev(List<string> args) : base(description, args) { }
    public command_vdev() : base(description) { }

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
            // create vdev STUB
            return;
          }
        case "delete":
          {
            // delete vdev STUB
            return;
          }
        case "list":
          {
            // list vdevs
            return;
          }
      }
    }
  }
}