using System;
using AkkaStats.Persistance.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace AkkaStats.Core.Messages
{

    public class PlayerQuery
    {

        public Guid Id { get; private set; }
        public string Action { get; private set; }

        public PlayerQuery(string action)
        {
            Action = action;
        }

        public PlayerQuery(string action, Guid id) : this(action)
        {
            Id = id;
        }


        public static PlayerQuery Create(string action)
        {
            return new PlayerQuery(action);
        }

        public static PlayerQuery Create(string action, Guid id)
        {
            return new PlayerQuery(action, id);
        }
    }

    public abstract class PlayerBase
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public CRUDState State { get; set; }
    }

    public class HitterMessage : PlayerBase, IMongoEntity, ICommand
    {
        public int Hrs { get; set; }
    }

    public class CreateHitterMessage : ICommand
    {
        public CreateHitterMessage(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
    }

    public class HitHomeRunMessage : ICommand
    {
        public HitHomeRunMessage(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }

    public class PitcherMessage : PlayerBase, IMongoEntity
    {
        public int Wins { get; set; }
    }
}