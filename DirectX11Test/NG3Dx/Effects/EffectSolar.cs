using NG3Dx.Managers;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX;
using System.IO;
using System;

namespace NG3Dx.Effects
{
    public class EffectSolar : EffectData
    {
        public EffectVectorVariable effectLightPosition;
        public EffectScalarVariable effectLightIntensity;
        public EffectScalarVariable effectLightRadius;
        
        public EffectSolar() : base()
        {
            elements = new[] 
            { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0)
            };

            try
            {
                using (ShaderBytecode effectByteCode = new ShaderBytecode(new DataStream(File.ReadAllBytes("Shaders/model.hlsl"), false, false)))
                {
                    effect = new Effect(DeviceManager.DirectXDevice.device, effectByteCode);
                    technique = effect.GetTechniqueByIndex(0);
                    pass = technique.GetPassByIndex(0);
                    inputSignature = pass.Description.Signature;
                    effectLightPosition = effect.GetVariableByName( "LightPosition" ).AsVector();
                    effectLightIntensity = effect.GetVariableByName("LightIntensity").AsScalar();
                    effectLightRadius = effect.GetVariableByName("LightRadius").AsScalar();
                    technique = effect.GetTechniqueByName("Render");
                    layout = new InputLayout(DeviceManager.DirectXDevice.device, inputSignature, elements);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
