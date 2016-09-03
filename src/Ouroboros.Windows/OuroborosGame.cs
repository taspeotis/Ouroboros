using Microsoft.Xna.Framework;

namespace Ouroboros.Windows
{
    internal sealed class OuroborosGame : Game
    {
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(63, 124, 182));

            base.Draw(gameTime);
        }
    }
}