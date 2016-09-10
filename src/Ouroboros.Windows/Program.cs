using System.Reflection;
using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ouroboros.Windows.Attributes;

namespace Ouroboros.Windows
{
    public static class Program
    {
        public static void Main()
        {
            var assembly = typeof(Program).Assembly;
            var containerBuilder = new ContainerBuilder();

            foreach (var registerScopedAttribute in assembly.GetCustomAttributes<RegisterScopedAttribute>())
            {
                var interfaceType = registerScopedAttribute.InterfaceType;
                var implementationType = registerScopedAttribute.ImplementationType;

                containerBuilder.RegisterType(implementationType).As(interfaceType).InstancePerLifetimeScope();
            }

            containerBuilder.RegisterType<GraphicsDeviceManager>().SingleInstance().AutoActivate();
            containerBuilder.RegisterType<OuroborosGame>().As<Game, OuroborosGame>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<Game>().Content).SingleInstance();
            containerBuilder.Register(c => c.Resolve<Game>().GraphicsDevice).SingleInstance();

            using (var container = containerBuilder.Build())
            {
                var contentManager = container.Resolve<ContentManager>();
                var ouroborosGame = container.Resolve<OuroborosGame>();

                contentManager.RootDirectory = "Assets";

                ouroborosGame.Run();
            }
        }
    }
}