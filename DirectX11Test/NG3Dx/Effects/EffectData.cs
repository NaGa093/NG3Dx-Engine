using System;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace NG3Dx.Effects
{
    public class EffectData : IDisposable
    {
        public ShaderSignature inputSignature;
        public EffectTechnique technique;
        public EffectPass pass;
        public Effect effect;
        public InputLayout layout;
        public EffectVectorVariable effectColor;
        public EffectMatrixVariable effectMatrix;
        public EffectResourceVariable effectTexture;
        public EffectVariable effectMaterial;
        public EffectVariable effectDirectionalLight;
        public EffectVectorVariable effectEyePosition;

        protected InputElement[] elements;

        public EffectData()
        { 
        }

        public void Dispose()
        {
            layout.Dispose();
            effect.Dispose();
        }
    }
}
