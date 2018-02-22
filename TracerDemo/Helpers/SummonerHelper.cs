

using TracerDemo.Data;

namespace TracerDemo.Helpers
{
  public class SummonerHelper
  {
    private SqliteContext db { get; set; }
    public SummonerHelper(SqliteContext context)
    {
      db = context;
    }

  }
}