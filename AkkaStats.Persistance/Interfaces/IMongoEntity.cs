using System;
using System.Collections.Generic;
using AkkaStats.Persistance.Messages;

namespace AkkaStats.Persistance.Interfaces
{
    public interface IHubMessageService
    {
        List<HubMessage> SavedMessages();
        void Add(HubMessage obj);
        void Clear();
    }


    public class HubMessageService : IHubMessageService
    {
        public List<HubMessage> Messages;

        public HubMessageService()
        {
            Messages = new List<HubMessage>();
        }

        public List<HubMessage> SavedMessages()
        {
            return Messages;
        }

        public void Add(HubMessage obj)
        {
            Messages.Add(obj);
        }

        public void Clear()
        {
            Messages.Clear();
        }
    }


    public interface IMongoEntity
    {
        Guid Id { get; }
        string Name { get; }
    }
}
