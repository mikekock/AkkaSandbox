using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;

namespace AkkaStats.Core
{
    public class ActorSystemFactory : IActorSystemFactory
    {

        private readonly ILifetimeScope _lifetimeScope;

        public ActorSystemFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ActorSystem Create(string name)
        {
            ActorSystem statsActorSystem = ActorSystem.Create(name);
            IDependencyResolver resolver = new AutoFacDependencyResolver(_lifetimeScope, statsActorSystem);
            return statsActorSystem;
        }

    }
}