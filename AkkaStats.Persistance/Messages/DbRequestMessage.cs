using System;
using AkkaStats.Persistance.Interfaces;

namespace AkkaStats.Persistance.Messages
{
    public class DbRequestMessage : IMongoEntity
    {
        public DbRequestType Query { get; set; }
        public Guid Id { get; set; }
        public string Name { get; private set; }
    }
}
