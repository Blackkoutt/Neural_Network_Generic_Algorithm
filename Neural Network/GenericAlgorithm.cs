using System.Diagnostics;

namespace Neural_Network
{
    public class GenericAlgorithm
    {
        private List<string> CitiesList;    // Lista miast
        private int IndexOfStartCity;   // Index miasta startowego w macierzy odległości
        private int[,] dist;    // Macierz odległości
        private double CrossoverPropability;    // Prawdopodobieństwo krzyżowania
        private double MutationPropability; // Prawdopodobieństwo mutacji
        private int PopulationSize; // Rozmiar populacji
        private int BestDist;   // Najmniejsza uzyskana długość trasy
        private int[] BestIndividualOfPopulation;   // Genotyp najlepiej przystosowanego osobnika
        private int ChromosomesCount;   // Liczba chromosomów - liczba miast
        private List<List<int>> Population; // Cała populacja [osobnik, jego genotyp]
        private List<int> FitnessValueList; // Lista najlepszej wartości fitness w każdym pokoleniu (do wykresu FitnessPlot)
        private double[] FitnessTab;    // Tablica dopasowania

        public GenericAlgorithm(List<string> CitiesList, double CrossoverPropability, double MutationPropability, int PopulationSize)
        {
            this.CrossoverPropability = CrossoverPropability;
            this.MutationPropability = MutationPropability;
            this.PopulationSize = PopulationSize;
            this.CitiesList = CitiesList;
            ChromosomesCount = CitiesList.Count - 1;
            dist = new int[CitiesList.Count, CitiesList.Count];
            Population = new List<List<int>>();
            FitnessTab = new double[PopulationSize];
            FitnessValueList = new List<int>();
            BestDist = int.MaxValue;    // Najmniejsza długość trasy początkowo ustawiona na nieskończoność aby móc porównywać
            BestIndividualOfPopulation = new int[ChromosomesCount];
            ReadDataFromFile(CitiesList);   // Odczyt macierzy odległości z pliku
            InitPopulation();   // Inicjalizacja populacji - losowe początkowe dobranie chromosomów każdemu osobnikowi
        }
        // W przypadku np 7 miast wybranych przez sieć neuronową mamy 6!=720 możliwości ułożenia miast

        // Inicjalizacja populacji poprzez przypisanie losowych wartości chromosomów
        private void InitPopulation()
        {
            Random rand = new Random();
            int randomNum;

            // Stworz tyle osobników jaki jest podany rozmiar populacji
            for (int i = 0; i < PopulationSize; i++)
            {
                List<int> pomList = new List<int>();

                // Dla każdego osobnika wylosuj chromosomy (indexy miast)
                for (int j = 0; j < ChromosomesCount; j++)
                {
                    // Jeżeli nie jest to pierwszy chromosom to wylosuj wartości chromosomów tak aby się nie powtarzały oraz 
                    // żeby nie wylosować indexu miasta startowego
                    if (pomList.Count != 0)
                    {
                        do
                        {
                            randomNum = rand.Next(0, ChromosomesCount + 1);
                        } while (randomNum == IndexOfStartCity || pomList.Contains(randomNum));
                        pomList.Add(randomNum);
                    }
                    // Jeśli jest to pierwszy chromosom wylosuj jego wartość, która nie może być indexem miasta startowego
                    else
                    {
                        do
                        {
                            randomNum = rand.Next(0, ChromosomesCount);
                        } while (randomNum == IndexOfStartCity);
                        pomList.Add(randomNum);
                    }
                }
                Population.Add(pomList);    // Dodanie nowego osobnika do populacji
            }
        }

