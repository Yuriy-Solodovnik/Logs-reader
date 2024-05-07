using Logs_reader;

var processor = new LogFileProcessor();
var inputValidator = new InputValidator();

Dictionary<string, Action<string, LogFileProcessor, InputValidator>> modeActions = new Dictionary<string, Action<string, LogFileProcessor, InputValidator>>
    {
        { "Get All Stats", GetAndSaveAllStats },
        { "Get All Stats By Uri", GetAndSaveAllStatsByUri },
        { "Search Mode", SearchAndSaveLogs }
    };

Console.WriteLine("Select a mode:");
foreach (var mode in modeActions.Keys)
{
    Console.WriteLine(mode);
}

string selectedMode = inputValidator.GetValidInput();
if (!modeActions.ContainsKey(selectedMode))
{
    Console.WriteLine("Invalid mode selection.");
    return;
}

string baseFolderPath;
Console.WriteLine("Write folder path");
baseFolderPath = inputValidator.GetValidInput();

if (string.IsNullOrEmpty(baseFolderPath) || !Directory.Exists(baseFolderPath))
{
    Console.WriteLine("Invalid path or folder does not exist.");
    return;
}

modeActions[selectedMode](baseFolderPath, processor, inputValidator);


static void SearchAndSaveLogs(string baseFolderPath, LogFileProcessor processor, InputValidator inputValidator)
{
    try
    {
        Console.WriteLine("Write search term");
        var searchTerm = inputValidator.GetValidInput();

        var lines = processor.FindLinesContainingTest(baseFolderPath, searchTerm);

        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }

        bool continueProcessing = inputValidator.GetYesNoInput("Do you want to continue?");
        if (!continueProcessing)
        {
            return;
        }
        var filePath = inputValidator.GetValidFileInput();
        processor.WriteResultsToFile(lines, filePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

static void GetAndSaveAllStats(string baseFolderPath, LogFileProcessor processor, InputValidator inputValidator)
{
    try
    {
        Console.WriteLine("Generation in process...");
        var webServersFolders = Directory.GetDirectories(baseFolderPath);
        var allStats = new Dictionary<string, Dictionary<string, dynamic>>();

        foreach (var webServerFolder in webServersFolders)
        {
            var folder = Path.Combine(baseFolderPath, webServerFolder);
            var files = Directory.GetFiles(folder);
            var folderStats = new Dictionary<string, dynamic>();
            foreach (var file in files)
            {
                var stats = processor.GetStatsFromFile(file);
                folderStats[Path.GetFileName(file)] = stats;
            }
            allStats[webServerFolder] = folderStats;
        }

        Console.WriteLine("Input file name");
        var fileName = inputValidator.GetValidInput();
        processor.SaveToJsonFile(baseFolderPath, fileName, allStats);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

static void GetAndSaveAllStatsByUri(string baseFolderPath, LogFileProcessor processor, InputValidator inputValidator)
{
    try
    {
        Console.WriteLine("Generation in process...");
        var webServersFolders = Directory.GetDirectories(baseFolderPath);
        var allAverageStats = new Dictionary<string, IEnumerable<dynamic>>();
        foreach (var webServerFolder in webServersFolders)
        {
            var folder = Path.Combine(baseFolderPath, webServerFolder);
            var averageStats = processor.GetAverageStatsFromFolder(folder);
            allAverageStats[webServerFolder] = averageStats;
        }

        bool print = inputValidator.GetYesNoInput("Do you want to print?");
        if (print)
        {
            foreach (var averageStats in allAverageStats)
            {
                Console.WriteLine(averageStats.Key);

                foreach (var averageStat in averageStats.Value)
                {
                    Console.WriteLine($"Uri: {averageStat.Uri}\n" +
                        $"Count: {averageStat.Count}\n" +
                        $"AverageTime: {averageStat.AverageTime}\n" +
                        $"MinTime: {averageStat.MinTime}\n" +
                        $"MaxTime: {averageStat.MaxTime}");
                }
            }
        }

        bool continueProcessing = inputValidator.GetYesNoInput("Do you want to save?");
        if (!continueProcessing)
        {
            return;
        }

        Console.WriteLine("Input file name");
        var fileName = inputValidator.GetValidInput();
        processor.SaveToJsonFile(baseFolderPath, fileName, allAverageStats);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}