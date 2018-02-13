using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiotNet;

namespace TracerDemo.Model
{
    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public RiotNet.Models.Player {get; set;}
    }
}
