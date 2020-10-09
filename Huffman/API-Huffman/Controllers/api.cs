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
using Microsoft.AspNetCore.ResponseCompression;
using API_Huffman.Models;
using Microsoft.AspNetCore.Builder;
using Huffman;

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

        Huffman.Huffman huffman = new Huffman.Huffman();

        [HttpPost]
        [Route("compress/{name}")]
        public async Task<ActionResult> Compression([FromForm] IFormFile file, string name)
        {
            //COMPRESSION
            byte[] result = null;
            byte[] copy = null;
            using (var memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);                
                copy = memory.ToArray();
                char[] array = new char[copy.Length];
                for (int i = 0; i < copy.Length; i++)
                {
                    array[i] = (char)copy[i];
                }
                result = huffman.Compression(array);
            }
            Archive response = new Archive
            {
                content = result,
                contentType = "compressedFile / huff",
                fileName = name
            };
            //JSON
            HuffCompressions jsonValues = new HuffCompressions
            {
                OriginalName = file.Name,
                CompressedFilePath = _env.ContentRootPath,
                CompressionRatio = result.Length / copy.Length,
                CompressionFactor = copy.Length / result.Length,
                ReductionPorcentage = 1 - ((result.Length * 100)/ copy.Length)
            };
            JsonFile addToJson = new JsonFile();
            addToJson.WriteInJson(jsonValues, _env.ContentRootPath);
            //PATH
            string compressionPath = _env.ContentRootPath + "/Compressions/" + file.Name + ".huff";
            using (FileStream fs = System.IO.File.Create(compressionPath))
            {
                fs.Write(result);
            }
            return File(response.content, response.contentType, response.fileName + ".huff");
        }

        [HttpPost]
        [Route("decompress")]
        public async Task<ActionResult> Decompression([FromForm] IFormFile file)
        {
            byte[] result = null;
            using (var memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);
                byte[] bytes = memory.ToArray();          
                List<char> content = huffman.Decompression(bytes);
                result = new byte[content.Count];
                for (int i = 0; i < content.Count; i++)                
                result[i] = (byte)content[i];                
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
        public ActionResult Compressions()
        {
            List<HuffCompressions> list = new List<HuffCompressions>();
            JsonFile addToJson = new JsonFile();
            if (System.IO.File.Exists(_env.ContentRootPath + "/Compressions.json"))
            {
                using (FileStream fileRead = System.IO.File.OpenRead(_env.ContentRootPath + "/Compressions.json"))
                {
                    string result = "";
                    MemoryStream memory = new MemoryStream();
                    fileRead.CopyTo(memory);
                    result = Encoding.ASCII.GetString(memory.ToArray());
                    list = addToJson.Deselearize(result);
                }
                return Ok(list);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