        // Funkcja przystosowania
        private void Fitness()
        {
            int fitness_value;
            // Dla każdego osobnika populacji
            for (int i = 0; i < PopulationSize; i++)
            {
                fitness_value = 0;
                // Dla każdego chromosomu danego osobnika
                for (int j = 0; j < ChromosomesCount; j++)
                {
                    // Wartość funkcji dopasowania to suma odległości między kolejnymi miastami 
                    if (j != ChromosomesCount - 1)
                    {
                        fitness_value += dist[Population[i][j], Population[i][j + 1]];
                    }
                }
                // Do tego wchodzi jeszcze odległość między miastem startowym a pierwszym miastem osobnika (pierwzym chromosomem)
                // i odległość między miastem startowym a ostatnim miastem osobnika (ostatnim chromosomem)
                fitness_value += dist[IndexOfStartCity, Population[i][0]];
                fitness_value += dist[Population[i][ChromosomesCount - 1], IndexOfStartCity];
                // Jeśli wartość funkcji dopasowania (długość trasy) jest mniejsza od aktualnie znalezionej najlepszej wartości
                // Ustaw daną wartość jako najlepszą i zapisz pełny genotyp osobnika
                if (fitness_value < BestDist)
                {
                    BestDist = fitness_value;
                    BestIndividualOfPopulation = Population[i].ToArray();
                }
                // Odwróć wartość fitness i zapisz w tablicy
                if (fitness_value != 0)
                {
                    FitnessTab[i] = (1 / (double)fitness_value);
                }
                else
                {
                    FitnessTab[i] = (double)fitness_value;
                }

            }
        }

        // Funkcja określająca prawdopodobieństwo selekcji dla wszytkich osobników
        private void SelectionPropability()
        {
            double sum = 0;
            // Zsumuj wszystkie wartości fitness osobników danego pokolenia
            for (int i = 0; i < FitnessTab.Length; i++)
            {
                sum += FitnessTab[i];
            }

            // Każdą wartość fitness podziel przez sumę i ponownie zapisz do tablicy fitness - w ten sposób uzyskiwane jest prawdopodobieństwo selekcji
            // Im wyższe tym osobnik ma większą szansę na zostanie rodzicem
            for (int i = 0; i < FitnessTab.Length; i++)
            {
                FitnessTab[i] = FitnessTab[i] / sum;
            }
        }

        // Funkcja przeprowadzająca selekcję osobników rodzicielskich zgodnie z algorytmem koła ruletki
        private List<int> SelectParentFromPopulation()
        {
            int i = 0;
            Random rand = new Random();
            double random_value = rand.NextDouble();
            // Dopóki wylosowana liczba jest większa od 0
            while (random_value > 0)
            {
                // Odejmuj od wylosowanej liczby wartość zapisaną w funckji fitness
                // Na ten moment są tutaj prawodpodobieństwa selekcji każdego z osobników
                random_value = random_value - FitnessTab[i];
                i++;
            }
            i--;
            return Population[i];   // Zwróć osobnika który został wybrany jako rodzic
        }

        // Funkcja przeprowadzająca krzyżowanie osobników rodzicielskich
        private void Crossover(List<List<int>> Parents)
        {
            Random rand = new Random();
            int i;
            List<int> FirstChild = new List<int>();   // Pierwszy potomek
            List<int> SecondChild = new List<int>(); // Drugi potomek

            // Jeśli liczba rodziców jest nieparzysta (nieparzysta ilość osobników)
            // Losowo wybierz czy zacząć od pierwszego osobnika czy od drugiego
            // W ten sposób każdorazowo nie jest pomijany ostatni osobnik w przypadku nieparzystej ich ilości
            if (Parents.Count % 2 == 1)
            {
                i = rand.Next(0, 2);
            }
            else
            {
                i = 0;
            }
            // i w pętli zwiększane jest o 2 ponieważ każde dwa kolejne osobniki zostają rodzicami
            while (i < Parents.Count - 1)
            {
                // Sprawdź czy wylosowana liczba znajduje się w przedziale [0, prawdopodobieństwo krzyżowania]
                if (rand.NextDouble() <= CrossoverPropability)
                {
                    // Czyszczenie list zawierających chromosomy potomków
                    FirstChild.Clear();
                    SecondChild.Clear();
                    // Losowo wybierz index chromosomu od którego zacząć
                    int First_Value = rand.Next(ChromosomesCount);
                    // Losowo wybierz index chromosomu na którym skończyć 
                    int Second_Value = rand.Next(First_Value + 1, ChromosomesCount);

                    // Z pierwszego rodzica brane są chromosomy wybrane losowo od indexu FristValue do indexu SecondValue
                    // I przypisywane są pierwszemu potomkowi
                    for (int j = First_Value; j < Second_Value; j++)
                    {
                        FirstChild.Add(Parents[i][j]);
                    }

                    // Pozostałe chromosomy pierwszego rodzica które nie zostały wybrane do pierwszego potomka przypisywane są drugiemu
                    for (int j = 0; j < ChromosomesCount; j++)
                    {
                        if (!FirstChild.Contains(Parents[i][j]))
                        {
                            SecondChild.Add(Parents[i][j]);
                        }
                    }


                    for (int j = 0; j < ChromosomesCount; j++)
                    {
                        // Z drugiego rodzica przypisz pierwszemu potomkowi te chromosomy których jeszcze nie posiada
                        if (!FirstChild.Contains(Parents[i + 1][j]))
                        {
                            FirstChild.Add(Parents[i + 1][j]);
                        }
                        // Te które posiada przypisz drugiemu potomkowi
                        else
                        {
                            SecondChild.Add(Parents[i + 1][j]);
                        }
                    }
                    // Zaktualizuj rodziców ich potomkami
                    Population[i].Clear();
                    Population[i].AddRange(FirstChild);
                    Population[i + 1].Clear();
                    Population[i + 1].AddRange(SecondChild);
                }
                i += 2; // Przejdź do następnych rodziców
            }
            // Takie rozwiązanie zapewnia unikalność chromosomów
            // nie ma sytuacji gdy w genotypie danego osobnika znajdują się takie same chromosomy
        }

