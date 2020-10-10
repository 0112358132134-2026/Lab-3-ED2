using System;
using System.Collections.Generic;
namespace Huffman
{
    public class HuffQueue <T> where T : IComparable
    {
        //Debe tener una lista que servirá para reordenar luego de eliminar:
        public List<string> listAux = new List<string>();
        public NodoCola Root;
        public int Count, Nodes;
        public HuffQueue()
        {
            Root = null;
            Count = 0;
            Nodes = 0;
        }

        public void Insert(T _data, double _priority)
        {
            NodoCola nuevo = new NodoCola(_data, _priority, null);
            if (Root == null)
            {
                Root = nuevo;
                Nodes++;
            }
            else
            {
                Root = InsertCP(nuevo, Root, null);
                Nodes++;
            }
            //Se inserta el valor que se insertó, a la listAux
            object aux = _data;
            NodeTable add = (NodeTable)aux;
            listAux.Add(add.character);
        }
        public NodoCola InsertCP(NodoCola _new, NodoCola _root, NodoCola _nodoFather)
        {
            Count = 0;
            if ((_root.NodoLeft == null) && (_root.NodoRight == null))
            {
                _root.NodoLeft = _new;
                _root.NodoFather = _nodoFather;
                _root.NodoLeft.NodoFather = _root;
                Swing(_new);
            }
            else if ((_root.NodoLeft != null) && (_root.NodoRight == null))
            {
                _root.NodoRight = _new;
                _root.NodoFather = _nodoFather;
                _root.NodoRight.NodoFather = _root;
                Swing(_new);
            }
            else if ((Sons(_root.NodoLeft, _root.NodoLeft)) == (Sons(_root.NodoRight, _root.NodoRight)))
            {
                InsertCP(_new, _root.NodoLeft, _root);
            }
            else if ((Sons(_root.NodoLeft, _root.NodoLeft)) > (Sons(_root.NodoRight, _root.NodoRight)))
            {
                if ((Sons(_root.NodoLeft.NodoLeft, _root.NodoLeft.NodoLeft)) == (Sons(_root.NodoLeft.NodoRight, _root.NodoLeft.NodoRight)))
                {
                    InsertCP(_new, _root.NodoRight, _root);
                }
                else
                {
                    InsertCP(_new, _root.NodoLeft, _root);
                }
            }
            //Update height:
            if ((_root.NodoLeft == null) && (_root.NodoRight != null))
            {
                _root.Height = _root.NodoRight.Height + 1;
            }
            else if ((_root.NodoRight == null) && (_root.NodoLeft != null))
            {
                _root.Height = _root.NodoLeft.Height + 1;
            }
            else
            {
                _root.Height = Math.Max(ObtainsFE(_root.NodoLeft), ObtainsFE(_root.NodoRight)) + 1;
            }
            return _root;
        }
        public int Sons(NodoCola _new, NodoCola _root)
        {
            int R = S(_new, _root);
            Count = 0;
            return R;
        }
        public int S(NodoCola _new, NodoCola _root)
        {
            if (_new != null)
            {
                S(_new.NodoLeft, _root);
                S(_new.NodoRight, _root);
                Count += 1;
            }
            return Count;
        }
        public void Swing(NodoCola _new)
        {
            if (_new.NodoFather != null)
            {
                if (_new.Priority < _new.NodoFather.Priority)
                {
                    T DataAux1 = _new.Data, DataAux2 = _new.NodoFather.Data;
                    double Aux1 = _new.Priority, Aux2 = _new.NodoFather.Priority;
                    _new.Data = DataAux2;
                    _new.NodoFather.Data = DataAux1;
                    _new.Priority = Aux2;
                    _new.NodoFather.Priority = Aux1;
                }
                else if (_new.Priority == _new.NodoFather.Priority)
                {
                    if (_new.Data.CompareTo(_new.NodoFather.Data) == -1)
                    {
                        T DataAux1 = _new.Data, DataAux2 = _new.NodoFather.Data;
                        double Aux1 = _new.Priority, Aux2 = _new.NodoFather.Priority;
                        _new.Data = DataAux2;
                        _new.NodoFather.Data = DataAux1;
                        _new.Priority = Aux2;
                        _new.NodoFather.Priority = Aux1;
                    }
                }
                Swing(_new.NodoFather);
            }
        }
        public void DeleteRoot(HuffQueue<T> _root)
        {
            //Se elimina primero de la lista
            object deleteList = _root.ReturnRoot();
            NodeTable delete = (NodeTable)deleteList;
            listAux.Remove(delete.character);
            //
            if (Nodes > 0)
            {
                HuffQueue<T> Aux = (HuffQueue<T>)_root.Clone();
                List<int> Fathers = new List<int>();
                int Pos = Nodes;
                while (Pos > 1)
                {
                    Fathers.Add(Pos);
                    Pos /= 2;
                }
                for (int i = Fathers.Count - 1; i >= 0; i--)
                {
                    if (Fathers[i] % 2 != 0)
                    {
                        Aux.Root = Aux.Root.NodoRight;
                    }
                    else
                    {
                        Aux.Root = Aux.Root.NodoLeft;
                    }
                }
                _root.Root.Priority = Aux.Root.Priority;
                _root.Root.Data = Aux.Root.Data;
                List<int> Invert = new List<int>();
                for (int i = Fathers.Count - 1; i >= 0; i--)
                {
                    Invert.Add(Fathers[i]);
                }       
                DeleteRootF(Invert, _root.Root);                
                Order(_root.Root);
                Nodes--;
            }
        }
        public NodoCola DeleteRootF(List<int> _Sons, NodoCola _root)
        {
            while ((_root.NodoLeft != null) || (_root.NodoRight != null))
            {
                int Pos0 = _Sons[0];
                _Sons.Remove(Pos0);
                if (Pos0 % 2 != 0)
                {
                    return DeleteRootF(_Sons, _root.NodoRight);
                }
                else
                {
                    return DeleteRootF(_Sons, _root.NodoLeft);
                }
            }
            if ((_root.NodoLeft == null) && (_root.NodoRight == null))
            {
                if (_root.NodoFather == null)
                {
                    Root = null;
                    return Root;
                }
                else if (_root.NodoFather != null)
                {
                    if (Nodes % 2 != 0)
                    {
                        _root.NodoFather.NodoRight = null;
                        return _root;
                    }
                    else
                    {
                        _root.NodoFather.NodoLeft = null;
                        return _root;
                    }
                }
            }
            return _root;
        }
        public void Order(NodoCola _root)
        {
            if (_root != null)
            {
                if (_root.NodoRight != null && _root.NodoLeft != null)
                {
                    if ((_root.Priority >= _root.NodoLeft.Priority) && (_root.Priority >= _root.NodoRight.Priority))
                    {
                        if (_root.NodoLeft.Priority < _root.NodoRight.Priority)
                        {
                            Change(_root, _root.NodoLeft);
                            Order(_root.NodoLeft);
                        }
                        else if (_root.NodoLeft.Priority > _root.NodoRight.Priority)
                        {
                            Change(_root, _root.NodoRight);
                            Order(_root.NodoRight);
                        }
                        else if (_root.NodoLeft.Priority == _root.NodoRight.Priority)
                        {
                            //Primero se valida si los valores de los hijos son menores o iguales al del padre:
                            //Si son menores, se debe validar quién de los 2 hijo debe subir al padre:
                            if (_root.NodoLeft.Priority < _root.Priority)
                            {
                                //Se convierte la data del _root.NodoLeft en NodeTable y la data del _root.NodoRight en Node Table
                                object aux1 = _root.NodoLeft.Data;
                                NodeTable auxiliar1 = (NodeTable)aux1;
                                object aux2 = _root.NodoRight.Data;
                                NodeTable auxiliar2 = (NodeTable)aux2;
                                //Ya que existen, se van a buscar a la listAux y se valida si va antes o después:
                                int posLeft = 0;
                                int posRight = 0;
                                for (int i = 0; i < listAux.Count; i++)
                                {
                                    if (listAux[i] == auxiliar1.character)
                                    {
                                        posLeft = i;
                                    }
                                    if (listAux[i] == auxiliar2.character)
                                    {
                                        posRight = i;
                                    }
                                }
                                //Si el character del _root.NodoLeft se insertó antes, entonces se sube el izquierdo:
                                if (posLeft < posRight)
                                {
                                    Change(_root, _root.NodoLeft);
                                    Order(_root.NodoLeft);
                                }
                                else if (posRight < posLeft)
                                {
                                    Change(_root, _root.NodoRight);
                                    Order(_root.NodoRight);
                                }
                            }
                            //Si son iguales
                            else if (_root.NodoLeft.Priority == _root.Priority)
                            {
                                //Validar si la ráiz se insertó primero:
                                //Se convierte la data del _root.NodoLeft en NodeTable, la data del _root.NodoRight en Node Table y la data del _root en Nodo Table:
                                object aux1 = _root.NodoLeft.Data;
                                NodeTable auxiliar1 = (NodeTable)aux1;
                                object aux2 = _root.NodoRight.Data;
                                NodeTable auxiliar2 = (NodeTable)aux2;
                                object aux3 = _root.Data;
                                NodeTable auxiliar3 = (NodeTable)aux3;
                                //Ya que existen, se van a buscar a la listAux y se valida:
                                int posLeft = 0;
                                int posRight = 0;
                                int postRoot = 0;
                                for (int i = 0; i < listAux.Count; i++)
                                {
                                    if (listAux[i] == auxiliar1.character)
                                    {
                                        posLeft = i;
                                    }
                                    if (listAux[i] == auxiliar2.character)
                                    {
                                        posRight = i;
                                    }
                                    if (listAux[i] == auxiliar3.character)
                                    {
                                        postRoot = i;
                                    }
                                }
                                //Si la raíz no se insertó antes que los dos hijos, no se hace nada:
                                if ((postRoot < posLeft) && (postRoot < posRight))
                                {
                                }
                                //
                                else if (posLeft < posRight)
                                {
                                    Change(_root, _root.NodoLeft);
                                    Order(_root.NodoLeft);
                                }
                                else if (posRight < posLeft)
                                {
                                    Change(_root, _root.NodoRight);
                                    Order(_root.NodoRight);
                                }
                            }
                        }
                    }
                    else if ((_root.Priority >= _root.NodoLeft.Priority) && (_root.Priority < _root.NodoRight.Priority))
                    {
                        if (_root.Priority == _root.NodoLeft.Priority)
                        {
                            //Se convierte la data del _root en NodeTable y la data del _root.NodoLeft en Node Table
                            object aux1 = _root.Data;
                            NodeTable auxiliar1 = (NodeTable)aux1;
                            object aux2 = _root.NodoLeft.Data;
                            NodeTable auxiliar2 = (NodeTable)aux2;
                            //Ya que existen, se van a buscar a la listAux y se valida si va antes o después:
                            int posRoot = 0;
                            int posLeft = 0; 
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i] == auxiliar1.character)
                                {
                                    posRoot = i;
                                }
                                if (listAux[i] == auxiliar2.character)
                                {
                                    posLeft = i;
                                }
                            }
                            //Si el character del _root.NodoLeft se insertó antes, entonces se cambia de lugar:
                            if (posLeft < posRoot)
                            {
                                Change(_root, _root.NodoLeft);
                                Order(_root.NodoLeft);
                            }
                        }
                        else
                        {
                            Change(_root, _root.NodoLeft);
                            Order(_root.NodoLeft);
                        }
                    }
                    else if ((_root.Priority < _root.NodoLeft.Priority) && (_root.Priority >= _root.NodoRight.Priority))
                    {
                        if (_root.Priority == _root.NodoRight.Priority)
                        {
                            //Se convierte la data del _root en NodeTable y la data del _root.NodoRight en Node Table
                            object aux1 = _root.Data;
                            NodeTable auxiliar1 = (NodeTable)aux1;
                            object aux2 = _root.NodoRight.Data;
                            NodeTable auxiliar2 = (NodeTable)aux2;
                            //Ya que existen, se van a buscar a la listAux y se valida si va antes o después:
                            int posRoot = 0;
                            int posRight = 0;
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i] == auxiliar1.character)
                                {
                                    posRoot = i;
                                }
                                if (listAux[i] == auxiliar2.character)
                                {
                                    posRight = i;
                                }
                            }
                            //Si el character del _root.NodoRight se insertó antes, entonces se cambia de lugar:
                            if (posRight < posRoot)
                            {
                                Change(_root, _root.NodoRight);
                                Order(_root.NodoRight);
                            }
                        }
                        else
                        {
                            Change(_root, _root.NodoRight);
                            Order(_root.NodoRight);
                        }
                    }
                }
                else if ((_root.NodoRight != null && _root.NodoLeft == null))
                {
                    if (_root.Priority >= _root.NodoRight.Priority)
                    {
                        if (_root.Priority == _root.NodoRight.Priority)
                        {
                            //Se debe validar quién se insertó primero
                            //Se convierte la data del _root en NodeTable y la data del _root.NodoRight en Node Table
                            object aux1 = _root.Data;
                            NodeTable auxiliar1 = (NodeTable)aux1;
                            object aux2 = _root.NodoRight.Data;
                            NodeTable auxiliar2 = (NodeTable)aux2;
                            //Ya que existen, se van a buscar a la listAux y se valida si va antes o después:
                            int posRoot = 0;
                            int posRight = 0;
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i] == auxiliar1.character)
                                {
                                    posRoot = i;
                                }
                                if (listAux[i] == auxiliar2.character)
                                {
                                    posRight = i;
                                }
                            }
                            if (posRight < posRoot)
                            {
                                Change(_root, _root.NodoRight);
                                Order(_root.NodoRight);
                            }                                                       
                        }
                        else
                        {
                            Change(_root, _root.NodoRight);
                            Order(_root.NodoRight);
                        }
                    }
                }
                else if ((_root.NodoLeft != null && _root.NodoRight == null))
                {
                    if (_root.Priority >= _root.NodoLeft.Priority)
                    {
                        if (_root.Priority == _root.NodoLeft.Priority)
                        {
                            //Se debe validar quién se insertó primero
                            //Se convierte la data del _root en NodeTable y la data del _root.NodoLeft en Node Table
                            object aux1 = _root.Data;
                            NodeTable auxiliar1 = (NodeTable)aux1;
                            object aux2 = _root.NodoLeft.Data;
                            NodeTable auxiliar2 = (NodeTable)aux2;
                            //Ya que existen, se van a buscar a la listAux y se valida si va antes o después:
                            int posRoot = 0;
                            int posLeft = 0;
                            for (int i = 0; i < listAux.Count; i++)
                            {
                                if (listAux[i] == auxiliar1.character)
                                {
                                    posRoot = i;
                                }
                                if (listAux[i] == auxiliar2.character)
                                {
                                    posLeft = i;
                                }
                            }
                            if (posLeft < posRoot)
                            {
                                Change(_root, _root.NodoLeft);
                                Order(_root.NodoLeft);
                            }
                        }
                        else
                        {
                            Change(_root, _root.NodoLeft);
                            Order(_root.NodoLeft);
                        }
                    }
                }
            }
        }
        public void Change(NodoCola _change, NodoCola _other)
        {
            if ((_change.NodoLeft != null) || (_change.NodoRight != null))
            {
                double Aux1 = _change.Priority, Aux2 = _other.Priority;
                T DataAux1 = _change.Data, DataAux2 = _other.Data;
                _change.Data = DataAux2;
                _other.Data = DataAux1;
                _change.Priority = Aux2;
                _other.Priority = Aux1;
            }
        }
        public int ObtainsFE(NodoCola _root)
        {
            int number;
            if (_root == null)
            {
                number = -1;
            }
            else
            {
                number = _root.Height;
            }
            return number;
        }        
        public object ReturnRoot()
        {
            return Root.Data;
        }
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public class NodoCola
        {
            public int Height;
            public double Priority;
            public T Data;
            public NodoCola NodoLeft, NodoRight, NodoFather;
            public NodoCola(T _data, double _priority, NodoCola _nodoFather)
            {
                this.Data = _data;
                this.Priority = _priority;
                this.NodoLeft = null;
                this.NodoRight = null;
                this.NodoFather = _nodoFather;
            }
        }
    }
}
