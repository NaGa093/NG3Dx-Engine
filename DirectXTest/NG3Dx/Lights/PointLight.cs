using System.Runtime.InteropServices;
using SlimDX;

namespace NG3Dx.Lights
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLight
    {
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Vector3 Position;
        public float Range;
        public Vector3 Attenuation;
        public float Pad;
    }
}
