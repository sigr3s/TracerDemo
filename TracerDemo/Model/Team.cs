
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public ICollection<TeamTracerPlayer> TeamsRelation {get; set;}
        public DateTime LastUpdate { get; set; }
    }
}
