using MangaParser.Core.Client;
using MangaParser.Core.Interfaces;
using MangaParser.Parsers.MangaFox;
using MangaParser.Parsers.Mangapanda;
using MangaParser.Parsers.Mangareader;
using MangaParser.Parsers.Mangatown;
using MangaParser.Parsers.MintManga;
using MangaParser.Parsers.ReadManga;
using System;
using System.Linq;

namespace MangaParser.ConsoleApp
{
    public class Program
    {
        private static readonly MangaClient client = new MangaClient(new IParser[]
        {
            new ReadMangaParser(),
            new MintMangaParser(),
            new MangaFoxParser(),
            new MangareaderParser(),
            new MangapandaParser(),
            new MangatownParser(),
        });

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "get":
                        {
                            if (Uri.TryCreate(args[1], UriKind.Absolute, out Uri uri))
                            {
                                Get(uri);
                            }
                            else
                            {
                                Console.WriteLine("Wrong url: " + args[1]);
                            }
                        }
                        break;

                    case "search":
                        {
                            switch (args[1])
                            {
                                case "all":
                                    Search(args[2]);
                                    break;

                                case "read":
                                    Search(args[2], client.GetParser<ReadMangaParser>());
                                    break;

                                case "mint":
                                    Search(args[2], client.GetParser<MintMangaParser>());
                                    break;

                                case "fox":
                                    Search(args[2], client.GetParser<MangaFoxParser>());
                                    break;

                                case "reader":
                                    Search(args[2], client.GetParser<MangareaderParser>());
                                    break;

                                case "panda":
                                    Search(args[2], client.GetParser<MangapandaParser>());
                                    break;

                                case "town":
                                    Search(args[2], client.GetParser<MangatownParser>());
                                    break;

                                default:
                                    Search(args[1]);
                                    break;
                            }
                        }
                        break;

                    case "pages":
                        {
                            if (args.Length == 2)
                            {
                                if (Uri.TryCreate(args[1], UriKind.Absolute, out Uri uri))
                                {
                                    Pages(uri);
                                }
                                else
                                {
                                    Console.WriteLine("Wrong url: " + args[1]);
                                }
                            }
                            else if (args.Length == 3)
                            {
                                if (Uri.TryCreate(args[1], UriKind.Absolute, out Uri uri))
                                {
                                    if (int.TryParse(args[2], out int number))
                                    {
                                        Pages(uri, number);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Wrong number: " + args[2]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Wrong url: " + args[1]);
                                }
                            }
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

        private static IManga Get(Uri uri)
        {
            var manga = client.GetManga(uri);

            Console.WriteLine(manga);

            return manga;
        }

        private static void Search(string query)
        {
            foreach (var item in client.SearchManga(query))
            {
                Console.WriteLine(item);
            }
        }

        private static void Search(string query, IParser parser)
        {
            foreach (var item in parser.SearchManga(query))
            {
                Console.WriteLine(item);
            }
        }

        private static void Pages(Uri uri)
        {
            IManga manga = client.GetManga(uri);

            if (manga?.Name.ToString() != null)
            {
                Pages(manga);
                return;
            }

            int i = 0;

            foreach (var page in client.GetPages(uri))
            {
                Console.WriteLine($"Page {++i}: {page.PageUri}");
            }
        }

        private static void Pages(IChapter chapter)
        {
            Pages(chapter.ChapterUri);
        }

        private static void Pages(IManga manga)
        {
            foreach (var item in client.GetChapters(manga.Url))
            {
                Console.WriteLine("Chapter: " + item.Name);

                int i = 0;

                foreach (var page in client.GetPages(item.ChapterUri))
                {
                    Console.WriteLine($"Page {++i}: {page.PageUri}");
                }

                TimeSeparate('.', 2000);
            }
        }

        private static void Pages(Uri uri, int chapterNumber)
        {
            IManga manga = Get(uri);

            if (manga == null)
                return;

            IChapter chapter = client.GetChapters(manga.Url).ElementAt(chapterNumber - 1);

            Console.WriteLine("Chapter: " + chapter.Name);

            Pages(chapter);
        }

        private static void Test()
        {
            Console.Write("\n" + new string('#', Console.WindowWidth) + "\n");

            foreach (var item in client.SearchManga("a"))
            {
                Console.WriteLine("\n" + new string('#', Console.WindowWidth) + "\n");
                Console.WriteLine(item);
            }

            Console.Write(new string('#', Console.WindowWidth) + "\n");

            foreach (var item in client.SearchManga("a"))
            {
                TimeSeparate('.', 5000);

                var manga = Get(item.Url);

                var chapters = client.GetChapters(manga.Url);

                foreach (var chapter in chapters)
                {
                    TimeSeparate('.', 2000);

                    Pages(chapter);
                }
            }
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