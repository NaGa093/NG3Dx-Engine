using NG3Dx.Enums;
using SlimDX;

namespace NG3Dx.Cameras
{
    public abstract class CameraBase
    {
        protected Matrix ViewMatrix;
        protected Matrix PerspectiveMatrix;
        protected Matrix OrthographicMatrix;
        protected Matrix ViewPerspectiveMatrix;

        public Projection Projection;
        
        public abstract void Move(int value);
        public abstract void Strafe(int value);
        public abstract void Yaw(int value);
        public abstract void Pitch(int value);

        public Vector3 Eye { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Look { get; set; }

        public Matrix View
        {
            get { return ViewMatrix; }
        }

        public void SetPerspective(float fov, float aspect, float znear, float zfar)
        {
            PerspectiveMatrix = Matrix.PerspectiveFovLH(fov, aspect, znear, zfar);
        }

        public void SetOrthographic(float width, float height, float znear, float zfar)
        {
            OrthographicMatrix = Matrix.OrthoLH(width, height, znear, zfar);
        }

        public void SetView()
        {
            Target = Eye + Look;
            ViewMatrix = Matrix.LookAtLH(Eye, Target, Up);
        }

        public void SetView(Vector3 eye, Vector3 target, Vector3 up)
        {
            ViewMatrix = Matrix.LookAtLH(eye, target, up);
        }

        public Matrix Perspective
        {
            get { return PerspectiveMatrix; }
        }

        public Matrix Orthographic
        {
            get { return OrthographicMatrix; }
        }

        public Matrix ViewPerspective
        {
            get { return ViewMatrix * Perspective; }
        }

        public Matrix ViewOrthographic
        {
            get { return ViewMatrix * Orthographic; }
        }

        public Matrix ViewProjection
        {
            get
            {
                return Projection == Projection.Orthographic ? ViewOrthographic : ViewPerspective;
            }
        }

        protected CameraBase()
        {
            ViewMatrix = Matrix.Identity;
            PerspectiveMatrix = Matrix.Identity;
            OrthographicMatrix = Matrix.Identity;
            ViewPerspectiveMatrix = Matrix.Identity;
        }
    }
}

