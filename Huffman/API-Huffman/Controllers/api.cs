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

        [HttpPost]
        [Route("compress/{name}")]
        public ActionResult Compression([FromForm] IFormFile file, string name)
        {
            string path = _env.ContentRootPath;
            HuffmanClass huffman = new HuffmanClass();
            byte[] response = null;
            using (var memory = new MemoryStream())
            {
                file.CopyToAsync(memory);
                string content = Encoding.ASCII.GetString(memory.ToArray());
                response = huffman.Compression(content);
            }
            //using FileStream fileStream = new FileStream(@"C:\Users\68541\Desktop\XD.txt", FileMode.OpenOrCreate);
            //fileStream.Write(response, 0, response.Length);
            Archieve respueste = new Archieve();
            respueste.content = response;
            respueste.contentType = "compressFile / huff";
            respueste.fileName = name;
            return File(respueste.content, respueste.contentType, respueste.fileName + ".huff");
        }

        [HttpPost]
        [Route("decompress")]
        public HttpStatusCode Decompression([FromForm] string oka)
        {
            string hola = oka + ": Bien hecho número 2";
            return HttpStatusCode.OK;
        }

        [HttpGet]
        [Route("compressions")]
        public HttpStatusCode Compressions()
        {
            return HttpStatusCode.OK;
        }
    }
}
