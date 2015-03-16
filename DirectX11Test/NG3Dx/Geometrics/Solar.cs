using NG3Dx.Managers;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Geometrics
{
    public class Solar
    {
        private Vector3 LightPosition;
        private float LightIntensity;
        private float LightRadius;
        private Effects.EffectSolar effectsolar;

        public Solar(Vector3 _LightPosition, float _LightIntensity, float _LightRadius)
        {
            effectsolar = new Effects.EffectSolar();
            SetLightPosition(_LightPosition);
            SetLightIntensity(_LightIntensity);
            SetLightRadius(_LightRadius);
        }

        public void SetLightPosition(Vector3 _LightPosition)
        {
            LightPosition = _LightPosition;
            effectsolar.effectLightPosition.AsVector().Set(LightPosition);
        }

        public void SetLightIntensity(float _LightIntensity)
        {
            LightIntensity = _LightIntensity;
            effectsolar.effectLightIntensity.AsScalar().Set(_LightIntensity);
        }

        public void SetLightRadius(float _LightRadius)
        {
            LightRadius = _LightRadius;
            effectsolar.effectLightRadius.AsScalar().Set(_LightRadius);
        }
        public void Render()
        {

        }
    }
}
