using System;
using System.Collections.Generic;
using System.IO;

namespace KBuildSystem.App.Build
{
    public class BuildHistoryObject
    {
        Dictionary<string, string> FileData = new Dictionary<string, string>();
        public string FilePath { get; private set; }

        public bool Valid { get; private set; }

        public BuildHistoryObject(string filePath)
        {
            FilePath = filePath;
            Read();

            Valid = false;
        }

        public void Write()
        {
            List<string> fileLines = new List<string>();

            foreach (KeyValuePair<string, string> pair in FileData)
            {
                fileLines.Add(String.Format(@"{0}={1}", pair.Key, pair.Value));
            }

            File.WriteAllLines(FilePath, fileLines);
        }
        public void Read()
        {
            string[] lines = File.ReadAllLines(FilePath);
            ReadLines(lines);
        }
        public void ReadLines(string[] lines)
        {
            foreach (string line in lines)
            {
                string[] pair = line.Split('=');
                string key = pair[0];
                string value = line.Replace(String.Format(@"{0}=", key), @"");

                if (FileData.ContainsKey(key))
                    FileData[key] = value;
                else
                    FileData.Add(key, value);
            }
        }

        public string ID { get { return FileData[@"id"]; } set { FileData[@"id"] = value; } }
        public string Repository { get { return FileData[@"repo"]; } set { FileData[@"repo"] = value; } }
        public string Organization { get { return FileData[@"org"]; } set { FileData[@"org"] = value; } }
        public string Branch { get { return FileData[@"branch"]; } set { FileData[@"branch"] = value; } }
        public string SourceFolder { get { return FileData[@"srcfolder"]; } set { FileData[@"srcfolder"] = value; } }
        public string OutputFolder { get { return FileData[@"outfolder"]; } set { FileData[@"outfolder"] = value; } }
    }
}
