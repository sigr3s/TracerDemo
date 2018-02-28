

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiotNet;
using RiotNet.Models;
using TracerDemo.Data;
using TracerDemo.Model;

namespace TracerDemo.Helpers
{
  public class SummonerHelper
  {
    private SqliteContext db { get; set; }
    private IRiotClient client;
    
    public SummonerHelper(SqliteContext context)
    {
      db = context;
      client = new RiotClient();

    }

    public async Task<TracerPlayer> FromSummonerName(string name){
      TracerPlayer player = null;
      //check the db
      Summoner summoner = await client.GetSummonerBySummonerNameAsync(name).ConfigureAwait(false);
      player = db.TracerPlayers.Include(tp => tp.Summoner ).Where(tp => tp.Summoner.Id == (long) summoner.Id).FirstOrDefault();

      if(player != null) return player;  
      player = new TracerPlayer{
        Summoner = summoner
      };
      db.TracerPlayers.Add(player);
      db.SaveChanges();

      //check riot api

       return player;
    }


    public TracerPlayer UpdateStats(){
      TracerPlayer player = null;
      //check the db


      //check riot api

       return player;
    }

  }
}