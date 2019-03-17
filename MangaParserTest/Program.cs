using MangaParser.Core.Client;
using MangaParser.Core.Interfaces;
using MangaParser.Parsers.MintManga;
using MangaParser.Parsers.ReadManga;
using System;
using System.Linq;

namespace MangaParserTest
{
    internal class Program
    {
        private static MangaClient mangaClient;

        private static void Main(string[] args)
        {
            mangaClient = new MangaClient();

            mangaClient.AddParser(new ReadMangaParser());
            mangaClient.AddParser(new MintMangaParser());

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
                        foreach (var item in mangaClient.GetParser<ReadMangaParser>().SearchManga(query))
                        {
                            Console.WriteLine("\n" + GetSeparator('#') + "\n");
                            Console.WriteLine(item);
                        }
                    }
                    break;

                case "mint":
                    {
                        foreach (var item in mangaClient.GetParser<MintMangaParser>().SearchManga(query))
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

        private static IManga GetByLink(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("\n" + GetSeparator('-') + "\n");

                var manga = mangaClient.GetManga(url);

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

                var chapters = mangaClient.GetChapters(url);

                foreach (var item in chapters)
                {
                    Console.WriteLine($"\n---\nChapter: {item.Name}\n---\n");

                    int i = 0;

                    foreach (var page in mangaClient.GetChapterPages(item.ChapterUri))
                    {
                        Console.WriteLine($"\nPage {++i}: {page.PageUri}");
                    }
                }

                Console.Write("\n" + GetSeparator('*') + "\n");
            }
        }

        private static void GetPages(IManga manga)
        {
            if (manga == null)
            {
                throw new ArgumentNullException(nameof(manga));
            }

            Console.Write("\n" + GetSeparator('*'));

            var chapters = mangaClient.GetChapters(manga.MangaUri).ToArray();

            foreach (var item in chapters)
            {
                Console.WriteLine($"\n---\nChapter: {item.Name}\n---\n");

                int i = 0;

                foreach (var page in mangaClient.GetChapterPages(item.ChapterUri))
                {
                    Console.WriteLine($"\nPage {++i}: {page.PageUri}");
                }
            }

            Console.Write("\n" + GetSeparator('*') + "\n");
        }

        private static void GetPages(string url, int chapterNumber)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("\n" + GetSeparator('*'));

                var manga = mangaClient.GetManga(url);

                var chapters = mangaClient.GetChapters(manga.MangaUri).ToArray();

                if (chapterNumber > 0 && chapterNumber <= chapters.Length)
                {
                    Console.WriteLine($"\n---\nChapter: {chapters[chapterNumber-1].Name}\nLink: {chapters[chapterNumber - 1].ChapterUri}\n---\n");

                    int i = 0;

                    foreach (var page in mangaClient.GetChapterPages(chapters[chapterNumber-1].ChapterUri))
                    {
                        Console.WriteLine($"\nPage {++i}: {page.PageUri}");
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

        private static void GetPages(IChapter chapter)
        {
            Console.Write("\n" + GetSeparator('*'));

            Console.WriteLine($"\n---\nChapter: {chapter.Name}\n---\n");

            int i = 0;

            foreach (var page in mangaClient.GetChapterPages(chapter.ChapterUri))
            {
                Console.WriteLine($"\nPage {++i}: {page.PageUri}");
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

            foreach (var item in mangaClient.SearchManga("a"))
            {
                TimeSeparate('.', 5000);

                var manga = GetByLink(item.MangaUri.OriginalString);

                var chapters = mangaClient.GetChapters(manga.MangaUri);

                foreach (var chapter in chapters)
                {
                    TimeSeparate('.', 2000);

                    GetPages(chapter);
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