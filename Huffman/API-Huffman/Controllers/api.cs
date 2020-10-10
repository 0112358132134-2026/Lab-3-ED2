using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Huffman.Models;
using System.Linq;

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
            try
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
                    result = huffman.Compression(array, file.FileName);
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
                    OriginalName = file.FileName,
                    CompressedFilePath = _env.ContentRootPath + "\\Compressions",
                    CompressionRatio = (double)result.Length / (double)copy.Length,
                    CompressionFactor = (double)copy.Length / (double)result.Length,
                    //ReductionPorcentage = ((double)result.Length / (double)copy.Length) * 100
                    ReductionPorcentage = 1 - ((double)result.Length / (double)copy.Length)
                };
                JsonFile addToJson = new JsonFile();
                addToJson.WriteInJson(jsonValues, _env.ContentRootPath);
                //PATH
                string compressionPath = _env.ContentRootPath + "/Compressions/" + name + ".huff";
                using (FileStream fs = System.IO.File.Create(compressionPath))
                {
                    fs.Write(result);
                }
                return File(response.content, response.contentType, response.fileName + ".huff");
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }              
        }

        [HttpPost]
        [Route("decompress")]
        public async Task<ActionResult> Decompression([FromForm] IFormFile file)
        {
            try
            {
                byte[] result = null;
                string originalName = "";
                using (var memory = new MemoryStream())
                {
                    await file.CopyToAsync(memory);
                    byte[] bytes = memory.ToArray();
                    List<byte> aux = bytes.OfType<byte>().ToList();
                    originalName = huffman.GetOriginalName(aux);
                    List<char> content = huffman.Decompression(aux);
                    result = new byte[content.Count];
                    for (int i = 0; i < content.Count; i++)
                        result[i] = (byte)content[i];
                }
                Archive response = new Archive
                {
                    content = result,
                    contentType = "compressedFile / txt",
                    fileName = originalName
                };
                return File(response.content, response.contentType, response.fileName);
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
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
                return StatusCode(500);
            }
        }
    }
}