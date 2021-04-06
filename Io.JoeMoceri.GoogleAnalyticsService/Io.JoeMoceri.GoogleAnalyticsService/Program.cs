using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace Io.JoeMoceri.GoogleAnalyticsService
{
    class Program
    {
        static void Main(string[] args)
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
                foreach (DataColumn column in result.Columns)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }
    }
}
