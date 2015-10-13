using System;

namespace AkkaStats.Persistance.Interfaces
{
    public interface IMongoEntity
    {
        Guid Id { get; }
    }
}
