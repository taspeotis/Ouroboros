using Microsoft.Xna.Framework.Graphics;

namespace Ouroboros.Windows.Contracts
{
    public interface IModelService
    {
        Model Load(string modelPath);
    }
}