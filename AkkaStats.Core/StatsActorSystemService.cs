using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Messages;

namespace AkkaStats.Core
{

    public interface IStatsActor
    {
        Task<string> HollaBack(string msg);
        void AddPlayer(PlayerMessage msg);
        PlayerMessage GetById(Guid id);
    }

    public class StatsActorSystemService :IStatsActor
    {
        private readonly ActorSystem StatsActorSystem;
        private readonly IActorRef statActorRef;

        public StatsActorSystemService()
        {
            StatsActorSystem = ActorSystem.Create("StatsActorSystem");
            Props statsActorProps = Props.Create<ValidateStatsActor>();

            statActorRef = StatsActorSystem.ActorOf(statsActorProps, "ValidateStatsActor");
        }

        public void AddPlayer(PlayerMessage msg)
        {
             statActorRef.Tell(msg);
        }

        public PlayerMessage GetById(Guid id)
        {
            var result = statActorRef.Ask<PlayerMessage>(id);
            return result;
        }

        public async Task<string> HollaBack(string msg)
        {
            var result = await statActorRef.Ask<string>("sup brotha man!");
            return result;
        }
    }
}
