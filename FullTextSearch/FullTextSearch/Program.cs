using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FullTextSearch
{
    class Program
    {
        public static string IndexFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"))+"index";
        public static Dictionary<string, Dictionary<string, int>> InvertedIndex;
        public static Dictionary<string, int> DictTemp;

        static void Main(string[] args)
        {
            InvertedIndex = new Dictionary<string, Dictionary<string, int>>();

            Console.WriteLine("Full Text Search System\n");

            Call();

            Console.Read();
        }

        /// <summary>
        /// Get the user input for index program or search program and works appropriately
        /// </summary>        
        private static void Call()
        {
            string Input = Console.ReadLine();
            if (Input != string.Empty)
            {
                string[] InputArray = Input.Split(new char[] { ' ' }, 2);
                if (InputArray[0].ToLower() == "index")
                {
                    Console.WriteLine(Index(InputArray[1]));
                }
                else if (InputArray[0].ToLower() == "search")
                {
                    Search(InputArray[1]);
                } else
                {
                    Console.WriteLine("Program not found");
                }
                Call();
            }
        }

        /// <summary>
        /// Reads input file path, Index the contents and write to a file
        /// </summary> 
        /// <param name="FilePath">a string, passed by reference, 
        /// that contains the file path
        /// </param>
        private static string Index(string FilePath)
        {
            if (FileExists(FilePath))
            {
                string Folder = Path.GetDirectoryName(FilePath);
                var FileContent = System.IO.File.ReadAllText(FilePath);
                if (FileContent != string.Empty)
                {
                    FileContent = Regex.Replace(FileContent, @"[^0-9a-zA-Z \r\n]+", "").ToLower();
                    List<string> Content = FileContent.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    AddToIndex(Content, FilePath.Replace(Folder + "\\", ""));
                }
                else
                {
                    return "File is empty";
                }
            }
            else
            {
                return "File doesn't exist";
            }

            return "Index File Updated";
        }

        /// <summary>
        /// Setup word dictionary
        /// </summary> 
        /// <param name="Words">list of string, passed by reference, /// that contains the list of words/// </param>
        /// <param name="Document">string, passed by reference, /// that contains the file name/// </param>
        private static void AddToIndex(List<string> Words, string Document)
        {
            InvertedIndex = ReadFromIndexFile(Document);
            if (InvertedIndex == null)
                InvertedIndex = new Dictionary<string, Dictionary<string, int>>();
            foreach (var Word in Words)
            {
                if (!InvertedIndex.ContainsKey(Word))
                {
                    DictTemp = new Dictionary<string, int>();
                    DictTemp.Add(Document, 1);
                    InvertedIndex.Add(Word, DictTemp);
                }
                else
                {
                    if(InvertedIndex.Count > 0)
                    {
                        if (InvertedIndex[Word].ContainsKey(Document))
                        {   
                            InvertedIndex[Word][Document] = InvertedIndex[Word][Document] + 1;
                        }
                        else
                        {
                            DictTemp = new Dictionary<string, int>();
                            DictTemp = InvertedIndex[Word];
                            DictTemp.Add(Document, 1);
                            InvertedIndex[Word] = DictTemp;
                        }
                    }
                }
            }
            
            if (InvertedIndex != null && InvertedIndex.Count > 0)
                WriteIndexToFile(InvertedIndex);
            
        }

        /// <summary>
        /// Write index dictionary to a file
        /// </summary> 
        /// <param name="InvertedIndex">dictionary, passed by reference, /// that contains the index/// </param>
        private static void WriteIndexToFile(Dictionary<string, Dictionary<string, int>> InvertedIndex)
        {
            String Json = JsonConvert.SerializeObject(InvertedIndex, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(IndexFilePath, Json);

            InvertedIndex.Clear();
        }

        /// <summary>
        /// Read existing data from index file
        /// </summary> 
        /// <param name="Document">string, passed by reference, /// that contains the filename/// </param>
        private static Dictionary<string, Dictionary<string, int>> ReadFromIndexFile(string Document)
        {
            InvertedIndex = new Dictionary<string, Dictionary<string, int>>();

            if (FileExists(IndexFilePath))
                InvertedIndex = IndexFile();
            
            if (InvertedIndex!=null && InvertedIndex.Count > 0)
            {
                foreach(var item in InvertedIndex)
                {
                    if (item.Value.ContainsKey(Document))
                    {
                        InvertedIndex[item.Key][Document] = 0; //Reset text occurance count if same file input again
                    }
                }
            }

            return InvertedIndex;
        }

        /// <summary>
        /// Search keywords in index file
        /// </summary> 
        /// <param name="Text">string, passed by reference, /// that contains the keywords/// </param>
        private static void Search(string Text)
        {            
            if (Text!="")
            {
                List<string> Keywords = new List<string>();

                if (FileExists(IndexFilePath))
                    InvertedIndex = IndexFile();
                
                Keywords = Text.Split(' ').ToList();
                if (Keywords.Count > 0)
                {   
                    foreach (var Word in Keywords)
                    {
                        Console.WriteLine("Searching for " + Word + "...\n");
                        if (InvertedIndex.ContainsKey(Word))
                        {
                            var KeyValuePair = InvertedIndex.Single(x => x.Key == Word).Value.OrderByDescending(x => x.Value);

                            Console.WriteLine("Found in:");
                            if(KeyValuePair.Count() > 0)
                            {
                                foreach(var item in KeyValuePair)
                                {
                                    Console.WriteLine(item.Key+"("+item.Value+")");
                                }
                            }
                            Console.WriteLine("");
                        } else
                        {
                            Console.WriteLine("No Matches Found\n");
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// Check if input file exists
        /// </summary> 
        /// <param name="FilePath">string, passed by reference, /// that contains the filepath/// </param>
        private static bool FileExists(string FilePath)
        {
            if (FilePath.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && File.Exists(FilePath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get Contents from Index File
        /// </summary>
        private static Dictionary<string, Dictionary<string, int>> IndexFile()
        {   
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(File.ReadAllText(IndexFilePath));
        }
    }
}
