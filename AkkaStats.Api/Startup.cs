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

[assembly: OwinStartup(typeof(Startup))]

namespace AkkaStats.Api
{
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
        }

        public static IContainer CreateKernel()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<StatsActorSystemService>().As<IStatsActor>().InstancePerRequest();
            builder.RegisterType<ActorSystemFactory>().As<IActorSystemFactory>().InstancePerRequest();
            builder.RegisterType<HubMessageService>().As<IHubMessageService>().SingleInstance();
            builder.RegisterType<StatsCoordinatorActor>();
            builder.RegisterType<DbReader<PitcherMessage>>();
            builder.RegisterType<DbWriter<PitcherMessage>>();
            builder.RegisterType<DbReader<HitterMessage>>();
            builder.RegisterType<DbWriter<HitterMessage>>();
            var container = builder.Build();
            return container;
        }
    }
}
