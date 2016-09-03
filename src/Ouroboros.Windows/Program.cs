using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Ouroboros.Windows
{
    public static class Program
    {
        public static void Main()
        {
            using (var ouroborosGame = new OuroborosGame())
            using (new GraphicsDeviceManager(ouroborosGame))
                ouroborosGame.Run();
        }
    }
}