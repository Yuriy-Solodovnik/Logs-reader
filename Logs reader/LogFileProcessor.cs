namespace Logs_reader
{
    public class LogFileProcessor
    {
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

    }
}
