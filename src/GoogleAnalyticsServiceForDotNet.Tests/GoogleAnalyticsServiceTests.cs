using Google.Apis.AnalyticsReporting.v4.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;

namespace GoogleAnalyticsServiceForDotNet.Tests
{
    [TestClass]
    public class GoogleAnalyticsServiceTests
    {
        [TestMethod]
        public void GoogleAnalyticsServiceTests_Smoketest()
        {
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

            var dataTable = new DataTable();
            dataTable.Columns.Add("ga:date");
            dataTable.Columns.Add("Sessions");

            dataTable.Rows.Add("20210101", "1");
            dataTable.Rows.Add("20210102", "2");
            dataTable.Rows.Add("20210103", "3");

            var mockService = new Mock<IGoogleAnalyticsService>();

            mockService.Setup(mock => mock.GetReport(serviceAccountFilePath, viewId, startDate, endDate, metrics, dimensions)).Returns(dataTable);

            // this will return the DataTable with the columns being unique dimension combinations
            var result = mockService.Object.GetReport(serviceAccountFilePath, viewId, startDate, endDate, metrics, dimensions);

            Assert.AreEqual(result.Columns.Count, 2);

            Assert.AreEqual(result.Columns[0].ColumnName, "ga:date");
            Assert.AreEqual(result.Columns[1].ColumnName, "Sessions");

            Assert.AreEqual(string.Join(' ', result.Rows[0].ItemArray), "20210101 1");
            Assert.AreEqual(string.Join(' ', result.Rows[1].ItemArray), "20210102 2");
            Assert.AreEqual(string.Join(' ', result.Rows[2].ItemArray), "20210103 3");

            Assert.AreEqual(result.Rows.Count, 3);
        }
    }
}
