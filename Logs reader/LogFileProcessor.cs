using Newtonsoft.Json;
using System.IO;

namespace Logs_reader
{
    public class LogFileProcessor
    {
        private readonly string _optionsHttpVerb = "OPTIONS";
        public IEnumerable<string> FindLinesContainingTest(string folderPath, string searchTerm)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"The directory {folderPath} was not found.");
            }

            var logFiles = Directory.GetFiles(folderPath, "*.log");

            var linesContainingTest = new List<string>();

            foreach (var file in logFiles)
            {
                var lines = File.ReadLines(file);

                linesContainingTest.AddRange(lines.Where(line => line.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            return linesContainingTest;
        }

        public void WriteResultsToFile(IEnumerable<string> lines, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath, append: true))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public dynamic GetStatsFromFile(string path)
        {
            IISLogRecord[] records = ParseIISLogFile(path);

            double averageTime = records.Average(x => x.TimeTaken);
            int mostExpensiveCount = 1000;
            IISLogRecord[] topExpensiveRequests = records.OrderByDescending(x => x.TimeTaken).Take(mostExpensiveCount).ToArray();
            var grouppedRequests = records.GroupBy(x => x.Uri).Select(x => new
            {
                Uri = x.Key,
                Count = x.Count(),
                AverageTime = x.Average(c => c.TimeTaken),
                MinTime = x.Min(c => c.TimeTaken),
                MaxTime = x.Max(c => c.TimeTaken)
            });

            var mostPopularRequests = grouppedRequests.OrderByDescending(x => x.Count).ToArray();
            var mostExpensiveRequests = grouppedRequests.OrderByDescending(x => x.AverageTime).ToArray();

            return new
            {
                totalCount = records.Length,
                averageTime,
                topExpensiveRequests,
                mostPopularRequests,
                mostExpensiveRequests
            };
        }

        public IEnumerable<dynamic> GetAverageStatsFromFolder(string folder)
        {
            List<IISLogRecord> records = new List<IISLogRecord>();
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                records.AddRange(ParseIISLogFile(file).Where(r => r.HttpVerb != _optionsHttpVerb));
            }

            var grouppedRequests = records.GroupBy(x => x.Uri).Select(x => new
            {
                Uri = x.Key,
                Count = x.Count(),
                AverageTime = x.Average(c => c.TimeTaken),
                MinTime = x.Min(c => c.TimeTaken),
                MaxTime = x.Max(c => c.TimeTaken)
            });

            return grouppedRequests.OrderByDescending(r => r.Count);
        }

        public void SaveToJsonFile(string folder, string fileName, object objectToSave)
        {
            var json = JsonConvert.SerializeObject(objectToSave);
            var outputFile = Path.Combine(folder, $"{fileName}.json");
            File.WriteAllText(outputFile, json);
        }

        #region private methods
        private IISLogRecord[] ParseIISLogFile(string path)
        {
            List<string> fileLines = new List<string>(File.ReadAllLines(path));
            List<IISLogRecord> records = new List<IISLogRecord>();
            int headerStringsCount = 4;
            fileLines.RemoveRange(0, headerStringsCount);

            foreach (string line in fileLines)
            {
                string[] parts = line.Split(' ');
                int index = 0;

                if (parts[index].StartsWith("#"))
                    continue;

                IISLogRecord record = new IISLogRecord
                {
                    Date = parts[index++],
                    Time = parts[index++],
                    ServerIp = parts[index++],
                    HttpVerb = parts[index++],
                    Uri = parts[index++],
                    Query = parts[index++],
                    Port = parts[index++],
                    Username = parts[index++],
                    ClientIp = parts[index++],
                    UserAgent = parts[index++],
                    Referrer = parts[index++],
                    StatusCode = parts[index++],
                    SubStatusCode = parts[index++],
                    Win32StatusCode = parts[index++],
                    TimeTaken = int.Parse(parts[index++])
                };
                records.Add(record);
            }

            return records.ToArray();
        }
        #endregion
    }
}
