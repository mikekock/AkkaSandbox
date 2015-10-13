using System.Collections.Generic;
using System.Threading.Tasks;
using AkkaStats.Core.Messages;

namespace AkkaStats.Core
{
    public interface IStatsActor
    {
        //Task<string> HollaBack(string msg);
        Task AddPlayer(PlayerMessage msg);
        Task<PlayerMessage> GetById(string id);
        Task<List<PlayerMessage>> GetAll();
        Task DeleteAllPlayers();
        Task BulkPlayers(List<PlayerMessage> list);
    }
}