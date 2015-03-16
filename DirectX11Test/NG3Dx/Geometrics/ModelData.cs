using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NG3Dx.Effects;
using NG3Dx.Managers;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using NG3Dx.Ligths;

namespace NG3Dx.Geometrics
{
    public enum eModelType
    {
        None = 1,
        Model = 2,
    }

    public class ModelData
    {
        protected int numVertices;
        protected int numIndices;
        protected int vertexStride;
        protected int indexStride;

        protected Buffer vertexBuffer;
        protected Buffer indexBuffer;
        protected VertexBufferBinding vertexBufferBinding;

        protected List<Triangle> ListTriangle;
        protected BoundingBox ModelBoundingBox;
        protected EffectData effectData;

        public Matrix Transform;
        public eModelType modelType;
        public Color4 color;

        public ModelData()
        {
            numVertices = 0; 
            numIndices = 0;
            vertexStride = Vertex.SizeOfVertexInBytes;
            indexStride = Marshal.SizeOf(typeof(uint));
            effectData = new EffectData();
            ListTriangle = new List<Triangle>();
            Transform = Matrix.Identity;
            color = new Color4(1, 1, 1);
            modelType = eModelType.Model;
        }

        public void SetMaterial(Material _material)
        {
            effectData.effectTexture.SetResource(new ShaderResourceView(DeviceManager.DirectXDevice.device, _material.Texture));
            var d = GetArray(_material.data);
            effectData.effectMaterial.SetRawValue(new DataStream(d, false, false), MaterialData.Stride);
        }

        public void SetDirLights(DirectionalLight dirlight)
        {
            if (effectData.effectDirectionalLight != null)
            {
                var d = GetArray(dirlight);
                effectData.effectDirectionalLight.SetRawValue(new DataStream(d, false, false), DirectionalLight.Stride);
            }
        }

        public void SetEyePosW(Vector3 v)
        {
            if (effectData.effectEyePosition != null)
            {
                effectData.effectEyePosition.Set(v);
            }
        }

        public byte[] GetArray(object o)
        {
            byte[] _unmanagedStaging = _unmanagedStaging = new byte[1024];
            Array.Clear(_unmanagedStaging, 0, _unmanagedStaging.Length);
            var len = Marshal.SizeOf(o);
            if (len >= _unmanagedStaging.Length)
            {
                _unmanagedStaging = new byte[len];
            }
            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(o, ptr, true);
            Marshal.Copy(ptr, _unmanagedStaging, 0, len);
            Marshal.FreeHGlobal(ptr);
            return _unmanagedStaging;
        }

        public void ComputeNormalsAndTextCoords()
        {
            int nElementCount = vertexBuffer.Description.SizeInBytes / vertexStride;
            int nIndicesCount = numIndices;
            int bufferSizeInBytes = vertexBuffer.Description.SizeInBytes;
            int elementSizeInBytes = vertexStride;

            ListTriangle = new List<Triangle>();

            BufferDescription stagingBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.Shared,
                SizeInBytes = bufferSizeInBytes,
                StructureByteStride = elementSizeInBytes,
                Usage = ResourceUsage.Staging,
            };

            Buffer stagingBuffer = new Buffer(DeviceManager.DirectXDevice.device, stagingBufferDescription);
            DeviceManager.DirectXDevice.device.ImmediateContext.CopyResource(vertexBuffer, stagingBuffer);

            DataBox output = DeviceManager.DirectXDevice.device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Read, SlimDX.Direct3D11.MapFlags.None);

            List<Vertex> listVertices = new List<Vertex>();
            output.Data.Position = 0;
            for (int index = 0; index < nElementCount; index++)
            {
                listVertices.Add(output.Data.Read<Vertex>());
            }

            stagingBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.Shared,
                SizeInBytes = indexBuffer.Description.SizeInBytes,
                StructureByteStride = indexBuffer.Description.SizeInBytes / numIndices,
                Usage = ResourceUsage.Staging,
            };

            stagingBuffer = new Buffer(DeviceManager.DirectXDevice.device, stagingBufferDescription);
            DeviceManager.DirectXDevice.device.ImmediateContext.CopyResource(indexBuffer, stagingBuffer);


            output = DeviceManager.DirectXDevice.device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Read, SlimDX.Direct3D11.MapFlags.None);

            List<uint> listIndices = new List<uint>();
            output.Data.Position = 0;
            for (int index = 0; index < numIndices; index++)
            {
                listIndices.Add(output.Data.Read<uint>());
            }
            for (int i = 0; i < numIndices / 3; i++)
            {
                ListTriangle.Add(new Triangle(listVertices[(int)listIndices[i * 3]], listVertices[(int)listIndices[i * 3 + 1]], listVertices[(int)listIndices[i * 3 + 2]]));
            }
            stagingBuffer.Dispose();

            stagingBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.Shared,
                SizeInBytes = bufferSizeInBytes,
                StructureByteStride = elementSizeInBytes,
                Usage = ResourceUsage.Staging,
            };

            stagingBuffer = new Buffer(DeviceManager.DirectXDevice.device, stagingBufferDescription);
            DataBox input = DeviceManager.DirectXDevice.device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Write, SlimDX.Direct3D11.MapFlags.None);

            foreach (Triangle triangle in ListTriangle)
            {
                ComputeNormalsOfATriangle(ref triangle.point1, ref triangle.point2, ref triangle.point3);
            }
            ComputeTexCoords(ref listVertices);
            foreach (var vert in listVertices)
            {
                input.Data.Write<Vertex>(vert);
            }
            DeviceManager.DirectXDevice.device.ImmediateContext.CopyResource(stagingBuffer, vertexBuffer);
            stagingBuffer.Dispose();
        }

        public static void ComputeNormalsOfATriangle(ref Vertex pointFirst, ref Vertex pointSecond, ref Vertex pointThird)
        {
            Vector3 v1 = pointThird.Position - pointFirst.Position;
            Vector3 v2 = pointSecond.Position - pointFirst.Position;
            Vector3 crossOne = Vector3.Cross(v1, v2);
            crossOne.Normalize();
            Vertex normalOne = new Vertex(pointFirst.Position, crossOne, pointFirst.TextPos);
            pointFirst = normalOne;
            Vertex normalTwo = new Vertex(pointSecond.Position, crossOne, pointSecond.TextPos);
            pointSecond = normalTwo;
            Vertex normalThree = new Vertex(pointThird.Position, crossOne, pointThird.TextPos);
            pointThird = normalThree;
        }

        public void ComputeTexCoords(ref List<Vertex> listPoints)
        {
            var center = (ModelBoundingBox.Minimum - ModelBoundingBox.Maximum) * 0.5f;
            for (int i = 0; i < listPoints.Count; i++)
            {
                Vertex vertex = new Vertex(listPoints[i].Position, listPoints[i].Normal, new Vector2((float)(Math.Asin(Vector3.Normalize(listPoints[i].Position - center).X) / Math.PI), (float)(Math.Asin(Vector3.Normalize(listPoints[i].Position - center).Y) / Math.PI)));
                listPoints[i] = vertex;
            }
        }

        public void ComputeTexCoords(ref Vertex point, Vector3 vecMin, Vector3 vecMax)
        {
            var center = (vecMin - vecMax) * 0.5f;
            Vertex vertex = new Vertex(point.Position, point.Normal, new Vector2((float)(Math.Asin(Vector3.Normalize(point.Position - center).X) / Math.PI), (float)(Math.Asin(Vector3.Normalize(point.Position - center).Y) / Math.PI)));
            point = vertex;
        }

        public void Render()
        {
            if (effectData.effectMatrix == null) return;
            effectData.effectMatrix.SetMatrix(Transform * CameraManager.OrbitPanCamera.ViewProjection);
            DeviceManager.DirectXDevice.context.InputAssembler.InputLayout = effectData.layout;
            DeviceManager.DirectXDevice.context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            effectData.effectColor.Set(color);

            if (modelType == eModelType.None)
            {
                DeviceManager.DirectXDevice.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                effectData.technique.GetPassByIndex(0).Apply(DeviceManager.DirectXDevice.context);
                DeviceManager.DirectXDevice.context.Draw(numVertices, 0);
            }
            else
            {

                NG3Dx.Ligths.DirectionalLight light = new NG3Dx.Ligths.DirectionalLight();
                light.Ambient = new Color4(0.2f, 0.2f, 0.2f);
                light.Diffuse = new Color4(0.7f, 0.7f, 0.7f);
                light.Specular = new Color4(0.8f, 0.8f, 0.8f);
                light.Direction = new Vector3(-0.57735f, -0.57735f, 0.57735f);
                SetDirLights(light);

                SetEyePosW(new Vector3(10,10,10));

                DeviceManager.DirectXDevice.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                DeviceManager.DirectXDevice.context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
                effectData.technique.GetPassByIndex(1).Apply(DeviceManager.DirectXDevice.context);
                DeviceManager.DirectXDevice.context.DrawIndexed(numIndices, 0, 0);
            }
        }
    }
}
