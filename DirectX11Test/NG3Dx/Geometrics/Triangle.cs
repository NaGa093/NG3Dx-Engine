using SlimDX;

namespace NG3Dx.Geometrics
{
    public class Triangle
    {
        public Vertex point1;
        public Vertex point2;
        public Vertex point3;


        public Triangle()
        {

        }

        public Triangle(Vertex p1, Vertex p2, Vertex p3)
        {
            point1 = p1;
            point2 = p2;
            point3 = p3;
        }

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            point1 = new Vertex(p1, new Vector3(), new Vector2());
            point2 = new Vertex(p2, new Vector3(), new Vector2());
            point3 = new Vertex(p3, new Vector3(), new Vector2());
        }
    }

}
