using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace NG3Dx.Cameras
{
    public class OrbitPanCamera : Camera
    {
        private float rotOrtho = 0;
        private float rotY = 0;
        private float maxZoom = 3.0f;

        public OrbitPanCamera()
        {
            
        }

        public void rotateY(int value)
        {
            rotY = (value / 100.0f);
            Vector3 eyeLocal = eye - target;

            Matrix rotMat = Matrix.RotationY(rotY);
            eyeLocal = Vector3.TransformCoordinate(eyeLocal, rotMat);
            eye = eyeLocal + target;

            SetView(eye, target, up);
        }

        public void rotateOrtho(int value)
        {
            Vector3 viewDir = target - eye;
            Vector3 orhto = Vector3.Cross(viewDir, up);

            rotOrtho = (value / 100.0f);
            Matrix rotOrthoMat = Matrix.RotationAxis(orhto, rotOrtho);

            Vector3 eyeLocal = eye - target;
            eyeLocal = Vector3.TransformCoordinate(eyeLocal, rotOrthoMat);
            Vector3 newEye = eyeLocal + target;
            Vector3 newViewDir = target - newEye;
            float cosAngle = Vector3.Dot(newViewDir, up) / (newViewDir.Length() * up.Length());
            if (cosAngle < 0.999f && cosAngle > -0.999f)
            {
                eye = eyeLocal + target;
                SetView(eye, target, up);
            }
        }

        public void panX(int value)
        {
            float scaleFactor = 0.0f;
            if (value > 1)
            {
                scaleFactor = -0.05f;
            }
            else if (value < -1)
            {
                scaleFactor = 0.05f;
            }
            Vector3 viewDir = target - eye;
            Vector3 orhto = Vector3.Cross(viewDir, up);
            orhto.Normalize();
            scaleFactor = scaleFactor * (float)Math.Sqrt(viewDir.Length()) * 0.5f;
            Matrix scaling = Matrix.Scaling(scaleFactor, scaleFactor, scaleFactor);
            orhto = Vector3.TransformCoordinate(orhto, scaling);

            target = target + orhto;
            eye = eye + orhto;
            SetView(eye, target, up);
        }

        public void panY(int value)
        {
            float scaleFactor = 0.00f;
            if (value > 1)
            {
                scaleFactor = -0.05f;
            }
            else if (value < -1)
            {
                scaleFactor = 0.05f;
            }
            Vector3 viewDir = target - eye;
            scaleFactor = scaleFactor * (float)Math.Sqrt(viewDir.Length()) * 0.5f;
            viewDir.Y = 0.0f;
            viewDir.Normalize();
            Matrix scaling = Matrix.Scaling(scaleFactor, scaleFactor, scaleFactor);
            viewDir = Vector3.TransformCoordinate(viewDir, scaling);

            target = target + viewDir;
            eye = eye + viewDir;
            SetView(eye, target, up);
        }

        public void zoom(int value)
        {
            Vector3 viewDir = eye - target;

            float scaleFactor = 1.0f;
            if (value > 0)
            {
                scaleFactor = 100.1f;
            }
            else
            {
                if (viewDir.Length() > maxZoom)
                    scaleFactor = 90.0f;
            }

            Matrix scale = Matrix.Scaling(scaleFactor, scaleFactor, scaleFactor);
            viewDir.Normalize();
            viewDir = Vector3.TransformCoordinate(viewDir, scale);
            if (value < 0)
            {
                eye = eye + viewDir;
            }
            else
            {
                eye = eye - viewDir;
            }

            SetView(eye, target, up);
        }



        public override void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            dragging = false;
        }

        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            dragging = true;
            startX = e.X;
            startY = e.Y;
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dragging)
            {
                int currentX = e.X;
                deltaX = startX - currentX;
                startX = currentX;

                int currentY = e.Y;
                deltaY = startY - currentY;
                startY = currentY;

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    this.rotateY(-deltaX);
                    this.rotateOrtho(deltaY);
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    this.panX(deltaX);
                    this.panY(deltaY);
                }
            }
        }

        public override void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int delta = e.Delta;
            this.zoom(delta);
        }

        public override void KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        public override void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
        }
    }
}
