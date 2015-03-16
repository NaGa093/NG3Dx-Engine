using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace NG3Dx.Cameras
{
    public enum eProjection
    {
        Perspective = 1,
        Orthographic = 2
    };

    public abstract class Camera
    {
        public Vector3 eye;
        public Vector3 target;
        public Vector3 up;

        protected Matrix view = Matrix.Identity;
        protected Matrix perspective = Matrix.Identity;
        protected Matrix orthographic = Matrix.Identity;
        protected Matrix viewPerspective = Matrix.Identity;

        public eProjection Projections;

        public Matrix View
        {
            get { return view; }
        }

        public void SetPerspective(float fov, float aspect, float znear, float zfar)
        {
            perspective = Matrix.PerspectiveFovLH(fov, aspect, znear, zfar);
        }

        public void SetOrthographic(float width, float height, float znear, float zfar)
        {
            orthographic = Matrix.OrthoLH(width, height, znear, zfar);
        }

        public void SetView()
        {
            view = Matrix.LookAtLH(eye, target, up);
        }

        public void SetView(Vector3 eye, Vector3 target, Vector3 up)
        {
            view = Matrix.LookAtLH(eye, target, up);
        }

        public Matrix Perspective
        {
            get { return perspective; }
        }

        public Matrix Orthographic
        {
            get { return orthographic; }
        }

        public Matrix ViewPerspective
        {
            get { return view * perspective; }
        }

        public Matrix ViewOrthographic
        {
            get { return view * orthographic; }
        }

        public Matrix ViewProjection
        {
            get
            {
                if (Projections == eProjection.Orthographic)
                    return ViewOrthographic;
                else
                    return ViewPerspective;
            }
        }

        public bool dragging = false;
        public int startX = 0;
        public int deltaX = 0;

        public int startY = 0;
        public int deltaY = 0;

        public abstract void MouseUp(object sender, MouseEventArgs e);
        public abstract void MouseDown(object sender, MouseEventArgs e);
        public abstract void MouseMove(object sender, MouseEventArgs e);
        public abstract void MouseWheel(object sender, MouseEventArgs e);

        public abstract void KeyPress(object sender, KeyPressEventArgs e);
        public abstract void KeyDown(object sender, KeyEventArgs e);
        public abstract void KeyUp(object sender, KeyEventArgs e);
    }
}
