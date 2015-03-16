using System;
using System.Collections.Generic;
using NG3Dx.Managers;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Geometrics
{
    public class Sphere : ModelData
    {
        public Sphere(float radius, uint slices, uint stacks) : base()
        {
            effectData = new NG3Dx.Effects.EffectModel(); 

            Vector2 texCoord = new Vector2(1, 1);
            uint numVerticesPerRow = slices + 1;
            uint numVerticesPerColumn = stacks + 1;

            numVertices = (int)(numVerticesPerRow * numVerticesPerColumn);

            List<Vertex> listVertices = new List<Vertex>();
            ModelBoundingBox = new BoundingBox(new Vector3(-radius, -radius, -radius), new Vector3(radius, radius, radius));
            
            using (DataStream vertices = new DataStream(numVertices * vertexStride, true, true))
            {
                float theta = 0.0f;
                float phi = 0.0f;

                float verticalAngularStride = (float)Math.PI / (float)stacks;
                float horizontalAngularStride = ((float)Math.PI * 2) / (float)slices;

                

                for (int verticalIt = 0; verticalIt < numVerticesPerColumn; verticalIt++)
                {
                    // beginning on top of the sphere:
                    theta = ((float)Math.PI / 2.0f) - verticalAngularStride * verticalIt;

                    for (int horizontalIt = 0; horizontalIt < numVerticesPerRow; horizontalIt++)
                    {
                        phi = horizontalAngularStride * horizontalIt;

                        // position
                        float x = radius * (float)Math.Cos(theta) * (float)Math.Cos(phi);
                        float y = radius * (float)Math.Cos(theta) * (float)Math.Sin(phi);
                        float z = radius * (float)Math.Sin(theta);

                        Vector3 position = new Vector3(x, y + radius / 2, z);
                        Vertex act = new Vertex(position, new Vector3(), texCoord);
                        ComputeTexCoords(ref act, ModelBoundingBox.Minimum, ModelBoundingBox.Maximum);
                        vertices.Write(act);
                        listVertices.Add(act);
                    }
                }

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

            numIndices = (int)(slices * stacks * 6);
            using (DataStream indices = new DataStream(indexStride * numIndices, true, true))
            {
                for (uint verticalIt = 0; verticalIt < stacks; verticalIt++)
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
