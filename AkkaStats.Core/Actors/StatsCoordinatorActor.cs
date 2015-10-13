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
        private readonly IActorRef _dbHitterWriter;
        private readonly IActorRef _dbHitterReader;
        private readonly IActorRef _dbPitcherWriter;
        private readonly IActorRef _dbPitcherReader;

        public StatsCoordinatorActor()
        {
            _dbHitterWriter = Context.ActorOf(Context.DI().Props<DbWriter<HitterMessage>>(), "DbHitterWriter");
            _dbHitterReader = Context.ActorOf(Context.DI().Props<DbReader<HitterMessage>>(), "DbHitterReader");
            _dbPitcherWriter = Context.ActorOf(Context.DI().Props<DbWriter<PitcherMessage>>(), "DbPitcherWriter");
            _dbPitcherReader = Context.ActorOf(Context.DI().Props<DbReader<PitcherMessage>>(), "DbPitcherReader");

            Receive<PitcherMessage>(HandleAddPitcher, x => x.State == State.Create);
            Receive<HitterMessage>(x => HandleAddHitter(x), x => x.State == State.Create);

            Receive<PlayerQuery>(x => HandleGetPitcherById(x), x => x.Action == "get_pitcher");
            Receive<PlayerQuery>(x => HandleGetHitterById(x), x => x.Action == "get_hitter");

            Receive<PlayerQuery>(x => HandleGetAllPitchers(x), message => message.Action.Equals("all_pitchers"));
            Receive<PlayerQuery>(x => HandleGetAllHitters(x), message => message.Action.Equals("all_hitters"));

            Receive<PlayerQuery>(x => HandleDeleteAllPitchers(x), message => message.Action.Equals("delete_pitchers"));
            Receive<PlayerQuery>(x => HandleDeleteAllHitters(x), message => message.Action.Equals("delete_hitters"));

            Receive<PlayerQuery>(x => HandleDeletePitcherById(x), message => message.Action.Equals("delete_pitcher"));
            Receive<PlayerQuery>(x => HandleDeleteHitterById(x), message => message.Action.Equals("delete_hitter"));
        }

        /// <summary>
        /// Ask: Fire the request for all the pitchers
        /// </summary>
        private void HandleGetAllPitchers(PlayerQuery message)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetAllPitchers");
            var dbRequest = new DbRequestMessage {Query = DbRequestType.GetAll};
            _dbPitcherReader.Ask(dbRequest).PipeTo(Sender);
        }

        /// <summary>
        /// Ask: Fire the request for all the hitters
        /// </summary>
        private void HandleGetAllHitters(PlayerQuery message)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetAllHitters");
            var dbRequest = new DbRequestMessage { Query = DbRequestType.GetAll };
            _dbHitterReader.Ask(dbRequest).PipeTo(Sender);
        }

        /// <summary>
        /// Ask: Fire the request for the player
        /// </summary>
        private void HandleGetPitcherById(PlayerQuery obj)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetPitcherById");
            var dbRequest = new DbRequestMessage { Query = DbRequestType.GetById, Id = obj.Id};
            _dbPitcherReader.Ask(dbRequest).PipeTo(Sender);
        }

        private void HandleGetHitterById(PlayerQuery obj)
        {
            Debug.WriteLine("StatsCoordinatorActor HandleGetHitterById");
            var dbRequest = new DbRequestMessage { Query = DbRequestType.GetById, Id = obj.Id };
            _dbHitterReader.Ask(dbRequest).PipeTo(Sender);
        }

        /// <summary>
        /// Tell: Add pitcher to database
        /// </summary>
        private void HandleAddPitcher(PitcherMessage message)
        {
            message.State = State.Read;
            Debug.WriteLine(String.Format("StatsCoordinatorActor HandleAddPitcher {0}", message.Name));
            _dbPitcherWriter.Tell(message);
        }

        /// <summary>
        /// Tell: Add hitter to database
        /// </summary>
        private void HandleAddHitter(HitterMessage message)
        {
            message.State = State.Read;
            Debug.WriteLine(String.Format("StatsCoordinatorActor HandleAddHitter {0}", message.Name));
            _dbHitterWriter.Tell(message);
        }

        /// <summary>
        /// Tell: Delete all the pitchers in the database
        /// </summary>
        private void HandleDeleteAllPitchers(PlayerQuery obj)
        {
            var dbRequest = new DbRequestMessage { Query = DbRequestType.DeleteMany };
            _dbPitcherWriter.Tell(dbRequest);
        }

        /// <summary>
        /// Tell: Delete all the hitters in the database
        /// </summary>
        private void HandleDeleteAllHitters(PlayerQuery obj)
        {
            var dbRequest = new DbRequestMessage { Query = DbRequestType.DeleteMany };
            _dbHitterWriter.Tell(dbRequest);
        }

        /// <summary>
        /// Tell: Delete a pitcher in the database
        /// </summary>
        private void HandleDeletePitcherById(PlayerQuery obj)
        {
            var dbRequest = new DbRequestMessage { Query = DbRequestType.DeleteOne, Id = obj.Id };
            _dbPitcherWriter.Tell(dbRequest);
        }

        /// <summary>
        /// Tell: Delete a hitter in the database
        /// </summary>
        private void HandleDeleteHitterById(PlayerQuery obj)
        {
            var dbRequest = new DbRequestMessage { Query = DbRequestType.DeleteOne, Id = obj.Id };
            _dbHitterWriter.Tell(dbRequest);
        }

    }
}
