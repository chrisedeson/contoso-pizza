using System.Globalization;
using System.Text;
using Newtonsoft.Json;

CultureInfo culture = new CultureInfo("en-US");

string currentDirectory = Directory.GetCurrentDirectory();
string storesDirectory = Path.Combine(currentDirectory, "stores");

string salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

IEnumerable<string> salesFiles = FindFiles(storesDirectory);

double salesTotal = 0;
foreach (string file in salesFiles)
{
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);
    salesTotal += data?.Total ?? 0;
}

File.WriteAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

WriteSalesSummaryReport(salesFiles, salesTotalDir, culture);

Console.WriteLine($"Wrote totals.txt and salesSummary.txt to {salesTotalDir}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    foreach (string file in Directory.GetFiles(folderName, "*", SearchOption.AllDirectories))
    {
        FileInfo fi = new FileInfo(file);
        if (fi.Extension.Equals(".json"))
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

// Additional function required by Part 2 of the assignment.
// Writes a Sales Summary report file containing the total sales across all
// store files plus a per-file currency breakdown.
static void WriteSalesSummaryReport(IEnumerable<string> salesFiles, string outputDir, CultureInfo culture)
{
    var report = new StringBuilder();
    double total = 0;
    var perFile = new List<(string FileName, double Amount)>();

    foreach (string file in salesFiles)
    {
        string json = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData>(json);
        double amount = data?.Total ?? 0;
        total += amount;

        string storeId = Directory.GetParent(file)?.Name ?? "unknown";
        perFile.Add(($"{storeId}/{Path.GetFileName(file)}", amount));
    }

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {total.ToString("C", culture)}");
    report.AppendLine();
    report.AppendLine(" Details:");
    foreach (var (fileName, amount) in perFile)
    {
        report.AppendLine($"  {fileName}: {amount.ToString("C", culture)}");
    }

    File.WriteAllText(Path.Combine(outputDir, "salesSummary.txt"), report.ToString());
}

record SalesData(double Total);
