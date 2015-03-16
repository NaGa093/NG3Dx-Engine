using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NG3Dx.Effects;
using NG3Dx.Managers;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Geometrics
{
    public class Grid : ModelData
    {
        public Grid(int cellsPerSide, float cellSize) : base()
        {
            effectData = new EffectGrid();

            int numLines = cellsPerSide + 1;
            int gridVertexStride = Marshal.SizeOf(typeof(Vector3));
            float lineLength = cellsPerSide * cellSize;

            float xStart = -lineLength / 2.0f;
            float yStart = -lineLength / 2.0f;

            float xCurrent = xStart;
            float yCurrent = yStart;

            numVertices = 2 * 2 * numLines;
            int SizeInBytes = gridVertexStride * numVertices;

            using (DataStream vertices = new DataStream(SizeInBytes, true, true))
            {
                for (int y = 0; y < numLines; y++)
                {
                    vertices.Write(new Vector3(xCurrent, 0, yStart));
                    vertices.Write(new Vector3(xCurrent, 0, yStart + lineLength));
                    xCurrent += cellSize;
                }

                for (int x = 0; x < numLines; x++)
                {
                    vertices.Write(new Vector3(xStart, 0, yCurrent));
                    vertices.Write(new Vector3(xStart + lineLength, 0, yCurrent));
                    yCurrent += cellSize;
                }

                vertices.Position = 0;
                vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.DirectXDevice.device, vertices, SizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }
            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, gridVertexStride, 0);
        } 
    }
}
