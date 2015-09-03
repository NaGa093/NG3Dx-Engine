using System;
using System.Collections.Generic;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX;
using System.IO;
using Device = SlimDX.Direct3D11.Device;

namespace NG3Dx.Effects
{
    public class EffectModel : EffectBase
    {
        public EffectResourceVariable Texture { get; set; }

        public EffectModel(Device device)
        {
            try
            {
                using (var effectByteCode = new ShaderBytecode(new DataStream(File.ReadAllBytes("Shaders/model.hlsl"), false, false)))
                {
                    Effect = new Effect(device, effectByteCode);
                    EffectTechnique = Effect.GetTechniqueByIndex(0);
                    EffectPass = EffectTechnique.GetPassByIndex(0);
                    ShaderSignature = EffectPass.Description.Signature;
                    EffectVectorVariable = Effect.GetVariableByName("Color").AsVector();
                    EffectMatrixVariable = Effect.GetVariableByName("WorldViewProjection").AsMatrix();
                    EffectTechnique = Effect.GetTechniqueByName("Render");
                    InputLayout = new InputLayout(device, ShaderSignature, InputElements);
                }
                Texture = Effect.GetVariableByName("texColorMap").AsResource();  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
