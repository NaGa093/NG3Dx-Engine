using SlimDX;
using System.Runtime.InteropServices;

namespace NG3Dx.Ligths
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DirectionalLight
    {
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Vector3 Direction;
        public float Pad;

        public static int Stride = Marshal.SizeOf(typeof(DirectionalLight));
    }
}
