using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman
{
    public class HuffTree
    {
        public NodeHuffTree rootOriginal;
        public int proxNode; 
        public HuffTree()
        {
            rootOriginal = null;
            proxNode = 1;
        }

        public void Insert(HuffQueue<NodeTable> valuesToInsert, HuffTree _root)
        {
            List<NodeHuffTree> listAux = new List<NodeHuffTree>();
            //Validar si aún hay parejar para sacar en la cola:
            while (valuesToInsert.Nodes >= 2)
            {
                //Si está vacío el árbol se realiza el primer proceso de inserción:
                if (rootOriginal == null)
                {
                    //Se sacan los dos nodos de la cola y se guardan:
                    object first = valuesToInsert.ReturnRoot();
                    valuesToInsert.DeleteRoot(valuesToInsert);
                    NodeTable firstSon = (NodeTable)first;
                    object second = valuesToInsert.ReturnRoot();
                    valuesToInsert.DeleteRoot(valuesToInsert);
                    NodeTable secondSon = (NodeTable)second;
                    //Se crean los primeros dos nodos hermanos del HuffTree
                    NodeHuffTree firstNewSon = new NodeHuffTree
                    {
                        character = firstSon.character,
                        probability = firstSon.probability,
                    };
                    NodeHuffTree secondNewSon = new NodeHuffTree
                    {
                        character = secondSon.character,
                        probability = secondSon.probability,
                    };
                    //Se crea la raíz por primera vez:
                    NodeHuffTree root = new NodeHuffTree
                    {
                        character = "N" + proxNode.ToString(),
                        probability = firstNewSon.probability + secondNewSon.probability,
                    };
                    //Se valida quién será el hijo izquierdo y quién el hijo derecho
                    if (secondNewSon.probability > firstNewSon.probability)
                    {
                        root.nodeRight = secondNewSon;
                        root.nodeLeft = firstNewSon;
                    }
                    else
                    {
                        root.nodeRight = firstNewSon;
                        root.nodeLeft = secondNewSon;
                    }
                    //Se agrega el padre a los dos hermanos:
                    firstNewSon.nodeFather = root;
                    secondNewSon.nodeFather = root;
                    //Se asigna la raíz:
                    rootOriginal = root;
                    //Se agrega el padre creado a la cola:
                    NodeTable toInsertAgainOnQueue = new NodeTable
                    {
                        character = "N" + proxNode.ToString(),
                        probability = root.probability
                    };
                    proxNode++;
                    valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                }
                //Si el árbol ya tiene algo, se realiza el siguiente proceso:
                else
                {
                    //Se sacan los dos nodos de la cola y se guardan:
                    object first = valuesToInsert.ReturnRoot();
                    valuesToInsert.DeleteRoot(valuesToInsert);
                    NodeTable firstSon = (NodeTable)first;
                    object second = valuesToInsert.ReturnRoot();
                    valuesToInsert.DeleteRoot(valuesToInsert);
                    NodeTable secondSon = (NodeTable)second;
                    //Se crean los dos nodos hermanos del HuffTree:
                    NodeHuffTree firstNewSon = new NodeHuffTree
                    {
                        character = firstSon.character,
                        probability = firstSon.probability,
                    };
                    NodeHuffTree secondNewSon = new NodeHuffTree
                    {
                        character = secondSon.character,
                        probability = secondSon.probability,
                    };
                    //Se crea la nueva raíz que será padre de los dos nodos hermanos sin importar cuál sea el caso:
                    NodeHuffTree root = new NodeHuffTree
                    {
                        character = "N" + proxNode.ToString(),
                        probability = firstNewSon.probability + secondNewSon.probability,
                    };
                    //Ya que tenemos los hermanos, se valida si alguno de los dos ya está en la Raiz del HuffTree:  
                    //Si hay alguno, entonces se busca en la listAux si el otro que no era igual a la raíz está:
                    if ((rootOriginal.character == firstNewSon.character) || (rootOriginal.character == secondNewSon.character))
                    {
                        bool matchOnList = false;
                        int posMatch = 0;
                        //Se busca en la listAux:
                        for (int i = 0; i < listAux.Count; i++)
                        {
                            if ((listAux[i].character == firstNewSon.character) || (listAux[i].character == secondNewSon.character))
                            {
                                matchOnList = true;
                                posMatch = i;
                            }
                        }
                        //Si se encontró, entonces se saca... se hace hermano con el que ya está en la raíz del HuffTree:
                        if (matchOnList)
                        {
                            //Se obtiene el nodo de la lista y se elimina:
                            NodeHuffTree valueMatchOnList = listAux[posMatch];
                            listAux.RemoveAt(posMatch);
                            //Se hace una copia de la rootOriginal;
                            HuffTree auxilarClone = (HuffTree)_root.Clone();
                            //Ya que tenemos los dos valores, solo insertamos de la manera correcta:
                            if (valueMatchOnList.probability > auxilarClone.rootOriginal.probability)
                            {
                                root.nodeLeft = auxilarClone.rootOriginal;
                                root.nodeRight = valueMatchOnList;
                            }
                            else
                            {
                                root.nodeLeft = valueMatchOnList;
                                root.nodeRight = auxilarClone.rootOriginal;
                            }
                            //Se agrega el padre a los dos hermanos:
                            valueMatchOnList.nodeFather = root;
                            auxilarClone.rootOriginal.nodeFather = root;
                            //Se cambia la raíz
                            rootOriginal = root;
                            //Se agrega el padre creado a la cola:
                            NodeTable toInsertAgainOnQueue = new NodeTable
                            {
                                character = "N" + proxNode.ToString(),
                                probability = root.probability
                            };
                            proxNode++;
                            valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                        }
                        //Si no se encontró el otro en la lista... entonces solo se hace hermano con la raíz (el que no es igual):
                        else
                        {
                            //Se hace una copia de la rootOriginal;
                            HuffTree auxilarClone = (HuffTree)_root.Clone();
                            //Si es igual al firstNewSon, entonces la raíz se volverá hermano con el secondNewSon 
                            if (auxilarClone.rootOriginal.character == firstNewSon.character)
                            {                                
                                if (secondNewSon.probability > auxilarClone.rootOriginal.probability)
                                {
                                    root.nodeLeft = auxilarClone.rootOriginal;
                                    root.nodeRight = secondNewSon;
                                }
                                else
                                {
                                    root.nodeLeft = secondNewSon;
                                    root.nodeRight = auxilarClone.rootOriginal;
                                }
                                auxilarClone.rootOriginal.nodeFather = root;
                                secondNewSon.nodeFather = root;
                            }
                            //Si es igual al secondNewSon, entonces la raíz se volverá hermano con el firstNewSon 
                            else if (auxilarClone.rootOriginal.character == secondNewSon.character)
                            {              
                                if (firstNewSon.probability > auxilarClone.rootOriginal.probability)
                                {
                                    root.nodeLeft = auxilarClone.rootOriginal;
                                    root.nodeRight = firstNewSon;
                                }
                                else
                                {
                                    root.nodeLeft = firstNewSon;
                                    root.nodeRight = auxilarClone.rootOriginal;
                                }
                                auxilarClone.rootOriginal.nodeFather = root;
                                firstNewSon.nodeFather = root;
                            }
                            //Se cambia la raíz
                            rootOriginal = root;
                            //Se agrega el padre creado a la cola:
                            NodeTable toInsertAgainOnQueue = new NodeTable
                            {
                                character = "N" + proxNode.ToString(),
                                probability = root.probability
                            };
                            proxNode++;
                            valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                        }
                    }
                    //Si no hay alguno, entonces:
                    else
                    {
                        //Se busca en la listAux para encontrar alguno de los 2:
                        bool matchOnList = false;
                        for (int i = 0; i < listAux.Count; i++)
                        {
                            if ((listAux[i].character == firstNewSon.character) || (listAux[i].character == secondNewSon.character))
                            {
                                matchOnList = true;
                            }
                        }
                        //Si no se encuentra, entonces solo se inserta el root en la lista:
                        if (!matchOnList)
                        {
                            if (secondNewSon.probability > firstNewSon.probability)
                            {
                                root.nodeLeft = firstNewSon;
                                root.nodeRight = secondNewSon;
                            }
                            else
                            {
                                root.nodeLeft = secondNewSon;
                                root.nodeRight = firstNewSon;
                            }
                            //Se agrega el padre a los dos hermanos:
                            firstNewSon.nodeFather = root;
                            secondNewSon.nodeFather = root;
                            //Se agrega el padre creado a la cola:
                            NodeTable toInsertAgainOnQueue = new NodeTable
                            {
                                character = "N" + proxNode.ToString(),
                                probability = root.probability
                            };
                            proxNode++;
                            valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                            //Se inserta en la listAux
                            listAux.Add(root);
                        }
                        //Si se encuentra, entonces se busca si los 2 están en la lista o si solo 1 está:
                        else
                        {
                            //Se busca denuevo en la lista, pero esta vez, ambos por separado:
                            bool matchFirstNewSon = false;
                            int posMatchFirstNewSon = 0;
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i].character == firstNewSon.character)
                                {
                                    matchFirstNewSon = true;
                                    posMatchFirstNewSon = i;
                                }
                            }
                            bool matchSecondNewSon = false;
                            int posMatchSecondNewSon = 0;
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i].character == secondNewSon.character)
                                {
                                    matchSecondNewSon = true;
                                    posMatchSecondNewSon = i;
                                }
                            }  
                            //Si encuentra a los dos en la lista:
                            if (matchFirstNewSon && matchSecondNewSon)
                            {
                                //Se guardan y se remueven de la lista:
                                NodeHuffTree valueMatchOnList1 = listAux[posMatchFirstNewSon];
                                listAux.RemoveAt(posMatchFirstNewSon);
                                NodeHuffTree valueMatchOnList2 = listAux[posMatchSecondNewSon];
                                listAux.RemoveAt(posMatchSecondNewSon);
                                //Se valida en qué posición van (izquierda, derecha):
                                if (valueMatchOnList2.probability > valueMatchOnList1.probability)
                                {
                                    root.nodeLeft = valueMatchOnList1;
                                    root.nodeRight = valueMatchOnList2;
                                }
                                else
                                {
                                    root.nodeLeft = valueMatchOnList2;
                                    root.nodeRight = valueMatchOnList1;
                                }
                                //Se agrega el padre a los dos hermanos:
                                valueMatchOnList1.nodeFather = root;
                                valueMatchOnList2.nodeFather = root;
                                //Se agrega el padre creado a la cola:
                                NodeTable toInsertAgainOnQueue = new NodeTable
                                {
                                    character = "N" + proxNode.ToString(),
                                    probability = root.probability
                                };
                                proxNode++;
                                valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                                //Se inserta en la listAux
                                listAux.Add(root);
                            }
                            //Si solo se encuentra uno en la lista:
                            else if(matchFirstNewSon || matchSecondNewSon)
                            {
                                //Si solo se encuentra el matchFirstNewSon
                                if (matchFirstNewSon)
                                {
                                    //Se guardan y se remueven de la lista:
                                    NodeHuffTree valueMatchOnList1 = listAux[posMatchFirstNewSon];
                                    listAux.RemoveAt(posMatchFirstNewSon);
                                    //Se valida en qué posición van (izquierda, derecha):
                                    if (secondNewSon.probability > valueMatchOnList1.probability)
                                    {
                                        root.nodeLeft = valueMatchOnList1;
                                        root.nodeRight = secondNewSon;
                                    }
                                    else
                                    {
                                        root.nodeLeft = secondNewSon;
                                        root.nodeRight = valueMatchOnList1;
                                    }
                                    //Se agrega el padre a los dos hermanos:
                                    valueMatchOnList1.nodeFather = root;
                                    secondNewSon.nodeFather = root;
                                    //Se agrega el padre creado a la cola:
                                    NodeTable toInsertAgainOnQueue = new NodeTable
                                    {
                                        character = "N" + proxNode.ToString(),
                                        probability = root.probability
                                    };
                                    proxNode++;
                                    valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                                    //Se inserta en la listAux
                                    listAux.Add(root);
                                }
                                //Si solo se encuentra el matchSecondNewSon
                                else if (matchSecondNewSon)
                                {
                                    //Se guardan y se remueven de la lista:
                                    NodeHuffTree valueMatchOnList2 = listAux[posMatchSecondNewSon];
                                    listAux.RemoveAt(posMatchSecondNewSon);
                                    //Se valida en qué posición van (izquierda, derecha):
                                    if (firstNewSon.probability > valueMatchOnList2.probability)
                                    {
                                        root.nodeLeft = valueMatchOnList2;
                                        root.nodeRight = firstNewSon;
                                    }
                                    else
                                    {
                                        root.nodeLeft = firstNewSon;
                                        root.nodeRight = valueMatchOnList2;
                                    }
                                    //Se agrega el padre a los dos hermanos:
                                    valueMatchOnList2.nodeFather = root;
                                    firstNewSon.nodeFather = root;
                                    //Se agrega el padre creado a la cola:
                                    NodeTable toInsertAgainOnQueue = new NodeTable
                                    {
                                        character = "N" + proxNode.ToString(),
                                        probability = root.probability
                                    };
                                    proxNode++;
                                    valuesToInsert.Insert(toInsertAgainOnQueue, root.probability);
                                    //Se inserta en la listAux
                                    listAux.Add(root);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddBinary(NodeHuffTree paint, string listbinary)
        {
            if (paint != null)
            {
                Console.WriteLine(paint.character);
                
                if (paint.probability == 1)
                {
                    paint.nodeLeft.binary += "0";
                    paint.nodeRight.binary += "1";
                    Console.WriteLine("Lista del izquierdo: "+paint.nodeLeft.binary+ " Lista del derecho: " + paint.nodeRight.binary);
                    AddBinary(paint.nodeLeft, paint.binary);

                }
                else
                {
                    if (paint.nodeLeft != null)
                    {
                        paint.binary += listbinary + "0";
                        Console.WriteLine(paint.binary);
                        AddBinary(paint.nodeLeft, paint.binary);
                    }
                    else if (paint.nodeLeft == null & paint.nodeRight!=null)
                    {
                        AddBinary(paint.nodeRight, paint.binary);
                    }
                }
                //AddBinary(paint.nodeLeft,paint.binary);
                //AddBinary(paint.nodeRight);
            }
        }

        //public void AddBinary(NodeHuffTree paint)
        //{
        //    if (paint != null)
        //    {
        //        if (paint.probability != 1)
        //        {
        //            Console.WriteLine(paint.character);
        //            AddBinary(paint.nodeLeft);
        //            AddBinary(paint.nodeRight);
        //        }

        //    }
        //}

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public class NodeHuffTree
        {
            public double probability;
            public string character;
            public string binary;            
            public NodeHuffTree nodeLeft, nodeRight, nodeFather;
        }
    }
}