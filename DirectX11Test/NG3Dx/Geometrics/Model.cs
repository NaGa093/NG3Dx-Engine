using System;
using NG3Dx.Imports;
using NG3Dx.Managers;
using SlimDX;
using SlimDX.Direct3D11;
using System.Collections.Generic;
using LibTessDotNet;
using System.Drawing;

namespace NG3Dx.Geometrics
{
    public class Model : ModelData
    {
        public Model(List<Vector3> listVertices)
        {
            Tess _tess = new Tess();
            var v = new ContourVertex[listVertices.Count];

            int nIndexContour = 0;
            foreach (var point in listVertices)
            {
                v[nIndexContour].Position = new Vec3 { X = (float)point.X, Y = (float)point.Y, Z = (float)point.Z };
                v[nIndexContour].Data = Color.Red;
                nIndexContour++;
            }

            _tess.AddContour(v, ContourOrientation.Original);
            _tess.Tessellate(WindingRule.Positive, ElementType.Polygons, 3);

            List<Triangle> listTriangle = new List<Triangle>();

            for (int i = 0; i < _tess.ElementCount; i++)
            {
                Triangle poly = new Triangle();
                for (int j = 0; j < 3; j++)
                {
                    int index = _tess.Elements[i * 3 + j];
                    if (index == -1)
                        continue;
                    Vector3 z = new Vector3
                    {
                        X = _tess.Vertices[index].Position.X,
                        Y = _tess.Vertices[index].Position.Y,
                        Z = _tess.Vertices[index].Position.Z
                    };
                    if (j == 0)
                    {
                        poly.point1.Position = z;
                    }
                    else if (j == 1)
                    {
                        poly.point2.Position = z;
                    }
                    else if (j == 2)
                    {
                        poly.point3.Position = z;
                    }
                }
                listTriangle.Add(poly);
            }

            if (listTriangle.Count == 0) return;

            effectData = new NG3Dx.Effects.EffectModel();

            ListTriangle = listTriangle;
            numVertices = 3 * listTriangle.Count;
            numIndices = 3 * listTriangle.Count;

            using (DataStream vertices = new DataStream(numVertices * vertexStride, true, true))
            {
                Vector3 MinPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 MaxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                foreach (Triangle triangle in listTriangle)
                {
                    MinPos = Vector3.Minimize(MinPos, triangle.point1.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point1.Position);
                    vertices.Write(triangle.point1);
                    MinPos = Vector3.Minimize(MinPos, triangle.point2.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point2.Position);
                    vertices.Write(triangle.point2);
                    MinPos = Vector3.Minimize(MinPos, triangle.point3.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point3.Position);
                    vertices.Write(triangle.point3);
                }
                vertices.Position = 0;
                ModelBoundingBox = new BoundingBox(MinPos, MaxPos);

                vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.DirectXDevice.device,
                    vertices,
                    numVertices * vertexStride,
                    ResourceUsage.Default,
                    BindFlags.VertexBuffer,
                    CpuAccessFlags.None,
                    ResourceOptionFlags.None,
                    0);
            }

            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, vertexStride, 0);

            using (DataStream indices = new DataStream(indexStride * numIndices, true, true))
            {
                for (uint i = 0; i < listTriangle.Count * 3; i++)
                    indices.Write(i);

                indices.Position = 0;

                indexBuffer = new SlimDX.Direct3D11.Buffer(
                    DeviceManager.DirectXDevice.device,
                    indices,
                    indexStride * numIndices,
                    ResourceUsage.Default,
                    BindFlags.IndexBuffer,
                    CpuAccessFlags.None,
                    ResourceOptionFlags.None,
                    0);
            }
        }

        public Model(List<Triangle> listTriangle) : base()
        {
            if (listTriangle.Count == 0) return;

            effectData = new NG3Dx.Effects.EffectModel();

            ListTriangle = listTriangle;
            numVertices = 3 * listTriangle.Count;
            numIndices = 3 * listTriangle.Count;

            using (DataStream vertices = new DataStream(numVertices * vertexStride, true, true))
            {
                Vector3 MinPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 MaxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                foreach (Triangle triangle in listTriangle)
                {
                    MinPos = Vector3.Minimize(MinPos, triangle.point1.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point1.Position);
                    vertices.Write(triangle.point1);
                    MinPos = Vector3.Minimize(MinPos, triangle.point2.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point2.Position);
                    vertices.Write(triangle.point2);
                    MinPos = Vector3.Minimize(MinPos, triangle.point3.Position);
                    MaxPos = Vector3.Maximize(MaxPos, triangle.point3.Position);
                    vertices.Write(triangle.point3);
                }
                vertices.Position = 0;
                ModelBoundingBox = new BoundingBox(MinPos, MaxPos);

                vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.DirectXDevice.device,
                    vertices,
                    numVertices * vertexStride,
                    ResourceUsage.Default,
                    BindFlags.VertexBuffer,
                    CpuAccessFlags.None,
                    ResourceOptionFlags.None,
                    0);
            }

            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, vertexStride, 0);

            using (DataStream indices = new DataStream(indexStride * numIndices, true, true))
            {
                for (uint i = 0; i < listTriangle.Count * 3; i++)
                    indices.Write(i);

                indices.Position = 0;

                indexBuffer = new SlimDX.Direct3D11.Buffer(
                    DeviceManager.DirectXDevice.device,
                    indices,
                    indexStride * numIndices,
                    ResourceUsage.Default,
                    BindFlags.IndexBuffer,
                    CpuAccessFlags.None,
                    ResourceOptionFlags.None,
                    0);
            }
        }
    }
}
