using System;
using System.Collections.Generic;
using Huffman;
namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string palbo = "pablo";
            //palbo.Remove(0, 2);
            //Console.WriteLine(palbo);
            HuffmanClass huffman = new HuffmanClass();
            string compressedText = huffman.Compression("ddabdccedchafbadgdcgabgccddbcdgg");
            Console.WriteLine(compressedText);
            Console.ReadKey();
        }
    }
}
