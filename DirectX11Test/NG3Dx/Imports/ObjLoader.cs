using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NG3Dx.Geometrics;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Imports
{
    public struct Face
    { 
        public int VerticeIndex;
        public int NormalIndex;
        public int TextureIndex;

        public Face(int _verticeIndex, int _normalIndex, int _textureIndex)
        {
            VerticeIndex = _verticeIndex;
            NormalIndex = _normalIndex;
            TextureIndex = _textureIndex;
        }
    };

    public struct FaceGroup
    { 
        public List<Face> Faces;

        public FaceGroup(List<Face> _faces)
        {
            Faces = _faces;
        }
    };

    public class ObjLoader
    {
        private static StreamReader _lineStreamReader;
        private static List<Vector3> vertices;
        private static List<Vector3> normals;
        private static List<Vector2> textures;
        private static List<FaceGroup> listFaceGroup;
        private static String path;
        private static List<Material> materials;
        private static Material material;
        private static String modelName;
        private static String groupName;
        private static List<Model> listModel;
        private static bool bSmooth;

        public ObjLoader()
        {
            
        }

        public static List<Model> LoadFile(String directory, String filename) 
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            textures = new List<Vector2>();
            listFaceGroup = new List<FaceGroup>();
            listModel = new List<Model>();
            bSmooth = false;

            _lineStreamReader = new StreamReader(new FileStream(directory + filename, FileMode.Open, FileAccess.Read));
            path = directory;

            while (!_lineStreamReader.EndOfStream)
            {
                ParseLine();
            }

            return listModel;
        }

        private static void ParseLine()
        {
            var currentLine = _lineStreamReader.ReadLine();

            if (string.IsNullOrWhiteSpace(currentLine) || currentLine[0] == '#')
            {
                return;
            }

            string[] fields = currentLine.Trim().Split(null, 2);
            if (fields.Length > 1)
            {
                var keyword = fields[0].Trim();
                var data = fields[1].Trim();
                ParseLine(keyword, data);
            }
        }

        private static void ParseLine(string keyword, string data)
        {
            switch (keyword)
            {
                case "mtllib":
                    {
                        materials = MaterialLibraryLoader.LoadMaterial(path, data);
                    }
                    break;
                case "o":
                    {
                        modelName = data;
                    }
                    break;
                case "g":
                    {
                        groupName = data;
                    }
                    break;
                case "v":
                    {
                        string[] vertice = data.Split(' ');
                        vertices.Add(new Vector3(float.Parse(vertice[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(vertice[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(vertice[2], CultureInfo.InvariantCulture.NumberFormat)));
                    }
                    break;
                case "vt":
                    {
                        string[] texture = data.Split(' ');
                        textures.Add(new Vector2(float.Parse(texture[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(texture[1], CultureInfo.InvariantCulture.NumberFormat) * -1));
                    }
                    break;
                case "vn":
                    {
                        string[] normal = data.Split(' ');
                        normals.Add(new Vector3(float.Parse(normal[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(normal[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(normal[2], CultureInfo.InvariantCulture.NumberFormat)));
                    }
                    break;
                case "usemtl":
                    {
                        if (material!= null && listFaceGroup.Count > 0)
                        {
                            Model model = new Model(GetListTriangle());
                            model.SetMaterial(material);
                            listModel.Add(model);
                            material = null;
                            listFaceGroup.Clear();
                        }
                        foreach (Material mat in materials)
                        {
                            if (mat.Name == data)
                            {
                                material = mat;
                            }
                        }
                    }
                    break;
                case "s":
                    {
                        if (data != "off")
                        {
                            bSmooth = true;
                        }
                    }
                    break;
                case "f":
                    {
                        List<Face> Faces = new List<Face>();
                        string[] facesGroup = data.Split(' ');
                        for (int i = 0; i < facesGroup.Length; i++)
                        {
                            int VerticeIndex = 0;
                            int NormalIndex = 0;
                            int TextureIndex = 0;

                            string[] faces = facesGroup[i].Split('/');
                            for (int j = 0; j < faces.Length; j++)
                            {
                                switch (j)
                                {
                                    case 0:
                                        VerticeIndex = Int32.Parse(faces[0]);
                                        continue;
                                    case 1:
                                        TextureIndex = Int32.Parse(faces[1]);
                                        continue;
                                    case 2:
                                        NormalIndex = Int32.Parse(faces[2]);
                                        break;
                                }
                            }
                            Faces.Add(new Face(VerticeIndex, NormalIndex, TextureIndex));
                        }
                        listFaceGroup.Add(new FaceGroup(Faces));
                        break;
                    }
            }
        }

        private static void GetListNormal(ref List<Triangle> listTriangle)
        {
            foreach (Triangle triangle in listTriangle)
            {
                ModelData.ComputeNormalsOfATriangle(ref triangle.point1,ref triangle.point2,ref triangle.point3);
            }
        }

        private static List<Triangle> GetListTriangle()
        {
            List<Triangle> listTriangle = new List<Triangle>();
            List<Vertex> listVertex = new List<Vertex>();
            Vector3 normal = new Vector3();
            Vector2 texture = new Vector2();
            Vertex additionalVertex = new Vertex();

            foreach (FaceGroup group in listFaceGroup)
            {
                if (group.Faces.Count == 4)
                {
                    for (int i = 0; i < group.Faces.Count - 1; i++)
                    {
                        if (listVertex.Count == 3)
                        {
                            listVertex.Clear();
                        }
                        if (normals.Count > 0)
                        {
                            normal = normals[group.Faces[i].NormalIndex -1];
                        }
                        if (textures.Count > 0)
                        {
                            texture = textures[group.Faces[i].TextureIndex -1];
                        }
                        if (i == 0)
                        {
                            additionalVertex = new Vertex(vertices[group.Faces[i].VerticeIndex - 1], normal, texture);
                        }
                        listVertex.Add(new Vertex(vertices[group.Faces[i].VerticeIndex - 1], normal, texture));
                    }
                    listTriangle.Add(new Triangle(listVertex[0], listVertex[1], listVertex[2]));

                    for (int i = 2; i < group.Faces.Count; i++)
                    {
                        if (listVertex.Count == 3)
                        {
                            listVertex.Clear();
                        }
                        if (normals.Count > 0)
                        {
                            normal = normals[group.Faces[i].NormalIndex - 1];
                        }
                        if (textures.Count > 0)
                        {
                            texture = textures[group.Faces[i].TextureIndex - 1];
                        }
                        listVertex.Add(new Vertex(vertices[group.Faces[i].VerticeIndex - 1], normal, texture));
                    }
                    listVertex.Add(additionalVertex);
                    listTriangle.Add(new Triangle(listVertex[0], listVertex[1], listVertex[2]));
                }
                else if (group.Faces.Count == 3)
                {
                    for (int i = 0; i < group.Faces.Count; i++)
                    {
                        if (listVertex.Count == 3)
                        {
                            listVertex.Clear();
                        }
                        if (normals.Count > 0)
                        {
                            normal = normals[group.Faces[i].NormalIndex - 1];
                        }
                        if (textures.Count > 0)
                        {
                            texture = textures[group.Faces[i].TextureIndex - 1];
                        }
                        listVertex.Add(new Vertex(vertices[group.Faces[i].VerticeIndex - 1], normal, texture));
                    }
                    listTriangle.Add(new Triangle(listVertex[0], listVertex[1], listVertex[2]));
                }
            }
            if (normals.Count == 0)
            {
                GetListNormal(ref listTriangle);
            }
            return listTriangle;
        }
    }
}
