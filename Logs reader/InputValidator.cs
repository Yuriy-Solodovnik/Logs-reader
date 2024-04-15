namespace Logs_reader
{
    public class InputValidator
    {
        public bool GetYesNoInput(string question)
        {
            while (true)
            {
                Console.WriteLine(question + " (yes/no): ");
                string? input = Console.ReadLine()?.Trim().ToLower();

                if (input == "yes" || input == "y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string GetValidInput()
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (ValidateInput(input))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Error: Try again.");
                }
            }
        }
        public string GetValidFileInput()
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (IsValidFilePath(input))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Error: Try again.");
                }
            }
        }


        private bool ValidateInput(string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        private bool IsValidFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path cannot be empty.");
                return false;
            }

            if (!Path.IsPathRooted(filePath))
            {
                Console.WriteLine("File path must be a full path including the drive letter.");
                return false;
            }

            try
            {
                string fullPath = Path.GetFullPath(filePath);
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine("File does not exist: " + fullPath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid file path. Error: " + ex.Message);
                return false;
            }

            return true;
        }
    }
}
