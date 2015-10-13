using System.Reflection;
using System.Web.Http;
using AkkaStats.Api;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Factories;
using AkkaStats.Core.Messages;
using AkkaStats.Persistance.Actors;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace AkkaStats.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var container = CreateKernel();
            app.UseAutofacMiddleware(container).UseAutofacWebApi(config);
            WebApiConfig.Register(config);
            app.UseWebApi(config);

        }

        public static IContainer CreateKernel()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<StatsActorSystemService>().As<IStatsActor>().InstancePerRequest();
            builder.RegisterType<ActorSystemFactory>().As<IActorSystemFactory>().InstancePerRequest();
            builder.RegisterType<StatsCoordinatorActor>();
            builder.RegisterType<DbReader<PlayerMessage>>();
            builder.RegisterType<DbWriter<PlayerMessage>>();
            var container = builder.Build();
            return container;
        }
    }
}
