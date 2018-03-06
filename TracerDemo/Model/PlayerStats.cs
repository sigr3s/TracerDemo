using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class PlayerStats
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual TracerPlayer player { get; set; }
        public string TracerPlayerId {get; set;}
        public virtual List<ChampionStats> championStats { get; set; }
        public virtual Stats stats { get; set; }
        public string StatsId {get; set;}
    }
}
