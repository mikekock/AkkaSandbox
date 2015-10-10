using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using AkkaStats.Core;
using AkkaStats.Core.Messages;
using MongoDB.Driver;

namespace AkkaStats.Api.Controllers
{
    public class ValuesController : ApiController
    {

        private IStatsActor _statsActor;

        public ValuesController(IStatsActor statsActor)
        {
            _statsActor = statsActor;
        }

        // GET api/values
        [HttpGet]
        [Route("api/stats/add/{name}")]
        public async Task<IHttpActionResult> Add(string name)
        {
            var newPlayer = new PlayerMessage() { Id = Guid.NewGuid(), Name = name };
            _statsActor.AddPlayer(newPlayer);

            return Ok(newPlayer);
        }

        [HttpGet]
        [Route("api/stats/{id}")]
        public async Task<IHttpActionResult> Get(string id)
        {
          
            var result = await _statsActor.GetById(id);

            return Ok(result);
        }


        [HttpGet]
        [Route("api/stat")]
        public async Task<IHttpActionResult> GetManyMongo()
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            IMongoCollection<PlayerMessage> mongoCollection = mongoDatabase.GetCollection<PlayerMessage>(typeof(PlayerMessage).Name);
    
            var result = await( await mongoCollection.FindAsync(_ => true)).ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        [Route("api/stat/{id}")]
        public async Task<IHttpActionResult> GetOneMongo(string id)
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            IMongoCollection<PlayerMessage> mongoCollection = mongoDatabase.GetCollection<PlayerMessage>(typeof(PlayerMessage).Name);
            
            var result = await mongoCollection.Find(x => x.Id == Guid.Parse(id)).SingleOrDefaultAsync();

            return Ok(result);
        }

    }
}
