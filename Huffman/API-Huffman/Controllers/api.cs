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
            HuffmanClass huffman = new HuffmanClass();
            byte[] result = null;
            using (var memory = new MemoryStream())
            {
                file.CopyToAsync(memory);
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
            HuffmanClass huffman = new HuffmanClass();
            using (var memory = new MemoryStream())
            {
                file.CopyToAsync(memory);
                byte[] bytes = memory.ToArray();
                huffman.Decompression(bytes);
            }
            return Ok();
        }

        [HttpGet]
        [Route("compressions")]
        public HttpStatusCode Compressions()
        {
            return HttpStatusCode.OK;
        }
    }
}
