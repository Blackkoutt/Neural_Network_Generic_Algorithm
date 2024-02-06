import matplotlib.pyplot as plt

# Odczyt danych z pliku
data = []
with open('fitness.txt', 'r') as file:
    for line in file:
        value = int(line.strip())
        data.append(value)

# Generowanie wykresu
x = range(0, len(data))
plt.plot(x, data, 'o-')
plt.xlabel('Generation')
plt.ylabel('Fitness value')
plt.title('Wykres dopasowania kolejnych pokoleń')
plt.grid()
plt.xticks(range(len(data)))
plt.show()