using System.Runtime.InteropServices;
using NG3Dx.Cameras;
using NG3Dx.Effects;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Models
{
    public abstract class ModelBase
    {
        protected int Vertices;
        protected int Indices;
        protected int VertexStride;
        protected int IndexStride;

        protected Buffer VertexBuffer;
        protected Buffer IndexBuffer;
        protected VertexBufferBinding VertexBufferBinding;

        protected EffectBase EffectBase;
        protected Matrix Transform;
        protected Color4 Color;

        protected ModelBase()
        {
            Vertices = 0; 
            Indices = 0;
            VertexStride = Vertex.SizeOfVertexInBytes;
            IndexStride = Marshal.SizeOf(typeof(uint));
            Transform = Matrix.Identity;
            Color = new Color4(1, 1, 1);
        }

        public abstract void Render(DeviceContext context, CameraBase camera);
    }
}
