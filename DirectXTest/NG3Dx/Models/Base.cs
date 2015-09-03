using NG3Dx.Cameras;
using NG3Dx.Effects;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Models
{
    public abstract class Base
    {
        protected int Vertices;

        protected Buffer VertexBuffer;
        protected VertexBufferBinding VertexBufferBinding;

        protected EffectBase Effectbase;
        public Matrix Transform;

        protected Base()
        {
            Vertices = 0; 
            Transform = Matrix.Identity;
        }

        public abstract void Render(DeviceContext context, CameraBase camera);
        public abstract void SetColor(Color4 color);
    }
}
