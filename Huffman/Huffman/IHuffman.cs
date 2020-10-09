using System.Collections.Generic;
namespace Huffman
{
    public interface IHuffman
    {
        public byte[] Compression(char[] textToEncrypt);
        public List<char> Decompression(byte[] bytes);
    }
}
