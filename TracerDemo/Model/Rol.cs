using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class Rol
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string value { get; set; }
    }
}
