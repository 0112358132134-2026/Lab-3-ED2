﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Huffman;
namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            HuffmanClass huffman = new HuffmanClass();
            string compressedText = huffman.Compression("ddabdccedchafbadgdcgabgccddbcdgg");
            huffman.Descompression(@"C:\Users\68541\Desktop\Pruebita.txt");
            Console.ReadLine();

        }
    }
}
