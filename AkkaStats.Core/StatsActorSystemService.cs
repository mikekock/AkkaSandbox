using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Messages;

namespace AkkaStats.Core
{
    public class StatsActorSystemService : IStatsActor
    {
  
        private readonly ActorSystem StatsActorSystem;
        private readonly IActorRef statActorRef;

        public StatsActorSystemService(IActorSystemFactory actorSystemFactory)
        {
            StatsActorSystem = actorSystemFactory.Create("ValidateStatsActor");
            //Props statsActorProps = Props.Create<ValidateStatsActor>();
            //statActorRef = StatsActorSystem.ActorOf(statsActorProps, "ValidateStatsActor");
            statActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<ValidateStatsActor>(), "ValidateStatsActor");
        }

        public async Task<PlayerMessage> GetById(string id)
        {
            var result = await statActorRef.Ask<PlayerMessage>(Guid.Parse(id));
            return result;
        }

        public async Task<IEnumerable<PlayerMessage>> GetAll()
        {
            var result = await statActorRef.Ask<IEnumerable<PlayerMessage>>("all");
            return result;
        }

        public async Task AddPlayer(PlayerMessage msg)
        {
            msg.State = State.Create;
            statActorRef.Tell(msg);
        }

    }
}
