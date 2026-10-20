using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoftwareArchiveTask2
{
    // Клас для бази даних "Архів програмного забезпечення" (Варіант 9)
    public class SoftwareArchive
    {
        private string appName;
        private string operatingSystem;
        private double sizeMB;
        private DateTime recordDate;

        public SoftwareArchive()
        {
            appName = "Невідомо";
            operatingSystem = "Невідомо";
            sizeMB = 0.0;
            recordDate = DateTime.Now;
        }

        public SoftwareArchive(string appName, string operatingSystem, double sizeMB, DateTime recordDate)
        {
            AppName = appName;
            OperatingSystem = operatingSystem;
            SizeMB = sizeMB;
            RecordDate = recordDate;
        }

        public string AppName
        {
            get { return appName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва програми не може бути порожньою.");
                appName = value.Trim();
            }
        }

        public string OperatingSystem
        {
            get { return operatingSystem; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Операційна система не може бути порожньою.");
                operatingSystem = value.Trim();
            }
        }

        public double SizeMB
        {
            get { return sizeMB; }
            set
            {
                if (value < 0)
                    throw new FormatException("Розмір програми не може бути від'ємним.");
                sizeMB = value;
            }
        }

        public DateTime RecordDate
        {
            get { return recordDate; }
            set
            {
                if (value > DateTime.Now)
                    throw new FormatException("Дата запису не може бути в майбутньому.");
                recordDate = value;
            }
        }

        public override string ToString()
        {
            return $"Назва: {AppName,-18} | ОС: {OperatingSystem,-12} | Розмір: {SizeMB,6:F1} МБ | Дата: {RecordDate.ToShortDateString()}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            List<SoftwareArchive> archiveList = new List<SoftwareArchive>();
            string filePath = "archive_db.txt";

            InitializeFileIfNotExists(filePath);
            LoadFromFile(filePath, archiveList);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n================ МЕНЮ АРХІВУ ПРОГРАМНОГО ЗАБЕЗПЕЧЕННЯ ================");
                Console.WriteLine("[д] - Додати новий запис");
                Console.WriteLine("[р] - Редагувати існуючий запис");
                Console.WriteLine("[в] - Видалити запис");
                Console.WriteLine("[о] - Вивести інформацію з файла на екран");
                Console.WriteLine("[п] - Пошук за операційною системою (параметр пошуку)");
                Console.WriteLine("[с] - Сортування за назвою програми (параметр сортування)");
                Console.WriteLine("[з] - Зберегти зміни у файл");
                Console.WriteLine("[Enter] - Вихід з програми");
                Console.Write("Натисніть відповідну клавішу на клавіатурі: ");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                Console.WriteLine(keyInfo.KeyChar);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    exit = true;
                    Console.WriteLine("Вихід з програми. До побачення!");
                }
                else
                {
                    char key = char.ToLower(keyInfo.KeyChar);
                    switch (key)
                    {
                        case 'д':
                            AddRecord(archiveList, filePath);
                            break;
                        case 'р':
                            EditRecord(archiveList, filePath);
                            break;
                        case 'в':
                            DeleteRecord(archiveList, filePath);
                            break;
                        case 'о':
                            DisplayFromFile(archiveList);
                            break;
                        case 'п':
                            SearchByOS(archiveList);
                            break;
                        case 'с':
                            SortByName(archiveList);
                            break;
                        case 'з':
                            SaveToFile(filePath, archiveList);
                            break;
                        default:
                            Console.WriteLine("Невідома команда. Будь ласка, оберіть літеру з меню або натисніть Enter для виходу.");
                            break;
                    }
                }
            }
        } // Кінець методу Main

        static void InitializeFileIfNotExists(string path)
        {
            if (!File.Exists(path))
            {
                string[] initialData = {
                    "Visual Studio;Windows;2500,5;2025-01-15",
                    "Photoshop;Windows;1800,0;2024-11-20",
                    "VLC Media Player;Windows;45,2;2023-05-10",
                    "Blender;Linux;450,0;2025-02-01",
                    "GIMP;Linux;250,3;2024-08-12",
                    "Xcode;macOS;4200,0;2025-01-10",
                    "Spotify;Windows;120,5;2024-10-05",
                    "IntelliJ IDEA;Windows;3100,0;2025-03-01",
                    "LibreOffice;Linux;550,0;2023-12-19",
                    "Docker;Linux;800,0;2025-02-25"
                };
                File.WriteAllLines(path, initialData);
            }
        }

        static void DisplayFromFile(List<SoftwareArchive> list)
        {
            Console.WriteLine("\n--- ІНФОРМАЦІЯ З ФАЙЛУ (АРХІВ ПРОГРАМ) ---");
            if (list.Count == 0)
            {
                Console.WriteLine("Архів порожній.");
                return;
            }
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"{"№",-3} | {"Назва програми",-18} | {"ОС",-12} | {"Розмір (МБ)",-10} | {"Дата запису",-10}");
            Console.WriteLine(new string('-', 70));

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                Console.WriteLine($"{i + 1,-3} | {item.AppName,-18} | {item.OperatingSystem,-12} | {item.SizeMB,-10:F1} | {item.RecordDate.ToShortDateString(),-10}");
            }
            Console.WriteLine(new string('-', 70));
        }

        static void AddRecord(List<SoftwareArchive> list, string filePath)
        {
            try
            {
                Console.WriteLine("\n--- ДОДАВАННЯ НОВОГО ЗАПИСУ ---");
                Console.Write("Введіть назву програми: ");
                string name = Console.ReadLine();

                Console.Write("Введіть операційну систему: ");
                string os = Console.ReadLine();

                Console.Write("Введіть розмір програми в МБ: ");
                double size = Convert.ToDouble(Console.ReadLine());

                Console.Write("Введіть дату запису у форматі РРРР-ММ-ДД: ");
                DateTime date = Convert.ToDateTime(Console.ReadLine());

                SoftwareArchive newRecord = new SoftwareArchive(name, os, size, date);
                list.Add(newRecord);

                SaveToFile(filePath, list);
                Console.WriteLine("Запис успішно додано та збережено у файл!");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[ПОМИЛКА ФОРМАТУ]: Некоректні дані. {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[ПОМИЛКА ДАНИХ]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА]: {ex.Message}");
            }
        }

        static void EditRecord(List<SoftwareArchive> list, string filePath)
        {
            DisplayFromFile(list);
            if (list.Count == 0) return;

            try
            {
                Console.Write("\nВведіть номер запису для редагування: ");
                int index = Convert.ToInt32(Console.ReadLine()) - 1;

                if (index < 0 || index >= list.Count)
                {
                    Console.WriteLine("Помилка: запис з таким номером не знайдено.");
                    return;
                }

                Console.Write("Введіть нову назву програми (або Enter для пропуску): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name)) list[index].AppName = name;

                Console.Write("Введіть нову операційну систему (або Enter для пропуску): ");
                string os = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(os)) list[index].OperatingSystem = os;

                Console.Write("Введіть новий розмір у МБ (або -1 для пропуску): ");
                string sizeStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(sizeStr) && double.TryParse(sizeStr, out double size) && size != -1)
                {
                    list[index].SizeMB = size;
                }

                SaveToFile(filePath, list);
                Console.WriteLine("Запис успішно відредаговано та збережено у файл!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА РЕДАГУВАННЯ]: {ex.Message}");
            }
        }

        static void DeleteRecord(List<SoftwareArchive> list, string filePath)
        {
            DisplayFromFile(list);
            if (list.Count == 0) return;

            try
            {
                Console.Write("\nВведіть номер запису для видалення: ");
                int index = Convert.ToInt32(Console.ReadLine()) - 1;

                if (index >= 0 && index < list.Count)
                {
                    list.RemoveAt(index);
                    SaveToFile(filePath, list);
                    Console.WriteLine("Запис успішно видалено з бази та файлу!");
                }
                else
                {
                    Console.WriteLine("Помилка: введено невірний номер запису.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА ВИДАЛЕННЯ]: {ex.Message}");
            }
        }

        static void SearchByOS(List<SoftwareArchive> list)
        {
            Console.Write("\nВведіть операційну систему для пошуку (наприклад, Windows, Linux): ");
            string query = Console.ReadLine().Trim();

            var results = list.Where(x => x.OperatingSystem.Equals(query, StringComparison.OrdinalIgnoreCase)).ToList();

            Console.WriteLine($"\n--- РЕЗУЛЬТАТИ ПОШУКУ (Операційна система: {query}) ---");
            if (results.Count == 0)
            {
                Console.WriteLine("За заданою ознакою нічого не знайдено.");
            }
            else
            {
                foreach (var item in results)
                {
                    Console.WriteLine(item);
                }
            }
        }

        static void SortByName(List<SoftwareArchive> list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine("Архів порожній.");
                return;
            }

            var sortedList = list.OrderBy(x => x.AppName).ToList();

            Console.WriteLine("\n--- ВПОРЯДКОВАНИЙ СПИСОК (Сортування за назвою програми) ---");
            foreach (var item in sortedList)
            {
                Console.WriteLine(item);
            }
        }

        static void LoadFromFile(string path, List<SoftwareArchive> list)
        {
            try
            {
                list.Clear();
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length == 4)
                        {
                            string name = parts[0];
                            string os = parts[1];
                            double size = Convert.ToDouble(parts[2]);
                            DateTime date = Convert.ToDateTime(parts[3]);
                            list.Add(new SoftwareArchive(name, os, size, date));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка читання файлу: {ex.Message}");
            }
        }

        static void SaveToFile(string path, List<SoftwareArchive> list)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    foreach (var item in list)
                    {
                        sw.WriteLine($"{item.AppName};{item.OperatingSystem};{item.SizeMB};{item.RecordDate.ToString("yyyy-MM-dd")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження файлу: {ex.Message}");
            }
        }

    } // Кінець класу Program
} // Кінець простору імен SoftwareArchiveTask2