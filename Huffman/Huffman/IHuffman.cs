using System;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace Huffman
{
    public interface IHuffman
    {
        public byte[] Compression(char[] textToEncrypt, string originalName);
        public List<char> Decompression(List<byte> bytes);
    }
}
