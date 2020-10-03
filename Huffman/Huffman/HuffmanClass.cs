using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml;

namespace Huffman
{
    public class HuffmanClass
    {
        public List<NodeTable> GenerateTable(string text)
        {
            //Se agregan los caracteres y sus frecuencias:
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

        public string Compression(string textToEncrypt)
        {
            //Calculamos la tabla:
            List<NodeTable> table = GenerateTable(textToEncrypt);
            //Incicializamos una cola:
            HuffQueue<NodeTable> queue = new HuffQueue<NodeTable>();
            AddToQueue(table, queue);
            //Agregamos al árbol de Huffman:
            HuffTree tree = new HuffTree();
            tree.Insert(queue, tree);
            //Agregar al árbol, las codificaciones:
            tree.AddBinary(tree.rootOriginal, 0, "");
            //Añadimos a la tabla las codificaciones de cada caracter en su lugar correspondiente:
            //Para eso debemos llenar una lista con los caracteres y codificaciones del árbol:
            List<NodeTable> auxiliar = new List<NodeTable>();
            tree.BinarysIncludes(tree.rootOriginal, auxiliar);
            //Ya con la lista, se lo agregamos a la "table":
            for (int i = 0; i < auxiliar.Count; i++)
            {
                for (int j = 0; j < table.Count; j++)
                {
                    if (auxiliar[i].character == table[j].character)
                    {
                        table[j].binary = auxiliar[i].binary;
                    }
                }
            }
            //Escribimos la codificación en lugar del texto original:
            string result = "";
            for (int i = 0; i < textToEncrypt.Length; i++)
            {
                for (int j = 0; j < table.Count; j++)
                {
                    if (textToEncrypt[i].ToString() == table[j].character)
                    {
                        result += table[j].binary;
                    }
                }
            }
            //Separaramos por 8 bits y si no completa, agregar ceros:
            List<string> bytes = SeparateBytes(result);
            return result;
        }

        public List<string> SeparateBytes(string largeBinary)
        {
            string auxiliar = largeBinary;
            List<string> result = new List<string>();
            bool OK = false;
            while (!OK)
            {
                
                if (auxiliar.Length >= 8)
                {

                    result.Add(auxiliar.Substring(0, 8));
                    //Se eliminan:
                    string remove = "";
                    for (int i = 0; i < auxiliar.Length; i++)
                    {
                        if (i >= 8)
                        {
                            remove += auxiliar[i].ToString();
                        }
                    }
                    auxiliar = remove;
                }
                else
                {
                    //Agregamos la cantidad de 0´s que hagan falta para alcanzar los 8
                    int length = auxiliar.Length;
                    for (int i = length; i < 8; i++)
                    {
                        auxiliar += "0";
                    }
                    result.Add(auxiliar);
                    OK = true;
                }
            }  
            return result;
        }
    }
}
