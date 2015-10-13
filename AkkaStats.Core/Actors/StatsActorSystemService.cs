using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using AkkaStats.Core.Factories;
using AkkaStats.Core.Messages;

namespace AkkaStats.Core.Actors
{
    public class StatsActorSystemService : IStatsActor
    {
  
        private readonly ActorSystem StatsActorSystem;
        private readonly IActorRef statActorRef;

        public StatsActorSystemService(IActorSystemFactory actorSystemFactory)
        {
            StatsActorSystem = actorSystemFactory.Create("StatsCoordinatorActor");
            statActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorActor>()
                .WithRouter(new RoundRobinPool(2)), "StatsCoordinatorActor");
        }

        public async Task<PlayerMessage> GetById(string id)
        {
            var result = await statActorRef.Ask<PlayerMessage>(Guid.Parse(id));
            return result;
        }

        public async Task<List<PlayerMessage>> GetAll()
        {
            var result = await statActorRef.Ask<List<PlayerMessage>>("all");
            return result;
        }

        public async Task DeleteAllPlayers()
        {
            statActorRef.Tell("delete");
        }

        public async Task BulkPlayers(List<PlayerMessage> list)
        {
            foreach (var item in list)
            {
                item.State = State.Create;
                statActorRef.Tell(item);
            }
        }

        public async Task AddPlayer(PlayerMessage msg)
        {
            msg.State = State.Create;
            statActorRef.Tell(msg);
        }

    }
}
