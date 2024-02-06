using Neural_Network;

class Program
{
    private static int TryConvertStringToInt(string value)
    {
        int ConvertedString;
        try { ConvertedString = int.Parse(value); }
        catch
        {
            throw new ConvertingErrorException("Podano wartość której nie da się przekonwertować na typ int.");
        }
        if(ConvertedString <=0)
        {
            throw new ConvertingErrorException("Podana wartość nie jest liczbą dodatnią");
        }
        return ConvertedString;
    }
    private static double TryConvertStringToDouble(string value)
    {
        double ConvertedString;
        try { ConvertedString = Double.Parse(value); }
        catch
        {
            throw new ConvertingErrorException("Podano wartość której nie da się przekonwertować na typ double.");
        }
        if (ConvertedString < 0 || ConvertedString>1)
        {
            throw new ConvertingErrorException("Podana wartość nie jest wartością z przedziału [0,1]");
        }
        return ConvertedString;
    }

    public static void Main(string[] args)
    {
        int[] neurons;
        List<string> citiesList = new List<string>();
        NeuralNetwork NN=null;
        GenericAlgorithm GA=null;
        char SelectedOption;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Solution for Neural Network & TSP Problem");
            Console.WriteLine("Wybierz jedną z poniższych opcji:");
            Console.WriteLine();
            Console.WriteLine("[1] Utwórz sieć neuronową");
            Console.WriteLine("[2] Trenuj sieć neuronową na danych treningowych");
            Console.WriteLine("[3] Przeprowadź klasyfikację miast");
            Console.WriteLine("[4] Podaj parametry algorytmu genetycznego");
            Console.WriteLine("[5] Wyznacz optymalną trasę przejazdu pomiędzy miastami");
            Console.WriteLine("[6] Wygeneruj wykres FitnessPlot");
            Console.WriteLine("[7] Zakończ program");

            ConsoleKeyInfo key = Console.ReadKey();
            SelectedOption = key.KeyChar;
            switch (SelectedOption)
            {
                case '1':
                    {
                        Console.Clear();
                        Console.WriteLine("Wcisnij [Q] i ENTER aby wrócić do menu.");
                        Console.WriteLine();
                        Console.WriteLine("Podana ilość warstw to warstwy ukryte + warstwa wyjściowa.");
                        Console.WriteLine("Nie jest uwzględniana wartswa wejściowa.");
                        Console.WriteLine();
                        Console.WriteLine("Podaj ilość warstw sieci: ");
                        string Layers_Count_In_String = Console.ReadLine();
                        if (Layers_Count_In_String == "q" || Layers_Count_In_String == "Q")
                        {
                            break;
                        }
                        int Layers_Count;
                        try { Layers_Count = TryConvertStringToInt(Layers_Count_In_String); }
                        catch (ConvertingErrorException CEE)
                        {
                            Console.WriteLine(CEE.Message);
                            Thread.Sleep(800);
                            break;
                        }
                        Console.WriteLine();
                        neurons = new int[Layers_Count];
                        string Neurons_Count;
                        bool Error;
                        for (int i = 0; i < Layers_Count; i++)
                        {
                            Error = false;
                            if (i != Layers_Count - 1)
                            {
                                Console.WriteLine("Podaj ilość neuronów " + (i + 1) + " warstwy ukrytej: ");
                            }
                            else
                            {
                                Console.WriteLine("Podaj ilość neuronów warstwy wyjściowej: ");
                            }
                            Neurons_Count = Console.ReadLine();
                            int Neurons_Count_Int = 0;
                            try { Neurons_Count_Int = TryConvertStringToInt(Neurons_Count); }
                            catch (ConvertingErrorException CEE)
                            {
                                Console.WriteLine(CEE.Message);
                                Thread.Sleep(1200);
                                Error = true;
                                i--;
                            }
                            if (!Error)
                            {
                                neurons[i] = Neurons_Count_Int; 
                            }                                                   
                        }
                        NN = new NeuralNetwork(neurons.Length, neurons);
                        Console.WriteLine();
                        Console.WriteLine("Pomyślnie utworzono sieć neuronową");
                        Thread.Sleep(1200);
                        break;
                    }
                case '2':
                    {
                        if (NN == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Aby móc trenować sieć najpierw należy ją utworzyć");
                            Thread.Sleep(1500);
                            break;
                        }
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Wcisnij [Q] i ENTER aby wrócić do menu");
                            Console.WriteLine("Podaj ilość cykli treningowych: ");
                            string TrainingCyclesInString = Console.ReadLine();
                            if (TrainingCyclesInString == "q" || TrainingCyclesInString == "Q")
                            {
                                break;
                            }
                            int TrainingCycles;
                            try { TrainingCycles = TryConvertStringToInt(TrainingCyclesInString); }
                            catch (ConvertingErrorException CEE)
                            {
                                Console.WriteLine(CEE.Message);
                                Thread.Sleep(800);
                                break;
                            }
                            NN.Training(TrainingCycles);
                            Console.WriteLine();
                            Console.WriteLine("Wybierz jedną z poniższych opcji");
                            Console.WriteLine("[1] Trenuj sieć dalej ");
                            Console.WriteLine("[2] Wygeneruj wykres TotalErrorPlot ");
                            Console.WriteLine("[3] Przeprowadz klasyfikację danych testowych ");
                            Console.WriteLine("[4] Wróć do menu ");
                            Console.WriteLine();
                            bool Exit=false;
                            key = Console.ReadKey();
                            SelectedOption = key.KeyChar;
                            switch (SelectedOption)
                            {
                                case '1':
                                    {
                                        break;
                                    }
                                case '2':
                                    {
                                        NN.GenerateTotalErrorPlot();
                                        break;
                                    }
                                case '3':
                                    {
                                        Console.Clear();
                                        NN.CalculateTestOutput();
                                        Console.WriteLine();
                                        Console.WriteLine("Wcisnij dowolny przycisk aby wrócić do treningu sieci");
                                        key = Console.ReadKey();
                                        break;
                                    }
                                case '4':
                                    {
                                        Exit = true;
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("Wybierz jedną z podanych opcji.");
                                        Thread.Sleep(800);
                                        Console.Clear();
                                        break;
                                    }
                            }
                            if (Exit) { break; }
                        }
                        break;
                    }
                case '3':
                    {
                        if (NN == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Aby móc klasyfikować dane najpierw należy utworzyć sieć");
                            Thread.Sleep(1500);
                            break;
                        }
                        if (citiesList.Count != 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Dane z klasyfikacji zostały już zapisane w liście");
                            Thread.Sleep(1500);
                            break;
                        }
                        Console.Clear();
                        Console.WriteLine("Podaj ścieżkę do pliku z danymi: ");
                        string FilePath = Console.ReadLine();
                        Console.Clear();
                        try
                        {
                           NN.ReadClassificationDataFromFile(FilePath);
                           citiesList = NN.CalculateClassificationOutput();
                        }
                        catch(FileException FE)
                        {
                            Console.WriteLine(FE.Message+".");
                            Thread.Sleep(1000);
                            break;
                        }
                        Console.WriteLine();
                        Console.WriteLine("Wcisnij dowolny przycisk aby kontynuować.");
                        Console.ReadKey();
                        break;
                    }
                case '4':
                    {
                        if (citiesList.Count == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Lista miast jest pusta.");
                            Console.WriteLine("Najpierw przeprowadz klasyfikację miast za pomocą sieci neuronowej");
                            Thread.Sleep(2000);
                            break;
                        }
                        Console.Clear();
                        double p_crossover = 0;
                        double p_mutation = 0;
                        int pop_size = 0;
                        bool Exit=false;
                        Console.WriteLine("Wcisnij [Q] i ENTER aby wrócić do menu");
                        for(int i=0; i<3; i++)
                        {
                            if (i == 0)
                            {
                                Console.WriteLine("Podaj wartość prawdopodobieństwa krzyżowania: ");
                                string p_crosover_string = Console.ReadLine();
                                if (p_crosover_string == "q" || p_crosover_string == "Q")
                                {
                                    Exit = true;
                                    break;
                                }
                                try { p_crossover = TryConvertStringToDouble(p_crosover_string); }
                                catch (ConvertingErrorException CEE)
                                {
                                    Console.WriteLine(CEE.Message);
                                    Thread.Sleep(800);
                                    i--;
                                }
                            }
                            if (i == 1)
                            {
                                Console.WriteLine("Podaj wartość prawdopodobieństwa mutacji: ");
                                string p_mutation_string = Console.ReadLine();
                                if (p_mutation_string == "q" || p_mutation_string == "Q")
                                {
                                    Exit = true;
                                    break;
                                }
                                try { p_mutation = TryConvertStringToDouble(p_mutation_string); }
                                catch (ConvertingErrorException CEE)
                                {
                                    Console.WriteLine(CEE.Message);
                                    Thread.Sleep(800);
                                    i--;
                                }
                            }
                            if (i == 2)
                            {
                                Console.WriteLine("Podaj rozmiar populacji: ");
                                string pop_size_string = Console.ReadLine();
                                if (pop_size_string == "q" || pop_size_string == "Q")
                                {
                                    Exit = true;
                                    break;
                                }
                                try { pop_size = TryConvertStringToInt(pop_size_string); }
                                catch (ConvertingErrorException CEE)
                                {
                                    Console.WriteLine(CEE.Message);
                                    Thread.Sleep(800);
                                    i--;
                                }
                            }
                        }
                        if (!Exit)
                        {
                            GA = new GenericAlgorithm(citiesList, p_crossover, p_mutation, pop_size);
                            Console.WriteLine();
                            Console.WriteLine("Algorytm genetyczny został pomyślnie skonfigurowany i jest gotowy do użycia");
                            Thread.Sleep(1200);
                        }
                        break;
                    }
                case '5':
                    {
                        if (GA == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Aby móc wyznaczyć optymalną trasę przejazdu należy podać parametry algorytmu genetycznego.");
                            Thread.Sleep(2000);
                            break;
                        }
                        Console.Clear();
                        int max_iter=0;
                        Console.WriteLine("Wcisnij [Q] i ENTER aby wrócić do menu");
                        Console.WriteLine("Podaj maksymalną liczbę iteracji: ");
                        string max_iter_string = Console.ReadLine();
                        if (max_iter_string == "q" || max_iter_string == "Q")
                        {
                            break;
                        }
                        try { max_iter = TryConvertStringToInt(max_iter_string); }
                        catch (ConvertingErrorException CEE)
                        {
                            Console.WriteLine(CEE.Message);
                            Thread.Sleep(1200);
                            break;
                        }
                        GA.RunAlgorithm(max_iter);
                        Console.WriteLine("Wcisnij dowolny przycisk aby kontynuować.");
                        Console.ReadKey();
                        break;
                    }
                case '6':
                    {
                        if (GA == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Aby wygenerować wykres FitnessPlot należy najpierw podać parametry algorytmu genetycznego.");
                            Thread.Sleep(2000);
                            break;
                        }
                        GA.ShowFitnessPlot();
                        break;
                    }
                case '7':
                    {
                        return;
                    }
                default:
                    {
                        Console.WriteLine();
                        Console.WriteLine("Wybierz jedną z podanych opcji.");
                        Thread.Sleep(800);
                        Console.Clear();
                        break;
                    }

            }
        }
    }
}
