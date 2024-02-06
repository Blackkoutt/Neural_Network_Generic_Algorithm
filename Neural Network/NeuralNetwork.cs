using System.Diagnostics;

namespace Neural_Network
{
    class NeuralNetwork
    {
        private const double LearningRate = 0.1;    // Współczynnik uczenia
        private const double Threshold = 0.5;   // Wartość progowa
        private Layer[] Layers_Tab; // Tablica warstw
        private List<List<int>> InputsList; // Wartości wejściowych danych treningowych - warstwa wejściowa
        private List<List<int>> ClassificationDataList; // Wartości wejściowe danych klasyfikowanych
        private List<List<int>> TestInputsList; // Wartości wejściowe danych testowych - warstwa wejściowa
        private List<int> OutputsList;  // Wartości wyjściowe danych treningowych
        private List<int> TestOutputsList;  // Wartości wyjściowe danych testowych
        private List<string> CitiesList;    // Wyznaczona lista miast wartych odwiedzenia
        private int[] TrainingSetOrder; // Kolejność wchodzenia obiektów treningowych do sieci
        private int TrainingExamplesCount;  // Ilość obiektów treningowych
        private double[] PredictedOutputs;  // Przewidziane przez sieć wartości wyjściowe dla każdego przypadku
        private int TrainCyclesCount;   // Liczba cykli treningowych
        private List<double> ErrorsList;    // Wartości błędów w warstwie wyjściowej

        public NeuralNetwork(int NumberOfLayers, int[] LayersNeuronsCount)
        {
            // INICJALIZACJA
            TrainCyclesCount = 0;
            InputsList = new List<List<int>>();
            TestInputsList = new List<List<int>>();
            ClassificationDataList = new List<List<int>>();
            OutputsList = new List<int>();
            CitiesList = new List<string>();
            TestOutputsList = new List<int>();
            ErrorsList = new List<double>();
            ReadTrainingDataFromFile(); // Odczyt danych treningowych z pliku
            RandomChooseTestData(); // Losowe wybranie obiektów testowych z zbioru obiektów treningowych
            TrainingExamplesCount = OutputsList.Count;
            TrainingSetOrder = new int[OutputsList.Count];
            PredictedOutputs = new double[OutputsList.Count];
            for (int i = 0; i < TrainingSetOrder.Length; i++)
            {
                TrainingSetOrder[i] = i;    // Inicjacja początkowej kolejności w której wchodzą dane
            }

            Layers_Tab = new Layer[NumberOfLayers];
            int AttributesCount = InputsList[0].Count;

            int j = 0;

            // Tworzenie poszczególnych warstw sieci
            for (int i = 0; i < NumberOfLayers; i++)
            {
                // Pierwsza warstwa ukryta
                if (i == 0)
                {
                    // Liczba wejść pierszej warstwy ukrytej jest równa ilości atrybutów
                    // Liczba wyjść pierwszej warstwy ukrytej jest równa ilości neuronów w tej warstwie
                    Layer NewLayer = new Layer(AttributesCount, LayersNeuronsCount[i]);
                    Layers_Tab[j] = NewLayer;
                    j++;
                }
                // Kolejne warstwy ukryte i warstwa wyjściowa
                if (i > 0)
                {
                    // Liczba wejść danej warstwy ukrytej jest równa ilości neuronów w poprzedniej warstwie
                    // Liczba wyjść danej warstwy ukrytej jest równa ilości neuronów w danej warstwie
                    Layer NewLayer = new Layer(LayersNeuronsCount[i - 1], LayersNeuronsCount[i]);
                    Layers_Tab[j] = NewLayer;
                    j++;
                }
            }

        }

        // Losowy wybór obiektów testowych (1/3 obiektów treningowych)
        private void RandomChooseTestData()
        {
            int ProcentOfTestData = OutputsList.Count / 3;
            Random rand = new Random();
            int index;
            for (int i = 0; i < ProcentOfTestData; i++)
            {
                index = rand.Next(0, OutputsList.Count);
                TestOutputsList.Add(OutputsList[i]);
                TestInputsList.Add(InputsList[i]);
                OutputsList.RemoveAt(i);
                InputsList.RemoveAt(i);
            }
        }

