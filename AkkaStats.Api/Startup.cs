using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using AkkaStats.Core;
using AkkaStats.Core.Actors;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AkkaStats.Api.Startup))]

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
            builder.RegisterType<ValidateStatsActor>();
            var container = builder.Build();
            return container;
        }
    }
}
