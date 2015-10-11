using System;
using System.Collections.Generic;
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

            //Receive<string>(message => HandleStatsMessage(message));
            Receive<PlayerMessage>(HandleAddPlayer, x => x.State == State.Create);
            Receive<Guid>(x => HandleGetPlayerById(x));
            Receive<PlayerMessage>(ResponseGetPlayerByIdResult, x => x.State != State.Create);
            Receive<string>(x => HandleGetAllPlayers(x), message => message.Equals("all"));
            Receive<List<PlayerMessage>>(x => ResponseGetAllPlayers(x));
        }

        /// <summary>
        /// Ask: Fire the request for all the players
        /// </summary>
        private void HandleGetAllPlayers(string message)
        {
            if(message == "all")
            _mongoCollection.Find(_ => true).ToListAsync().PipeTo(Self, Sender);
        }

        /// <summary>
        /// Tell: Send the Array of player data back
        /// </summary>
        private void ResponseGetAllPlayers(List<PlayerMessage> message)
        {
            Sender.Tell(message);
        }

        /// <summary>
        /// Ask: Fire the request for the player
        /// </summary>
        private void HandleGetPlayerById(Guid id)
        {
            _mongoCollection.Find(x => x.Id.Equals(id)).SingleOrDefaultAsync().PipeTo(Self, Sender);
        }

        /// <summary>
        /// Tell: Send the Player Data Back
        /// </summary>
        private void ResponseGetPlayerByIdResult(PlayerMessage message)
        {
            Sender.Tell(message);
        }
   
        /// <summary>
        /// Tell: Add player to database
        /// </summary>
        private void HandleAddPlayer(PlayerMessage message)
        {
            message.State = State.Read;
            _mongoCollection.InsertOneAsync(message);
        }



    }
}
