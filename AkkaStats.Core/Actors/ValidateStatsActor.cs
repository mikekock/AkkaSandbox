using System;
using System.Configuration;
using System.Threading.Tasks;
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

            //Receive<string>(message => HandleStatsMessage(message));
            //Receive<PlayerMessage>(message => HandleAddPlayer(message));
            Receive<Guid>(message => HandleGetPlayerById(message));
            Receive<PlayerMessage>(message => HandleGetPlayerByIdResult(message));
        }

        private void HandleGetPlayerByIdResult(PlayerMessage message)
        {
            Sender.Tell(message);
        }

        //private void HandleStatsMessage(string mess)
        //{
        //    Sender.Tell((string)mess);
        //}

        private async void HandleAddPlayer(PlayerMessage message)
        {
            await _mongoCollection.InsertOneAsync(message);
        }

        private void HandleGetPlayerById(Guid id)
        {
      
            _mongoCollection.Find(x => x.Id.Equals(id)).SingleOrDefaultAsync().PipeTo(Self, Sender);
          
           
        }




    }
}
