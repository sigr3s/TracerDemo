using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TracerDemo.Data;
using TracerDemo.Model;

namespace TracerDemo.Controllers
{
    public class TeamController : Controller {
        private SqliteContext db { get; set; }


        public TeamController(SqliteContext sqliteContext)
		{
			db = sqliteContext;
		}


        [HttpPost]
        [Authorize]
        [Route("api/v1/team")]
        public IActionResult CreateTeam(string id, [FromBody]TeamItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Team t = db.Teams.Where(team => team.Name.Equals(model.TeamName)).FirstOrDefault();
                if(t != null) return BadRequest("Team already exsists");
                List<TracerPlayer> players = new List<TracerPlayer>();
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
                        players.Add(player);
                    }
                }

                Team item = new Team()
                {
                    Name = model.TeamName,
                    Players = players
                };

                db.Teams.Add(item);
                db.SaveChanges();
                return Ok(item);
            }
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/team/{name}/addPlayers")]
        public IActionResult AddPlayers(string name, [FromBody] PlayerList playersToAdd)
        {

            return Ok("todo");
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/team/{name}/removePlayers")]
        public IActionResult RemovePlayers(string name, [FromBody] PlayerList playerToRemove)
        {

            return Ok("todo");
        }


        [HttpGet]        
        [Authorize]
        [Route("api/v1/teams")]
        public IActionResult GetTeams(string id, string teamName)
        {
            List<Team> teams = db.Teams.Where(t => !string.IsNullOrEmpty(t.Name)).ToList();
            return Ok(teams);
        }




    }

}