using System.Collections.Generic;
using System.Threading.Tasks;
using AkkaStats.Core.Messages;

namespace AkkaStats.Core.Actors
{
    public interface IStatsActor
    {
        //Task<string> HollaBack(string msg);
        Task AddPitcher(PitcherMessage msg);
        Task AddHitter(HitterMessage msg);

        Task<PitcherMessage> GetByPitcherId(string id);
        Task<HitterMessage> GetByHitterId(string id);

        Task<List<PitcherMessage>> GetAllPitchers();
        Task<List<HitterMessage>> GetAllHitters();

        Task DeleteAllPitchers();
        Task DeleteAllHitters();

        Task DeletePitcherById(string id);
        Task DeleteHitterById(string id);

        Task BulkHitters(List<HitterMessage> list);
        Task BulkPitchers(List<PitcherMessage> list);
    }
}