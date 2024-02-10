# Table Of Content

- [General info](#general-info)
    - [Functionalities](#functionalities)
    - [Neural network](#neural-network)
    - [Genetic algorithm](#genetic-algorithm)
- [Technologies](#technologies)
- [Getting Started](#getting-started)
    - [Instruction](#instruction)
    - [Additional informations](#additional-informations)

# General info
The problem addressed in the field of artificial intelligence is the determination of
collection of cities only those that are worth visiting and then based on
designated cities to solve the TSP problem (traveling salesman problem). Down
2 artificial intelligence tools were selected to solve this problem, namely
neural network and genetic algorithm. Both algorithms have been implemented
in C# from scratch without using any additional libraries.

### Functionalities
- Reading all necessary data from files
- Random selection of test cases from the training set
- Possibility to create any neural network structure
- Possibility to repeatedly train the network on training data
- Monitor progress in network training on an ongoing basis
- Classification of test data and determination of its quality (data with which the network
had no contact with teaching)
- Classificaton cities to determine which ones
are worth visiting and feeding them to the genetic algorithm
- Possibility to generate charts: errors made by the network
neuron in subsequent learning cycles - TotalErrorPlot and the best value
fit function in each FitnessPlot generation.
- Basic error handling related to files, given values ​​and
an attempt to complete individual points in the wrong order
- Possibility to freely adapt the genetic algorithm to your needs –
providing your own parameters
- Execution of the genetic algorithm for any given number
iteration
- Observation the genotypes of all individual individuals
generations
- Observation the best value of the fitness function (the shortest
distances between cities) and the best individual found so far
in every generation
- Possibility to determine the optimal driving route from a given city
starting point through cities worth visiting designated by the network back
to the starting city

### Neural network
The neural network is designed to classify cities based on their characteristics
(entertainment, monuments, nature, architecture, prices, gastronomy, culture, opinions)
designate only those that are worth visiting.

The program enables the user creating any neural network structure (you can specify any
number of layers and number of neurons in each layer). Additionally, the user can
continuously observe the network training process and perform data classification
test samples randomly selected from the training set and generate a graph
TotalErrorPlot showing the errors made by the network in subsequent training cycles.

The set of all cities is defined as the space of all possible solutions
subject to classification located in the ClassificationData.txt file. To measure
assessing the quality of a single solution, the designated initial value was adopted
over the network for a given case. The closer this value is to 1, the solution
is closer to correct. It is worth mentioning that the network classifies
solutions as correct already for values ​​​​greater than 0.5 due to
Threshold factor. However, you can freely modify it in the code
program so that the network recognizes the solution as correct with a different value
output.

Before you start classifying cities, it is worth retraining the network on
training data. The better the classification of training data, the better
classification of "new" objects (cities) can be obtained. Number of cycles needed for
the correct classification of cities depends on several factors:
- network structure
- random values ​​of initial weights
- amount of training data.
The sigmoid function was assumed as the activation function of each neuron.

### Genetic algorithm
Cities classified by the neural network become input to the algorithm
genetic. Its task is to determine the shortest route from a given city
starting point through all the cities designated by the network back to the startup city - solving the TSP problem.

By creating an object of the Generic Algorithm class
the user can provide any parameters of the genetic algorithm:
- crossover probability
- mutation probability
- size of the population
Both probabilities must be selected from the range [0,1] as
actual value.

The genetic algorithm reads information about the distances of the data
cities from the "CitiesDist.txt" file. Unfortunately, in this case, this file should
define each city's distance from every other city, where it should be here
include all cities subject to classification in the neural network. It happens
because it is impossible to predict which cities the network will classify
as worth visiting. In the program, distance data is stored in
dist matrix, but these are only individual distances between cities
determined by the neural network.

In this case, each was defined as the space of all possible solutions
possible to arrange the cities designated by the network in such a way that they do not overlap
they repeated. The space of possible solutions can be truly huge because
if the network has designated n cities, then possible ways to arrange these cities so that
the order was not repeated is n!. What, for example, already in 10 cities
designated by the network gives as many as 10! = 3,628,800 possible solutions.
The minimum fitness value was adopted as a measure assessing the quality of a single solution for a given individual.
So the solution - setting up the cities
the better the smaller the sum of the distances between them.

It's worth it to find a solution in the best/fastest way possible
experiment with genetic algorithm settings. It is certain that a large amount
iterations or a large population size increases the probability of being found
correct solution, however, it is worth noting that increasing these
parameters significantly affects the execution time. Algorithm parameters
should be selected so that the determined solution does not depend on a random factor, at
so the execution of the algorithm did not take much time.

# Technologies
![Static Badge](https://img.shields.io/badge/C%23%20programming%20language-%236a1577?style=for-the-badge&logo=csharp)

![Static Badge](https://img.shields.io/badge/Python-%233874a4?style=for-the-badge&logo=python&logoColor=%23ffe15c)

# Getting Started
The application can be run in Microsoft Visual Studio by opening the Neural
Network.sln from here you will be able to access the full solution with the code
source.

Otherwise, the application can also be launched via a file
executable Neural Network.exe located in the Neural directory
Network/bin/Debug/net6.0.

After launching the application, the interface appears
user from where it is possible to select 7 options. It is recommended to select options in
the order in which they are given. Options can be selected by pressing a button on the keyboard
from 1 to 7 (does not require confirmation with the ENTER key).

### Instruction
**1. Create a neural network** - the program goes to the neural network wizard, where
first, the user is asked to enter the number of network layers
(the input layer is not included). After entering the number of layers, the program asks for
specifying the number of neurons in each layer. Each of the entered values ​​should be
confirmed with ENTER. After providing all the parameters regarding
network structure, the program informs about the correct creation of the network and returns to the menu
main page, where new options regarding the neural network are unlocked.

**2. Train the neural network on the training data** - the program goes to training
neural network. The user is asked to enter the number of network training cycles.
After entering the number of cycles, confirm the operation with the ENTER key. Program
displays all training data classification information in steps of 10
cycle. If the user has defined an extensive network structure for any
improvement can be counted after a thousand or several thousand learning cycles. Otherwise
In this case, the quality of classification of training data should be at
approximately 80-90% after less than a thousand learning cycles. After training
additional options are displayed to the user:
  - **Keep training the network** – This goes back to training the network where it needs to be re-trained
providing the number of training cycles
  - **Generate a TotalErrorPlot chart** – generates a chart showing errors
committed by the network in subsequent learning cycles. After closing the window
graph, the program returns to network training, where it is possible to enter another one
number of cycles or return to the menu by pressing the Q key and confirming
it with ENTER.
  - **Classify test data** – performs classification
test data randomly selected from the training set. Classification quality
test data allows you to determine whether the network is sufficiently trained
to classify cities. Enough to get back to net training
press any button on the keyboard.
  - **Return to menu** – the program returns to the main menu
    
**3. Classify cities** - the program asks you to enter the path to the z file
the data to be classified, the default file is "ClassificationData.txt",
which should also be entered when specifying the path. A possible option is to create
your own file with the data to be classified and specifying the path to it, but the file
must be created according to the pattern given in the "ClassificationData.txt" file, v
otherwise, you may be exposed to unexpected program behavior.
After entering the file path, the program displays the classification results along with
exit designated by the network and a summary of the city selection. To get back to
main menu, just press any button on the keyboard.

**4. Specify the parameters of the genetic algorithm** - this option is only enabled
only after completing point 3, i.e. classification
cities. After selecting this option, the program asks the user to provide appropriate information
values ​​for crossover probability, mutation probability and size
population. Each entered value should be confirmed with the ENTER button.
Crossover and mutation probability values ​​should be reported as numbers
real in the interval [0,1]. Numbers should be entered in the form, e.g. 0.2, not 0.2.
After entering all required parameters, the program informs you
user about the correct configuration of the genetic algorithm and returns to
main menu.

**5. Determine the optimal driving route between cities** - this option is active
only after providing the parameters of the genetic algorithm
(point 4). After selecting this option, the program asks for the maximum number of iterations.
The larger the number, the longer the execution time, but theoretically it is possible
getting a better solution. After entering the number of iterations, the program displays everyone
individuals (the chromosomes of individuals correspond to the indexes of cities in the dist table) in
each subsequent generation and at each generation it displays the best one
the individual found so far and the value of its fitness function (sum of distances
between cities, including the starting city at the beginning and at the end). On
at the end the program displays the best individual found among all
generations and the value of its fitness function. Additionally, there are city indexes
are replaced with the corresponding city names and the full route is calculated
sightseeing. After viewing the information displayed, the user can return to
main menu by pressing any button on the keyboard.

**6. Generate a graph FitnessPlot** – generates a graph showing the fitness values ​​in
each subsequent generation - this way you can more easily see 
which generation was designated the best-adapted individual and what age it has
value of the fitness function. To return to program execution, simply close it
window with a chart.

**7. End program** – ends program execution

### Additional informations
**All data used by the program can be freely modify. However, it is worth following the instructions presented in the files
pattern. Otherwise, you may expose yourself to the unexpected
program behavior.** The data is located in the directory
NeuralNetwork/bin/Debug/net6.0/ in files:
- TrainingData.txt – neural network training data
- ClassificationData.txt – data subjected to classification by the neural network
to designate cities worth visiting
- CitiesDist.txt – data on the distance between each city
cities

If you add a new city to the ClassificationData.txt file **you must
also add them to the CitiesDist.txt file, specifying its distance from the others
cities included in this file according to the formula.**

When changing the starting city in the CitiesDist.txt file, **make sure that
it is also in the ClassificationData.txt file and has all
attribute values ​​preferably as 1 so that the network can add them correctly to
lists of cities worth visiting**. Otherwise, you may endanger yourself
to unexpected program behavior.

If you want to use the program's option to generate charts, **it is necessary
having the Python interpreter installed and installed
matplotlib.pyplot library**, because the program runs scripts written in
Python that generates charts.


> [!TIP]
> The python interpreter can be downloaded from the official website **[python.org]**
or **from Microsoft Store** (it is best to choose the latest version 3.11) - it's worth it
is to install it in the default directory, no impact is expected
another location for the interpreter to execute scripts
generating charts

> [!TIP]
> The matplotlib library can be installed after installation
Python interpreter by typing this command in the system console: **"pip install matplotlib"**