        // Funkcja mieszająca dane treningowe tak aby nie wchodzily w każdym cyklu w tej samej kolejności
        private void TrainingDataMix()
        {
            Random random = new Random();
            int rand_index;
            int pom;
            for (int i = 0; i < TrainingSetOrder.Length; i++)
            {
                rand_index = random.Next(0, TrainingSetOrder.Length);
                pom = TrainingSetOrder[rand_index];
                TrainingSetOrder[rand_index] = TrainingSetOrder[i];
                TrainingSetOrder[i] = pom;
            }
        }

        // Średnie dopasowanie (w procentach) wartości klasyfikacji wszystkich obiektów do prawidłowych wyjść
        private double AverageAccurancy(List<int> OutputList)
        {
            double sum = 0;
            for (int i = 0; i < OutputList.Count; i++)
            {
                // Wyznaczenie rozbieżności pomiędzy klasyfikacją, a prawidłowymi wyjściami
                if (OutputList[i] == 0)
                {
                    sum += PredictedOutputs[i];
                }
                else
                {
                    sum += 1 - PredictedOutputs[i];
                }
            }
            // Wyznaczenie jakości klasyfikacji w % w zaokrągleniu do 2 miejsc po przecinku
            return Math.Round((100 - (sum / OutputList.Count) * 100), 2);
        }

        // Funkcja trenująca sieć
        public void Training(int NumOfLearningCycles)
        {
            for (int i = 0; i < NumOfLearningCycles; i++)
            {
                if ((i + 1) % 10 == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Training " + (TrainCyclesCount + i + 1));
                    Console.WriteLine();
                }
                double TotalError = 0;

                // W każdym cyklu wykonywane jest mieszanie danych treningowych tak, aby za każdym razem ich kolejność była inna 
                TrainingDataMix();
                // W każdym cyklu uczenia należy przejść po wszytskich obiektach treningowych
                for (int j = 0; j < TrainingExamplesCount; j++)
                {
                    int k = TrainingSetOrder[j]; // Dane wchodzą w wymieszanej kolejności
                    ForwardPropagation(k, InputsList); // Propagacja sieci "do przodu"
                    TotalError += BackwardPropagation(k); // Propagacja wsteczna i zliczenie błędu
                }

                // Dodanie sumy błędów popełnionych przez sieć w danym cyklu do listy 
                // Na tej podstawie generowwany będzie wykres TotalErrorPlot
                ErrorsList.Add(TotalError);

                // Co 10 cykl uczenia wypisywana jest informacja o klasyfikacji
                if ((i + 1) % 10 == 0)
                {
                    PrintInfo(InputsList, OutputsList);
                }

            }
            // Dodanie aktualnej liczby cykli do pełnej liczby przeprowadzonych cykli uczenia
            TrainCyclesCount += NumOfLearningCycles;
        }

        // Propagacja sieci "do przodu"
        private void ForwardPropagation(int k, List<List<int>> InputList)
        {
            int OutputLayerIndex = Layers_Tab.Length - 1;

            // Przejście od warstwy wejściowej do końcowej
            for (int m = 0; m < Layers_Tab.Length; m++)
            {
                // Przejście po wszystkich neuronach danej warstwy
                // NumberOfOutputs wskazuje na liczbę neuronów w danej wartswie - tyle ile wyjść w danej warstwie tyle jest neuronów
                for (int n = 0; n < Layers_Tab[m].NumberOfOutputs; n++)
                {
                    // Dodanie wartości bias neuronu do sumy
                    double sum = Layers_Tab[m].bias[n];

                    // Przejście po wszytskich neuronach poprzedniej warstwy
                    // Tyle ile wejść do danego neuronu tyle jest neuronów w poprzeniej warstwie
                    for (int o = 0; o < Layers_Tab[m].NumberOfInputs; o++)
                    {
                        // Jeśli jest to pierwsza wartswa pod uwagę brane są dane wejściowe 
                        // k - wskazuje na index danego przypadku (wcześniej wszytskie przypadki zostały wymieszane)
                        if (m == 0)
                        {
                            // Do sumy dodawany jest wynik mnożenia wartości danego atrybutu (wartości neuronu wejściowego) przez 
                            // wagę krawędzi pomiędzy każdym kolejnym neuronem wejściowym a neuronem aktualnej warstwy, m=0 wskazuje na pierwszą warstwę ukrytą
                            sum += InputList[k][o] * Layers_Tab[m].weights[o, n];
                        }
                        // Jeśli nie jest to pierwsza warstwa pod uwagę brane są wartości uzyskane w poprzedniej warstwie
                        else
                        {
                            // Do sumy dodawany jest wynik wartości neuronów poprzedniej warstwy pomnożony przez 
                            // wagę krawędzi pomiędzy każdym neuronem poprzedniej warstwy a aktualnej
                            sum += Layers_Tab[m - 1].values[o] * Layers_Tab[m].weights[o, n];
                        }
                    }
                    // Na wyznaczoną sumę nakładana jest funckja aktywacji sigmoid i tak wyznaczana jest wartość danego neuronu
                    Layers_Tab[m].values[n] = Sigmoid(sum);
                    // Jeśli jest to już ostatnia warstwa przypisz wartości neuronów wartswy wyjściowej jako wyniki klasyfikacji
                    if (m == OutputLayerIndex)
                    {
                        PredictedOutputs[k] = Layers_Tab[m].values[n];
                    }
                }

            }
        }

