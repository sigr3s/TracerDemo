using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracerDemo.Data;
using TracerDemo.Helpers;
using TracerDemo.Model;

namespace TracerDemo.Controllers
{
    public class TeamController : Controller {
        private SqliteContext db { get; set; }

        private SummonerHelper summonerHelper {get; set;}


        public TeamController(SqliteContext sqliteContext, SummonerHelper summonerHelper)
		{
			db = sqliteContext;
            this.summonerHelper = summonerHelper;
		}


        [HttpPost]
        [Authorize]
        [Route("api/v1/team")]
        public IActionResult CreateTeam(string id, [FromBody]TeamItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Team t = db.Teams.Where(tm => tm.Name.Equals(model.TeamName)).FirstOrDefault();
                if(t != null) return BadRequest("Team already exsists");

                Team team = new Team()
                {
                    Name = model.TeamName
                };

                team.TeamsRelation = new List<TeamTracerPlayer>();

                if(model.players != null){
                    for(int i = 0; i < model.players.Count; i++){
                        TracerPlayer player = null;
                        if(!string.IsNullOrEmpty(model.players[i].id)){
                            player = db.TracerPlayers.Find(model.players[i].id);
                            if(player == null) return BadRequest("Player don't exist as a tracer player");
                        }
                        else if(!string.IsNullOrEmpty(model.players[i].summonerName)){

                        }
                        else if(player ==null){
                            return BadRequest("Player not found");
                        }
                        team.TeamsRelation.Add(new TeamTracerPlayer {
                            Team = team,
                            TracerPlayer = player
                        });
                    }
                }


                db.Teams.Add(team);
                db.SaveChanges();
                return Ok(team);
            }
            else
                return BadRequest();
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/team/{name}/addPlayers")]
        public async System.Threading.Tasks.Task<IActionResult> AddPlayersAsync(string name, [FromBody] PlayerList playersToAdd)
        {
            if (ModelState.IsValid && playersToAdd.players != null)
            {
                Team  t = db.Teams.Where(te => te.Name == name).FirstOrDefault();
                if(t == null) return BadRequest();

                if(t.TeamsRelation == null) t.TeamsRelation = new List<TeamTracerPlayer>();

                TracerPlayer tracerPlayer;
                for(int i = 0; i < playersToAdd.players.Count; i++){
                    if(!string.IsNullOrEmpty(playersToAdd.players[i].id)){
                        tracerPlayer = db.TracerPlayers.Find(playersToAdd.players[i].id);
                        if(tracerPlayer == null) return BadRequest();
                        t.TeamsRelation.Add(new TeamTracerPlayer {
                            Team = t,
                            TracerPlayer = tracerPlayer
                        });
                    }
                    else if(!string.IsNullOrEmpty((playersToAdd.players[i].summonerName))){
                         tracerPlayer = await summonerHelper.FromSummonerName(playersToAdd.players[i].summonerName);
                        if(tracerPlayer == null) return BadRequest();
                        t.TeamsRelation.Add(new TeamTracerPlayer {
                            Team = t,
                            TracerPlayer = tracerPlayer
                        });
                    }
                    else{
                        return BadRequest();
                    }
                }

                db.Teams.Update(t);
                db.SaveChanges();
                return Ok(t);
            }
            else{
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/team/{name}/addPlayerBySummoner/{summonerName}")]
        public async System.Threading.Tasks.Task<IActionResult> AddPlayerBySummonerAsync(string name, string summonerName)
        {
            if (ModelState.IsValid)
            {
                Team  t = db.Teams.Where(te => te.Name == name).FirstOrDefault();
                if(t == null) return BadRequest();
                
                TracerPlayer tracerPlayer = await summonerHelper.FromSummonerName(summonerName);
                if(tracerPlayer == null) return BadRequest();
                
                if(t.TeamsRelation == null) t.TeamsRelation = new List<TeamTracerPlayer>();
                t.TeamsRelation.Add(new TeamTracerPlayer {
                    TeamId = t.Id,
                    TracerPlayerId = tracerPlayer.Id
                });

                db.Teams.Update(t);
                db.SaveChanges();
                return Ok(t);
            }
            else{
                return BadRequest();
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/v1/team/{name}/removePlayers")]
        public IActionResult RemovePlayers(string name, [FromBody] PlayerList playerToRemove)
        {
            if (ModelState.IsValid && playerToRemove.players != null)
            {
                Team  t = db.Teams.Where(te => te.Name == name).FirstOrDefault();
                if(t == null) return BadRequest();

                TracerPlayer tracerPlayer;
                for(int i = 0; i < playerToRemove.players.Count; i++){
                    if(!string.IsNullOrEmpty(playerToRemove.players[i].id)){
                        tracerPlayer = db.TracerPlayers.Find(playerToRemove.players[i].id);
                        if(tracerPlayer == null) return BadRequest();

                        TeamTracerPlayer ttp = t.TeamsRelation.Where(tr => tr.TeamId == t.Id && tr.TracerPlayerId == tracerPlayer.Id).FirstOrDefault();

                        if(ttp == null) return BadRequest();
                        else{
                            t.TeamsRelation.Remove(ttp);
                        }
                    }
                }

                db.Teams.Update(t);
                db.SaveChanges();
                return Ok(t);
            }
            else{
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("api/v1/teams")]
        public async System.Threading.Tasks.Task<IActionResult> GetTeamsAsync()
        {
            await summonerHelper.FromSummonerName("parpi", true);
            List<Team> teams = db.Teams.Include(t => t.TeamsRelation).
                                        ThenInclude(t => t.TracerPlayer).
                                        ThenInclude(t => t.Summoner).
                                        Include(t => t.TeamsRelation).
                                        ThenInclude(t => t.TracerPlayer).
                                        ThenInclude(t => t.Stats).
                                        ThenInclude(t => t.championStats).
                                        Where(t => !string.IsNullOrEmpty(t.Name)).ToList();
            return Ok(teams);
        }
    }

}