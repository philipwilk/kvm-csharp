namespace Main
{
  class command_hosts : run
  {
    const string description = "Gets info about and manipulates hosts.";
    public command_hosts(List<string> args) : base(description, args)
    { }

    public command_hosts() : base(description)
    { }

    public override void execute(string subaction, IDictionary<param.parameters, String> parameters, host self)
    {
      switch (subaction)
      {
        case "list":
          {
            // STUB
            return;
          }
        case "remove":
          {
            // STUB
            return;
          }
        case "help":
          {
            this.help();
            return;
          }
      }
      return;
    }

    public override string get_sub_action()
    {
      switch (commands.comm(args!))
      {
        case "list":
          {
            return "list";
          }
        case "remove":
          {
            return "remove";
          }
        default:
          {
            return "help";
          }
      }
    }
  }
}