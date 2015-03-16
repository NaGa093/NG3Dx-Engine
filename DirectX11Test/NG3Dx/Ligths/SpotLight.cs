using SlimDX;
using System.Runtime.InteropServices;

namespace NG3Dx.Ligths
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpotLight
    {
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Attenuation;
        public float Range;
        public float Spot;
        public float Pad;
    }
}
