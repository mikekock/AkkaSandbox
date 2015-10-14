using System;
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
        private readonly IHubMessageService _hubMessageService;
        private readonly IMongoCollection<T> _mongoCollection;

        public DbReader(IHubMessageService hubMessageService)
        {
            _hubMessageService = hubMessageService;
            var mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            var mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);

            Receive<DbRequestMessage>(GetMany, x => x.Query == DbRequestType.GetAll);
            Receive<DbRequestMessage>(GetOne, x => x.Query == DbRequestType.GetById);
        }

        private void GetMany(DbRequestMessage requestMessage)
        {
            const string message = "Calling DbReader GetMany";
            _hubMessageService.Add(HubMessage.Create(DateTime.UtcNow, message));
            Debug.WriteLine(message);
            _mongoCollection.Find(_ => true).ToListAsync().PipeTo(Sender);
        }

        private void GetOne(DbRequestMessage requestMessage)
        {
            const string message = "Calling DbReader GetOne";
            _hubMessageService.Add(HubMessage.Create(DateTime.UtcNow, message));
            Debug.WriteLine(message);
            var filter = Builders<T>.Filter.Eq("_id", requestMessage.Id);
            _mongoCollection.Find(filter).SingleOrDefaultAsync().PipeTo(Sender);
        }

    }
}
