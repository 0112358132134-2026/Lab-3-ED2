using System;
using System.Collections.Generic;

namespace Huffman
{
    public class HuffQueue <T> where T : IComparable
    {
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
                DeleteRoot(Invert, _root.Root);
                Order(_root.Root);
                Nodes--;
            }
        }
        public NodoCola DeleteRoot(List<int> _Sons, NodoCola _root)
        {
            while ((_root.NodoLeft != null) || (_root.NodoRight != null))
            {
                int Pos0 = _Sons[0];
                _Sons.Remove(Pos0);
                if (Pos0 % 2 != 0)
                {
                    return DeleteRoot(_Sons, _root.NodoRight);
                }
                else
                {
                    return DeleteRoot(_Sons, _root.NodoLeft);
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
                            if (_root.NodoLeft.Data.CompareTo(_root.NodoRight.Data) == 1)
                            {
                                Change(_root, _root.NodoRight);
                                Order(_root.NodoRight);
                            }
                            else if (_root.NodoLeft.Data.CompareTo(_root.NodoRight.Data) == -1)
                            {
                                Change(_root, _root.NodoLeft);
                                Order(_root.NodoLeft);
                            }
                            else if (_root.NodoLeft.Data.CompareTo(_root.NodoRight.Data) == 0)
                            {
                                Change(_root, _root.NodoLeft);
                                Order(_root.NodoLeft);
                            }
                        }
                    }
                    else if ((_root.Priority >= _root.NodoLeft.Priority) && (_root.Priority < _root.NodoRight.Priority))
                    {
                        if (_root.Priority == _root.NodoLeft.Priority)
                        {
                            if (_root.Data.CompareTo(_root.NodoLeft.Data) == 1)
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
                            if (_root.Data.CompareTo(_root.NodoRight.Data) == 1)
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
                            if (_root.Data.CompareTo(_root.NodoRight.Data) == 1)
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
                            if (_root.Data.CompareTo(_root.NodoLeft.Data) == 1)
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
