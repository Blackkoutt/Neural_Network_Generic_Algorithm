import matplotlib.pyplot as plt

# Wczytaj dane z pliku
data = []
with open('errors.txt', 'r') as file:
    for line in file:
        value = float(line.strip().replace(',', '.'))
        data.append(value)


# Utwórz listę indeksów dla osi x
x = list(range(1, len(data)+1))

# Wygeneruj wykres
plt.plot(x, data)
plt.xlabel('Step Count')
plt.ylabel('Total Error')
plt.title('Total Error Plot')
plt.grid()
plt.show()