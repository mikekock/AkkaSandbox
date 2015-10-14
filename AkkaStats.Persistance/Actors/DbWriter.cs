using System;
using System.Configuration;
using System.Diagnostics;
using Akka.Actor;
using AkkaStats.Persistance.Interfaces;
using AkkaStats.Persistance.Messages;
using MongoDB.Driver;

namespace AkkaStats.Persistance.Actors
{
    public class DbWriter<T> : ReceiveActor where T : IMongoEntity
    {
        private readonly IHubMessageService _hubMessageService;
        private readonly IMongoCollection<T> _mongoCollection;
        private string EntityName { get; set; }

        public DbWriter(IHubMessageService hubMessageService)
        {
            _hubMessageService = hubMessageService;
            EntityName = typeof (T).Name;
            var mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            var mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(EntityName);

            Receive<T>(x => Add(x));
            Receive<DbRequestMessage>(x => DeleteMany(x), x => x.Query == DbRequestType.DeleteMany);
            Receive<DbRequestMessage>(x => DeleteOne(x), x => x.Query == DbRequestType.DeleteOne);
        }

        private void Add(T obj)
        {
            var message = String.Format("Added {0} {1} to the database", EntityName, obj.Name);
            _hubMessageService.Add(HubMessage.Create(DateTime.UtcNow, message));
            Debug.WriteLine(message);
            _mongoCollection.InsertOneAsync(obj);
        }

        private void DeleteOne(DbRequestMessage requestMessage)
        {
            var message = String.Format("Deleted {0} from the database", EntityName);
            _hubMessageService.Add(HubMessage.Create(DateTime.UtcNow, message));
            Debug.WriteLine(message);

            var filter = Builders<T>.Filter.Eq("_id", requestMessage.Id);
            _mongoCollection.DeleteOneAsync(filter);
        }

        private void DeleteMany(DbRequestMessage requestMessage)
        {
            var message = String.Format("Deleted all {0} from the database", EntityName);
            _hubMessageService.Add(HubMessage.Create(DateTime.UtcNow, message));
            Debug.WriteLine(message);

            _mongoCollection.DeleteManyAsync(_ => true);
        }

    }
}