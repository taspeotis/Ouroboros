using System;
using System.Runtime.InteropServices;
using Ouroboros.Windows.Attributes;
using Ouroboros.Windows.Contracts;
using Ouroboros.Windows.Services;

[assembly: RegisterScoped(typeof(TesselationService), typeof(ITesselationService))]

namespace Ouroboros.Windows.Services
{
    internal sealed class TesselationService : ITesselationService
    {
        public void Tesselate()
        {
            // if == 3
            // just return

            var tessellation = NewTessellation();

            if (tessellation == IntPtr.Zero)
                throw new OutOfMemoryException();

            try
            {
                // set up callback
                Callback(tessellation, TessellationCallback.GLU_TESS_VERTEX, TestCallback);

                // Begin can just be the one call
                BeginPolygon(tessellation, IntPtr.Zero);
                BeginContour(tessellation);

                TesellateVertex(tessellation, new double[] {0, 2, 0}, (IntPtr) 1);
                TesellateVertex(tessellation, new double[] {1, 1, 0}, (IntPtr) 2);
                TesellateVertex(tessellation, new double[] {2, 2, 0}, (IntPtr) 3);
                TesellateVertex(tessellation, new double[] {1, 0, 0}, (IntPtr) 4);

                // End can be one call
                EndContour(tessellation);
                EndPolygon(tessellation);
            }
            finally
            {
                DeleteTesellation(tessellation);
            }
        }

        private void TestCallback(IntPtr callbackdata)
        {
            
        }

        // TODO: A tesselation class that news a tesselation object up
        // Implements IDisposable

        [DllImport("glu32.dll", EntryPoint = "gluTessBeginPolygon")]
        private static extern void BeginPolygon(IntPtr tessellation, IntPtr userData);

        [DllImport("glu32.dll", EntryPoint = "gluTessBeginContour")]
        public static extern void BeginContour(IntPtr tessellation);

        [DllImport("glu32.dll", EntryPoint = "gluTessVertex")]
        public static extern void TesellateVertex(IntPtr tessellation, double[] vertexPoints, IntPtr vertexData);

        [DllImport("glu32.dll", EntryPoint = "gluTessEndContour")]
        public static extern void EndContour(IntPtr tessellation);

        [DllImport("glu32.dll", EntryPoint = "gluTessEndPolygon")]
        private static extern void EndPolygon(IntPtr tessellation);

        [DllImport("glu32.dll", EntryPoint = "gluDeleteTess")]
        private static extern void DeleteTesellation(IntPtr tessellation);

        [DllImport("glu32.dll", EntryPoint = "gluNewTess")]
        private static extern IntPtr NewTessellation();

        [DllImport("glu32.dll", EntryPoint = "gluTessCallback")]
        private static extern void Callback(IntPtr tessellation, TessellationCallback tessellationCallback, VertexCallback vertexCallback);

        private delegate void VertexCallback(IntPtr vertexData);

        [Flags]
        public enum TessellationCallback : uint
        {
            GLU_TESS_BEGIN = 100100,
            GLU_TESS_VERTEX = 100101,
            GLU_TESS_END = 100102,
            GLU_TESS_ERROR = 100103,
            GLU_TESS_EDGE_FLAG = 100104,
            GLU_TESS_COMBINE = 100105,
            GLU_TESS_BEGIN_DATA = 100106,
            GLU_TESS_VERTEX_DATA = 100107,
            GLU_TESS_END_DATA = 100108,
            GLU_TESS_ERROR_DATA = 100109,
            GLU_TESS_EDGE_FLAG_DATA = 100110,
            GLU_TESS_COMBINE_DATA = 100111,
        }
    }
}