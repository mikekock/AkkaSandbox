using System;
using System.Diagnostics;
using Akka.Actor;
using Akka.DI.Core;
using AkkaStats.Core.Messages;
using AkkaStats.Persistance.Actors;
using AkkaStats.Persistance.Messages;

namespace AkkaStats.Core.Actors
{

    public class StatsCoordinatorActor : ReceiveActor
    {
        private readonly IActorRef _dbWriters;
        private readonly IActorRef _dbReaders;

        public StatsCoordinatorActor()
        {
            _dbWriters = Context.ActorOf(Context.DI().Props<DbWriter<PlayerMessage>>(), "DbWriter");
            _dbReaders = Context.ActorOf(Context.DI().Props<DbReader<PlayerMessage>>(), "DbReader");

            Receive<PlayerMessage>(HandleAddPlayer, x => x.State == State.Create);
            Receive<Guid>(x => HandleGetPlayerById(x));
            Receive<string>(x => HandleGetAllPlayers(x), message => message.Equals("all"));
            Receive<string>(x => HandleDeleteAllPlayers(x), message => message.Equals("delete"));
        }

        /// <summary>
        /// Ask: Fire the request for all the players
        /// </summary>
        private void HandleGetAllPlayers(string message)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetAllPlayers");
            var dbRequest = new DbRequestMessage {Query = DbRequestType.GetAll};
            _dbReaders.Ask(dbRequest).PipeTo(Sender);
        }

        /// <summary>
        /// Ask: Fire the request for the player
        /// </summary>
        private void HandleGetPlayerById(Guid id)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetPlayerById");
            var dbRequest = new DbRequestMessage { Query = DbRequestType.GetById, Id = id};
            _dbReaders.Ask(dbRequest).PipeTo(Sender);
        }
   
        /// <summary>
        /// Tell: Add player to database
        /// </summary>
        private void HandleAddPlayer(PlayerMessage message)
        {
            message.State = State.Read;
            Debug.WriteLine(String.Format("StatsCoordinatorActor Adding {0}", message.Name));
            _dbWriters.Tell(message);
        }

        /// <summary>
        /// Tell: Delete all the players in the database
        /// </summary>
        private void HandleDeleteAllPlayers(string message)
        {
            var dbRequest = new DbRequestMessage { Query = DbRequestType.DeleteMany };
            _dbWriters.Tell(dbRequest);
        }


    }
}
