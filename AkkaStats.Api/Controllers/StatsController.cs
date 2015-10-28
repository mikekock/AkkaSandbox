using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Messages;

namespace AkkaStats.Api.Controllers
{
    public class StatsController : ApiController
    {

        private readonly IStatsActor _statsActor;

        public StatsController(IStatsActor statsActor)
        {
            _statsActor = statsActor;
        }

        /// <summary>
        /// Get An Records Array
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/stats/hitter", Name = "GetAllHitters")]
        public async Task<IHttpActionResult> GetAllHitters()
        {
            var results = await _statsActor.GetAllHitters();
            if (results == null) return NotFound();
            // Used to make a simple link from the json object to the GetById Route
            foreach (var result in results)
            {
                result.Url = String.Format("{0}{1}", "http://localhost:49781", Url.Route("GetByHitterId", new { id = result.Id }));
            }
            return Ok(results);
        }

        [HttpGet]
        [Route("api/stats/pitcher", Name = "GetAllPitchers")]
        public async Task<IHttpActionResult> GetAllPitchers()
        {
            var results = await _statsActor.GetAllPitchers();
            if (results == null) return NotFound();
            // Used to make a simple link from the json object to the GetById Route
            foreach (var result in results)
            {
                result.Url = String.Format("{0}{1}", "http://localhost:49781", Url.Route("GetPitcherById", new { id = result.Id }));
            }
            return Ok(results);
        }

        /// <summary>
        /// Add an entity and get the record back
        /// </summary>
        [HttpPost]
        [Route("api/stats/hitter", Name = "AddHitter")]
        public async Task<IHttpActionResult> Add([FromBody] HitterMessage vm)
        {
            vm.Id = Guid.NewGuid();
            //await _statsActor.AddHitter(vm);
            await _statsActor.AddHomeRuns(vm);
            return Ok(vm);
        }


        [HttpPost]
        [Route("api/stats/pitcher", Name = "AddPitcher")]
        public async Task<IHttpActionResult> AddPitcher([FromBody] PitcherMessage vm)
        {
            vm.Id = Guid.NewGuid();
            await _statsActor.AddPitcher(vm);
            return Ok(vm);
        }

        /// <summary>
        /// Get an entity by its identity
        /// </summary>
        [HttpGet]
        [Route("api/stats/hitter/{id}", Name = "GetByHitterId")]
        public async Task<IHttpActionResult> GetHitterById(string id)
        {
            var result = await _statsActor.GetByHitterId(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/stats/pitcher/{id}", Name = "GetPitcherById")]
        public async Task<IHttpActionResult> GetPitcherById(string id)
        {
            var result = await _statsActor.GetByPitcherId(id);
            return Ok(result);
        }

        /// <summary>
        /// Delete all existing records
        /// </summary>
        [HttpGet]
        [Route("api/stats/hitter/delete", Name = "DeleteHitters")]
        public async Task<IHttpActionResult> DeleteHitters()
        {
            await _statsActor.DeleteAllHitters();
            return Ok(true);
        }

        [HttpGet]
        [Route("api/stats/pitcher/delete", Name = "DeletePlayers")]
        public async Task<IHttpActionResult> DeletePlayers()
        {
            await _statsActor.DeleteAllHitters();
            return Ok(true);
        }

        [HttpDelete]
        [Route("api/stats/hitter/{id}", Name = "DeleteById")]
        public async Task<IHttpActionResult> DeleteById(string id)
        {
            await _statsActor.DeleteHitterById(id);
            return Ok(true);
        }

        [HttpDelete]
        [Route("api/stats/pitcher/{id}", Name = "DeletePitcherById")]
        public async Task<IHttpActionResult> DeletePitcherById(string id)
        {
            await _statsActor.DeletePitcherById(id);
            return Ok(true);
        }

        /// <summary>
        /// Bulk in many players
        /// </summary>
        [HttpGet]
        [Route("api/stats/bulk", Name = "Bulk")]
        public async Task<IHttpActionResult> Bulk()
        {
            var listOfHitters = new List<HitterMessage>()
            {
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Barry Bonds" },              
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Brandon Belt" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Brandon Crawford" },              
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Bruce Bochey" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Buster Posey" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Cody Ross" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Edger Renteria" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Hunter Pence" },      
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Joe Panik" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Kevin Mitchell" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Marco Scutaro" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Matt Duffy" },                             
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Matt Williams" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Pablo Sandoval" },
                 new HitterMessage { Id = Guid.NewGuid(), Name = "Will Clark" }         
            };

            var listOfPitchers = new List<PitcherMessage>()
            {              
                 new PitcherMessage() { Id = Guid.NewGuid(), Name = "Barry Zito" },            
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Brian Wilson" },           
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Javier Lopez" },
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Jake Peavy" },
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Jeremy Affeldt" },                
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Madision Bumgardner" },           
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Matt Cain" },     
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Ryan Vogelsong" },
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Sergio Romo" },
                 new PitcherMessage { Id = Guid.NewGuid(), Name = "Tim Lincecum" }
                   
            };



            await _statsActor.BulkHitters(listOfHitters);

            return Ok(listOfHitters);
        }


    }
}
