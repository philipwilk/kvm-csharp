namespace Main
{
  class command_info : run
  {
    const string description = "Gets info about the program.";
    public command_info(List<string> args) : base(description, args)
    { }

    public command_info() : base(description)
    { }

    public override void execute(string subaction, IDictionary<param.parameters, String> parameters, host self)
    {
      switch (subaction)
      {
        case "help":
          {
            this.help();
            return;
          }
        default:
          {
            // STUB
            return;
          }
      }
    }

    public override string get_sub_action()
    {
      switch (commands.comm(args!))
      {
        case "help":
          {
            return "help";
          }
        default:
          {
            return "";
          }
      }
    }
  }
}