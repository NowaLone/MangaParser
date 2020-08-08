using MangaParser.Core.Client;
using MangaParser.Core.Interfaces;
using MangaParser.Parsers.HtmlWebParsers.MangaFox;
using MangaParser.Parsers.HtmlWebParsers.Mangapanda;
using MangaParser.Parsers.HtmlWebParsers.Mangareader;
using MangaParser.Parsers.HtmlWebParsers.Mangatown;
using MangaParser.Parsers.HtmlWebParsers.MintManga;
using MangaParser.Parsers.HtmlWebParsers.ReadManga;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MangaParser.ConsoleApp
{
    public class Program
    {
        private static readonly IClient client = new MangaClient(new IParser[]
        {
            new ReadMangaParser(),
            new MintMangaParser(),
            new MangaFoxParser(),
            new MangareaderParser(),
            new MangapandaParser(),
            new MangatownParser(),
        });

        private static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Console client representation for the MangaParser library. The app can search for manga, get detailed info about manga and chapters, and get chapter pages.");

            Command searchCommand = new Command("search", "Search for manga")
            {
                new Option<string>(new string[] { "-q", "--query" }, "Search query"),
                new Option<string>(new string[] { "-p", "--parser" }, () => "all", "Use a specific parser for search")
            };
            searchCommand.Handler = CommandHandler.Create<string, string>(Search);

            Command getMangaCommand = new Command("manga", "Get a detailed info about manga")
            {
                new Option<string>(new string[] { "-u", "--url" }, "Manga url"),
            };
            getMangaCommand.Handler = CommandHandler.Create<string>(GetManga);

            Command getChaptersCommand = new Command("chapters", "Get all info about manga chapters")
            {
                new Option<string>(new string[] { "-u", "--url" }, "Manga url"),
            };
            getChaptersCommand.Handler = CommandHandler.Create<string>(GetChapters);

            Command getPagesCommand = new Command("pages", "Get chapter pages urls")
            {
                new Option<string>(new string[] { "-u", "--url" }, "Chapter url"),
            };
            getPagesCommand.Handler = CommandHandler.Create<string>(GetPages);

            Command getCommand = new Command("get", "Get data")
            {
                getMangaCommand,
                getChaptersCommand,
                getPagesCommand,
            };

            rootCommand.Add(searchCommand);
            rootCommand.Add(getCommand);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Search(string query, string parser)
        {
            if (parser == "all")
            {
                foreach (var item in client.SearchManga(query))
                {
                    WriteSeparator('-');
                    Write(item);
                    WriteSeparator('-');
                }
            }
            else
            {
                foreach (var p in client.GetParsers(parser))
                {
                    foreach (var item in p.SearchManga(query))
                    {
                        WriteSeparator('-');
                        Write(item);
                        WriteSeparator('-');
                    }
                }
            }
        }

        private static void GetManga(string url)
        {
            Uri uri = new Uri(url);

            var result = client.GetManga(uri);

            WriteSeparator('-');
            Write(result);
            WriteSeparator('-');
        }

        private static void GetChapters(string url)
        {
            Uri uri = new Uri(url);

            var result = client.GetChapters(uri);

            foreach (var item in result)
            {
                WriteSeparator('-');
                Write(item);
                WriteSeparator('-');
            }
        }

        private static void GetPages(string url)
        {
            Uri uri = new Uri(url);

            var result = client.GetPages(uri);

            int i = 0;

            WriteSeparator('-');

            foreach (var item in result)
            {
                Console.WriteLine($"Page {++i}:");
                Write(item);
            }

            WriteSeparator('-');
        }

        private static void Write(IMangaObject manga)
        {
            Console.WriteLine("Name:");
            Console.WriteLine(manga.Value);
            Console.WriteLine();

            if (manga.ReleaseDate != null)
            {
                Console.WriteLine("ReleaseDate:");
                Console.WriteLine(manga.ReleaseDate.Value.ToLongDateString());
                Console.WriteLine();
            }

            if (manga.Volumes != null)
            {
                Console.WriteLine("Volumes:");
                Console.WriteLine(manga.Volumes);
                Console.WriteLine();
            }

            if (!String.IsNullOrWhiteSpace(manga.Description?.Value))
            {
                Console.WriteLine("Description:");
                Console.WriteLine(manga.Description);
                Console.WriteLine();
            }

            if (manga.Covers.Count > 0)
            {
                Console.WriteLine("Covers:");

                foreach (var item in manga.Covers)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine();
            }

            if (manga.Genres.Count > 0)
            {
                Console.WriteLine("Genres:");

                Console.WriteLine(String.Join(", ", manga.Genres));

                Console.WriteLine();
            }

            if (manga.Authors.Count > 0)
            {
                Console.WriteLine("Authors:");

                Console.WriteLine(String.Join(", ", manga.Authors));

                Console.WriteLine();
            }

            if (manga.Writers.Count > 0)
            {
                Console.WriteLine("Writers:");

                Console.WriteLine(String.Join(", ", manga.Writers));

                Console.WriteLine();
            }

            if (manga.Illustrators.Count > 0)
            {
                Console.WriteLine("Illustrators:");

                Console.WriteLine(String.Join(", ", manga.Illustrators));

                Console.WriteLine();
            }

            if (manga.Magazines.Count > 0)
            {
                Console.WriteLine("Magazines:");

                Console.WriteLine(String.Join(", ", manga.Magazines));

                Console.WriteLine();
            }

            if (manga.Publishers.Count > 0)
            {
                Console.WriteLine("Publishers:");

                Console.WriteLine(String.Join(", ", manga.Publishers));

                Console.WriteLine();
            }

            Console.WriteLine(manga.Url);
        }

        private static void Write(IChapter chapter)
        {
            Console.WriteLine("Name:");
            Console.WriteLine(chapter.Value);
            Console.WriteLine();

            if (chapter.AddedDate != null)
            {
                Console.WriteLine("AddedDate:");
                Console.WriteLine(chapter.AddedDate.ToLongDateString());
                Console.WriteLine();
            }

            if (chapter.Cover != null)
            {
                Console.WriteLine("Cover:");
                Console.WriteLine(chapter.Cover);
                Console.WriteLine();
            }

            Console.WriteLine(chapter.Url);
        }

        private static void Write(IDataBase data)
        {
            if (data.Value != null)
            {
                Console.WriteLine("Value:");
                Console.WriteLine(data.Value);
                Console.WriteLine();
            }

            if (data.Url != null)
            {
                Console.WriteLine("Url:");
                Console.WriteLine(data.Url);
                Console.WriteLine();
            }
        }

        private static void WriteSeparator(char symbol)
        {
            WriteSeparator(symbol, 0);
        }

        private static void WriteSeparator(char symbol, int millisecondsTimeout)
        {
            int lenght = millisecondsTimeout / Console.BufferWidth;

            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write(symbol);

                System.Threading.Thread.Sleep(lenght);
            }

            Console.ResetColor();
        }
    }
}