        // Funkcja przeprowadzająca mutację osobników
        private void Mutation(int Individual)
        {
            Random rand = new Random();
            // Przejdź po wszystkich chromosomach danego osobnika
            for (int i = 0; i < ChromosomesCount; i++)
            {
                // Jeśli wylosowana liczba jest z przedziału [0, prawdopodobieństwo mutacji] przeprowadź mutację
                if (rand.NextDouble() < MutationPropability)
                {
                    // Mutacja polega na losowej zamianie miejscami chromosomów osobnika 
                    // Wylosowane zostają różne indexy miast i są one zamieniane miejscami
                    int FirstIndex = rand.Next(0, ChromosomesCount);
                    int SecondIndex;
                    do
                    {
                        SecondIndex = rand.Next(0, ChromosomesCount);
                    } while (SecondIndex == FirstIndex);
                    int temp = Population[Individual][FirstIndex];
                    Population[Individual][FirstIndex] = Population[Individual][SecondIndex];
                    Population[Individual][SecondIndex] = temp;
                }
            }
        }

        // Funkcja tworząca kolejne pokolenie
        private void NextGeneration()
        {
            List<List<int>> Parents = new List<List<int>>(); // Lista rodziców
            Fitness();  // Obliczenie wartości fitness
            SelectionPropability(); // Wyznaczenie prawdopodobieństwa selekcji

            // Wyznacz rodziców w danym pokoleniu poprzez wybranie ich zgodnie z algorytmem koła ruletki
            for (int i = 0; i < PopulationSize; i++)
            {
                Parents.Add(SelectParentFromPopulation()); // Zapisz rodziców do listy
            }
            Crossover(Parents); // Spróbuj przeprowadzić krzyżowanie osobników rodzicielskich

            // Dla każdego osobnika spróbuj przeprowadzić mutację
            for (int i = 0; i < PopulationSize; i++)
            {
                Mutation(i);
            }
        }

        // Funkcja uruchamiająca algorytm genetyczny
        public void RunAlgorithm(int Iter)
        {
            FitnessValueList.Clear();   // Wyczyść liste wartości fitness w każdym pokoleniu
            Fitness();  // Oblicz wartość fitness
            FitnessValueList.Add(BestDist); // Dodaj najlepszą wartośc dopasowania do listy
            PrintGeneration(-1);    // Wypisz informacje o 0 pokoleniu (te po zainicjowaniu)

            // Przeprowadź tyle iteracji algorytmu ile podano
            for (int i = 0; i < Iter; i++)
            {
                NextGeneration();   // Wyznacz następne pokolenie
                FitnessValueList.Add(BestDist); // Dodaj do listy najlepszą wartość dopasowania
                PrintGeneration(i); // Wypisz informacje o danym pokoleniu
            }
            ShowBest(); // Pokaż informacje o najlepiej przystosowanym osobniku
        }

