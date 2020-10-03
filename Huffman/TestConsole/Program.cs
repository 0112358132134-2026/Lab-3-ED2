using System;
using System.Collections.Generic;
using Huffman;
namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            HuffmanClass huffman = new HuffmanClass();
            string compressedText = huffman.Compression("ddabdccedchafbadgdcgabgccddbcdgg");
            Console.WriteLine(compressedText);
            Console.ReadKey();
        }
    }
}
