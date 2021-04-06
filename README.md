# google-analytics-service

> This is a C# .NET service wrapper for Google Analytics Reporting v4 that accepts Metrics and Dimensions and returns a DataTable with the result.

* [Overview](#overview)
* [Install](#install)
* [Usage](#usage)

<a name="overview"></a>
## Overview
This is a C# .NET service wrapper for Google Analytics Reporting v4 that accepts Metrics and Dimensions and returns a DataTable with the result. Please see usage for how to use.

<a name="install"></a>
## Install
Using NuGet
```sh
Install-Package Io.JoeMoceri.GoogleAnalyticsService
```

<a name="usage"></a>
## Usage
```csharp
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace GoogleAnalyticsExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var applicationName = "Your application name";
            var fileDataStoreFolder = "Your folder";
            var service = new GoogleAnalyticsService(applicationName, fileDataStoreFolder);
            
            var loginEmail = "example@example.com";
            var startDate = DateTime.Now.AddYears(-1);
            var endDate = DateTime.Now;

            var dimensions = new List<Dimension>();
            var dimension = new Dimension();
            dimension.Name = "ga:date";
            dimensions.Add(dimension);
            var metrics = new List<Metric>();
            var metric = new Metric();
            metric.Expression = "ga:sessions";
            metric.Alias = "Sessions";
            metrics.Add(metric);

            var viewId = "{yourViewId}";

            var clientSecretPath = "client_secret.json"; // path to your client_secret json file

            try
            {
                var result = service.GetReport(loginEmail, clientSecretPath, viewId, startDate, endDate, metrics, dimensions).Result;
                foreach(DataColumn column in result.Columns)
                {
                    Console.Write($"{column.ColumnName} ");
                }

                foreach (DataRow row in result.Rows)
                {
                    foreach (DataColumn column in result.Columns)
                    {
                        Console.Write($"{row[column]} ");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }
    }
}

```
