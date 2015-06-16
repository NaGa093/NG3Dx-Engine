using System.Runtime.InteropServices;
using NG3Dx.Cameras;
using NG3Dx.Effects;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Models
{
    public class Grid : ModelBase
    {
        public Grid(Device device, int cellsPerSide, float cellSize)
        {
            EffectBase = new EffectGrid(device);

            var numLines = cellsPerSide + 1;
            var gridVertexStride = Marshal.SizeOf(typeof(Vector3));
            var lineLength = cellsPerSide * cellSize;

            var xStart = -lineLength / 2.0f;
            var yStart = -lineLength / 2.0f;

            var xCurrent = xStart;
            var yCurrent = yStart;

            Vertices = 2 * 2 * numLines;
            var sizeInBytes = gridVertexStride * Vertices;

            using (var vertices = new DataStream(sizeInBytes, true, true))
            {
                for (var y = 0; y < numLines; y++)
                {
                    vertices.Write(new Vector3(xCurrent, 0, yStart));
                    vertices.Write(new Vector3(xCurrent, 0, yStart + lineLength));
                    xCurrent += cellSize;
                }

                for (var x = 0; x < numLines; x++)
                {
                    vertices.Write(new Vector3(xStart, 0, yCurrent));
                    vertices.Write(new Vector3(xStart + lineLength, 0, yCurrent));
                    yCurrent += cellSize;
                }

                vertices.Position = 0;
                VertexBuffer = new Buffer(device, vertices, sizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }
            VertexBufferBinding = new VertexBufferBinding(VertexBuffer, gridVertexStride, 0);
        }

        public override void Render(DeviceContext context, CameraBase camera)
        {
            EffectBase.EffectMatrixVariable.SetMatrix(Transform * camera.ViewProjection);
            context.InputAssembler.InputLayout = EffectBase.InputLayout;
            context.InputAssembler.SetVertexBuffers(0, VertexBufferBinding);
            EffectBase.EffectVectorVariable.Set(Color);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            EffectBase.EffectTechnique.GetPassByIndex(0).Apply(context);
            context.Draw(Vertices, 0);
        }
    }
}
