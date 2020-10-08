using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Huffman
{
    public class HuffmanClass
    {
        //COMPRESSION:
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

        public byte[] Compression(string textToEncrypt)
        {
            //Calculamos la tabla:
            List<NodeTable> table = GenerateTable(textToEncrypt);
            //Incicializamos una cola e insertamos los valores:
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
            //Convertimos los bytes a decimales y los agregamos a otra lista:
            List<int> decimals = new List<int>();
            for (int i = 0; i < bytes.Count; i++)
            {
                decimals.Add(ConvertBinaryToDecimal(bytes[i]));
            }
            //Mandamos a escribir todo el texto (incluyendo su metadata):
            byte[] response = returnBytesToWrite(table, decimals);
            return response;
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
                    auxiliar = auxiliar.Remove(0,8);                   
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
            //Se eliminan:
            //string remove = "";
            //for (int i = 0; i < auxiliar.Length; i++)
            //{
            //    if (i >= 8)
            //    {
            //        remove += auxiliar[i].ToString();
            //    }
            //}
            //auxiliar = remove;
            return result;
        }
       
        public int ConvertBinaryToDecimal(string binary)
        {
            int exponent = binary.Length - 1;
            int decimalNumber = 0;

            for (int i = 0; i < binary.Length; i++)
            {
                if (int.Parse(binary.Substring(i, 1)) == 1)
                {
                    decimalNumber += int.Parse(System.Math.Pow(2, double.Parse(exponent.ToString())).ToString());
                }
                exponent--;
            }
            return decimalNumber;
        }

        public string ConvertDecimalToBinary(int number)
        {
            string result = "";
            while (number > 0)
            {
                if (number % 2 == 0)
                {
                    result = "0" + result;
                }
                else
                {
                    result = "1" + result;
                }
                number = (int)(number / 2);
            }
            return result;
        }

        public byte[] returnBytesToWrite(List<NodeTable> metaData, List<int> finalText)
        {
            //Arreglo resultante:
            byte[] result = null;
            //Encontramos cuál es la mayor frecuencia de entre todos los caracteres:
            int maxValue = metaData[0].frequency;
            for (int i = 0; i < metaData.Count; i++)
            {
                if (metaData[i].frequency > maxValue)
                {
                    maxValue = metaData[i].frequency;
                }
            }
            //Se valida si la frecuencia mayor supera los 255---
            //Si no los supera, entonces la cantidad de bytes que se utilizará para cada frecuencia solo será de 1
            //De lo contrario, si lo supera, la candtida de bytes a utilizar para cada frecuencia será de 2:
            int numberOfBytes = 1;
            if (maxValue > 255)
            {
                numberOfBytes = 2;
            }
            //Definimos el tamaño del arreglo de bytes:
            int size = 2 + (metaData.Count * (numberOfBytes + 1)) + finalText.Count;
            //Mandamos a trear el arreglo de bytes para dos, si "numberOfBytes" es = 2:
            if (numberOfBytes == 2)
            {
                result = bytesToMetadata2(metaData.Count, metaData, finalText, size);
            }
            //Mandamos a trear el arreglo de bytes para uno, si "numberOfBytes" es = 1:
            else if (numberOfBytes == 1)
            {
                result = bytesToMetadata1(metaData.Count, metaData, finalText, size);
            }
            //Se manda a imprimir el resultado compreso:
            return result;
            //using FileStream fileStream = new FileStream(@"C:\Users\68541\Desktop\Pruebita.txt", FileMode.OpenOrCreate);
            //fileStream.Write(result, 0, result.Length);
        }

        //Devuelve el arreglo de Bytes si ninguna de las frecuencias supera las 255:
        public byte[] bytesToMetadata1(int totalCharacters, List<NodeTable> metaData, List<int> finalText, int arraySize)
        {
            //TODOS LOS PARÁMETROS QUE RECIBE... DEBEN CONVERTISE A BYTES Y AGREGARSE AL ARREGLO RESULTANTE--
            //Esta lista contedrá la metaData en ints:
            List<int> metaDataDecimal = new List<int>();
            for (int i = 0; i < metaData.Count; i++)
            {
                //Se agrega el caracter de cada posición:
                char[] toChar = metaData[i].character.ToCharArray();
                byte toByte = (byte)toChar[0];
                metaDataDecimal.Add((int)toByte);
                //Se agrega la frecuencia de cada posición:
                metaDataDecimal.Add(metaData[i].frequency);
            }
            //Ya con todos los parámetros convertidos a enteros... los agregamos todos a una sola lista de enteros:
            List<int> allParameters = new List<int>();
            allParameters.Add(totalCharacters);
            allParameters.Add(1);
            for (int i = 0; i < metaDataDecimal.Count; i++)
            {
                allParameters.Add(metaDataDecimal[i]);
            }
            for (int i = 0; i < finalText.Count; i++)
            {
                allParameters.Add(finalText[i]);
            }
            //Ya que tenemos todos los parámetros en una sola lista... se convierten a bytes y se agrega al arreglo resultante:
            byte[] result = new byte[arraySize];
            for (int i = 0; i < allParameters.Count; i++)
            {
                result[i] = (byte)allParameters[i];
            }
            return result;
        }

        //Devuelve el arreglo de Bytes si alguna de las frecuencias supera las 255:
        public byte[] bytesToMetadata2(int totalCharacters, List<NodeTable> metaData, List<int> finalText, int arraySize)
        {
            //TODOS LOS PARÁMETROS QUE RECIBE... DEBEN CONVERTISE A BYTES Y AGREGARSE AL ARREGLO RESULTANTE--
            //Esta lista contendrá los pares de binarios para cada frecuencia (si la frecuencia, en binario, supera los 8 dígitos... se divide el binario en 2... de lo contrario se divide en 2 pero el primero será 0):
            List<int> metaDataDecimal = new List<int>();
            for (int i = 0; i < metaData.Count; i++)
            {
                //Se agrega el caracter de cada posición:
                char[] toChar = metaData[i].character.ToCharArray();
                byte toByte = (byte)toChar[0];
                metaDataDecimal.Add((int)toByte);
                //Convertimos cada frecuancia a binario y validamos:
                string binary = ConvertDecimalToBinary(metaData[i].frequency);
                //Separamos pares de 8 para cada binario:
                if (binary.Length < 9)
                {
                    metaDataDecimal.Add(0);
                    metaDataDecimal.Add(ConvertBinaryToDecimal(binary));
                }
                else if (binary.Length >= 9)
                {
                    int subtraction = binary.Length - 8;
                    metaDataDecimal.Add(ConvertBinaryToDecimal(binary.Substring(0, subtraction)));
                    metaDataDecimal.Add(ConvertBinaryToDecimal(binary.Substring(subtraction, 8)));
                }
            }
            //Ya con todos los paráemtros convertidos a enteros... los agregamos todos a una sola lista de enteros:
            List<int> allParameters = new List<int>();
            allParameters.Add(totalCharacters);
            allParameters.Add(2);
            for (int i = 0; i < metaDataDecimal.Count; i++)
            {
                allParameters.Add(metaDataDecimal[i]);
            }
            for (int i = 0; i < finalText.Count; i++)
            {
                allParameters.Add(finalText[i]);
            }
            //Ya que tenemos todos los parámetros en una sola lista... se convierten a bytes y se agrega al arreglo resultante:
            byte[] result = new byte[arraySize];
            for (int i = 0; i < allParameters.Count; i++)
            {
                result[i] = (byte)allParameters[i];
            }
            return result;
        }
        
        //DESCOMPRESSION:
        public string Decompression(byte[] bytes)
        {
            int startOfCompressedText = 0; 
            //La primera posición del arreglo nos dirá cuántos carateres diferentes tiene:
            int diferentsCharacters = bytes[0];
            //La segunda posición del arreglo nos dirá cuántos bytes ocupan las frecuencias:
            int bytesOfFrequencies = bytes[1];
            //Se valida cuántos son:
            List<NodeTable> table = new List<NodeTable>();
            //Si solo ocupan 1 byte, entonces...
            if (bytesOfFrequencies == 1)
            {
                int numberToReadMetadata = (diferentsCharacters * 2) + 1;
                startOfCompressedText = numberToReadMetadata;
                for (int i = 2; i <= numberToReadMetadata; i++)
                {
                    //Leemos primero los caracteres:
                    char character = (char)bytes[i];
                    i++;
                    int frequency = bytes[i];
                    //Agregamos a la "table":
                    NodeTable aux = new NodeTable
                    {
                        character = character.ToString(),
                        frequency = frequency
                    };
                    table.Add(aux);
                }
            }
            //Si ocupan 2 bytes, entonces...
            else if (bytesOfFrequencies == 2)
            {
                int numberToReadMetadata = (diferentsCharacters * 3) + 1;
                startOfCompressedText = numberToReadMetadata;
                for (int i = 2; i < numberToReadMetadata; i++)
                {
                    //Leemos primero los caracteres:
                    char character = (char)bytes[i];
                    i++;
                    //Ya que las frecuencias ocupan dos bytes, debemos:
                    //Primero: Convertir la 2da. y 3ra. posición a bytes
                    int frequency1 = bytes[i];
                    i++;
                    int frequency2 = bytes[i];
                    //Segundo: Ya convertidos a bytes, ambos se deben convertir a binarios
                    //
                    //NUEVO
                    //
                    string binary1 = ConvertDecimalToBinary(frequency1);
                    if (binary1 == "")
                    {
                        binary1 = "0";
                    }
                    string binary2 = ConvertDecimalToBinary(frequency2);
                    if (binary2.Length < 8)
                    {
                        string copy = binary2;
                        binary2 = "";
                        int restant = 8 - copy.Length;
                        for (int j = 0; j < restant; j++)
                        {
                            binary2 += "0";
                        }
                        binary2 += copy;
                    }
                    //
                    //NUEVI
                    //
                    //Tercero: Concatenamos los dos binarios, para formar uno solo
                    string resultantBinary = binary1 + binary2;
                    //Cuarto: Convertimos el binario en decimal para obtener la frecuencia total
                    int frequencyTotal = ConvertBinaryToDecimal(resultantBinary);
                    //Agregamos a la "table":
                    NodeTable aux = new NodeTable
                    {
                        character = character.ToString(),
                        frequency = frequencyTotal
                    };
                    table.Add(aux);
                }
            }
            //Se llena la tablita con sus probabilidades y se vuelve a hacer todo el proceso de la cola y el árbol, etc...
            //Se calcula la probabilidad de cada caracter:
            double totalFrequency = 0;
            for (int i = 0; i < table.Count; i++)
            {
                totalFrequency += table[i].frequency;
            }
            for (int i = 0; i < table.Count; i++)
            {
                table[i].probability = table[i].frequency / totalFrequency;
            }
            //Incicializamos una cola e insertamos los valores:
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
            //Ya con toda la table hecha, procedemos a leer el texto compreso para su descompresión:
            string largeBinary = "";
            for (int i = startOfCompressedText + 1; i < bytes.Length; i++)
            {
                //Se convierte cada decimal a binario y se agrega a un solo string con el binario largo original:
                string binaryIndividual = ConvertDecimalToBinary(bytes[i]);
                //Si el tamaño, es menor a 8, entonces se agregan 0´s al inicio:
                if (binaryIndividual.Length < 8 )
                {
                    int restants = 8 - binaryIndividual.Length;
                    string others = "";
                    for (int j = 0; j < restants; j++)
                    {
                        others += "0";
                    }
                    string ok = others + binaryIndividual;
                    largeBinary += ok;
                }
                else
                {
                    largeBinary += binaryIndividual;
                }
            }
            //Ya con la cadena larga de binario... se van haciendo comparaciones en la "table" para obtener el texto original:
            bool empty = false;
            string result = "";
            while (!empty)
            {
                bool match = false;
                int counter = 0;
                int posMatch = 0;
                while (!match)
                {
                    counter++;
                    for (int i = 0; i < table.Count; i++)
                    {
                        if (largeBinary.Substring(0, counter) == table[i].binary)
                        {
                            result += table[i].character;
                            posMatch = counter;
                            match = true;
                        }
                    }                    
                }
                //Se elimina lo que ya se encontró:
                if (match)
                {
                    largeBinary = largeBinary.Remove(0, posMatch);
                    //string copy = largeBinary;
                    //largeBinary = "";
                    //for (int i = posMatch; i < copy.Length; i++)
                    //{
                    //    largeBinary += copy[i].ToString();
                    //}
                }
                //Se comprueba si ya se debe dejar de leer:
                if (result.Length == totalFrequency)
                {
                    empty = true;
                }
            }
            return result;
        }
    }
}