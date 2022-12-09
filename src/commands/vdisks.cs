namespace Main
{
  class command_vdisk : run
  {
    const string description = "Create, delete and list virtual disks/drives";
    public command_vdisk(List<string> args) : base(description, args) { }
    public command_vdisk() : base(description) { }

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
            // create vdisk STUB
            return;
          }
        case "delete":
          {
            // delete vdisk STUB
            return;
          }
        case "list":
          {
            // list vdisk
            return;
          }
      }
    }
  }
}