using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaStats.Core.Events
{
    public class HomeRunHitEvent : IEvent
    {
        public HomeRunHitEvent()
        {
        }
    }

    public class HitterAddedEvent : IEvent
    {
        public HitterAddedEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
    }
}
