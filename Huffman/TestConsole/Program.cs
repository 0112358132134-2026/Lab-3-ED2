using System;
using System.Collections.Generic;
using Huffman;
namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            HuffmanClass prueba = new HuffmanClass();
            List<NodeTable> escrito = prueba.GenerateTable("ddabdccedchafbadgdcgabgccddbcdgg");
            //COLA
            HuffQueue<NodeTable> hola = new HuffQueue<NodeTable>();
            prueba.AddToQueue(escrito, hola);
            Console.ReadKey();
        }
    }
}
