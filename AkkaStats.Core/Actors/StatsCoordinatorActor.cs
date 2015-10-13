using System;
using System.Configuration;
using System.Diagnostics;
using Akka.Actor;
using Akka.DI.Core;
using AkkaStats.Core.Messages;
using MongoDB.Driver;

namespace AkkaStats.Core.Actors
{

    public enum DbRequestType
    {
        GetAll, GetById, Add, DeleteOne, DeleteMany
    }

    public class DbRequest
    {
        public DbRequestType Query { get; set; }
        public Guid Id { get; set; }
    }

    public class DbReader<T> : ReceiveActor where T : IMongoEntity
    {
        private readonly IMongoCollection<T> _mongoCollection;

        public DbReader()
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);

            Receive<DbRequest>(GetMany, x => x.Query == DbRequestType.GetAll);
            Receive<DbRequest>(GetOne, x => x.Query == DbRequestType.GetById);
        }

        private void GetMany(DbRequest request)
        {
             Debug.WriteLine("Calling DbReader GetMany");
            _mongoCollection.Find(_ => true).ToListAsync().PipeTo(Sender);
        }

        private void GetOne(DbRequest request)
        {
            Debug.WriteLine("Calling DbReader GetOne");
            _mongoCollection.Find(x => x.Id.Equals(request.Id)).SingleOrDefaultAsync().PipeTo(Sender);
        }

    }


    public class DbWriter<T> : ReceiveActor where T : IMongoEntity
    {
        private readonly IMongoCollection<T> _mongoCollection;

        public DbWriter()
        {
            IMongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnection"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDb"));
            _mongoCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);

            Receive<T>(x => Add(x));
            Receive<DbRequest>(x => DeleteMany(x), x => x.Query == DbRequestType.DeleteMany);
            Receive<DbRequest>(x => DeleteOne(x), x => x.Query == DbRequestType.DeleteOne);
        }

        private void Add(T obj)
        {
            Debug.WriteLine("Add using the DbWriter");
            _mongoCollection.InsertOneAsync(obj);
        }

        private void DeleteOne(DbRequest request)
        {
            Debug.WriteLine("DeleteOne using the DbWriter");
            _mongoCollection.DeleteOneAsync(x => x.Id.Equals(request.Id));
        }

        private void DeleteMany(DbRequest request)
        {
            Debug.WriteLine("DeleteMany using the DbWriter");
            _mongoCollection.DeleteManyAsync(_ => true);
        }

    }

    public class StatsCoordinatorActor : ReceiveActor
    {
        private readonly IActorRef _dbWriters;
        private readonly IActorRef _dbReaders;

        public StatsCoordinatorActor()
        {

            _dbWriters = Context.ActorOf(Context.DI().Props<DbWriter<PlayerMessage>>(), "DbWriter");
            _dbReaders = Context.ActorOf(Context.DI().Props<DbReader<PlayerMessage>>(), "DbReader");

            Receive<PlayerMessage>(HandleAddPlayer, x => x.State == State.Create);
            Receive<Guid>(x => HandleGetPlayerById(x));
            Receive<string>(x => HandleGetAllPlayers(x), message => message.Equals("all"));
            Receive<string>(x => HandleDeleteAllPlayers(x), message => message.Equals("delete"));

        }


     

        /// <summary>
        /// Ask: Fire the request for all the players
        /// </summary>
        private void HandleGetAllPlayers(string message)
        {
            Debug.WriteLine("Calling HandleGetAllPlayers");
            var dbRequest = new DbRequest {Query = DbRequestType.GetAll};
            _dbReaders.Ask(dbRequest).PipeTo(Sender);
        }

        /// <summary>
        /// Ask: Fire the request for the player
        /// </summary>
        private void HandleGetPlayerById(Guid id)
        {
            Debug.WriteLine("Calling HandleGetPlayerById");
            var dbRequest = new DbRequest { Query = DbRequestType.GetById, Id = id};
            _dbReaders.Ask(dbRequest).PipeTo(Sender);
        }
   
        /// <summary>
        /// Tell: Add player to database
        /// </summary>
        private void HandleAddPlayer(PlayerMessage message)
        {
            message.State = State.Read;
            Debug.WriteLine(String.Format("Adding {0}", message.Name));
            _dbWriters.Tell(message);
        }

        /// <summary>
        /// Tell: Add player to database
        /// </summary>
        private void HandleDeleteAllPlayers(string message)
        {
            //if (message == "delete") ;
            //_mongoCollection.DeleteManyAsync(_ => true);
        }


    }
}
