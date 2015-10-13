using Akka.Actor;

namespace AkkaStats.Core.Factories
{
    public interface IActorSystemFactory
    {
        ActorSystem Create(string name);
    }
}