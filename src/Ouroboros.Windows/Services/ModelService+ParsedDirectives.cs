using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ouroboros.Windows.Services
{
    internal partial class ModelService
    {
        private sealed class ParsedDirectives
        {
            // PositionIndex, TextureCoordinateIndex, NormalIndex (all one based)
            public readonly IDictionary<string, IList<Tuple<int, int?, int?>[]>> MaterialFaces =
                new Dictionary<string, IList<Tuple<int, int?, int?>[]>>(StringComparer.OrdinalIgnoreCase);

            public ParsedDirectives(GraphicsDevice graphicsDevice)
            {
                //newmtl Leafs
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.270588 0.407843 0.400000
                //Ks 0.330000 0.330000 0.330000
                Materials["Mesh1 Plate_River Model Mesh_Leafs"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.270588f, 0.407843f, 0.400000f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                //newmtl Stone
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.725490 0.752941 0.788235
                //Ks 0.330000 0.330000 0.330000
                Materials["Mesh1 Plate_River Model Mesh_Stone"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.725490f, 0.752941f, 0.788235f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                //newmtl Water
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.596078 0.898039 0.968627
                //Ks 0.330000 0.330000 0.330000
                Materials["Mesh1 Plate_River Model Mesh_Water"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.596078f, 0.898039f, 0.898039f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                //newmtl Wood
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.666667 0.545098 0.356863
                //Ks 0.330000 0.330000 0.330000
                Materials["Wood"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.666667f, 0.545098f, 0.356863f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                foreach (var basicEffect in Materials.Values)
                    basicEffect.EnableDefaultLighting();
            }

            // Hardcoded for now
            public IDictionary<string, BasicEffect> Materials { get; } = new Dictionary<string, BasicEffect>();

            public IList<Vector3> NormalList { get; } = new List<Vector3>();

            public IList<Vector3> PositionList { get; } = new List<Vector3>();
        }
    }
}