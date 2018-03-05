﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiotNet.Models;

namespace TracerDemo.Model
{
    public class TracerPlayer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Summoner Summoner { get; set; }
        public long SummonerId {get; set;}
        public PlayerStats PlayerStats {get; set;}
        public string PlayerStatsId {get; set;}
        public ICollection<TeamTracerPlayer> TeamsRelation {get; set;}

        public DateTime LastUpdate;
    }
}
