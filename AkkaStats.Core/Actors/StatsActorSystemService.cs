using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using AkkaStats.Core.Factories;
using AkkaStats.Core.Messages;
using Akka.Persistence.MongoDb;
using MongoDB.Bson.Serialization;

namespace AkkaStats.Core.Actors
{
    public class StatsActors
    {
        public IActorRef statActorRef = ActorRefs.Nobody;
        public IActorRef statCommandActorRef = ActorRefs.Nobody;
    }

    public class StatsActorSystemService : IStatsActor
    {
  
        //private ActorSystem StatsActorSystem;
        private readonly IActorRef statActorRef;
        private readonly IActorRef statCommandActorRef;

        public StatsActorSystemService(StatsActors stats)
        {
            
            /*StatsActorSystem = actorSystemFactory.Create("StatsCoordinatorActor");
            statActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorActor>()
                .WithRouter(new RoundRobinPool(2)), "StatsCoordinatorActor");

            statCommandActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorCommandActor>(), "StatsCoordinatorCommandActor");*/
            statActorRef = stats.statActorRef;
            statCommandActorRef = stats.statCommandActorRef;
        }

        public async Task AddPitcher(PitcherMessage msg)
        {
            msg.State = CRUDState.Create;
            statActorRef.Tell(msg);
        }

        public async Task AddHitter(HitterMessage msg)
        {
            msg.State = CRUDState.Create;
            statActorRef.Tell(msg);

            CreateHitterMessage hitter = new CreateHitterMessage(msg.Id, msg.Name);
            statCommandActorRef.Tell(hitter);
            
            for (int i = 0; i < msg.Hrs; i++)
            {
                HitHomeRunMessage hr = new HitHomeRunMessage(msg.Id);
                statCommandActorRef.Tell(hr);
            }
        }

        public async Task AddHomeRuns(HitterMessage msg)
        {
            for (int i = 0; i < msg.Hrs; i++)
            {
                HitHomeRunMessage hr = new HitHomeRunMessage(msg.Id); ////new Guid("7e8f6bf21ea944d886320c2079951cd6"));
                statCommandActorRef.Tell(hr);
            }
        }

        public async Task<PitcherMessage> GetByPitcherId(string id)
        {
            var request = PlayerQuery.Create("get_pitcher", Guid.Parse(id));
            var result = await statActorRef.Ask<PitcherMessage>(request);
            return result;
        }

        public async Task<HitterMessage> GetByHitterId(string id)
        {
            var request = PlayerQuery.Create("get_hitter", Guid.Parse(id));
            var result = await statActorRef.Ask<HitterMessage>(request);
            return result;
        }

        public async Task<List<PitcherMessage>> GetAllPitchers()
        {
            var request = PlayerQuery.Create("all_pitchers");
            var result = await statActorRef.Ask<List<PitcherMessage>>(request);
            return result.OrderByDescending(x => x.Wins).ToList();
        }

        public async Task<List<HitterMessage>> GetAllHitters()
        {
            var request = PlayerQuery.Create("all_hitters");
            var result = await statActorRef.Ask<List<HitterMessage>>(request);
            return result.OrderByDescending(x => x.Hrs).ToList();
        }

        public async Task DeleteAllPitchers()
        {
            var request = PlayerQuery.Create("delete_pitchers");
            statActorRef.Tell(request);
        }

        public async Task DeleteAllHitters()
        {
            var request = PlayerQuery.Create("delete_hitters");
            statActorRef.Tell(request);
        }

        public async Task DeletePitcherById(string id)
        {
            var request = PlayerQuery.Create("delete_pitcher", Guid.Parse(id));
            statActorRef.Tell(request);
        }

        public async Task DeleteHitterById(string id)
        {
            var request = PlayerQuery.Create("delete_hitter", Guid.Parse(id));
            statActorRef.Tell(request);
        }

        public async Task BulkHitters(List<HitterMessage> list)
        {
            foreach (var item in list)
            {
                item.State = CRUDState.Create;
                statActorRef.Tell(item);
            }
        }

        public async Task BulkPitchers(List<PitcherMessage> list)
        {
            foreach (var item in list)
            {
                item.State = CRUDState.Create;
                statActorRef.Tell(item);
            }
        }
    }
}
