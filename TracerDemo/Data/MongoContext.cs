using TracerDemo.Model;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Data
{
    public class MongoContext
    {
        private MongoClient Client { get; set; }
        /// <summary>
        /// Options from Startup, used to setup db connection
        /// </summary>
        /// <param name="settings"></param>
        public MongoContext(ApplicationSettings settings)
        {
            //Connect to the database
            Client = new MongoClient(settings.DatabaseConnectionString);

            Setup(settings.DatabaseName);
        }
              
        private void Setup(string database)
        {
            ConventionPack pack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreExtraElements", pack, t => true);

            IMongoDatabase Database = Client.GetDatabase(database);

            //Link the accessible collections to actual DB collections
            Users = Database.GetCollection<User>("users");
            Todos = Database.GetCollection<Todo>("todos");
        }

        //Define the collections which are accessible
        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<Todo> Todos { get; set; }
    }
}
