using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;

namespace AkkaStats.Core.Factories
{
    public class ActorSystemFactory : IActorSystemFactory
    {

        private readonly ILifetimeScope _lifetimeScope;
        private ActorSystem _actorSystem;

        public ActorSystemFactory(ILifetimeScope lifetimeScope, ActorSystem System)
        {
            _lifetimeScope = lifetimeScope;
            _actorSystem = System;
        }

        public ActorSystem Create(string name)
        {
            //ActorSystem statsActorSystem = ActorSystem.Create(name);
            IDependencyResolver resolver = new AutoFacDependencyResolver(_lifetimeScope, _actorSystem);
            return _actorSystem;
        }

    }
}