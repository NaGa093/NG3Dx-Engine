using System.Runtime.InteropServices;
using NG3Dx.Cameras;
using NG3Dx.Effects;
using SlimDX;
using SlimDX.Direct3D11;

namespace NG3Dx.Models
{
    public class Grid : Base
    {
        public Grid(Device device, int cellsPerSide, float cellSize)
        {
            Effectbase = new EffectGrid(device);

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

            SetColor(new Color4(1,1,1,1));
        }

        public override sealed void SetColor(Color4 color)
        {
            Effectbase.EffectVectorVariable.Set(color);
        }

        public override void Render(DeviceContext context, CameraBase camera)
        {
            Effectbase.EffectMatrixVariable.SetMatrix(Transform * camera.ViewProjection);
            context.InputAssembler.InputLayout = Effectbase.InputLayout;
            context.InputAssembler.SetVertexBuffers(0, VertexBufferBinding);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            Effectbase.EffectTechnique.GetPassByIndex(0).Apply(context);
            context.Draw(Vertices, 0);
        }
    }
}
