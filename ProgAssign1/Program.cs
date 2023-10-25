using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

class Program
{
    static int skippedRows = 0;
    static int validRows = 0;
    static string directoryPath = "Sample_Data";
    static string outputPath = "Output/merged_output.csv";
    static string logFilePath = "Logs/log.txt";

    static void Main(string[] args)
    {
        DateTime startTime = DateTime.Now;
        Log("-------------------------------------------------");
        Log("Program started");
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("Program started");

        var allRecords = new List<Record>();
        foreach (var file in Directory.GetFiles(directoryPath, "*.csv", SearchOption.AllDirectories))
        {
            Log("-------------------------------------------------");
            Log($"Reading CSV file: {file}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Reading CSV file: {file}");
            var recordsFromCurrentFile = ReadCSV(file);
            int initialCount = recordsFromCurrentFile.Count;

            recordsFromCurrentFile = recordsFromCurrentFile.Where(r => r.IsValid()).ToList();

            int currentValidRows = recordsFromCurrentFile.Count;
            int currentSkippedRows = initialCount - currentValidRows;

            validRows += currentValidRows;
            skippedRows += currentSkippedRows;


            Log($"Finished reading {file} -> Valid Rows: {currentValidRows}, Skipped Rows: {currentSkippedRows}");
            Console.WriteLine($"Finished reading {file} -> Valid Rows: {currentValidRows}, Skipped Rows: {currentSkippedRows}");
            
            // Add the current date to each record from the current file
            Log("Added Date Column to CSV File");
            Console.WriteLine("Added Date Column to CSV File");
            foreach (var record in recordsFromCurrentFile)
            {
                record.Date = DateTime.Now.ToString("yyyy-MM-dd");
            }

            allRecords.AddRange(recordsFromCurrentFile);
        }

        Log("-------------------------------------------------");
        Log("-------------------------------------------------");
        Log("Writing to merged CSV File");
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("Writing to merged CSV File");
        WriteCSV(outputPath, allRecords);
        Log($"Finished writing to {outputPath}");


        DateTime endTime = DateTime.Now;
        TimeSpan executionTime = endTime - startTime;

        Log("-------------------------------------------------");
        Log($"Total Valid Rows: {validRows}");
        Log($"Total Skipped Rows: {skippedRows}");
        Log($"Total Execution Time:  {executionTime.TotalSeconds} seconds");
        Log("-------------------------------------------------");

        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine($"Total Valid Rows: {validRows}");
        Console.WriteLine($"Total Skipped Rows: {skippedRows}");
        Console.WriteLine($"Total Execution Time: {executionTime.TotalSeconds} seconds");
        Console.WriteLine("Processing Finished. Check log.txt in Logs Folder for more details");
        Console.WriteLine("-------------------------------------------------");
    }


    static List<Record> ReadCSV(string path)
    {
        using var reader = new StreamReader(path);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<Record>().ToList();
        return records;
    }

    static void WriteCSV(string path, List<Record> records)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(records);
    }

    static void Log(string message)
    {
        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
        {
            logWriter.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}

public class Record
{
    [Name("First Name")]
    public string? FirstName { get; set; }

    [Name("Last Name")]
    public string? LastName { get; set; }

    [Name("Street Number")]
    public string? StreetNumber { get; set; }

    [Name("Street")]
    public string? Street { get; set; }

    [Name("City")]
    public string? City { get; set; }

    [Name("Province")]
    public string? Province { get; set; }

    [Name("Postal Code")]
    public string? PostalCode { get; set; }

    [Name("Country")]
    public string? Country { get; set; }

    [Name("Phone Number")]
    public string? PhoneNumber { get; set; }

    [Name("email Address")]
    public string? EmailAddress { get; set; }

    [Name("Date")]
    public string? Date { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(StreetNumber) &&
               !string.IsNullOrWhiteSpace(Street) &&
               !string.IsNullOrWhiteSpace(City) &&
               !string.IsNullOrWhiteSpace(Province) &&
               !string.IsNullOrWhiteSpace(Country) &&
               !string.IsNullOrWhiteSpace(PostalCode) &&
               !string.IsNullOrWhiteSpace(PhoneNumber) &&
               !string.IsNullOrWhiteSpace(EmailAddress);
    }
}