        // Algorytm propagacji wstecznej
        private double BackwardPropagation(int k)
        {
            int OutputLayerIndex = Layers_Tab.Length - 1;
            double TotalErrorInOutputs = 0;

            // Przejdź po wszystkich warstwach zaczynając od warstwy wyjściowej
            for (int i = OutputLayerIndex; i >= 0; i--)
            {
                // Jeśli jest to warstwa wyjściowa
                if (i == OutputLayerIndex)
                {
                    // Przejdź po wszytkich neuronach tej warstwy
                    for (int j = 0; j < Layers_Tab[i].NumberOfOutputs; j++)
                    {
                        // Wyznacz różnicę pomiędzy wartością oczekiwaną a wynikiem klasyfikacji
                        double deltaOutput = OutputsList[k] - Layers_Tab[i].values[j];
                        // Różnicę tą pomnóż przez pochodną funckji Sigmoid i przypisz wartości błędu do każdego neuronu
                        // warstwy wyjściowej
                        Layers_Tab[i].Error[j] = deltaOutput * dSigmoid(Layers_Tab[i].values[j]);

                        // Zsumuj wszystkie wartości błędów w warstwie wyjściowej - potrzebne do wygenerowania wykresu TotalErrorPlot
                        TotalErrorInOutputs += Math.Abs(Layers_Tab[i].Error[j]);
                    }
                }
                // Jeśli nie jest to warstwa wyjściowa
                else
                {
                    // Przejdź po wszystkich neuronach aktualnej warstwy
                    // Liczba wejść w następnej warstwie (i+1) jest równa liczbie neuronów poprzedniej (aktualnej) warstwy
                    for (int j = 0; j < Layers_Tab[i + 1].NumberOfInputs; j++)
                    {
                        double error = 0;
                        // Przejdź po wszystkich neuronach następnej warstwy
                        for (int l = 0; l < Layers_Tab[i + 1].NumberOfOutputs; l++)
                        {
                            // Wyznacz błąd danego neuronu (N1) aktualnej warstwy poprzez pomnożenie wszystkich
                            // kolejnych wartości błędów neuronów następnej warstwy przez wagę pomiędzy N1
                            // a kolejnymi neuronami następnej warstwy
                            error += Layers_Tab[i + 1].Error[l] * Layers_Tab[i + 1].weights[j, l];
                        }
                        // Pomnóż wyznaczony błąd przez pochodną funckji Sigmoid i przypisz danemu neuronowi aktualnej warstwy
                        Layers_Tab[i].Error[j] = error * dSigmoid(Layers_Tab[i].values[j]);
                    }
                }
            }
            // Jeśli wartości błędów zostały wyznaczone w każdej warstwie dostosuj poszczególne wagi
            ChangeWeights(k);
            return TotalErrorInOutputs; // Zwróć sumę błędów wartswy wyjściowej

        }

