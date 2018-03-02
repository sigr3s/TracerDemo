

using System;
using System.Collections.Generic;
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

    public async Task<TracerPlayer> FromSummonerName(string name, bool forceReload = false){
      TracerPlayer player = null;
      //check the db
      Summoner summoner = await client.GetSummonerBySummonerNameAsync(name).ConfigureAwait(false);
       player = db.TracerPlayers.Include(tp => tp.Summoner )
                                .Include(tp => tp.Stats)
                                .Where(tp => tp.Summoner.Id == (long) summoner.Id).FirstOrDefault();

      if(player == null){  
        player = new TracerPlayer{
          Summoner = summoner
        };
        db.TracerPlayers.Add(player);
        db.SaveChanges();
      }

      //check riot api
      await UpdateStatsAsync(player, summoner, forceReload);
      return player;
    }


    public async Task<TracerPlayer> UpdateStatsAsync(TracerPlayer tp, Summoner s, bool forceReload){
      if(tp.LastUpdate != null && !forceReload){
        if((DateTime.Now - tp.LastUpdate).TotalDays < 5){
            return tp;
        }
      }
      //check the db
      TracerDemo.Model.Stats generalStats = CleanStats();
      DateTime lastTwoMonths = DateTime.Today.AddMonths(-2);
      MatchList matchList = await client.GetMatchListByAccountIdAsync(s.AccountId, rankedQueues: new List<QueueType> { QueueType.TEAM_BUILDER_RANKED_SOLO}, beginTime: DateTime.Now , endTime: lastTwoMonths);
      List<MatchReference> matchReferneces = matchList.Matches;

      PlayerStats tracerStats = new PlayerStats();
      Dictionary<string, ChampionStats> champDict = new Dictionary<string, ChampionStats>();

      foreach(MatchReference mr in matchReferneces){
        int champId = mr.Champion;
        string champion = (await client.GetStaticChampionByIdAsync(mr.Champion)).Name;
        bool firstGame = false;
        if(!champDict.ContainsKey(champion)){
          ChampionStats cs = new ChampionStats {player = tp, champion = new Champion { Id = champId}, championStats = CleanStats()};
          champDict.Add(champion, cs);
          firstGame = true;
        }
        Match m = await client.GetMatchAsync(mr.GameId);
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
              champDict[champion].championStats.Games +=1;
              generalStats.Games += 1;
              champDict[champion].championStats.Minutes += (float) m.GameDuration.TotalMinutes;
              generalStats.Minutes += (float) m.GameDuration.TotalMinutes;

              if(matchPlayerStats.Win){
                champDict[champion].championStats.Wins +=1;
                generalStats.Wins +=1;
              }
              champDict[champion].championStats.WinRate = ((champDict[champion].championStats.Games * 1f) / (champDict[champion].championStats.Wins *1f));
              generalStats.WinRate = ((generalStats.Games * 1f) / (generalStats.Wins *1f));

              //Champion Wards
              champDict[champion].championStats.Wards += mp.Stats.WardsPlaced;
              champDict[champion].championStats.WardsKilled += mp.Stats.WardsKilled;
              champDict[champion].championStats.WardXmin = champDict[champion].championStats.Wards / champDict[champion].championStats.Minutes;
              champDict[champion].championStats.WardKillXmin = champDict[champion].championStats.WardsKilled / champDict[champion].championStats.Minutes;

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

              champDict[champion].championStats.Kills += mp.Stats.Kills;
              champDict[champion].championStats.Daths += mp.Stats.Deaths;
              champDict[champion].championStats.Asists += mp.Stats.Assists;
              champDict[champion].championStats.Minions += mp.Stats.TotalMinionsKilled;
              champDict[champion].championStats.MinionsMinute = champDict[champion].championStats.Minions/ champDict[champion].championStats.Minutes;

              playerKills += mp.Stats.Kills;
              playerDeaths += mp.Stats.Deaths;
              playerAssists += mp.Stats.Assists;

              if(mp.Stats.FirstBloodKill || mp.Stats.FirstBloodAssist){
                  champDict[champion].championStats.FirstBlood +=1;
                  generalStats.FirstBlood += 1;
              }

            }
        }

        switch(playerSide){
          case TeamSide.Team1:
            generalStats.KillParticipation += (playerKills + playerAssists) /  Math.Max(1f ,blueKills);
            generalStats.KillShare += (playerKills) /  Math.Max(1f ,blueKills);
            generalStats.DeathShare += (playerDeaths) /  Math.Max(1f ,blueDeaths);

            champDict[champion].championStats.KillParticipation +=(playerKills + playerAssists) /  Math.Max(1f ,blueKills);
            champDict[champion].championStats.KillParticipation += (playerKills) /  Math.Max(1f ,blueKills);
            champDict[champion].championStats.KillParticipation += (playerDeaths) /  Math.Max(1f ,blueDeaths);
          break;

          case TeamSide.Team2:
            generalStats.KillParticipation += (playerKills + playerAssists) /  Math.Max(1f ,redKills);
            generalStats.KillShare += (playerKills) /  Math.Max(1f ,redKills);
            generalStats.DeathShare += (playerDeaths) /  Math.Max(1f ,redDeaths);

            champDict[champion].championStats.KillParticipation +=(playerKills + playerAssists) /  Math.Max(1f ,redKills);
            champDict[champion].championStats.KillParticipation += (playerKills) /  Math.Max(1f ,redKills);
            champDict[champion].championStats.KillParticipation += (playerDeaths) /  Math.Max(1f ,redDeaths);
          break;
        }

      }
      
      List<ChampionStats> championStats = champDict.Values.ToList();

      tracerStats.stats = generalStats;
      tracerStats.championStats = championStats;

      tp.Stats = tracerStats;

      db.Update(tp);
      db.SaveChanges();

       return tp;
    }


    public TracerDemo.Model.Stats CleanStats(){
      return new TracerDemo.Model.Stats {
        Games =0, Kills =0, Asists =0, Daths = 0, Minions = 0, MinutesXMatch=0,WinRate=0, MinionsMinute =0,KillParticipation =0,KillShare=0, DeathShare=0,FirstBlood=0, CreepsAtTen=0,CreepsDifAtTen=0, WardXmin=0
      };
    }

  }
}