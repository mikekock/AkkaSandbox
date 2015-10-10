using Akka.Actor;

namespace AkkaStats.Core
{
    public interface IActorSystemFactory
    {
        ActorSystem Create(string name);
    }
}