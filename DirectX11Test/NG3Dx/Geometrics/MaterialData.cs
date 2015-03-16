using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Runtime.InteropServices;

namespace NG3Dx.Geometrics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MaterialData
    {
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Color4 Reflect;
        public float TransmissionFilter;
        public float Exponent;
        public float Sharpness;
        public float OpticalDensity;
        public static int Stride = Marshal.SizeOf(typeof(MaterialData));
    }
}
