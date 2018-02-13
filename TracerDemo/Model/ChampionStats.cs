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
        public Champion champion { get; set; }
        public Stats championStats { get; set; }
    }
}
