using System;
using System.Collections.Generic;
using System.IO;
using NG3Dx.Managers;
using SlimDX.Direct3D11;
using NG3Dx.Geometrics;
using System.Globalization;
using SlimDX;

namespace NG3Dx.Imports
{
    public class MaterialLibraryLoader
    {
        public MaterialLibraryLoader()
        {

        }

        public static List<Material> LoadMaterial(String directory, String filename)
        {
            List<Material> materials = new List<Material>();

            using (FileStream stream = new FileStream(directory + "\\" + filename, FileMode.Open))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    String line = String.Empty;
                    Material material = new Material();
                    while (reader.Peek() >= 0)
                    {
                        line = reader.ReadLine();
                        if (line.StartsWith("newmtl"))
                        {
                            material.Name = line.Substring(line.IndexOf(' ') + 1);
                        }
                        if (line.StartsWith("Ns"))
                        {
                            material.data.Exponent = float.Parse(line.Substring(line.IndexOf(' ') + 1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                        if (line.StartsWith("Ka"))
                        {
                            string[] ka = line.Substring(line.IndexOf(' ',0) + 1).Split(' ');
                            material.data.Ambient = new Color4(float.Parse(ka[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(ka[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(ka[2], CultureInfo.InvariantCulture.NumberFormat));
                        }
                        if (line.StartsWith("Kd"))
                        {
                            string[] Kd = line.Substring(line.IndexOf(' ', 0) + 1).Split(' ');
                            material.data.Diffuse = new Color4(float.Parse(Kd[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(Kd[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(Kd[2], CultureInfo.InvariantCulture.NumberFormat));
                        }
                        if (line.StartsWith("Ks"))
                        {
                            string[] Ks = line.Substring(line.IndexOf(' ', 0) + 1).Split(' ');
                            material.data.Specular = new Color4(float.Parse(Ks[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(Ks[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(Ks[2], CultureInfo.InvariantCulture.NumberFormat));
                        }
                        if (line.StartsWith("Ni"))
                        {
                            material.data.OpticalDensity = float.Parse(line.Substring(line.IndexOf(' ') + 1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                        if (line.StartsWith("d") || line.StartsWith("Tr"))
                        {
                            material.data.TransmissionFilter = float.Parse(line.Substring(line.IndexOf(' ') + 1), CultureInfo.InvariantCulture.NumberFormat);
                        }
                        if (line.StartsWith("illum"))
                        {
                            material.Illumination = Material.ConvertIntegerToIllumition(Int32.Parse(line.Substring(line.IndexOf(' ') + 1)));
                        }
                        if (line.StartsWith("refl"))
                        {
                            string[] refl = line.Substring(line.IndexOf(' ', 0) + 1).Split(' ');
                            material.data.Reflect = new Color4(float.Parse(refl[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(refl[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(refl[2], CultureInfo.InvariantCulture.NumberFormat));
                        }
                        if (material.SetMap(line.Split(' ')[0]))
                        {
                            ImageLoadInformation format = ImageLoadInformation.FromDefaults();
                            format.Format = (line.Substring(line.IndexOf(' ') + 1).EndsWith(".jpg")) ? SlimDX.DXGI.Format.B8G8R8X8_UNorm : SlimDX.DXGI.Format.R8G8B8A8_UNorm;
                            try
                            {
                                material.Texture = Texture2D.FromFile(DeviceManager.DirectXDevice.device, directory + "\\" + line.Substring(line.IndexOf(' ') + 1), format);                                
                            }
                            catch
                            {
                                material.Texture = null;
                            }
                            if (material.Texture != null)
                            {
                                materials.Add(material);
                            }
                            material = new Material();
                        }

                        if (reader.Peek() < 0) break;
                    }
                }
            }
            return materials;
        }
    }
}
