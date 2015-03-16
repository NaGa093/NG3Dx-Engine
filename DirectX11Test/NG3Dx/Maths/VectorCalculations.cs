using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG3Dx.Geometrics;
using SlimDX;

namespace NG3Dx.Maths
{
    public class VectorCalculations
    {
        public static Vector3 MiddlePoint(Vector3 startpoint, Vector3 stopPoint)
        {
            return (startpoint + stopPoint) / 2;
        }

        public static Vector3 MiddlePoint(Vertex startpoint, Vertex stopPoint)
        {
            return (startpoint.Position + stopPoint.Position) / 2;
        }

        public static float Distance(Vector3 startpoint, Vector3 stopPoint)
        {
            return (float)(Math.Sqrt(Math.Pow(stopPoint.X - startpoint.X, 2) + Math.Pow(stopPoint.Y - startpoint.Y, 2) + Math.Pow(stopPoint.Z - startpoint.Z, 2)));
        }

        public static float Distance(Vertex startpoint, Vertex stopPoint)
        {
            return (float)(Math.Sqrt(Math.Pow(stopPoint.Position.X - startpoint.Position.X, 2) + Math.Pow(stopPoint.Position.Y - startpoint.Position.Y, 2) + Math.Pow(stopPoint.Position.Z - startpoint.Position.Z, 2)));
        }

        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        public static Vector3 ComputeNormalsOfATriangle(Vector3 pointFirst, Vector3 pointSecond, Vector3 pointThird)
        {
            Vector3 crossOne = Vector3.Cross(pointThird - pointFirst, pointSecond - pointFirst);
            crossOne.Normalize();
            return crossOne;
        }

        public static float AngleBetweenVectors(Vector3 vector1, Vector3 vector2)
        {
            return RadianToDegree((float)Math.Acos(Math.Acos(Vector3.Dot(vector1, vector2) / (vector1.Length() * vector2.Length()))));
        }

        public static float AngleBetweenVectors(Vector3 vector1, Vector3 vector2, Vector3 vector3)
        {
            Vector3 normalVector1 = ComputeNormalsOfATriangle(vector1, vector2, new Vector3(0, 1, 0));
            Vector3 normalVector2 = ComputeNormalsOfATriangle(vector1, vector3, new Vector3(0, 1, 0));

            return AngleBetweenVectors(normalVector1, normalVector2);
        }

    }
}
