using System.Reflection;
using System.Web.Http;
using AkkaStats.Api;
using AkkaStats.Api.Hubs;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Factories;
using AkkaStats.Core.Messages;
using AkkaStats.Persistance.Actors;
using AkkaStats.Persistance.Interfaces;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Hosting;
using System;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Akka.Actor;
using Akka.Routing;
using Akka.DI.Core;
using Akka.Persistence.MongoDb;
using Akka.Configuration;
using MongoDB.Bson.Serialization;
using AkkaStats.Core;
using AkkaStats.Core.Events;

[assembly: OwinStartup(typeof(Startup))]

namespace AkkaStats.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:8080"))
            {
                Console.WriteLine("Web Server is running.");
                Console.WriteLine("Press any key to quit.");
                Console.ReadLine();
            }
        }
    }

    

    public class Startup
    {
        private BackgroundTicker _backgroundTicker;

        public void Configuration(IAppBuilder app)
        {
            
            var config = new HttpConfiguration();
            var container = CreateKernel();

            
            _backgroundTicker = new BackgroundTicker(container.Resolve<IHubMessageService>());

            app.UseAutofacMiddleware(container).UseAutofacWebApi(config);
            app.MapSignalR();
            WebApiConfig.Register(config);
            app.UseWebApi(config);

            var physicalFileSystem = new PhysicalFileSystem(@"..");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html"
            };

            app.UseFileServer(options);

            ActorSystem StatsActorSystem = container.Resolve<IActorSystemFactory>().Create("StatsCoordinatorActor");
            MongoDbPersistence.Instance.Apply(StatsActorSystem);
            BsonClassMap.RegisterClassMap<CreateHitterMessage>(cm =>
            {
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<HitHomeRunMessage>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<HomeRunHitEvent>(cm =>
            {
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<HitterAddedEvent>(cm =>
            {
                cm.AutoMap();
            });


            StatsActors stats = container.Resolve<StatsActors>();
            stats.statActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorActor>()
                .WithRouter(new RoundRobinPool(2)), "StatsCoordinatorActor");
            stats.statCommandActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorCommandActor>(), "StatsCoordinatorCommandActor");
            stats.statViewActorRef = StatsActorSystem.ActorOf(StatsActorSystem.DI().Props<StatsCoordinatorViewActor>(), "StatsCoordinatorViewActor");

        }

        public static IContainer CreateKernel()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterInstance<ActorSystem>(ActorSystem.Create("StatsCoordinatorActor", ConfigurationFactory.Load()));
            builder.RegisterType<StatsActorSystemService>().As<IStatsActor>().InstancePerRequest();
            builder.RegisterType<ActorSystemFactory>().As<IActorSystemFactory>().SingleInstance();
            builder.RegisterType<HubMessageService>().As<IHubMessageService>().SingleInstance();
            builder.RegisterType<StatsActors>().SingleInstance();
            builder.RegisterType<StatsCoordinatorActor>();
            builder.RegisterType<StatsCoordinatorCommandActor>();
            builder.RegisterType<StatsCoordinatorViewActor>();
            builder.RegisterType<DbReader<PitcherMessage>>();
            builder.RegisterType<DbWriter<PitcherMessage>>();
            builder.RegisterType<DbReader<HitterMessage>>();
            builder.RegisterType<DbWriter<HitterMessage>>();
            var container = builder.Build();
            return container;
        }
    }
}
