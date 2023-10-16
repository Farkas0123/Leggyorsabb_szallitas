﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leggyorsabb_szállítás
{
    internal class Program
    {
        class Kupac<T>
        {
            class Lista<R>
            {
                List<R> list = new List<R>();

                public void Add(R elem) => list.Add(elem);
                public void RemoveLast() => list.RemoveAt(list.Count - 1);
                public R this[int i]
                {
                    get => list[i - 1];
                    set => list[i - 1] = value;
                }
                public override string ToString()
                {
                    string sum = "[ ";
                    foreach (R item in list)
                    {
                        sum += item + " ";
                    }
                    return sum + "]";
                }
                public int Count { get => list.Count; }
            }

            Lista<T> lista = new Lista<T>();
            Func<T, T, int> relacio;

            public Kupac(Func<T, T, int> r)
            {
                this.relacio = r;
            }


            private int Szülő(int i) => i / 2;

            private void Csere(int i, int j) => (lista[i], lista[j]) = (lista[j], lista[i]);

            private bool Gyökér(int i) => i == 1;

            private void Bugyborékolj(int gyerek)
            {
                while (!Gyökér(gyerek) && relacio(lista[gyerek], lista[Szülő(gyerek)]) == 1)
                {
                    Csere(gyerek, Szülő(gyerek));
                    gyerek = Szülő(gyerek);
                }
            }

            private int Gyerekek(int szülő, out int gyerek1, out int gyerek2)
            {
                // emlékeztető
                /*
				int b;
				bool lehete = int.TryParse("5", out b);
				*/

                gyerek1 = 0;
                gyerek2 = 0;
                int gyerekszám = 0;
                if (2 * szülő <= lista.Count)
                {
                    gyerek1 = 2 * szülő;
                    gyerekszám = 1;
                    if (2 * szülő + 1 <= lista.Count)
                    {
                        gyerek2 = 2 * szülő + 1;
                        gyerekszám = 2;
                    }
                }
                return gyerekszám;
            }

            private bool Idősebbgyerek(int szülő, out int idősebb_gyerek)
            {
                int gyerek1;
                int gyerek2;
                int gyerekszám = Gyerekek(szülő, out gyerek1, out gyerek2);

                switch (gyerekszám)
                {
                    case 0:
                        idősebb_gyerek = 0;
                        return false;
                    case 1:
                        idősebb_gyerek = gyerek1;
                        return true;
                    case 2:
                        idősebb_gyerek = relacio(lista[gyerek1], lista[gyerek2]) == -1 ? gyerek2 : gyerek1;
                        return true;
                    default: // ide sose fog befutni a program
                        idősebb_gyerek = 0;
                        return true;
                }
            }

            private void Süllyesztés()
            {
                int szülő = 1;
                int idősebb_gyerek;
                bool vanegyerek = Idősebbgyerek(szülő, out idősebb_gyerek);
                while (vanegyerek && relacio(lista[szülő], lista[idősebb_gyerek]) == -1)
                {
                    Csere(szülő, idősebb_gyerek);
                    szülő = idősebb_gyerek;
                    vanegyerek = Idősebbgyerek(szülő, out idősebb_gyerek);
                }
            }

            public void Push(T elem)
            {
                lista.Add(elem);
                Bugyborékolj(lista.Count);
            }

            public T Pop()
            {
                Csere(1, lista.Count);
                T result = lista[lista.Count];
                lista.RemoveLast();
                Süllyesztés();
                return result;
            }

            public bool Empty() => lista.Count == 0;
            public override string ToString() => lista.ToString();

        }

        class Szomszedsagi_listas_graf
        {
            //List<List<int[]>> szomszedsagi_lista;

            int[,] Szomszedsagi_tomb;
            int[,] Szomszedsagi_suly;
            int N;
            int M;
            public int honnan;
            public int hova;
            public Szomszedsagi_listas_graf()
            {
                //szomszedsagi_lista = new List<List<int[]>>();


                string sor = Console.ReadLine();
                string[] sortomb = sor.Split(' ');
                N = int.Parse(sortomb[0]);
                M = int.Parse(sortomb[1]);

                Szomszedsagi_tomb = new int[N + 1, N + 1];
                Szomszedsagi_suly = new int[N + 1, N + 1];

                /*for (int i = 0; i < N+1; i++)//feltöltjük a listát, hogy ne fagyjon ki
                {
                    szomszedsagi_lista.Add(new List<int[]>());
                }*/

                for (int i = 0; i < N + 1; i++)
                {
                    for (int j = 0; j < N + 1; j++)
                    {
                        Szomszedsagi_tomb[i, j] = -1;
                    }
                }

                for (int i = 1; i < M + 1; i++)
                {
                    sor = Console.ReadLine();
                    sortomb = sor.Split(' ');
                    (int innen, int ide, int tav, int suly) = (int.Parse(sortomb[0]), int.Parse(sortomb[1]), int.Parse(sortomb[2]), int.Parse(sortomb[3]));


                    Szomszedsagi_tomb[innen, ide] = tav;
                    Szomszedsagi_tomb[ide, innen] = tav;
                    Szomszedsagi_suly[innen, ide] = suly;
                    Szomszedsagi_suly[ide, innen] = suly;

                    //szomszedsagi_lista[innen].Add(ide);
                }

                sor = Console.ReadLine();
                sortomb = sor.Split(' ');
                honnan = int.Parse(sortomb[0]);
                hova = int.Parse(sortomb[1]);
            }

            public List<int> szomszedai(int n)
            {
                List<int> s = new List<int>();

                for (int i = 0; i < N + 1; i++)
                {
                    if (Szomszedsagi_tomb[n, i] != -1)
                    {
                        s.Add(i);
                    }
                }
                return s;
            }

            public void Diagnosztika()
            {
                for (int i = 1; i < N + 1; i++)
                {
                    Console.Write($"[{i}]: ");
                    for (int j = 1; j < N + 1; j++)
                    {
                        //Console.Write($"| {Szomszedsagi_tomb[i,j]} ");

                        Console.Write(Szomszedsagi_tomb[i, j].ToString().Length == 2 ? $"| {Szomszedsagi_tomb[i, j]} " : $"|  {Szomszedsagi_tomb[i, j]} ");
                    }
                    Console.WriteLine();
                }
            }



            public (List<int>, int) Honnan_tomb_felgongyolitese(int[] honnan, int end)
            {
                List<int> result = new List<int>();

                int node = end;
                int maxsuly = int.MaxValue;

                while (node != -1)
                {
                    result.Add(node);
                    node = honnan[node];
                }

                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (maxsuly > Szomszedsagi_suly[result[i], result[i + 1]])
                    {
                        maxsuly = Szomszedsagi_suly[result[i], result[i + 1]];
                    }
                }

                result.Reverse();
                return (result, maxsuly);
            }



            /*public List<int> Legrovidebb_ut(int innen, int ide)
            {
                int fehér = 0;
                int szürke = 1;
                int fekete = 2;

                int[] szín = new int[szomszedsagi_lista.Count];

                Queue<int> tennivalok = new Queue<int>();
                tennivalok.Enqueue(innen);
                szín[innen] = szürke;


                List<int> result = new List<int>();

                int[] honnan = new int[szomszedsagi_lista.Count+1];


                for (int i = 0; i < szomszedsagi_lista.Count; i++)
                {
                    honnan[i] = -1;
                }

                while (tennivalok.Count != 0)
                {
                    int feldolgozando = tennivalok.Dequeue();
                    if (feldolgozando[0] == ide)
                    {
                        return Honnan_tomb_felgongyolitese(honnan, ide);
                    }
                    szín[feldolgozando] = fekete;

                    foreach (int szomszed in szomszedsagi_lista[feldolgozando])
                    {
                        if (szín[szomszed] == fehér)
                        {
                            tennivalok.Enqueue(szomszed);
                            honnan[szomszed] = feldolgozando;
                            szín[szomszed] = szürke;
                        }
                    }
                }
                return null;
            }*/


            public (int[], int[]) Dijkstra(int start)
            {
                //inicializáltuk a tav-ot
                int[] tav = new int[N + 1];
                for (int i = 1; i < N + 1; i++)
                    tav[i] = int.MaxValue; // végtelen megfelelője

                tav[start] = 0;

                // honnan inicializálás
                int[] honnan = new int[N + 1];
                for (int i = 1; i < N + 1; i++)
                    honnan[i] = -1;

                Kupac<int> kupac = new Kupac<int>((x, y) => tav[x] > tav[y] ? -1 : (tav[x] == tav[y] ? 0 : 1));

                for (int i = 1; i < N + 1; i++)
                {
                    kupac.Push(i);
                }

                //visited
                int suly = 0;
                bool[] voltamitt = new bool[N + 1];

                while (!kupac.Empty())
                {
                    int todo = kupac.Pop(); //kiszedünk egy elemet

                    if (tav[todo] == int.MaxValue) // amikor már végtelenek a legkisebbek, akkor abbahagyjuk
                        return (tav, honnan);


                    // Feldolgozás
                    voltamitt[todo] = true;

                    // Szomszédok:
                    List<int> szomszÉdai = szomszedai(todo);

                    foreach (int szomszed in szomszÉdai) //vesszük a szomszédokat
                    {
                        if (!voltamitt[szomszed] && tav[todo] + Szomszedsagi_tomb[todo, szomszed] < tav[szomszed])
                        {
                            honnan[szomszed] = todo;
                            tav[szomszed] = tav[todo] + Szomszedsagi_tomb[todo, szomszed];

                            kupac.Push(szomszed);
                            // megváltozott a súlya a szomszédnak, viszont a kupacban a helye nem változott. Ezért betesszük újra, így viszont sokszor lesz bent...
                            // kupac.Update(szomszed); // kellene egy ilyen!
                        }
                        else if (!voltamitt[szomszed] && tav[todo] + Szomszedsagi_tomb[todo, szomszed] == tav[szomszed])
                        {
                            if (suly < Szomszedsagi_suly[todo, szomszed])
                            {
                                suly = Szomszedsagi_suly[todo, szomszed];
                            }
                        }
                    }
                }
                return (tav, honnan);
            }
        }
        static void Main(string[] args)
        {
            Szomszedsagi_listas_graf graf = new Szomszedsagi_listas_graf();
            //graf.Diagnosztika();


            int cel = graf.hova;
            int start = graf.honnan;
            (int[] result, int[] oda) = graf.Dijkstra(start);

            (List<int> ut, int vegeredmeny) = graf.Honnan_tomb_felgongyolitese(oda, cel);
            /*Console.WriteLine($"{start}=>{cel}: {result[cel]} ");

            Console.WriteLine(string.Join(", ", ut));
            Console.WriteLine(vegeredmeny);*/

            Console.WriteLine($"{result[cel]} {vegeredmeny}");
            Console.WriteLine(string.Join(" ", ut));



        }
    }
}