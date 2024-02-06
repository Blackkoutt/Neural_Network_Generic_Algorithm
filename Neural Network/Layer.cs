namespace Neural_Network
{
    // Klasa dla warstw sieci neuronowej
    public class Layer
    {
        public int NumberOfInputs; // Ilość neuronów w poprzedniej warstwie
        public int NumberOfOutputs; // Ilość neuronów w danej warstwie
        public double[] values; // Wartości neuronów warstwy
        public double[,] weights;   // Wagi pomiędzy daną warstwą a poprzednią
        public double[] bias;   // Wartości bias neuronów warstwy
        public double[] Error;  // Wartości błędów neuronów warstwy

        public Layer(int NumberOfInputs, int NumberOfOutputs)
        {
            this.NumberOfInputs = NumberOfInputs;
            this.NumberOfOutputs = NumberOfOutputs;
            // Tablica wag definiuje wagi pomiędzy neuronami poprzedniej warstwy a neuronami aktualnej warstwy 
            weights = new double[NumberOfInputs, NumberOfOutputs];
            bias = new double[NumberOfOutputs];
            values = new double[NumberOfOutputs];
            Error = new double[NumberOfOutputs];

            InitializeWeights();    // Inicjalizacja wag początkowych
            InitializeBias();   // Inicjalizacja wartości bias neuronów warstwy
        }

        // Losowy wybór wartości wag oraz bias z przedzialu od -1 do 1
        private double Random_Weights_Bias()
        {
            Random random = new Random();
            return random.NextDouble() - 1.0;
        }

        // Funkcja inicjująca wagi początkowe
        private void InitializeWeights()
        {
            for (int i = 0; i < NumberOfInputs; i++)
            {
                for (int j = 0; j < NumberOfOutputs; j++)
                {
                    weights[i, j] = Random_Weights_Bias();
                }
            }
        }

        // Funkcja inicjująca początkowe wartości bias neuronów warstwy
        private void InitializeBias()
        {
            for (int i = 0; i < NumberOfOutputs; i++)
            {
                bias[i] = Random_Weights_Bias();
            }
        }

    }
}
