### Examples

**Table of Contents**
- [Description](#description)
- [Google Analytics Service](#google-analytics-service)

#### Description

To see it in action, check out the [Tests Project](https://github.com/joemoceri/google-analytics-service-dotnet/tree/main/src/GoogleAnalyticsServiceForDotNet.Tests) and [Sample Project](https://github.com/joemoceri/google-analytics-service-dotnet/tree/main/src/GoogleAnalyticsServiceForDotNet.Sample).

#### Google Analytics Service

You can find the example below in the Sample project.

```csharp
public void Run()
{
    // Give it a name, could also be null
    var applicationName = "Google Analytics App";
    // this is the main service class to use
    var service = new GoogleAnalyticsService(applicationName);

    // set your start and end dates here
    var startDate = DateTime.Now.AddYears(-1);
    var endDate = DateTime.Now;

    // add your dimensions and metrics here. The data table will have rows for every unique dimension combination. The columns will be the dimensions, then metrics, and the rows the same.
    var dimensions = new List<Dimension>();
    var dimension = new Dimension();
    dimension.Name = "ga:date";
    dimensions.Add(dimension);
    var metrics = new List<Metric>();
    var metric = new Metric();
    metric.Expression = "ga:sessions";
    metric.Alias = "Sessions";
    metrics.Add(metric);

    // the view id found inside google analytics (analytics.google.com)
    var viewId = "yourViewId";

    // path to your service account json file
    var serviceAccountFilePath = "******.json"; 

    try
    {
        // this will return the DataTable with the columns being unique dimension combinations
        var result = service.GetReport(serviceAccountFilePath, viewId, startDate, endDate, metrics, dimensions);

        // The following will output to the console. You can take this output and build structure off of it to suite your needs.
        foreach (DataColumn column in result.Columns)
        {
            Console.Write($"{column.ColumnName} ");
        }

        Console.WriteLine();

        foreach (DataRow row in result.Rows)
        {
            var r = string.Empty;
            foreach (DataColumn column in result.Columns)
            {
                r += $"{row[column]} ";
            }

            Console.WriteLine(r);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    Console.Read();
}
```