using System;
using System.Collections.Generic;
namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            Huffman.Huffman huffman = new Huffman.Huffman();
            //Compresión
            string prueba = "ddabdccedchafbadgdcgabgccddbcdgg";
            char[] arregloDeChars = prueba.ToCharArray();
            byte[] arregloDeCompresión = huffman.Compression(arregloDeChars);
            Console.WriteLine("--------------------------------");
            Console.WriteLine("El texto original es: " + prueba);
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Su texto compreso es:");
            Console.WriteLine();
            for (int i = 0; i < arregloDeCompresión.Length; i++)
            {
                char resultado = (char)arregloDeCompresión[i];
                Console.Write(resultado.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            //Descompresión
            Console.WriteLine("Presione cualquier tecla para descomprimir...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("El texto original es:");
            Console.WriteLine();
            List<char> arregloDecompreso = huffman.Decompression(arregloDeCompresión);
            for (int i = 0; i < arregloDecompreso.Count; i++)
            {
                Console.Write(arregloDecompreso[i].ToString());
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.ReadKey();
        }
    }
}
