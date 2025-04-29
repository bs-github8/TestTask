using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Не правильно заданы пути для файлов.");
                Console.ReadLine();
                return;
            }

            if (!File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Console.WriteLine("Не существует одного или двух файлов.");
                Console.ReadLine();
                return;
            }
            
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(ref singleLetterStats, CharType.Consonants);
                PrintStatistic(singleLetterStats);
            }

            using(IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(ref doubleLetterStats, CharType.Vowel);
                PrintStatistic(doubleLetterStats);
            }
            
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            SortedDictionary<char, LetterStats> statats = new SortedDictionary<char, LetterStats>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!char.IsLetter(c))
                {
                    continue;
                }

                if (statats.ContainsKey(c))
                {
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                    IncStatistic(statats[c]);
                }
                else
                {
                    statats.Add(c, new LetterStats() { Letter = c.ToString(), Count = 1 });
                }
            }

            stream.Close();

            return statats.Select(s => s.Value).ToList();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            SortedDictionary<string, LetterStats> statats = new SortedDictionary<string, LetterStats>();
            char previousChar = char.MinValue ;

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!char.IsLetter(c))
                {
                    continue;
                }

                if (char.ToUpperInvariant(previousChar) == char.ToUpperInvariant(c))
                {
                    var chars = $"{previousChar}{c}".ToUpper();
                    previousChar = char.MinValue;

                    if (statats.ContainsKey(chars))
                    {
                        // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                        IncStatistic(statats[chars]);
                    }
                    else
                    {
                        statats.Add(chars, new LetterStats() { Letter = chars, Count = 1 });
                    }
                }
                else
                {
                    previousChar = c;
                }
            }

            stream.Close();

            return statats.Select(s => s.Value).ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    letters = letters
                        .Where(l => l.IsVowel())
                        .Select( l => l)
                        .ToList();
                    break;
                case CharType.Vowel:
                    letters = letters
                        .Where(l => !l.IsVowel())
                        .Select(l => l)
                        .ToList();
                    break;
            }

        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

            foreach (var stat in letters.OrderBy(l => l.Letter, StringComparer.Ordinal))
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
            }

            Console.WriteLine($"ИТОГО : {letters.Sum(l => l.Count)}");

        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
