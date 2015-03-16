using System;
using NG3Dx.Managers;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX;
using System.IO;

namespace NG3Dx.Effects
{
    public class EffectGrid : EffectData
    {
        public EffectGrid() : base()
        {
            elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };

            try
            {
                using (ShaderBytecode effectByteCode = new ShaderBytecode(new DataStream(File.ReadAllBytes("Shaders/grid.hlsl"), false, false)))
                {
                    effect = new Effect(DeviceManager.DirectXDevice.device, effectByteCode);
                    technique = effect.GetTechniqueByIndex(0);
                    pass = technique.GetPassByIndex(0);
                    inputSignature = pass.Description.Signature;
                    effectColor = effect.GetVariableByName("Color").AsVector();
                    effectMatrix = effect.GetVariableByName("WorldViewProjection").AsMatrix();
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
