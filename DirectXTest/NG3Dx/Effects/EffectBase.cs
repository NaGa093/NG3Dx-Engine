using System;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace NG3Dx.Effects
{
    public abstract class EffectBase : IDisposable
    {
        protected Effect Effect;
        protected EffectPass EffectPass;
        public InputLayout InputLayout;
        public EffectTechnique EffectTechnique;
        protected ShaderSignature ShaderSignature;
        public EffectVectorVariable EffectVectorVariable;
        public EffectMatrixVariable EffectMatrixVariable;
        protected InputElement[] InputElements;

        protected EffectBase()
        {
            InputElements = new[] 
            { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0), 
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0, InputClassification.PerVertexData, 0) 
            };
        }

        public void Dispose()
        {
            InputLayout.Dispose();
            Effect.Dispose();
        }
    }
}
