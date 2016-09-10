using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ouroboros.Windows.Attributes;
using Ouroboros.Windows.Contracts;
using Ouroboros.Windows.Services;

[assembly: RegisterScoped(typeof(RunLoop), typeof(IRunLoop))]

namespace Ouroboros.Windows.Services
{
    internal sealed class RunLoop : IRunLoop
    {
        private static readonly Color ClearColor = new Color(63, 124, 182);

        private readonly GraphicsDevice _graphicsDevice;
        private readonly IModelService _modelService;
        private readonly Model _riverModel;

        public RunLoop(GraphicsDevice graphicsDevice, IModelService modelService)
        {
            _graphicsDevice = graphicsDevice;
            _modelService = modelService;

            _riverModel = _modelService.Load("Assets/Models/Plate_River_01.obj");
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(ClearColor);

            var aspectRatio = _graphicsDevice.Viewport.AspectRatio;

            var rotationY = Matrix.CreateRotationY(2 + (float)gameTime.TotalGameTime.TotalSeconds / 2);
            var world = Matrix.Multiply(rotationY, Matrix.Identity);
            var view = Matrix.CreateLookAt(new Vector3(3, 3, 3), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100);

            _riverModel.Draw(world, view, projection);
        }
    }
}