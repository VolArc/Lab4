namespace Lab4;

internal static class Program { 
    private static void Main() {
        Console.CursorVisible = false;
        var work = true;
        Console.Clear();
        var lastAction = "";
        var arrayString = "";
        var showArray = false;
        var arrayPrinted = false;
        var option = 0;
        const int limit = 200000;
        var arrayLength = InputWithLimit("Введите количество элементов массива: ", 1, limit);
        var array = FillArray(arrayLength, Menu(["Ввести элементы вручную.", "Сгенерировать случайные числа."], 0) == 1);
        var isSorted = CheckSort(array);
        while (work) {
            option = Menu(["Выйти",
                "Удалить элементы c чётными номерами.", 
                $"Добавить K элементов в конец (максимум: {limit - arrayLength}).",
                "Поменять местами максимальный и минимальный элементы", 
                "Найти первый отрицательный",
                "Отсортировать простым включением",
                "Отсортировать сортировкой Шелла",
                showArray ? "Скрыть массив" : "Показать массив", 
                "Найти элемент массива",
                "Сформировать массив заново."], 
                option, 
                $"Длина массива: #{arrayLength}#.\nМассив " + (!isSorted ? "не " : "") + "отсортирован.\n" + lastAction, 
                showArray ? arrayString : "");

            switch (option) {
                case 1:
                    array = DeleteOdd(array);
                    lastAction = "Удалены элементы в чётных позициях.";
                    arrayString = PrintArray(array);
                    arrayPrinted = true;
                    arrayLength = array.Length;
                    break;
                case 2:
                    if (arrayLength < limit) {
                        array = AddElements(array, limit - arrayLength);
                        lastAction = "Добавлены элементы в массив.";
                        arrayString = PrintArray(array);
                        arrayPrinted = true;
                        isSorted = CheckSort(array);
                        arrayLength = array.Length;
                    }
                    else lastAction = $"Достигнут лимит элементов в массиве: #{limit}#.";
                    break;
                case 3:
                    var (minIndex, maxIndex, minValue, maxValue) = SwitchMaxMin(array);
                    array[minIndex] = maxValue;
                    array[maxIndex] = minValue;
                    lastAction = "Переставлены минимальный и максимальный элементы.";
                    arrayString = PrintArray(array, [minIndex, maxIndex]);
                    arrayPrinted = true;
                    isSorted = CheckSort(array);
                    break;
                case 4:
                    (lastAction, arrayString) = FindFirstNegative(array);
                    arrayPrinted = true;
                    break;
                case 5:
                    if (!isSorted) {
                        array = InsertionSort(array);
                        arrayString = PrintArray(array);
                        arrayPrinted = true;
                        isSorted = true;
                    }
                    break;
                case 6:
                    if (!isSorted)
                    {
                        array = ShellSort(array);
                        arrayString = PrintArray(array);
                        arrayPrinted = true;
                        isSorted = true;
                    }
                    break;
                case 7:
                    if (!arrayPrinted) arrayString = PrintArray(array);
                    if (arrayLength <= 500) showArray = !showArray;
                    else {
                        Console.Clear();
                        PrintMessage(arrayString);
                        Console.Write("Нажмите любую кнопку, чтобы продолжить...");
                        Console.ReadKey();
                    }
                    break;
                case 8:
                    if (isSorted) {
                        var numberSearch = InputInt("Введите число для поиска: ");
                        var searchResult = BinarySearch(array, numberSearch);
                        var searchPosition = searchResult.Item1;
                        var count = searchResult.Item2;
                        if (searchPosition == -1) {
                            lastAction = $"Числа #{numberSearch}# нет в массиве.";
                            arrayString = PrintArray(array);
                            arrayPrinted = true;
                        }
                        else {
                            lastAction = $"Позиция найденного числа = #{searchPosition + 1}#.\nКоличество сравнений: #{count}#.";
                            arrayString = PrintArray(array, [searchPosition]);
                            arrayPrinted = true;
                        }
                    } else {lastAction = "Сначала отсортируйте массив.";}
                    break;
                case 9:
                    arrayLength = InputWithLimit("Введите количество элементов массива: ", 1, limit);
                    array = FillArray(arrayLength, Menu(["Ввести элементы вручную.", "Сгенерировать случайные числа."], 0) == 1);
                    isSorted = CheckSort(array);
                    arrayString = PrintArray(array);
                    arrayPrinted = true;
                    break;
                default:
                    work = false;
                    break;
            }

            
            if (arrayLength > 500) showArray = false;
        }
        Console.CursorVisible = true;
    }

