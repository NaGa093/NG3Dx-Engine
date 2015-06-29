using System;
using SlimDX;

namespace NG3Dx.Cameras
{
    public class CameraFps : CameraBase
    {
        private float _pitchVal;

        public override void Strafe(int val)
        {
            var axis = Vector3.Cross(Look, Up);
            var scale = Matrix.Scaling(10.0f, 10.0f, 10.0f);
            axis = Vector3.TransformCoordinate(axis, scale);

            Eye = val > 0 ? Eye + axis : Eye - axis;

            Target = Eye + Look;
            SetView(Eye, Target, Up);
        }

        public override void Move(int val)
        {
            var tempLook = Look;
            var scale = Matrix.Scaling(10.0f, 10.0f, 10.0f);
            tempLook = Vector3.TransformCoordinate(tempLook, scale);

            Eye = val > 0 ? Eye + tempLook : Eye - tempLook;

            Target = Eye + Look;
            SetView(Eye, Target, Up);
        }

        public override void Yaw(double x)
        {
            var rot = Matrix.RotationY((float)x / 100.0f);
            Look = Vector3.TransformCoordinate(Look, rot);

            Target = Eye + Look;
            SetView(Eye, Target, Up);
        }

        public override void Pitch(double y)
        {
            var axis = Vector3.Cross(Up, Look);
            var rotation = (float)y / 100.0f;
            _pitchVal = _pitchVal + rotation;

            const float halfPi = (float)Math.PI / 2.0f;

            if (_pitchVal < -halfPi)
            {
                _pitchVal = -halfPi;
                rotation = 0;
            }
            if (_pitchVal > halfPi)
            {
                _pitchVal = halfPi;
                rotation = 0;
            }

            var rot = Matrix.RotationAxis(axis, rotation);

            Look = Vector3.TransformCoordinate(Look, rot);

            Look.Normalize();

            Target = Eye + Look;
            SetView(Eye, Target, Up);
        }
    }
}
