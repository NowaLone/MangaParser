using MangaParser.Client;
using MangaParser.Models;
using MangaParser.Models.MangaData;
using System;

namespace MangaParserTest
{
    internal class Program
    {
        private static MangaClient mangaClient;

        private static void Main(string[] args)
        {
            mangaClient = new MangaClient();

            while (true)
            {
                var command = Console.ReadLine().Split(' ');

                switch (command[0])
                {
                    case "get":
                        GetByLink(command[1]);
                        break;
                    case "search":
                        {
                            if (command.Length == 2)
                                Search(command[1]);
                            else if (command.Length > 2)
                            {
                                Search(String.Join(" ", command, 2, command.Length - 2), command[1]);
                            }
                        }
                        break;
                    case "pages":
                        {
                            if (command.Length == 2)
                                GetPages(command[1]);
                            else if (command.Length == 3)
                                GetPages(command[1], int.Parse(command[2]));
                        }
                        break;
                    case "test":
                        Test();
                        break;
                    default:
                        Console.WriteLine("Wrong command");
                        break;
                }

            }
        }

        private static void Search(string query, string source = null)
        {
            Console.Write("\n" + GetSeparator('#') + "\n");

            switch (source)
            {
                case "read":
                    {
                        foreach (var item in mangaClient.ReadManga.SearchManga(query))
                        {
                            Console.WriteLine("\n" + GetSeparator('#') + "\n");
                            Console.WriteLine(item);
                        }
                    }
                    break;
                case "mint":
                    {
                        foreach (var item in mangaClient.MintManga.SearchManga(query))
                        {
                            Console.WriteLine("\n" + GetSeparator('#') + "\n");
                            Console.WriteLine(item);
                        }
                    }
                    break;
                default:
                    {
                        foreach (var item in mangaClient.SearchManga(query))
                        {
                            Console.WriteLine("\n" + GetSeparator('#') + "\n");
                            Console.WriteLine(item);
                        }
                    }
                    break;
            }

            Console.Write(GetSeparator('#') + "\n");
        }

        private static MangaObject GetByLink(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("\n" + GetSeparator('-') + "\n");

                var manga = mangaClient.GetMangaByLink(url);

                Console.WriteLine(manga);

                Console.Write(GetSeparator('-') + "\n");

                return manga;
            }
            else
                return null;
        }

        private static void GetPages(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("\n" + GetSeparator('*'));

                var manga = mangaClient.GetMangaByLink(url);

                for (int j = 0; j < manga.Chapters.Count; j++)
                {
                    Console.WriteLine($"\n---\nChapter: {manga.Chapters[j].Value}\n---\n");

                    int i = 0;

                    foreach (Uri page in mangaClient.GetChapterPages(manga.Chapters[j]))
                    {
                        Console.WriteLine($"\nPage {++i}: {page.OriginalString}");
                    }
                }

                Console.Write("\n" + GetSeparator('*') + "\n");
            }
        }
        private static void GetPages(MangaObject manga)
        {
            if (manga == null)
            {
                throw new ArgumentNullException(nameof(manga));
            }

            Console.Write("\n" + GetSeparator('*'));

            for (int j = 0; j < manga.Chapters.Count; j++)
            {
                Console.WriteLine($"\n---\nChapter: {manga.Chapters[j].Value}\n---\n");

                int i = 0;

                foreach (Uri page in mangaClient.GetChapterPages(manga.Chapters[j]))
                {
                    Console.WriteLine($"\nPage {++i}: {page.OriginalString}");
                }
            }

            Console.Write("\n" + GetSeparator('*') + "\n");
        }
        private static void GetPages(string url, int chapterNumber)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("\n" + GetSeparator('*'));

                var manga = mangaClient.GetMangaByLink(url);

                if (chapterNumber > 0 && chapterNumber <= manga.Chapters.Count)
                {
                    Console.WriteLine($"\n---\nChapter: {manga.Chapters[manga.Chapters.Count - chapterNumber].Value}\n---\n");

                    int i = 0;

                    foreach (var page in mangaClient.GetChapterPages(manga.Chapters[manga.Chapters.Count - chapterNumber]))
                    {
                        Console.WriteLine($"\nPage {++i}: {page.OriginalString}");
                    }

                    Console.Write("\n" + GetSeparator('*') + "\n");
                }
                else
                {
                    Console.WriteLine($"There is no chapter {chapterNumber}");
                    return;
                }
            }
        }
        private static void GetPages(ChapterData chapter)
        {
            Console.Write("\n" + GetSeparator('*'));

            Console.WriteLine($"\n---\nChapter: {chapter.Value}\n---\n");

            int i = 0;

            foreach (Uri page in mangaClient.GetChapterPages(chapter))
            {
                Console.WriteLine($"\nPage {++i}: {page.OriginalString}");
            }

            Console.Write("\n" + GetSeparator('*') + "\n");
        }

        private static void Test()
        {
            Console.Write("\n" + GetSeparator('#') + "\n");

            foreach (var item in mangaClient.SearchManga("a"))
            {
                Console.WriteLine("\n" + GetSeparator('#') + "\n");
                Console.WriteLine(item);
            }

            Console.Write(GetSeparator('#') + "\n");

            foreach (MangaTile item in mangaClient.SearchManga("a"))
            {
                TimeSeparate('.', 5000);

                var manga = GetByLink(item.MangaUrl);

                for (int j = 0; j < manga.Chapters.Count; j++)
                {
                    TimeSeparate('.', 2000);

                    GetPages(manga.Chapters[j]);
                }
            }
        }

        private static string GetSeparator(char symbol)
        {
            return new string(symbol, Console.WindowWidth);
        }

        private static void TimeSeparate(char symbol, int millisecondsTimeout)
        {
            Console.WriteLine("Waiting...");

            int lenght = millisecondsTimeout / Console.WindowWidth;

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(symbol);

                System.Threading.Thread.Sleep(lenght);
            }
        }
    }
}