using System;
using System.Configuration;
using Akka.Actor;
using AkkaStats.Core.Messages;
using MongoDB.Driver;

namespace AkkaStats.Core.Actors
{
    public class ValidateStatsActor : ReceiveActor
    {
        private readonly IMongoCollection<PlayerMessage> _mongoCollection;

        public ValidateStatsActor()
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<PlayerMessage>(typeof(PlayerMessage).Name);

            Receive<string>(message => HandleStatsMessage(message));
            Receive<PlayerMessage>(message => HandleAddPlayer(message));
            Receive<Guid>(message => HandleGetPlayerById(message));
        }

        private void HandleStatsMessage(string mess)
        {
            Sender.Tell((string)mess);
        }

        private async void HandleAddPlayer(PlayerMessage message)
        {
            await _mongoCollection.InsertOneAsync(message);
        }

        private async void HandleGetPlayerById(Guid id)
        {
            var result = _mongoCollection.Find(x => x.Id == id).SingleOrDefaultAsync();
            Sender.Tell(result);
           
        }

    }
}