    private static bool CheckSort(int[] array) {
        int i;
        for (i = 1; i < array.Length; i++) {
            if (array[i] < array[i - 1]) break;
            Console.Write($"Проверка сортировки {i + 1}/{array.Length}.");
            Console.Clear();
        }
        return array.Length == 1 || i == array.Length;
    }
    
    private static int InputInt(string message) {
        int number;
        Console.Write(message);
        Console.CursorVisible = true;

        while (!int.TryParse(Console.ReadLine(), out number))
            Console.Write($"Ошибка! Введите целое число.\n{message}");

        Console.CursorVisible = false;

        return number;
    }

    private static int InputWithLimit(string message, int limitMin, int limitMax = int.MaxValue) {
        int number;

        do {
            number = InputInt(message);
            if (number < limitMin || number > limitMax) Console.WriteLine($"Ошибка!!! Число должно быть больше {limitMin - 1} и меньше {limitMax + 1}!");
        } while (number < limitMin || number > limitMax);

        return number;
    }

    private static int Menu(string[] options, int option, string highlightedMessage = "", string arrayMessage = "") {
        var work = true;
        var length = options.Length;
        var selectedOption = option >= 0 && option < length ? option : 0;

        while (work) {
            Console.Clear();
            Console.WriteLine("Выберите один из следующих вариантов:");
            for (var i = 0; i < length; i++) {
                Console.ResetColor();
                if (i == selectedOption) {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{i}. {options[i]}");
                    Console.ResetColor();
                }
                else 
                    Console.WriteLine($"{i}. {options[i]}");
            }
            
            if (highlightedMessage != "") {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                var highlight = false;
                foreach (var symbol in highlightedMessage) {
                    if (symbol == '#') highlight = !highlight;
                    else if (highlight) {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(symbol);
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else Console.Write(symbol);
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            if (arrayMessage != "") PrintMessage(arrayMessage);

            var key = Console.ReadKey();
            ConsoleKey[] keys = [ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9];
            if (key.Key == ConsoleKey.UpArrow) selectedOption = (selectedOption - 1 + length) % length;
            else if (key.Key == ConsoleKey.DownArrow) selectedOption = (selectedOption + 1) % length;
            else if (key.Key == ConsoleKey.Enter) work = false;
            else {
                for (var i = 0; i < (length < keys.Length ? length : keys.Length); i++) {
                    if (key.Key == keys[i]) {
                        selectedOption = i;
                        break;
                    }
                }
            }
            Console.ResetColor();
            Console.Clear();
        }
        
        
        return selectedOption;
    }

    private static void PrintMessage(string message) {
        var highlight = false;
        foreach (var symbol in message) {
            if (symbol == '#') highlight = !highlight;
            else if (highlight) {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(symbol);
                Console.ResetColor();
            }
            else Console.Write(symbol);
        }
    }


    private static int[] FillArray(int length, bool randomFilling) {
        var array = new int[length];
        if (randomFilling) {
            var minValue = InputInt("Введите нижнюю границу: ");
            var maxValue = InputWithLimit("Введите верхнюю границу: ", minValue);
            var random = new Random();
            var fillArrayBar = new LoadBar("Заполнение массива.", 0, length - 1);

            for (var i = 0; i < length; i++) {
                array[i] = random.Next(minValue, maxValue);
                fillArrayBar.RenewIteration(i);
            }
            
        }
        else {
            Console.WriteLine("Введите элементы массива:");

            for (var i = 0; i < length; i++)
                array[i] = InputInt($"{i + 1} ? ");
        }
        
        return array;
    }
    

    private static int[] InsertionSort(int[] array, bool showBar = true) {
        if(showBar) {
            LoadBar sortBar = new LoadBar("Сортировка массива", 1, array.Length - 1);
            
            for (var i = 1; i < array.Length; i++) {
                for (var j = i - 1; j >= 0 && array[j + 1] < array[j]; j--)
                    (array[j], array[j + 1]) = (array[j + 1], array[j]);

                sortBar.RenewIteration(i);
            }
        }
        else {
            for (var i = 1; i < array.Length; i++) {
                for (var j = i - 1; j >= 0 && array[j + 1] < array[j]; j--)
                    (array[j], array[j + 1]) = (array[j + 1], array[j]);
            }
        }

        return array;
    }

    private static int[] ShellSort(int[] array) {
        int[] tempArray = [];
        LoadBar sortBar = new LoadBar("Сортировка массива.", 2, array.Length);
        for (var i = 2; i < array.Length; i *= 2)
        {
            tempArray = [];
            foreach (var subArray in SplitArray(array, i))
                tempArray = tempArray.Concat(InsertionSort(subArray, false)).ToArray();
            sortBar.RenewIteration(i);
            array = tempArray;
        }
        sortBar.RenewIteration (array.Length);
        return InsertionSort(array, false);
    }

    private static int[][] SplitArray(int[] array, int step) {
    int numberOfSubArrays = (array.Length + step - 1) / step;
    int[][] newArray = new int[numberOfSubArrays][];
    
    for (var i = 0; i < array.Length; i++) {
        if (i % step == 0) {
            int currentStep = Math.Min(step, array.Length - i);
            newArray[i / step] = new int[currentStep];
        }
        newArray[i / step][i % step] = array[i];
    }

    return newArray;
}

    private static Tuple<int, int> BinarySearch(int[] array, int element) {
        int start = 0, end = array.Length - 1, count = 1;
        var found = false;

        while (start <= end) {
            if (array[(start + end) / 2] == element) {
                found = true;
                break;
            }

            if (array[(start + end) / 2] > element) end = (start + end) / 2 - 1;
            else start = (start + end) / 2 + 1;
            count++;
        }

        return Tuple.Create(found ? (start + end) / 2 : -1, count);
    }

    private static int[] DeleteOdd(int[] array) {
        var arrayLength = array.Length;
        var tempArray = new int[arrayLength / 2 + arrayLength % 2];
        int i;
        for (i = 0; i < arrayLength; i += 2) tempArray[i / 2] = array[i];
        return tempArray;
    }

    private static int[] AddElements(int[] array, int limitMax) {
        var arrayLength = InputWithLimit("Введите количество элементов для добавления в массив:", 0, limitMax);
        return array.Concat(FillArray(arrayLength, Menu(["Ввести элементы вручную.", "Сгенерировать случайные числа."], 0) == 1)).ToArray();
    }

    private static Tuple<int, int, int, int> SwitchMaxMin(int[] array) {
        int max = array[0], maxIndex = 0;
        int min = array[0], minIndex = 0;

        for (var i = 1; i < array.Length; i++) {
            if (array[i] < min) {
                min = array[i];
                minIndex = i;
            }
            else if (array[i] > max) {
                max = array[i];
                maxIndex = i;
            }
        }

        return Tuple.Create(minIndex, maxIndex, min, max);
    }

    private static Tuple<string, string> FindFirstNegative(int[] array) {
        var firstNegative = 0;
        int i;
        
        for (i = 0; i < array.Length; i++)
            if (array[i] < 0) {
                firstNegative = array[i];
                break;
            }

        return Tuple.Create($"Количество сравнений = #{i + 1}#.\n" + (firstNegative == 0
            ? "В массиве нет отрицательных чисел"
            : $"Первое отрицательное число в массиве = #{firstNegative}#."), PrintArray(array, [i]));
    }

    private static string PrintArray(int[] array, int[]? highlights = null) {
        var arrayString = "";
        var printArrayBar = new LoadBar("Запись массива.", 0, array.Length - 1);
        if (highlights != null) {
            for (var i = 0; i < array.Length; i++) {
                arrayString += highlights.Contains(i) ? $"#{array[i]}# " : $"{array[i]} ";
                printArrayBar.RenewIteration(i);
            }
        }
        else {
            for (var i = 0; i < array.Length; i++) {
                arrayString += $"{array[i]} ";
                printArrayBar.RenewIteration(i);
            }
        }
        
        return arrayString;
    }
}

internal class LoadBar {
    private int _load;
    private readonly int _initialIteration;
    private readonly int _finalIteration;
    private readonly int _length;
    private const int BarSize = 100;

    public LoadBar(string message, int initialIteration, int finalIteration) {
        _initialIteration = initialIteration;
        _finalIteration = finalIteration;
        _length = message.Length;
        _load = 0;
        Console.Write(message + " [");
        Console.SetCursorPosition(_length + BarSize + 2, Console.CursorTop);
        Console.Write("]");
        Console.SetCursorPosition(_length + 2, Console.CursorTop);
    }

    public void RenewIteration(int currentIteration) {
        var portion = (double)(currentIteration - _initialIteration) / (_finalIteration - _initialIteration);
        for (; _load < portion * BarSize; _load++) {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" ");
            Console.ResetColor();
        }
        
        var position = Console.GetCursorPosition().Item1;
        Console.SetCursorPosition(_length + BarSize + 4, Console.CursorTop);
        Console.Write("        ");
        Console.SetCursorPosition(_length + BarSize + 4, Console.CursorTop);
        Console.Write($"{Math.Round(portion * 100, 2)}%");
        Console.SetCursorPosition(position, Console.CursorTop);
        if (portion == 1) Console.WriteLine();
    }
}