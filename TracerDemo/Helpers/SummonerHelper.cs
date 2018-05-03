using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
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
    private StaticChampionList scl;

    public SummonerHelper(SqliteContext context)
    {
      db = context;
      client = new RiotClient();
      Champion c = db.Champion.FirstOrDefault();
      if(c == null){
          Task.Run(PopulateChampionsAsync);
      }
    }

    private async Task PopulateChampionsAsync(){
        List<Champion> champions = (await client.GetChampionsAsync()).ToList();
        foreach(Champion c in champions){
          db.Add(c);
        }
        db.SaveChanges();
    }

    public struct SummonerResponse{
       public  TracerPlayer tracerPlayer;
       public RequestState requestState;
    }

    public enum RequestState{
      Done,
      Processing
    }

    public async Task<SummonerResponse> FromSummonerName(string name, bool forceReload = false){
      TracerPlayer player = null;
      //check the db
      Summoner summoner = await client.GetSummonerBySummonerNameAsync(name).ConfigureAwait(false);
       player = db.TracerPlayers.Include(tp => tp.Summoner )
                                .Include(tp => tp.PlayerStats)
                                .ThenInclude(tp => tp.championStats )
                                .ThenInclude(x => x.Stats)
                                .Where(tp => tp.Summoner.Id == (long) summoner.Id).FirstOrDefault();

      if(player == null){
        player = new TracerPlayer{
          Summoner = summoner
        };
        db.TracerPlayers.Add(player);
        db.SaveChanges();
      }

      player = db.TracerPlayers.Where(tp => tp.Summoner.Id == (long) summoner.Id).FirstOrDefault();

      //check riot api
      if(player.LastUpdate != 0 && !forceReload){
        if((DateTime.Now - DateTime.FromFileTimeUtc(player.LastUpdate)).TotalDays < 5){
            return new SummonerResponse{ tracerPlayer = player , requestState = RequestState.Done };
        }
      }

      if(!processingSummoners.Contains(player.Summoner.Name)){
        processingSummoners.Add(summoner.Name);
        BackgroundJob.Enqueue(() => UpdateStatsAsync(summoner.Id));
      }
      return new SummonerResponse{ tracerPlayer = player , requestState = RequestState.Processing };
;
    }

    public List<string> processingSummoners = new List<string>();

    [AutomaticRetry(Attempts = 3)]
    public async Task<TracerPlayer> UpdateStatsAsync(long tpID){
      if(scl == null){
        scl = (await client.GetStaticChampionsAsync());
        scl.Keys = new Dictionary<string, string>();

        foreach(var k in scl.Data.Keys){
           scl.Keys.Add(scl.Data[k].Id.ToString(), k);
        }
      }

      TracerPlayer tp = db.TracerPlayers.Include(t => t.Summoner )
                                .Include(t => t.PlayerStats)
                                .ThenInclude(t => t.championStats)
                                .ThenInclude(x => x.Stats)
                                .Where(t => t.Summoner.Id == (long) tpID).FirstOrDefault();

      bool doneSome = false;
      if(tp.PlayerStats != null && tp.PlayerStats.championStats != null && tp.PlayerStats.championStats.Count > 0 ){
        //Delete db trash..
        foreach(ChampionStats cs in tp.PlayerStats.championStats){
            db.Remove(cs);
            doneSome = true;
        }
        if(tp.PlayerStats.stats != null) {
          db.Remove(tp.PlayerStats.stats);
            doneSome = true;
        }
        if(tp.PlayerStats != null){
          db.Remove(tp.PlayerStats);
            doneSome = true;
        }
        if(doneSome) db.SaveChanges();
      }
      //check the db
      TracerDemo.Model.Stats generalStats = CleanStats();
      DateTime lastTwoMonths = DateTime.Today.AddMonths(-2);
      var endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTimeKind.Utc);
      var beginTime = endTime.AddMonths(-2);
      IEnumerable<QueueType> rankedQueues = new[] { QueueType.TEAM_BUILDER_RANKED_SOLO };

      MatchList matchList = await client.GetMatchListByAccountIdAsync
                            (tp.Summoner.AccountId, rankedQueues:rankedQueues );
      if(matchList == null) return tp;
      List<MatchReference> matchReferneces = matchList.Matches;
      if(matchReferneces == null) return tp;
      PlayerStats tracerStats = new PlayerStats();
      tracerStats.TracerPlayerId = tp.Id;

      Dictionary<long, ChampionStats> champDict = new Dictionary<long, ChampionStats>();

      int i = 0;
      foreach(MatchReference mr in matchReferneces){
        if(mr.Timestamp > beginTime && mr.Timestamp < endTime){
            i++;
        }
        else
        {
          Console.WriteLine("All rpocessed");
          break;
        }

        Console.WriteLine("Processing match " + i +" of " + matchReferneces.Count);
        int champId = mr.Champion;
        //bool firstGame = false;
        if(!champDict.ContainsKey(champId)){
          ChampionStats cs = new ChampionStats {TracerPlayerId = tp.Id, ChampionId = champId,ChampionName = scl.Keys[champId.ToString()], Stats = CleanStats()};
          champDict.Add(champId, cs);
          //firstGame = true;
        }
        Match m = await client.GetMatchAsync(mr.GameId);
        if(m == null) continue;
        int blueDeaths = 0;
        int blueAsists = 0;
        int blueKills = 0;
        int redDeaths = 0;
        int redAsists = 0;
        int redKills =0;

        int playerKills = 0;
        int playerAssists = 0;
        int playerDeaths = 0;
        TeamSide playerSide = TeamSide.Team1;

        foreach(MatchParticipant mp in m.Participants){
            switch(mp.TeamId){
              case TeamSide.Team1:
                blueDeaths += mp.Stats.Deaths;
                blueAsists += mp.Stats.Assists;
                blueKills += mp.Stats.Kills;
                break;
              case TeamSide.Team2:
                redDeaths += mp.Stats.Deaths;
                redAsists += mp.Stats.Assists;
                redKills += mp.Stats.Kills;
                break;
            }
            if(mp.ChampionId == champId){
              playerSide =mp.TeamId;
              MatchParticipantStats matchPlayerStats = mp.Stats;
              champDict[champId].Stats.Games +=1;
              generalStats.Games += 1;
              champDict[champId].Stats.Minutes += (float) m.GameDuration.TotalMinutes;
              generalStats.Minutes += (float) m.GameDuration.TotalMinutes;

              champDict[champId].Stats.MinutesXMatch += champDict[champId].Stats.Minutes / champDict[champId].Stats.Games;
              generalStats.MinutesXMatch += generalStats.Minutes / generalStats.Games;

              if(matchPlayerStats.Win){
                champDict[champId].Stats.Wins +=1;
                generalStats.Wins +=1;
              }
              champDict[champId].Stats.WinRate = ((champDict[champId].Stats.Games * 1f) / (champDict[champId].Stats.Wins *1f));
              generalStats.WinRate = ((generalStats.Games * 1f) / (generalStats.Wins *1f));

              //Champion Wards
              champDict[champId].Stats.Wards += mp.Stats.WardsPlaced;
              champDict[champId].Stats.WardsKilled += mp.Stats.WardsKilled;
              champDict[champId].Stats.WardXmin = champDict[champId].Stats.Wards / champDict[champId].Stats.Minutes;
              champDict[champId].Stats.WardKillXmin = champDict[champId].Stats.WardsKilled / champDict[champId].Stats.Minutes;

              //General Wards
              generalStats.Wards += mp.Stats.WardsPlaced;
              generalStats.WardsKilled += mp.Stats.WardsKilled;
              generalStats.WardXmin = generalStats.Wards / generalStats.Minutes;
              generalStats.WardKillXmin = generalStats.WardsKilled / generalStats.Minutes;

              generalStats.Kills += mp.Stats.Kills;
              generalStats.Daths += mp.Stats.Deaths;
              generalStats.Asists += mp.Stats.Assists;
              generalStats.Minions += mp.Stats.TotalMinionsKilled;
              generalStats.MinionsMinute = generalStats.Minions / generalStats.Minutes;

              champDict[champId].Stats.Kills += mp.Stats.Kills;
              champDict[champId].Stats.Daths += mp.Stats.Deaths;
              champDict[champId].Stats.Asists += mp.Stats.Assists;
              champDict[champId].Stats.Minions += mp.Stats.TotalMinionsKilled;
              champDict[champId].Stats.MinionsMinute = champDict[champId].Stats.Minions/ champDict[champId].Stats.Minutes;

              playerKills += mp.Stats.Kills;
              playerDeaths += mp.Stats.Deaths;
              playerAssists += mp.Stats.Assists;



              if(mp.Stats.FirstBloodKill || mp.Stats.FirstBloodAssist){
                  champDict[champId].Stats.FirstBlood +=1;
                  generalStats.FirstBlood += 1;
              }

            }
        }

        switch(playerSide){
          case TeamSide.Team1:
            generalStats.KillParticipation += (playerKills + playerAssists) /  Math.Max(1f ,blueKills);
            generalStats.KillShare += (playerKills) /  Math.Max(1f ,blueKills);
            generalStats.DeathShare += (playerDeaths) /  Math.Max(1f ,blueDeaths);

            champDict[champId].Stats.KillParticipation +=(playerKills + playerAssists) /  Math.Max(1f ,blueKills);
            champDict[champId].Stats.KillShare += (playerKills) /  Math.Max(1f ,blueKills);
            champDict[champId].Stats.DeathShare += (playerDeaths) /  Math.Max(1f ,blueDeaths);
          break;

          case TeamSide.Team2:
            generalStats.KillParticipation += (playerKills + playerAssists) /  Math.Max(1f ,redKills);
            generalStats.KillShare += (playerKills) /  Math.Max(1f ,redKills);
            generalStats.DeathShare += (playerDeaths) /  Math.Max(1f ,redDeaths);

            champDict[champId].Stats.KillParticipation +=(playerKills + playerAssists) /  Math.Max(1f ,redKills);
            champDict[champId].Stats.KillShare += (playerKills) /  Math.Max(1f ,redKills);
            champDict[champId].Stats.DeathShare += (playerDeaths) /  Math.Max(1f ,redDeaths);
          break;
        }

      }
      
      List<ChampionStats> championStats = champDict.Values.ToList();

      tp.PlayerStatsId = tracerStats.Id;
      tp.LastUpdate = endTime.ToFileTimeUtc();
      db.TracerPlayers.Update(tp);
      db.SaveChanges();

      foreach(ChampionStats cs in championStats){
        db.Add(cs);
      }

      db.SaveChanges();
      tracerStats.stats = generalStats;
      tracerStats.championStats = championStats;
      tracerStats.TracerPlayerId = tp.Id;
      tracerStats.player = tp;
      tracerStats.StatsId = generalStats.Id;
      db.Add(tracerStats);
      db.SaveChanges();

      processingSummoners.Remove(tp.Summoner.Name);

       return tp;
    }


    public TracerDemo.Model.Stats CleanStats(){
      return new TracerDemo.Model.Stats {
        Games =0, Kills =0, Asists =0, Daths = 0, Minions = 0, MinutesXMatch=0,WinRate=0, MinionsMinute =0,KillParticipation =0,KillShare=0, DeathShare=0,FirstBlood=0, CreepsAtTen=0,CreepsDifAtTen=0, WardXmin=0
      };
    }

  }
}