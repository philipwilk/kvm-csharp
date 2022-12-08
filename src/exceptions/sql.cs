namespace Main
{
  public class no_db_details : ArgumentException
  {
    public no_db_details() : base("Sql database user and password not provided.") { }
  }
}