        // Funkcja pokazująca informacje o najlepiej przystosowanym osobniku po zakończeniu wszytskich iteracji algorytmu
        public void ShowBest()
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine("Najlepiej dopasowany osobnik (indexy miast): ");
            for (int i = 0; i < BestIndividualOfPopulation.Length; i++)
            {
                Console.Write(BestIndividualOfPopulation[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Rozwiązanie problemu TSP (optymalna trasa zwiedzania):");
            Console.Write(CitiesList[IndexOfStartCity] + " -> ");

            for (int i = 0; i < BestIndividualOfPopulation.Length; i++)
            {
                Console.Write(CitiesList[BestIndividualOfPopulation[i]] + " -> ");
            }
            Console.Write(CitiesList[IndexOfStartCity]);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Wartość funkcji dopasowania najlepszego osobnika (długość trasy): " + BestDist);
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
        }

        // Funkcja wypisująca informacje o danym pokoleniu
        public void PrintGeneration(int i)
        {
            Console.WriteLine("Generation " + (i + 1) + ":");
            for (int j = 0; j < Population.Count; j++)
            {
                for (int k = 0; k < Population[j].Count; k++)
                {
                    Console.Write(Population[j][k] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Najlepszy osobnik: ");
            for (int k = 0; k < BestIndividualOfPopulation.Length; k++)
            {
                Console.Write(BestIndividualOfPopulation[k] + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Wartość dopasowania (fitness): " + BestDist);
            Console.WriteLine();
        }

        // Funkcja pokazująca Wykres dopasowania kolejnych pokoleń
        public void ShowFitnessPlot()
        {
            // Zapisz dane o wartościach fitness w każdym pokoleniu do pliku
            StreamWriter sw = new StreamWriter("fitness.txt");
            for (int i = 0; i < FitnessValueList.Count; i++)
            {
                sw.WriteLine(FitnessValueList[i]);
            }
            sw.Close();

            // Tworzenie procesu Pythona
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Ścieżka do interpretera Pythona
            start.Arguments = "FitnessPlot.py"; // Ścieżka do skryptu Pythona

            // Ustawienie opcji dla procesu
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;

            // Uruchomienie procesu
            using (Process process = Process.Start(start))
            {
                // Odczytanie wyniku działania skryptu Pythona
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
        }

        // Funkcja pomocniczna konwertująca string na int i wyrzucająca wyjątek w przypadku niepowodzenia
        private int TryConvertStringToInt(string value)
        {
            try { return int.Parse(value); }
            catch
            {
                throw new ConvertingErrorException("W pliku podano niepoprawna wartosc: " + value + " przez co nie mozna jej przekonwertować na typ int.");
            }
        }

        // Funkcja odczytująca dane wejściowe z pliku
        private void ReadDataFromFile(List<string> CitiesList)
        {
            try
            {
                StreamReader sr = new StreamReader("CitiesDist.txt");
                string pom = sr.ReadLine();
                int[] city_index_tab = new int[CitiesList.Count];
                string[] tab_pom = pom.Split(' ');
                string startCity = tab_pom[tab_pom.Length - 1];
                IndexOfStartCity = CitiesList.FindIndex(city => city == startCity);
                sr.ReadLine();
                sr.ReadLine();
                int city_index = 0;
                int iter = 0;
                while (!sr.EndOfStream)
                {
                    tab_pom = sr.ReadLine().Split(' ');
                    if (tab_pom[0] == startCity)
                    {
                        IndexOfStartCity = city_index;

                    }
                    if (CitiesList.Contains(tab_pom[0]))
                    {
                        city_index_tab[iter] = city_index;
                        iter++;
                    }
                    city_index++;
                }
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < 3; i++)
                {
                    sr.ReadLine();
                }
                int j = 0;
                int current_row = 0;
                while (!sr.EndOfStream)
                {
                    tab_pom = sr.ReadLine().Split(' ');
                    if (city_index_tab[j] == current_row)
                    {
                        int k = 0;
                        for (int current_column = 1; current_column < tab_pom.Length; current_column++)
                        {
                            if (current_column - 1 == city_index_tab[k])
                            {
                                try { dist[j, k] = TryConvertStringToInt(tab_pom[current_column]); }
                                catch (ConvertingErrorException CEE)
                                {
                                    Console.Write(CEE.Message);
                                    Environment.Exit(0);
                                }
                                k++;
                            }
                            if (k == city_index_tab.Length)
                            {
                                break;
                            }
                        }
                        j++;
                        if (j == city_index_tab.Length)
                        {
                            break;
                        }
                    }
                    current_row++;
                }
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Nie odnaleziono pliku CitiesDist.txt");
                Environment.Exit(0);
            }
        }
    }
}
