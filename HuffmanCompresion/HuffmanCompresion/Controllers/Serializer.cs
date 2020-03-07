using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace HuffmanCompresion.Controllers
{
    internal class Serializer
    {
        internal static void GuardarArchivoBinario<T>(string filePath, T pObject)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var formatBin = new BinaryFormatter();

                    formatBin.Serialize(memoryStream, pObject);

                    string path = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.WriteAllBytes(filePath, memoryStream.ToArray());

                    memoryStream.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
