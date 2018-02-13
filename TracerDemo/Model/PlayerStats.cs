using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class PlayerStats
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public TracerPlayer player { get; set; }
        public List<ChampionStats> championStats { get; set; }
        public Stats stats { get; set; }
    }
}