        // Dostosowanie wag po wyznaczeniu błędu każdego z neuronów
        private void ChangeWeights(int k)
        {
            int OutputLayerIndex = Layers_Tab.Length - 1;
            // Przejdź po wszystkich warstwach sieci od końca
            for (int i = OutputLayerIndex; i >= 0; i--)
            {
                // Przejdź po wszystkich neuronach poprzedniej warstwy (jeśli patrzeć od początku sieci)
                for (int j = 0; j < Layers_Tab[i].NumberOfInputs; j++)
                {
                    // Przejdź po wszystkich neuronach aktualnej warstwy
                    for (int l = 0; l < Layers_Tab[i].NumberOfOutputs; l++)
                    {
                        // Zmodyfikuj bias każdego neuronu aktualnej warstwy poprzez pomnożenie wartości błędu przez współczynnik uczenia
                        Layers_Tab[i].bias[l] += Layers_Tab[i].Error[l] * LearningRate;
                        // Jeśli jest to pierwsza wartswa (tak naprawdę druga zaraz po wejściowej której nie uwzględiono gdyż jest to lista InputsList)
                        if (i == 0)
                        {
                            // Wyznacz nową wagę pomiędzy j neuronem poprzedniej warstwy a l neuronem aktualnej warstwy
                            // Poprzez pomnożenie współczynnika uczenia przez wartość błędu l neuronu aktualnej warstwy
                            // przez wartość "j neuronu wejściowego" - j wartość atrybutu k przypadku
                            Layers_Tab[i].weights[j, l] += LearningRate * Layers_Tab[i].Error[l] * InputsList[k][j];
                        }
                        // Jeśli nie jest to pierwsza wartswa
                        else
                        {
                            // Postępuj analogicznie uwzględniając tym razem wartość j neuronu następnej wartswy (jeśli patrzeć od początku sieci)
                            Layers_Tab[i].weights[j, l] += LearningRate * Layers_Tab[i].Error[l] * Layers_Tab[i - 1].values[j];
                        }

                    }
                }

            }
        }
        // Funkcja do obliczania wartości funkcji aktywacji Sigmoid
        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        // Funkcja do obliczania wartości pochodnej funkcji aktywacji Sigmoid
        private double dSigmoid(double x)
        {
            return x * (1 - x);
        }

        // Funkcja obliczająca wartości klasyfikacji dla obiektów testowych
        public void CalculateTestOutput()
        {
            // Dla każdego obiektu testowego wywołaj propagację "do przodu" celem wyznaczenia wartości wyjściowych
            for (int i = 0; i < TestOutputsList.Count; i++)
            {
                ForwardPropagation(i, TestInputsList);
            }
            // Wypisz informację o uzyskanej klasyfikacji
            PrintInfo(TestInputsList, TestOutputsList);
        }

        // Funkcja obliczająca wartości klasyfikacji dla "nowych" obiektów 
        public List<string> CalculateClassificationOutput()
        {
            // Dla każdego "nowego" obiektu wywołaj propagację "do przodu" celem wyznaczenia wartości wyjściowych
            for (int i = 0; i < ClassificationDataList.Count; i++)
            {
                ForwardPropagation(i, ClassificationDataList);
            }
            // Wypisz informację o uzyskanej klasyfikacji i zapisz obiekty sklasyfikowane jako "1"
            PrintClassificationResultsAndSave();
            return CitiesList;  // Zwróć liste miast (obiektów wyznaczonych jako "1")
        }

        // Funkcja wypisująca informację o klasyfikacji danych wejściowych w danym cyklu
        private void PrintInfo(List<List<int>> InputList, List<int> OutputList)
        {
            int iter = 0;
            foreach (List<int> ExamplesList in InputList)
            {

                Console.Write("Inputs: ");
                foreach (int AtributeValue in ExamplesList)
                {
                    Console.Write(AtributeValue + " ");
                }

                Console.Write("Output: " + OutputList[iter] + " Predicted Output: " + PredictedOutputs[iter]);
                if (OutputList[iter] == 1 && PredictedOutputs[iter] >= Threshold || OutputList[iter] == 0 && PredictedOutputs[iter] < Threshold)
                {
                    Console.Write(" +");
                }
                else
                {
                    Console.Write(" -");
                }
                iter++;
                Console.WriteLine();
            }
            if (OutputList.Count == OutputsList.Count)
            {
                Console.WriteLine("Jakość klasyfikacji danych treningowych: " + AverageAccurancy(OutputList) + " %");
            }
            else
            {
                Console.WriteLine("Jakość klasyfikacji danych testowych: " + AverageAccurancy(OutputList) + " %");
            }

        }

