using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NG3Dx.Cameras;
using SlimDX;

namespace NG3Dx.Managers
{
    public class CameraManager
    {
        private static OrbitPanCamera orbitPanCamera;

        public CameraManager()
        {
            orbitPanCamera = new OrbitPanCamera();
            orbitPanCamera.Projections = eProjection.Perspective;
        }

        public static OrbitPanCamera OrbitPanCamera
        {
            get
            {
                return orbitPanCamera;
            }
        }
    }
}
