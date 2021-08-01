using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GoogleAnalyticsServiceForDotNet
{
    /// <summary>
    /// This is the main interface for <see cref="GoogleAnalyticsService"/>
    /// </summary>
    public interface IGoogleAnalyticsService
    {
        /// <summary>
        /// Get a Google Analytics v4 Reporting Report with the provided input
        /// </summary>
        /// <param name="credentialFilePath">This is the path to your service account .json file</param>
        /// <param name="viewId">This is the view id from analytics.google.com</param>
        /// <param name="startDate">The start date for the report</param>
        /// <param name="endDate">The end date for the report</param>
        /// <param name="metrics">The metrics to use when generating the report</param>
        /// <param name="dimensions">The dimensions to use when generating the report</param>
        /// <returns><see cref="DataTable"/> with the unique dimensions combinations for the rows</returns>
        DataTable GetReport(string credentialFilePath, string viewId, DateTime startDate, DateTime endDate, IList<Metric> metrics, IList<Dimension> dimensions);
    }

    /// <summary>
    /// This is the main class for Google Analytics Service For Dot Net. Use <see cref="GetReport(string, string, DateTime, DateTime, IList{Metric}, IList{Dimension})"/> to get a report with the provided input.
    /// </summary>
    public class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        /// <summary>
        /// The application name. Default is null.
        /// </summary>
        private readonly string applicationName;

        /// <summary>
        /// This constructor accepts an applicationName, which by default is null.
        /// </summary>
        /// <param name="applicationName">The Client Service Application Name used in the Google Analytics Initializer call.</param>
        public GoogleAnalyticsService(string applicationName = null)
        {
            this.applicationName = applicationName;
        }

        /// <summary>
        /// Get a Google Analytics v4 Reporting Report with the provided input
        /// </summary>
        /// <param name="credentialFilePath">This is the path to your service account .json file</param>
        /// <param name="viewId">This is the view id from analytics.google.com</param>
        /// <param name="startDate">The start date for the report</param>
        /// <param name="endDate">The end date for the report</param>
        /// <param name="metrics">The metrics to use when generating the report</param>
        /// <param name="dimensions">The dimensions to use when generating the report</param>
        /// <returns><see cref="DataTable"/> with the unique dimensions combinations for the rows</returns>
        public DataTable GetReport(string credentialFilePath, string viewId, DateTime startDate, DateTime endDate, IList<Metric> metrics, IList<Dimension> dimensions)
        {
            var credential = GoogleCredential.FromFile(credentialFilePath).CreateScoped(new [] 
            {
                AnalyticsReportingService.Scope.AnalyticsReadonly
            });

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            };

            using (var reportingService = new AnalyticsReportingService(initializer))
            {
                var reportRequest = new ReportRequest
                {
                    DateRanges = GenerateDateRange(startDate, endDate),
                    Dimensions = dimensions,
                    Metrics = metrics,
                    ViewId = viewId,
                };

                var getReportsRequest = new GetReportsRequest
                {
                    ReportRequests = new List<ReportRequest> 
                    { 
                        reportRequest 
                    }
                };

                var batchRequest = reportingService.Reports.BatchGet(getReportsRequest);
                var response = batchRequest.Execute();

                var result = GenerateDataTable(response.Reports.First());

                return result;
            }

            DataTable GenerateDataTable(Report report)
            {
                var result = new DataTable();

                var columns = new List<DataColumn>();

                var dimensionColumns = report.ColumnHeader.Dimensions;
                columns.AddRange(dimensionColumns.Select(c => new DataColumn(c)));

                var metricColumns = report.ColumnHeader.MetricHeader.MetricHeaderEntries;
                columns.AddRange(metricColumns.Select(m => new DataColumn(m.Name)));

                result.Columns.AddRange(columns.ToArray());

                var rows = report.Data.Rows;
                foreach (var row in rows)
                {
                    var r = result.NewRow();
                    var dimensions = row.Dimensions;
                    var metrics = row.Metrics.First().Values;
                    var data = new List<string>();
                    data.AddRange(dimensions);
                    data.AddRange(metrics);
                    for (var i = 0; i < data.Count; i++)
                    {
                        r[i] = data[i];
                    }

                    result.Rows.Add(r);
                }

                return result;
            }

            IList<DateRange> GenerateDateRange(DateTime startDate, DateTime endDate)
            {
                return new List<DateRange>
                {
                    new DateRange
                    {
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd")
                    }
                };
            }
        }
    }
}
