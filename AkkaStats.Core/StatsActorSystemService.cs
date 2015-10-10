using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Messages;
using Akka.DI.Core;
using Autofac;

namespace AkkaStats.Core
{



    public class ActorSystemFactory : IActorSystemFactory
    {

        private readonly ILifetimeScope lifetimeScope;

        public ActorSystemFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public ActorSystem Create(string name)
        {
            ActorSystem statsActorSystem = ActorSystem.Create(name);
            IDependencyResolver resolver = new AutoFacDependencyResolver(lifetimeScope, statsActorSystem);
            return statsActorSystem;
        }

    }

    public interface IStatsActor
    {
        //Task<string> HollaBack(string msg);
        void AddPlayer(PlayerMessage msg);
        Task<PlayerMessage> GetById(string id);
    }

    public class StatsActorSystemService : IStatsActor
    {
  
        private readonly ActorSystem StatsActorSystem;
        private readonly IActorRef statActorRef;

        public StatsActorSystemService(IActorSystemFactory actorSystemFactory)
        {
            ActorSystem StatsActorSystem = actorSystemFactory.Create("ValidateStatsActor");
            //Props statsActorProps = Props.Create<ValidateStatsActor>();
            //statActorRef = StatsActorSystem.ActorOf(statsActorProps, "ValidateStatsActor");
            statActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<ValidateStatsActor>(), "ValidateStatsActor");
        }

        public void AddPlayer(PlayerMessage msg)
        {
             statActorRef.Tell(msg);
        }

        public async Task<PlayerMessage> GetById(string id)
        {
            var result = await statActorRef.Ask<PlayerMessage>(Guid.Parse(id));
            return result;
        }

        //public async Task<string> HollaBack(string msg)
        //{
        //    var result = await statActorRef.Ask<string>("sup brotha man!");
        //    return result;
        //}
    }
}
