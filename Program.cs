using System;
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
            //Bevezetem a privát és publikus globális változókat
            int[,] Szomszedsagi_tomb;
            int[,] Szomszedsagi_suly;
            int N;
            int M;
            public int honnan;
            public int hova;

            // Létrehozom a gráfot
            public Szomszedsagi_listas_graf()
            {
                //Beolvasom az elsó sort
                string sor = Console.ReadLine();
                string[] sortomb = sor.Split(' ');

                //Az első sor adatait elmentem
                N = int.Parse(sortomb[0]);
                M = int.Parse(sortomb[1]);

                // Létrehozom a táv- és súlytömböket 
                Szomszedsagi_tomb = new int[N + 1, N + 1]; // Azért N + 1, mert a MESTER-rel így a legegyszerűbb dolgozni
                Szomszedsagi_suly = new int[N + 1, N + 1];

                // A tömböt feltöltöm '-1' értékekkel ami a 'nincs adat'-ot jelöli
                for (int i = 0; i < N + 1; i++)
                {
                    for (int j = 0; j < N + 1; j++)
                    {
                        Szomszedsagi_tomb[i, j] = -1;
                    }
                }

                // Beolvasom az összes többi sort az utolsó kivételével
                for (int i = 1; i < M + 1; i++)
                {
                    sor = Console.ReadLine();
                    sortomb = sor.Split(' ');
                    // Egy sor 4 értékének összesét elmentem
                    (int innen, int ide, int tav, int suly) = (int.Parse(sortomb[0]), int.Parse(sortomb[1]), int.Parse(sortomb[2]), int.Parse(sortomb[3]));

                    // A fentebb mentett értékek összesét felhasználva az adatokat elmentem
                    Szomszedsagi_tomb[innen, ide] = tav;
                    Szomszedsagi_tomb[ide, innen] = tav;
                    Szomszedsagi_suly[innen, ide] = suly;
                    Szomszedsagi_suly[ide, innen] = suly;
                }

                // Beolvasom majd egy globális változóba mentem az utolsó sort
                sor = Console.ReadLine();
                sortomb = sor.Split(' ');
                honnan = int.Parse(sortomb[0]);
                hova = int.Parse(sortomb[1]);
            }

            // Szomszéd kereső algoritmus
            public List<int> szomszedai(int n)
            {
                List<int> s = new List<int>();

                for (int i = 0; i < N + 1; i++)
                {
                    //Ha van a sorban érték hozzáadom az indexet
                    if (Szomszedsagi_tomb[n, i] != -1) 
                    {
                        s.Add(i);
                    }
                }

                return s;
            }

            // A gráf kiírásáért felelős
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

            public ((int, int)[], int[]) Dijkstra(int start)
            {
                // Inicializálom a 'tav'-ot
                (int,int)[] tav = new (int,int)[N + 1]; // ide kell számpárokat létrehozni, hogy jogosan meg tudjam változtatni a relációt amikor inicializálom a kupacot
                for (int i = 1; i < N + 1; i++)
                {
                    tav[i] = (int.MaxValue,int.MaxValue); // végtelen megfelelője
                }
                tav[start] = (0,0);

                // A 'honnan' tömböt inicializálom
                int[] honnan = new int[N + 1];
                for (int i = 1; i < N + 1; i++)
                {
                    honnan[i] = -1; // Az ismeretlen értékek megfelelője
                }

                // Ez a reláció rendezi el az új kupacunkat
                Kupac<int> kupac = new Kupac<int>((x, y) => tav[x].Item1 > tav[y].Item1 ? -1 : (tav[x] == tav[y] ? 0 : 1));

                // Feltöltöm a kupacot (?)
                for (int i = 1; i < N + 1; i++)
                {
                    kupac.Push(i);
                }

                // Meghatározom azon pontokat ahol már jártam
                bool[] voltamitt = new bool[N + 1];

                // Ide mentem az akt verziót
                int suly = -1;

                //Elkezdem a keresést
                while (!kupac.Empty())
                {
                    // Kiszedem a legfelső elemet ami a "todo" lesz
                    int todo = kupac.Pop();

                    // Amikor már végtelenek a legkisebbek, akkor abbahagyjuk
                    if (tav[todo].Item1 == int.MaxValue)
                    {
                        return (tav, honnan);
                    }

                    // Elmentem, hogy voltam itt
                    voltamitt[todo] = true;

                    // Megkeresem a "todo" elem összes szomszédját
                    List<int> szomszÉdai = szomszedai(todo);

                    // Bevezetek néhány olvasás egyszerűsítő változót
                    bool jártame            = false;
                    int todo_tavolsága      = 0;
                    int új_elem_a_todotól   = 0;
                    int szomszed_tavja      = 0;

                    // Végig veszem az összes szomszédját a "todo" elemnek
                    foreach (int szomszed in szomszÉdai) 
                    {
                        jártame             = voltamitt[szomszed];
                        todo_tavolsága      = tav[todo].Item1;
                        új_elem_a_todotól   = Szomszedsagi_tomb[todo, szomszed];
                        szomszed_tavja      = tav[szomszed].Item1;

                        // Két esetben kell figyelembe vennünk a relációt
                        // 1. : Ha az új út távolsága egyértelműen kisebb mint a régié. Ebben az esetben nem számít a súly.
                        if (!jártame && todo_tavolsága + új_elem_a_todotól < szomszed_tavja)
                        {
                            // Megváltoztatom az "elérés"-t
                            honnan[szomszed]    = todo;

                            // Elmentem az új elérés távját
                            szomszed_tavja      = todo_tavolsága + új_elem_a_todotól;

                            // Hozzáadom a teendőkhöz a szomszédot
                            kupac.Push(szomszed);
                        }
                        // 2. : Ha az új út távolsága megegyezik a régivel. Ilyenkor figyelembe kell vennünk a súlybeli eltérést.
                        else if (!jártame && todo_tavolsága + új_elem_a_todotól == szomszed_tavja)
                        {
                            if (suly < Szomszedsagi_suly[todo, szomszed])
                            {
                                //Megváltoztatom az aktuális maximum súlyt
                                suly                = Szomszedsagi_suly[todo, szomszed];

                                // Megváltoztatom az "elérés"-t
                                honnan[szomszed]    = todo;

                                // Elmentem az új elérés távját
                                szomszed_tavja      = todo_tavolsága + új_elem_a_todotól;

                                // Hozzáadom a teendőkhöz a szomszédot
                                kupac.Push(szomszed);
                            }
                        }
                    }
                }

                return (tav, honnan);
            }
        }
        static void Main(string[] args)
        {
            // A gráfot implementálom
            Szomszedsagi_listas_graf graf = new Szomszedsagi_listas_graf();

            // A változókat bevezetem
            int cel = graf.hova;
            int start = graf.honnan;

            // A függvényeket meghívom
            ((int, int)[] result, int[] oda) = graf.Dijkstra(start);
            (List<int> ut, int vegeredmeny) = graf.Honnan_tomb_felgongyolitese(oda, cel);

            // A végeredményt kiírom
            Console.WriteLine($"{result[cel].Item1} {vegeredmeny}");
            Console.WriteLine(string.Join(" ", ut));



        }
    }
}
