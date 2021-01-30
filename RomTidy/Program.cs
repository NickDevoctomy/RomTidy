using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RomTidy
{
    class Program
    {
        private static Dictionary<string, GameList> _gameLists = new Dictionary<string, GameList>();

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter path to scan (e.g. I:\\  or  c:\\roms\\");
            var scanPath = Console.ReadLine();
            var gamesLists = Directory.GetFiles(scanPath, "gamelist.xml", SearchOption.AllDirectories);
            foreach(var curGameList in gamesLists)
            {
                var gameList = new GameList();
                var xml = XDocument.Parse(File.ReadAllText(curGameList));
                var games = xml.Root.Elements("game");
                foreach(var curGame in games)
                {
                    var game = DeserializeXMLStringToObject<Game>(curGame.ToString());
                    gameList.Games.Add(game);
                }
                _gameLists.Add(curGameList, gameList);
            }

            


            while (true)
            {
                Console.Clear();
                var selectedList = SelectList();
                if (string.IsNullOrEmpty(selectedList))
                {
                    return;
                }

                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1) Remove all 'ZZZ(notgame)' entries from list.");
                Console.WriteLine("2) Remove all 'ZZZ_UNK' entries from list.");
                Console.WriteLine("Q) Quit");
                var atMainMenu = true;
                var mainMenuInput = Console.ReadKey(true);
                while (atMainMenu)
                {
                    if (mainMenuInput.Key == ConsoleKey.Q)
                    {
                        return;
                    }
                   
                    if(mainMenuInput.Key == ConsoleKey.D1 || mainMenuInput.Key == ConsoleKey.D2)
                    {
                        atMainMenu = false;
                        Console.WriteLine("Please wait...");
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("What would you like to do?");
                        Console.WriteLine("1) Remove all 'ZZZ(notgame)' entries from list.");
                        Console.WriteLine("2) Remove all 'ZZZ_UNK' entries from list.");
                        Console.WriteLine("Q) Quit");
                        mainMenuInput = Console.ReadKey(true);
                    }
                }

                var gamesToRemove = new List<Game>();
                var labelToRemove = "";
                switch (mainMenuInput.Key)
                {
                    case ConsoleKey.D1:
                        {
                            labelToRemove = "ZZZ(notgame)";
                            foreach (var curGame in _gameLists[selectedList].Games)
                            {
                                if (curGame.name.StartsWith("ZZZ(notgame)"))
                                {
                                    gamesToRemove.Add(curGame);
                                }
                            }

                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            labelToRemove = "ZZZ_UNK";
                            foreach (var curGame in _gameLists[selectedList].Games)
                            {
                                if (curGame.name.StartsWith("ZZZ_UNK"))
                                {
                                    gamesToRemove.Add(curGame);
                                }
                            }

                            break;
                        }
                }

                Console.WriteLine($"Remove {gamesToRemove.Count} '{labelToRemove}' from list '{selectedList}' (y/n)?");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    foreach (var curNoGame in gamesToRemove)
                    {
                        if (DeleteGame(selectedList, curNoGame))
                        {
                            _gameLists[selectedList].Games.Remove(curNoGame);
                        }
                    }
                    var newGameList = SerializeObjectToXML(_gameLists.Values.First());
                    //File.Copy(selectedList, $"{selectedList}{DateTime.Now.ToString("ddMMyyyy-HHmmss")}{".bak"}");
                    //File.WriteAllText(selectedList, newGameList);
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                }

            }
        }

        private static bool DeleteGame(
            string listPath,
            Game game)
        {
            var directory = new FileInfo(listPath).Directory.FullName;
            if (!directory.EndsWith("\\")) directory += "\\";
            var romPath = directory + game.path?.Substring(2);
            var imagePath = directory + game.image?.Substring(2);
            var videoPath = directory + game.video?.Substring(2);
            var marqueePath = directory + game.marquee?.Substring(2);
            var thumbPath = directory + game.thumbnail?.Substring(2);
            var fanart = directory + game.fanart?.Substring(2);
            var manualPath = directory + game.manual?.Substring(2);

            if(File.Exists(romPath))
            {
                File.Delete(romPath);
            }

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            if (File.Exists(videoPath))
            {
                File.Delete(videoPath);
            }

            if (File.Exists(marqueePath))
            {
                File.Delete(marqueePath);
            }

            if (File.Exists(thumbPath))
            {
                File.Delete(thumbPath);
            }

            if (File.Exists(fanart))
            {
                File.Delete(fanart);
            }

            if (File.Exists(manualPath))
            {
                File.Delete(manualPath);
            }

            return true;
        }

        public static T DeserializeXMLStringToObject<T>(string xml)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xml)) return default(T);

            StringReader xmlStream = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            returnObject = (T)serializer.Deserialize(xmlStream);
            return returnObject;
        }

        public static string SerializeObjectToXML(object value)
        {
            if (value == null) return string.Empty;

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var retVal = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding(false));
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializer serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(xmlTextWriter, value, ns);
                memoryStream.Flush();
                retVal = Encoding.UTF8.GetString(memoryStream.ToArray());
                retVal = retVal.Replace(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                    "<?xml version=\"1.0\"?>");
            }
            return retVal;
        }

        private static string SelectList()
        {
            while (true)
            {
                Console.WriteLine("Select a list,");
                var lists = _gameLists.Keys.ToArray();
                var i = 1;
                foreach (var curList in lists)
                {
                    Console.WriteLine($"{i}) {curList}");
                    i++;
                }
                Console.WriteLine($"Q) Quit");
                var selectedList = Console.ReadLine();
                if (selectedList.ToLower().StartsWith("q")) return null;
                var selectedListIndex = int.Parse(selectedList);
                if(selectedListIndex >=0 && selectedListIndex <= _gameLists.Keys.Count)
                {
                    return lists[selectedListIndex-1];
                }
            }
        }
    }
}
