using System;
namespace Huffman
{
    public class NodeTable : IComparable
    {
        public string character;
        public int frequency;
        public double probability;
        public string binary;

        public int CompareTo(object _objeto)
        {
            NodeTable c = (NodeTable)_objeto;
            return this.probability.CompareTo(c.probability);
        }
    }
}
