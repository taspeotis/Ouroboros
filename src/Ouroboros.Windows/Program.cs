using Autofac;
using Microsoft.Xna.Framework;

namespace Ouroboros.Windows
{
    public static class Program
    {
        public static void Main()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<GraphicsDeviceManager>().SingleInstance().AutoActivate();
            containerBuilder.RegisterType<OuroborosGame>().As<Game>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<Game>().Content).SingleInstance();
            containerBuilder.Register(c => c.Resolve<Game>().GraphicsDevice).SingleInstance();

            using (var container = containerBuilder.Build())
            {
                container.Resolve<Game>().Run();
            }
        }
    }
}