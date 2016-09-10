using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ouroboros.Windows.Attributes;
using Ouroboros.Windows.Contracts;
using Ouroboros.Windows.Services;

[assembly: RegisterScoped(typeof(ModelService), typeof(IModelService))]

namespace Ouroboros.Windows.Services
{
    internal sealed partial class ModelService : IModelService
    {
        private const string CommentDirective = "#";
        private const string FaceDirective = "f";
        private const string NormalDirective = "vn";
        private const string UseMaterialDirective = "usemtl";
        private const string VertexDirective = "v";

        private static readonly IDictionary<string, DirectiveParser> DirectiveParsers =
            new Dictionary<string, DirectiveParser>(StringComparer.OrdinalIgnoreCase)
            {
                {FaceDirective, FaceDirectiveParser},
                {NormalDirective, NormalDirectiveParser},
                {UseMaterialDirective, UseMaterialDirectiveParser},
                {VertexDirective, VertexDirectiveParser}
            };

        private readonly GraphicsDevice _graphicsDevice;
        private readonly ITesselationService _tesselationService;

        public ModelService(GraphicsDevice graphicsDevice, ITesselationService tesselationService)
        {
            _graphicsDevice = graphicsDevice;
            _tesselationService = tesselationService;
        }

        // This is very simplistic
        // We just patch up normals etc. as we go, we apply them to the vertex rather than the face
        // We only parse XYZ not XYZW
        // We also don't do relative indexes
        public Model Load(string modelPath)
        {
            using (var fileStream = File.Open(modelPath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
                return LoadInternal(streamReader);
        }

        private static void FaceDirectiveParser(
            string[] directiveComponents, ParsedDirectives parsedDirectives, ref string currentMaterial)
        {
            if (directiveComponents.Length < 4)
                return;

            var faceTupleIndex = 0;
            var faceTuples = new Tuple<int, int?, int?>[directiveComponents.Length - 1];

            foreach (var directiveComponent in directiveComponents.Skip(1))
            {
                var faceDirectives = directiveComponent.Split('/');

                int vertexIndex;
                int temporaryIndex;

                // TODO: Handle relative indexes

                // Required
                int.TryParse(faceDirectives[0], out vertexIndex);

                faceTuples[faceTupleIndex++] = Tuple.Create(vertexIndex,
                    int.TryParse(faceDirectives[1], out temporaryIndex) ? (int?) temporaryIndex : null,
                    int.TryParse(faceDirectives[2], out temporaryIndex) ? (int?) temporaryIndex : null);
            }

            parsedDirectives.MaterialFaces[currentMaterial].Add(faceTuples);
        }

        private static void NormalDirectiveParser(
            string[] directiveComponents, ParsedDirectives parsedDirectives, ref string currentMaterial)
        {
            if (directiveComponents.Length != 4)
                return;

            float u;
            float v;
            float w;

            float.TryParse(directiveComponents[1], out u);
            float.TryParse(directiveComponents[2], out v);
            float.TryParse(directiveComponents[3], out w);

            if (u*u + v*v + w*w > 0)
                parsedDirectives.NormalList.Add(Vector3.Normalize(new Vector3(u, v, w)));

            // Maybe throw an ArgumentOutOfRangeException if we didn't find a non-zero normal?
        }

        private static void UseMaterialDirectiveParser(
            string[] directiveComponents, ParsedDirectives parsedDirectives, ref string currentMaterial)
        {
            if (directiveComponents.Length < 2)
                return;

            // HACK
            currentMaterial = string.Join(" ", directiveComponents.Skip(1));

            if (!parsedDirectives.MaterialFaces.ContainsKey(currentMaterial))
                parsedDirectives.MaterialFaces.Add(currentMaterial, new List<Tuple<int, int?, int?>[]>());
        }

        private static void VertexDirectiveParser(
            string[] directiveComponents, ParsedDirectives parsedDirectives, ref string currentMaterial)
        {
            if (directiveComponents.Length < 4)
                return;

            float x;
            float y;
            float z;

            float.TryParse(directiveComponents[1], out x);
            float.TryParse(directiveComponents[2], out y);
            float.TryParse(directiveComponents[3], out z);

            parsedDirectives.PositionList.Add(new Vector3(x, y, z));
        }

        private Model LoadInternal(TextReader textReader)
        {
            var parsedDirectives = new ParsedDirectives(_graphicsDevice);
            string currentMaterial = null;

            for (string directive; (directive = textReader.ReadLine()) != null;)
            {
                // This implicitly trims whitespace from the start and end of the line.
                var directiveComponents = directive.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

                if (!directiveComponents.Any() || directiveComponents[0].StartsWith(CommentDirective))
                    continue; // We encountered an empty line, or a comment directive.

                DirectiveParser directiveParser;

                if (DirectiveParsers.TryGetValue(directiveComponents[0], out directiveParser))
                    directiveParser(directiveComponents, parsedDirectives, ref currentMaterial);
            }

            return LoadInternal(parsedDirectives);
        }

        private Model LoadInternal(ParsedDirectives parsedDirectives)
        {
            var modelBone = new ModelBone {Transform = Matrix.Identity};
            var vertexPositionNormalTextures = new List<VertexPositionNormalTexture>();
            var modelMeshParts = new List<ModelMeshPart>();

            _tesselationService.Tesselate();

            foreach (var materialFaces in parsedDirectives.MaterialFaces)
            {
                var basicEffect = parsedDirectives.Materials[materialFaces.Key];
                var faces = materialFaces.Value;
                var vertexIndexes = new List<int>();

                

                foreach (var face in faces)
                {
                    foreach (var faceTuple in face)
                    {
                        var position = parsedDirectives.PositionList[faceTuple.Item1 - 1];
                        // ReSharper disable once PossibleInvalidOperationException
                        var normal = parsedDirectives.NormalList[faceTuple.Item3.Value - 1]; // Take the NRE for now
                        var textureCoordinate = Vector2.Zero; // TODO: This

                        var vertexPositionNormalTexture =
                            new VertexPositionNormalTexture(position, normal, textureCoordinate);

                        // TODO: Don't just duplicate the positions/normals/coordinates. We can be more efficient.
                        vertexPositionNormalTextures.Add(vertexPositionNormalTexture);
                        vertexIndexes.Add(vertexPositionNormalTextures.Count - 1);
                    }
                }

                var indexBuffer = new IndexBuffer(_graphicsDevice,
                    typeof(ushort), vertexIndexes.Count, BufferUsage.None);

                indexBuffer.SetData(vertexIndexes.Select(i => (ushort) i).ToArray());

                modelMeshParts.Add(new ModelMeshPart
                {
                    IndexBuffer = indexBuffer,
                    NumVertices = vertexIndexes.Count,
                    PrimitiveCount = vertexIndexes.Count/3,
                    Tag = basicEffect
                });
            }

            var modelMesh = new ModelMesh(_graphicsDevice, modelMeshParts)
            {
                ParentBone = modelBone
            };

            var vertexBuffer = new VertexBuffer(_graphicsDevice,
                typeof(VertexPositionNormalTexture), vertexPositionNormalTextures.Count, BufferUsage.None);

            vertexBuffer.SetData(vertexPositionNormalTextures.ToArray());

            foreach (var modelMeshPart in modelMeshParts)
            {
                // Patch up the Effect
                modelMeshPart.Effect = (Effect) modelMeshPart.Tag;
                modelMeshPart.Tag = null;

                // Patch up the VertexBuffer
                modelMeshPart.VertexBuffer = vertexBuffer;
            }

            return new Model(_graphicsDevice, new List<ModelBone> {modelBone}, new List<ModelMesh> {modelMesh});
        }

        private delegate void DirectiveParser(
            string[] directiveComponents, ParsedDirectives parsedDirectives, ref string currentMaterial);
    }
}