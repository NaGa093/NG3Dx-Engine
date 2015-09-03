using System.Runtime.InteropServices;
using SlimDX;

namespace NG3Dx.Lights
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
