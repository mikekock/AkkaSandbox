using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AkkaStats.Core.Messages
{
    public enum State
    {
        Create = 1,
        Read = 0,
        Update = 2,
        Delete = 3
    }

    public class PlayerMessage
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public State State { get; set; }
    }

    public class GetLatestStatsMessage
    {
        public int Id { get; private set; }
        public string Player { get; private set; }
        public int Points { get; private set; }

        public GetLatestStatsMessage(int id, string player, int points)
        {
            Id = id;
            Player = player;
            Points = points;
        }

       
    }
}
