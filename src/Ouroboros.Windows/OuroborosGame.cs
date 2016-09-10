using Autofac;
using Microsoft.Xna.Framework;
using Ouroboros.Windows.Contracts;

namespace Ouroboros.Windows
{
    internal sealed class OuroborosGame : Game
    {
        private readonly ILifetimeScope _lifetimeScope;

        private IRunLoop _runLoop;

        public OuroborosGame(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        protected override void BeginRun()
        {
            base.BeginRun();

            _runLoop = _lifetimeScope.Resolve<IRunLoop>();
        }

        protected override void EndRun()
        {
            _runLoop = null;

            base.EndRun();
        }

        protected override void Draw(GameTime gameTime)
        {
            _runLoop.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}