using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
namespace API_Huffman.Models
{
    public class JsonFile
    {
        public void WriteInJson(HuffCompressions newCompression, string pathToWrite)
        {
            pathToWrite += "/Compressions.json";
            //Creamos una lista de objetos "Archive"
            List<HuffCompressions> listAux = new List<HuffCompressions>();
            //Si el archivo ya existe, entonces lo leemos
            if (File.Exists(pathToWrite))
            {
                using FileStream fileRead = File.OpenRead(pathToWrite);
                string result = "";
                MemoryStream memory = new MemoryStream();
                fileRead.CopyTo(memory);
                //Le asignamos al string "result" lo que va a leer...
                result = Encoding.ASCII.GetString(memory.ToArray());
                //Agregamos a la lista todos los objetos que ya contiene:
                listAux = Deselearize(result);
            }
            //Si no existe, solo agregamos por primera vez el objeto:
            listAux.Add(newCompression);
            //Mandamos a escribir nuevamente en el jason:
            using StreamWriter toWrite = new StreamWriter(pathToWrite);
            toWrite.Write(Serialize(listAux));
        }

        public List<HuffCompressions> Deselearize(string objects)
        {
            return JsonSerializer.Deserialize<List<HuffCompressions>>(objects, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public string Serialize(List<HuffCompressions> objects)
        {
            return JsonSerializer.Serialize(objects, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
