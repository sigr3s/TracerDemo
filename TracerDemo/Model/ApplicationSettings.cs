using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{
    public class ApplicationSettings
    {
        public string DatabaseConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SigningKey { get; set; }
    }
}
