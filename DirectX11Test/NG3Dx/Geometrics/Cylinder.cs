using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using NG3Dx.Maths;
using NG3Dx.Managers;

namespace NG3Dx.Geometrics
{
  public class Cylinder : ModelData
  {
      public Cylinder(Vertex pointStart, Vertex pointStop, float radiusBottom, float radiusTop, uint slices) : base()
      {
          effectData = new NG3Dx.Effects.EffectModel();

          Vector2 texCoord = new Vector2(0, 0);
          uint numVerticesPerRow = slices + 1;

          numVertices = (int)numVerticesPerRow * 2 + 2;

          List<Vertex> listVertices = new List<Vertex>();
          float Length = VectorCalculations.Distance(pointStart, pointStop);
          ModelBoundingBox = new BoundingBox(new Vector3(-radiusBottom, -Length / 2, -radiusBottom), new Vector3(radiusTop, Length / 2, radiusTop));
          Vector3 middle = VectorCalculations.MiddlePoint(pointStart, pointStop);

          using (DataStream vertices = new DataStream(numVertices * vertexStride, true, true))
          {
              float theta = 0.0f;
              float horizontalAngularStride = ((float)Math.PI * 2) / (float)slices;

              for (int verticalIt = 0; verticalIt < 2; verticalIt++)
              {
                  for (int horizontalIt = 0; horizontalIt < numVerticesPerRow; horizontalIt++)
                  {
                      float x;
                      float y;
                      float z;

                      theta = (horizontalAngularStride * horizontalIt);

                      if (verticalIt == 0)
                      {
                          // upper circle
                          x = radiusTop * (float)Math.Cos(theta);
                          y = radiusTop * (float)Math.Sin(theta);
                          z = Length / 2+1;
                      }
                      else
                      {
                          // lower circle
                          x = radiusBottom * (float)Math.Cos(theta);
                          y = radiusBottom * (float)Math.Sin(theta);
                          z = -Length / 2+1;
                      }

                      Vector3 position = new Vector3(x, z, y);
                      Vertex act = new Vertex(position, new Vector3(), texCoord);
                      ComputeTexCoords(ref act, ModelBoundingBox.Minimum, ModelBoundingBox.Maximum);
                      vertices.Write(act);
                      listVertices.Add(act);
                  }
              }

              vertices.Write(new Vertex(new Vector3(0, Length / 2+1, 0), new Vector3(), texCoord));
              vertices.Write(new Vertex(new Vector3(0, -Length / 2+1, 0), new Vector3(), texCoord));

              vertices.Position = 0;

              // create the vertex buffer
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
          
          numIndices = (int)slices * 2 * 6;
          using (DataStream indices = new DataStream(indexStride * numIndices, true, true))
          {

              for (uint verticalIt = 0; verticalIt < 1; verticalIt++)
              {
                  for (uint horizontalIt = 0; horizontalIt < slices; horizontalIt++)
                  {
                      uint lt = horizontalIt + verticalIt * (numVerticesPerRow);
                      uint rt = (horizontalIt + 1) + verticalIt * (numVerticesPerRow);

                      uint lb = horizontalIt + (verticalIt + 1) * (numVerticesPerRow);
                      uint rb = (horizontalIt + 1) + (verticalIt + 1) * (numVerticesPerRow);

                      indices.Write(lt);
                      indices.Write(rt);
                      indices.Write(lb);
                      ListTriangle.Add(new Triangle(listVertices[(int)lt], listVertices[(int)rt], listVertices[(int)lb]));

                      indices.Write(rt);
                      indices.Write(rb);
                      indices.Write(lb);
                      ListTriangle.Add(new Triangle(listVertices[(int)rt], listVertices[(int)rb], listVertices[(int)lb]));
                  }
              }

              for (uint verticalIt = 0; verticalIt < 1; verticalIt++)
              {
                  for (uint horizontalIt = 0; horizontalIt < slices; horizontalIt++)
                  {
                      uint lt = horizontalIt + verticalIt * (numVerticesPerRow);
                      uint rt = (horizontalIt + 1) + verticalIt * (numVerticesPerRow);

                      uint patchIndexTop = numVerticesPerRow * 2;

                      indices.Write(lt);
                      indices.Write(patchIndexTop);
                      indices.Write(rt);
                  }
              }

              for (uint verticalIt = 0; verticalIt < 1; verticalIt++)
              {
                  for (uint horizontalIt = 0; horizontalIt < slices; horizontalIt++)
                  {
                      uint lb = horizontalIt + (verticalIt + 1) * (numVerticesPerRow);
                      uint rb = (horizontalIt + 1) + (verticalIt + 1) * (numVerticesPerRow);

                      uint patchIndexBottom = numVerticesPerRow * 2 + 1;
                      indices.Write(lb);
                      indices.Write(rb);
                      indices.Write(patchIndexBottom);
                  }
              }
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

          Vector3 p = pointStart.Position - pointStop.Position;
          p.Normalize();
          Vector3 t = Vector3.Cross(new Vector3(0, 1, 0), p);
          double angle = 180 / Math.PI * Math.Acos((Vector3.Dot(new Vector3(0, 1, 0), p) / p.Length()));
          Transform = Matrix.RotationAxis(new Vector3(t.X, t.Y, t.Z), ConvertCalculations.DegreeToRadian((float)angle)) * Matrix.Translation(middle);
      }
  }
}
