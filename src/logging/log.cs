namespace Main
{
  class log
  {
    public enum Severity
    {
      Emerg = 0,
      Alert = 1,
      Crit = 2,
      Err = 3,
      Warning = 4,
      Notice = 5,
      Info = 6,
      Debug = 7
    }
    private Severity level;
    private String id, desc;

    public log(Severity _level, String _id, String _desc)
    {
      level = _level;
      id = _id;
      desc = _desc;
    }

    public Severity get_level()
    {
      return level;
    }

    public String get_id()
    {
      return id;
    }

    public String get_desc()
    {
      return desc;
    }
  }
}