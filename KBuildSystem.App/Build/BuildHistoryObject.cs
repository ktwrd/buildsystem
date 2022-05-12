using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            if (!File.Exists(filePath))
                File.Create(filePath);

            StatusHistory = new Dictionary<long, BuildStatusObject>();

            FileData.Add(@"statusHistory", @"0->Unknown");
            FileData.Add(@"id", Path.GetFileName(filePath).Replace(@".bhis", @""));
            FileData.Add(@"repo", @"");
            FileData.Add(@"org", @"");
            FileData.Add(@"branch", @"");
            FileData.Add(@"timestamp", @"0");
            FileData.Add(@"srcfolder", @"");
            FileData.Add(@"outfolder", @"");
            Read();
            FileData[@"id"] = Path.GetFileName(filePath).Replace(@".bhis", @"");
            Write();

            Valid = true;
        }

        public void Write()
        {
            List<string> fileLines = new List<string>();

            List<string> statusItems = new List<string>();
            foreach (KeyValuePair<long, BuildStatusObject> status in StatusHistory)
            {
                statusItems.Add(String.Format(@"{0}->{1}", status.Value.Timestamp.ToString(), status.Value.Status.ToString()));
            }
            if (FileData.ContainsKey(@"statusHistory"))
                FileData[@"statusHistory"] = String.Join(@" ", statusItems.ToArray());
            else
                FileData.Add(@"statusHistory", String.Join(@" ", statusItems.ToArray()));

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

            if (FileData.ContainsKey(@"statusHistory"))
            {
                string statusHistoryData = FileData[@"statusHistory"];
                string[] items = statusHistoryData.Split(' ');
                if (statusHistoryData.Length > 2)
                {
                    foreach (string pair in items)
                    {
                        if (pair.Length < 1) continue;
                        string[] pairSplit = pair.Split(new string[] { @"->" }, StringSplitOptions.None);
                        string timestampStr = pairSplit[0];
                        string enumStr = pairSplit[1];

                        long timestamp = System.Convert.ToInt64(timestampStr);
                        BuildStatus status = (BuildStatus)Enum.Parse(typeof(BuildStatus), enumStr);

                        BuildStatusObject statusObject = new BuildStatusObject(this)
                        {
                            Timestamp = timestamp,
                            Status = status
                        };

                        StatusHistory.Add(timestamp, statusObject);
                    }
                }
            }
        }

        public string ID { get { return FileData[@"id"]; } set { FileData[@"id"] = value; } }
        public string Repository { get { return FileData[@"repo"]; } set { FileData[@"repo"] = value; } }
        public string Organization { get { return FileData[@"org"]; } set { FileData[@"org"] = value; } }
        public long Timestamp { get { return Convert.ToInt64(FileData[@"timestamp"]); } set { FileData[@"timestamp"] = value.ToString(); } }
        public string Branch { get { return FileData[@"branch"]; } set { FileData[@"branch"] = value; } }
        public string SourceFolder { get { return FileData[@"srcfolder"]; } set { FileData[@"srcfolder"] = value; } }
        public string OutputFolder { get { return FileData[@"outfolder"]; } set { FileData[@"outfolder"] = value; } }
        public BuildStatusObject CurrentStatus { get { return StatusHistory.OrderBy(kvp => kvp.Key).ToArray()[0].Value;  } }
        public Dictionary<long, BuildStatusObject> StatusHistory { get; set; }
    }
}
