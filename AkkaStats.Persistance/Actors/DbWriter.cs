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
        private readonly IMongoCollection<T> _mongoCollection;

        public DbWriter()
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);

            Receive<T>(x => Add(x));
            Receive<DbRequestMessage>(x => DeleteMany(x), x => x.Query == DbRequestType.DeleteMany);
            Receive<DbRequestMessage>(x => DeleteOne(x), x => x.Query == DbRequestType.DeleteOne);
        }

        private void Add(T obj)
        {
            Debug.WriteLine("Add using the DbWriter");
            _mongoCollection.InsertOneAsync(obj);
        }

        private void DeleteOne(DbRequestMessage requestMessage)
        {
            Debug.WriteLine("DeleteOne using the DbWriter " + requestMessage.Id);
            var filter = Builders<T>.Filter.Eq("_id", requestMessage.Id);
            _mongoCollection.DeleteOneAsync(filter);
        }

        private void DeleteMany(DbRequestMessage requestMessage)
        {
            Debug.WriteLine("DeleteMany using the DbWriter");
            _mongoCollection.DeleteManyAsync(_ => true);
        }

    }
}