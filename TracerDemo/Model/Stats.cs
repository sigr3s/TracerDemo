using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class Stats
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int Games { get; set; }

        public int Kills { get; set; }
        public int Asists { get; set; }
        public int Daths { get; set; }
        public int Minions { get; set; }
        public float MinutesXMatch { get; set; }
        public float WinRate { get; set; }
        public float MinionsMinute { get; set; }
        public float KillParticipation { get; set; }
        public float KillShare { get; set; }
        public float DeathShare { get; set; }
        public float FirstBlood { get; set; }
        public float CreepsAtTen { get; set; }
        public float CreepsDifAtTen { get; set; }
        public float WardXmin { get; set; }
    }
}
