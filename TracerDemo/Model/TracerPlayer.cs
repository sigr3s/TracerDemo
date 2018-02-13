using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiotNet.Models;

namespace TracerDemo.Model
{
    public class TracerPlayer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Summoner summoner { get; set; }
        public PlayerStats stats {get; set;}
    }
}
