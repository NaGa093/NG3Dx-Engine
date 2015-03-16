using SlimDX;
using SlimDX.Direct3D11;
using System;
using System.Runtime.InteropServices;

namespace NG3Dx.Geometrics
{
    public enum eIllumination
    {
        ColorOnAndAmbientOff = 0,
        ColorOnAndAmbientOn = 1,
        HighlightOn = 2,
        ReflectionOnAndRayTraceOn = 3,
        TransparencyGlassOnReflectionRayTraceOn = 4,
        ReflectionFresnelOnAndRayTraceOn = 5,
        TransparencyRefractionOnReflectionFresnelOffAndRayTraceOn = 6,
        TransparencyRefractionOnReflectionFresnelOnAndRayTraceOn = 7,
        ReflectionOnAndRayTraceOff = 8,
        TransparencyGlassOnReflectionRayTraceOff = 9,
        CastsShadowsOntoInvisibleSurfaces = 10
    }

    public enum eMap
    {
        map_Ka = 0,
        map_Kd = 1,
        map_Ks = 2,
        map_Ns = 3,
        map_d = 4,
        map_bump = 5
    }

    
    public class Material
    {
        public String Name;
        public eIllumination Illumination;
        public eMap Map;
        public Texture2D Texture;
        public MaterialData data;

        public Material()
        {
            Illumination = eIllumination.HighlightOn;
            Map = eMap.map_Kd;
            Texture = null;
        }

        public static eIllumination ConvertIntegerToIllumition(int Index)
        {
            switch (Index)
            {
                case 0:
                    return eIllumination.ColorOnAndAmbientOff;
                case 1:
                    return eIllumination.ColorOnAndAmbientOn;
                case 2:
                    return eIllumination.HighlightOn;
                case 3:
                    return eIllumination.ReflectionOnAndRayTraceOn;
                case 4:
                    return eIllumination.TransparencyGlassOnReflectionRayTraceOn;
                case 5:
                    return eIllumination.ReflectionFresnelOnAndRayTraceOn;
                case 6:
                    return eIllumination.TransparencyRefractionOnReflectionFresnelOffAndRayTraceOn;
                case 7:
                    return eIllumination.TransparencyRefractionOnReflectionFresnelOnAndRayTraceOn;
                case 8:
                    return eIllumination.ReflectionOnAndRayTraceOff;
                case 9:
                    return eIllumination.TransparencyGlassOnReflectionRayTraceOff;
                case 10:
                    return eIllumination.CastsShadowsOntoInvisibleSurfaces;
            }
            return eIllumination.HighlightOn;
        }

        public bool SetMap(string map)
        {
            switch (map)
            {
                case "map_Ka":
                    {
                        Map = eMap.map_Ka;
                        return true;
                    }
                case "map_Kd":
                    {
                        Map = eMap.map_Kd;
                        return true;
                    }
                case "map_Ks":
                    {
                        Map = eMap.map_Ks;
                        return true;
                    }
                case "map_Ns":
                    {
                        Map = eMap.map_Ns;
                        return true;
                    }
                case "map_d":
                    {
                        Map = eMap.map_d;
                        return true;
                    }
                case "map_bump":
                    {
                        Map = eMap.map_bump;
                        return true;
                    }
            }
            return false;
        }
    }
}
