using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuffmanCompresion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Huffman : ControllerBase
    {
        public static IWebHostEnvironment _environment;

        public Huffman(IWebHostEnvironment environment)
        {
            _environment = environment;

        }
        public class FileUploadAPI
        {
            public IFormFile files { get; set; }

        }

        [HttpPost("compress")]
        public async Task<string> post([FromForm]FileUploadAPI objFile)
        {

            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        string s = @_environment.WebRootPath;
                        Implementacion imp = new Implementacion(fileStream.Name, s);
                        imp.Comprimir();

                        return "\\Upload\\" + objFile.files.FileName;

                    }


                }
                else
                {
                    return "Failed";
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }



        }

        [HttpPost("decompress")]
        public async Task<string> decompress([FromForm]FileUploadAPI objFile)
        {

            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        string s = @_environment.WebRootPath;
                        Implementacion imp = new Implementacion(fileStream.Name, s);
                        imp.Descomporimir();

                        return "\\Upload\\" + objFile.files.FileName;

                    }


                }
                else
                {
                    return "Failed";
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }



        }


    }
}