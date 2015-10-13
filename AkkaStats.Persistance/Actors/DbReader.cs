using System.Configuration;
using System.Diagnostics;
using Akka.Actor;
using AkkaStats.Persistance.Interfaces;
using AkkaStats.Persistance.Messages;
using MongoDB.Driver;

namespace AkkaStats.Persistance.Actors
{
    public class DbReader<T> : ReceiveActor where T : IMongoEntity
    {
        
        private readonly IMongoCollection<T> _mongoCollection;

        public DbReader()
        {
            var mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            var mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);

            Receive<DbRequestMessage>(GetMany, x => x.Query == DbRequestType.GetAll);
            Receive<DbRequestMessage>(GetOne, x => x.Query == DbRequestType.GetById);
        }

        private void GetMany(DbRequestMessage requestMessage)
        {
            Debug.WriteLine("Calling DbReader GetMany");
            _mongoCollection.Find(_ => true).ToListAsync().PipeTo(Sender);
        }

        private void GetOne(DbRequestMessage requestMessage)
        {
            Debug.WriteLine("Calling DbReader GetOne");
            var filter = Builders<T>.Filter.Eq("_id", requestMessage.Id);
            _mongoCollection.Find(filter).SingleOrDefaultAsync().PipeTo(Sender);
        }

    }
}
