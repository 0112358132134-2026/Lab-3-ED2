using System.Collections.Generic;
namespace Huffman
{
    public interface IHuffman
    {
        public byte[] Compression(char[] textToEncrypt, string originalName);
        public List<char> Decompression(List<byte> bytes);
    }
}
