using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Huffman
{
    public class HuffmanClass
    {
        public List<NodeTable> GenerateTable(string text)
        {
            //Se agregan los caracteres y sus frecuencias
            List<NodeTable> result = new List<NodeTable>();
            for (int i = 0; i < text.Length; i++)
            {
                if (result.Count == 0)
                {
                    NodeTable aux = new NodeTable
                    {
                        character = text[i].ToString(),
                        frequency = 1
                    };
                    result.Add(aux);
                }
                else
                {
                    bool match = false;
                    for (int j = 0; j < result.Count; j++)
                    {
                        if (result[j].character == text[i].ToString())
                        {
                            result[j].frequency++;
                            match = true;
                        }
                    }
                    if (match == false)
                    {
                        NodeTable aux = new NodeTable
                        {
                            character = text[i].ToString(),
                            frequency = 1
                        };
                        result.Add(aux);
                    }
                }
            }
            //Se calcula la probabilidad de cada caracter:
            double totalFrequency = text.Length;
            for (int i = 0; i < result.Count; i++)
            {
                result[i].probability = result[i].frequency / totalFrequency;
            }
            return result;
        }

        public void AddToQueue(List<NodeTable> values, HuffQueue<NodeTable> queue)
        {
            for (int i = 0; i < values.Count; i++)
            {
                queue.Insert(values[i], values[i].probability);
            }
        }
    }
}