        // Funkcja wypisująca rezultat klasyfikacji dla "nowych" obiektów (miast) i zapisująca miasta warte odwiedzenia
        private void PrintClassificationResultsAndSave()
        {
            int iter = 0;
            foreach (List<int> ExamplesList in ClassificationDataList)
            {
                if (iter == CitiesList.Count)
                {
                    break;
                }
                Console.Write("City: " + CitiesList[iter]);
                Console.Write(" Inputs: ");
                foreach (int AtributeValue in ExamplesList)
                {
                    Console.Write(AtributeValue + " ");
                }
                Console.Write(" Predicted Output: " + PredictedOutputs[iter]);
                Console.WriteLine();
                iter++;
            }
            int DeletedCount = 0;
            int CitiesCountBefore = CitiesList.Count;
            // Jeśli wartość klasyfikacji danego miasta jest mniejsza od wartości progowej Threshold
            // Usuń miasto z listy miast
            for (int i = 0; i < ClassificationDataList.Count; i++)
            {
                if (PredictedOutputs[i] < Threshold)
                {
                    CitiesList.RemoveAt(i - DeletedCount);
                    DeletedCount++;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Sieć wybrała " + CitiesList.Count + " z " + CitiesCountBefore + " miast.");
            Console.WriteLine();
            Console.WriteLine("Miasta warte odwiedzenia wyznaczone przez sieć: ");
            foreach (string city in CitiesList)
            {
                Console.WriteLine(city);
            }
        }

        // Funkcja uruchamiająca skrypt Pythona w celu wygenerowania wykresu TotalErrorPlot
        public void GenerateTotalErrorPlot()
        {
            // Zapis listy błędów do pliku (skrypt korzysta z pliku errors.txt)
            StreamWriter sw = new StreamWriter("errors.txt");
            for (int i = 0; i < ErrorsList.Count; i++)
            {
                sw.WriteLine(ErrorsList[i]);
            }
            sw.Close();

            // Tworzenie procesu Pythona
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Ścieżka do interpretera Pythona
            start.Arguments = "TotalErrorPlot.py"; // Ścieżka do skryptu Pythona

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

        // Funkcja konwertująca string na typ int i wyrzucająca wyjątek w przypadku niepowodzenia
        private int TryConvertStringToInt(string value)
        {
            try { return int.Parse(value); }
            catch
            {
                throw new ConvertingErrorException("W pliku podano niepoprawna wartosc: " + value + " przez co nie mozna jej przekonwertować na typ int.");
            }
        }

        // Funkcja czytająca dane dotyczące "nowych" miast do klasyfikacji z pliku
        public void ReadClassificationDataFromFile(string FilePath)
        {
            try
            {
                StreamReader sr = new StreamReader(FilePath);
                sr.ReadLine();
                string pom;
                while (!sr.EndOfStream)
                {
                    pom = sr.ReadLine();
                    string[] tab_pom = pom.Split(' ');
                    CitiesList.Add(tab_pom[0]);
                    List<int> pomList = new List<int>();
                    for (int i = 1; i < tab_pom.Length; i++)
                    {
                        try { pomList.Add(TryConvertStringToInt(tab_pom[i])); }
                        catch (ConvertingErrorException CEE)
                        {
                            Console.WriteLine(CEE.Message);
                            Environment.Exit(0);
                        }
                    }
                    ClassificationDataList.Add(pomList);
                }
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                throw new FileException("Nie odnaleziono pliku " + FilePath);
            }

        }

        // Funkcja czytająca dane treningowe z pliku
        private void ReadTrainingDataFromFile()
        {
            try
            {
                StreamReader sr = new StreamReader("TrainingData.txt");
                string pom1 = sr.ReadLine(); //1 linia jest zbędna
                string pom;
                while (!sr.EndOfStream)
                {
                    List<int> pomList = new List<int>();
                    pom = sr.ReadLine();
                    string[] tab_pom = pom.Split(' ');
                    for (int i = 1; i < tab_pom.Length; i++)
                    {
                        if (i < tab_pom.Length - 1)
                        {
                            try { pomList.Add(TryConvertStringToInt(tab_pom[i])); }
                            catch (ConvertingErrorException CEE)
                            {
                                Console.WriteLine(CEE.Message);
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            try { OutputsList.Add(TryConvertStringToInt(tab_pom[i])); }
                            catch (ConvertingErrorException CEE)
                            {
                                Console.WriteLine(CEE.Message);
                                Environment.Exit(0);
                            }
                        }
                    }
                    InputsList.Add(pomList);
                }
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Nie odnaleziono pliku TrainingData.txt");
                Environment.Exit(0);
            }

        }
    }
}
