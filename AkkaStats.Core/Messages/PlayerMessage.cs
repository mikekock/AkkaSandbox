using System;
using AkkaStats.Persistance.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace AkkaStats.Core.Messages
{
    public class PlayerMessage : IMongoEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public State State { get; set; }
    }
}