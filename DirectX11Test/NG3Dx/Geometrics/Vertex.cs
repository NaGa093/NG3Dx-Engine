using System.Runtime.InteropServices;
using SlimDX;

namespace NG3Dx.Geometrics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextPos;

        public static int SizeOfVertexInBytes = Marshal.SizeOf(typeof(Vertex));

        public Vertex(Vector3 position, Vector3 normal, Vector2 textposition)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextPos = textposition;
        }
    }
}
