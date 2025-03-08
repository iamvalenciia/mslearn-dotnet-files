using Newtonsoft.Json; 
using System.Text;


var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummaryReport(salesFiles, salesTotal, salesTotalDir);


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, double salesTotal, string outputDirectory)
{
    // Create a StringBuilder to build our report
    StringBuilder reportBuilder = new StringBuilder();
    
    // Add the header
    reportBuilder.AppendLine("Sales Summary");
    reportBuilder.AppendLine("----------------------------");
    reportBuilder.AppendLine($" Total Sales: {salesTotal:C}");
    reportBuilder.AppendLine();
    reportBuilder.AppendLine(" Details:");
    
    // Add details for each file
    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileTotal = data?.Total ?? 0;
        
        // Get just the filename instead of the full path
        string filename = Path.GetFileName(Path.GetDirectoryName(file)) + "/" + Path.GetFileName(file);
        
        reportBuilder.AppendLine($"  {filename}: {fileTotal:C}");
    }
    
    // Write the report to a file
    string reportPath = Path.Combine(outputDirectory, "salesSummaryReport.txt");
    File.WriteAllText(reportPath, reportBuilder.ToString());
    
    Console.WriteLine($"Sales summary report generated: {reportPath}");
}

record SalesData (double Total);
