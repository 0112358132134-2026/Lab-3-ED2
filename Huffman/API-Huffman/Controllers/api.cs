using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Huffman;
using Microsoft.AspNetCore.ResponseCompression;
using API_Huffman.Models;
using Microsoft.AspNetCore.Builder;

namespace API_Huffman.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class api : ControllerBase 
    {
        private IWebHostEnvironment _env;
        public api (IWebHostEnvironment env)
        {
            _env = env;
        }

        HuffmanClass huffman = new HuffmanClass();

        [HttpPost]
        [Route("compress/{name}")]
        public async Task<ActionResult> Compression([FromForm] IFormFile file, string name)
        {           
            byte[] result = null;
            using (var memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);
                //
                byte[] pruebita = memory.ToArray();
                char[] oki = new char[pruebita.Length];
                for (int i = 0; i < pruebita.Length; i++)
                {
                    oki[i] = (char)pruebita[i];
                }
                //
                string content = Encoding.ASCII.GetString(memory.ToArray());
                result = huffman.Compression(content);
            }
            Archive response = new Archive
            {
                content = result,
                contentType = "compressedFile / huff",
                fileName = name
            };
            return File(response.content, response.contentType, response.fileName + ".huff");
        }

        [HttpPost]
        [Route("decompress")]
        public ActionResult Decompression([FromForm] IFormFile file)
        {
            byte[] result = null;
            using (var memory = new MemoryStream())
            {
                file.CopyToAsync(memory);
                byte[] bytes = memory.ToArray();
                string content = huffman.Decompression(bytes);
                result = Encoding.ASCII.GetBytes(content);
            }
            Archive response = new Archive
            {
                content = result,
                contentType = "compressedFile / txt",
                fileName = "descompreso"
            };
            return File(response.content,response.contentType, response.fileName + ".txt");
        }

        [HttpGet]
        [Route("compressions")]
        public HttpStatusCode Compressions()
        {
            return HttpStatusCode.OK;
        }
    }
}
