using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStats.Core
{
    public interface IMessage { }

    public interface IAddressed
    {
        Guid RecipientId { get; }
    }

    public interface ICommand : IMessage { }

    public interface IEvent : IMessage { }

    public sealed class GetState : ICommand
    {
        public readonly Guid Id;

        public GetState(Guid id)
        {
            Id = id;
        }
    }
}
