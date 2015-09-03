using System.Collections.Generic;
using Assimp;
using Assimp.Configs;
using NG3Dx.Effects;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace NG3Dx.Models
{
    public class Model : Base
    {
        public Model(Device device)
        {
            Effectbase = new EffectModel(device);
        }
        
        public static List<Model> CreateModels(string filePath,string texturePath,Device device)
        {
            var listModel = new List<Model>();

            using (var importer = new AssimpContext())
            {
                importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                var scene = importer.ImportFile(filePath, PostProcessPreset.TargetRealTimeMaximumQuality);

                foreach (var mesh in scene.Meshes)
                {
                    var model = new Model(device)
                    {
                        Vertices = mesh.VertexCount * 3 * 3
                    };

                    model.SetTexture(ShaderResourceView.FromFile(device, texturePath + scene.Materials[mesh.MaterialIndex].TextureDiffuse.FilePath.Replace("..", string.Empty).Replace("./", "\\")));

                    var sizeInBytes = Vertex.SizeOfVertexInBytes * model.Vertices;

                    using (var vertices = new DataStream(sizeInBytes, true, true))
                    {
                        foreach (var face in mesh.Faces)
                        {
                            foreach (var indice in face.Indices)
                            {
                                vertices.Write(new Vertex(new Vector3(mesh.Vertices[indice].X, mesh.Vertices[indice].Y, mesh.Vertices[indice].Z), new Vector3(mesh.Normals[indice].X, mesh.Normals[indice].Y, mesh.Normals[indice].Z), new Vector2(mesh.TextureCoordinateChannels[0][indice].X, 1 - mesh.TextureCoordinateChannels[0][indice].Y)));
                            }
                        }
                        vertices.Position = 0;
                        model. VertexBuffer = new Buffer(device, vertices, sizeInBytes, ResourceUsage.Default,
                            BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                    }
                    model.VertexBufferBinding = new VertexBufferBinding(model.VertexBuffer, Vertex.SizeOfVertexInBytes, 0);
                    listModel.Add(model);
                }
            }

            return listModel;
        }

        public void SetTexture(ShaderResourceView shaderResourceView)
        {
            (Effectbase as EffectModel).Texture.SetResource(shaderResourceView);
        }

        public override void SetColor(Color4 color)
        {
            Effectbase.EffectVectorVariable.Set(color);
        }

        public override void Render(DeviceContext context, Cameras.CameraBase camera)
        {
            Effectbase.EffectMatrixVariable.SetMatrix(Transform * camera.ViewProjection);
            context.InputAssembler.InputLayout = Effectbase.InputLayout;
            context.InputAssembler.SetVertexBuffers(0, VertexBufferBinding);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Effectbase.EffectTechnique.GetPassByIndex(0).Apply(context);
            context.Draw(Vertices, 0);
        }
    }
}
