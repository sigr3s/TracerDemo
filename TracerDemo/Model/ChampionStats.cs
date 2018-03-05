using RiotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class ChampionStats
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public TracerPlayer player { get; set; }
        public string TracerPlayerId {get; set;}
        public Champion champion { get; set; }
        public long ChampionId {get; set;}
        public Stats Stats { get; set; }
        public string StatsId {get; set;}
    }
}
