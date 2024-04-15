using Logs_reader;

var processor = new LogFileProcessor();
var inputValidator = new InputValidator();

Console.WriteLine("Write folder path");
var folderPath = inputValidator.GetValidInput();

Console.WriteLine("Write search term");
var searchTerm = inputValidator.GetValidInput();

if (string.IsNullOrEmpty(folderPath))
{
    Console.WriteLine("Invalid path");
    return;
}

try
{
    var lines = processor.FindLinesContainingTest(folderPath, searchTerm);

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }

    bool continueProcessing = inputValidator.GetYesNoInput("Do you want to continue?");
    if (continueProcessing)
    {
        Console.WriteLine("Write file path");
        var filePath = inputValidator.GetValidFileInput();
        processor.WriteResultsToFile(lines, filePath);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}