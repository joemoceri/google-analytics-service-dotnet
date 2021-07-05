using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalyticsServiceForDotNet
{
    /// <summary>
    /// This is the main class for Google Analytics Service For Dot Net
    /// </summary>
    public class GoogleAnalyticsService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly string applicationName;

        /// <summary>
        /// 
        /// </summary>
        private readonly string fileDataStoreFolder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="fileDataStoreFolder"></param>
        public GoogleAnalyticsService(string applicationName, string fileDataStoreFolder)
        {
            this.applicationName = applicationName;
            this.fileDataStoreFolder = fileDataStoreFolder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginEmail"></param>
        /// <param name="clientSecretPath"></param>
        /// <param name="viewId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="metrics"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public async Task<DataTable> GetReport
        (
            string loginEmail, string clientSecretPath, string viewId,
            DateTime startDate, DateTime endDate,
            IList<Metric> metrics, IList<Dimension> dimensions
        )
        {
            var credential = await GetCredential(loginEmail, clientSecretPath);

            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            };

            using (var svc = new AnalyticsReportingService(initializer))
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
                    ReportRequests = new List<ReportRequest> { reportRequest }
                };

                var batchRequest = svc.Reports.BatchGet(getReportsRequest);
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

            async Task<UserCredential> GetCredential(string loginEmail, string clientSecretPath)
            {
                using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
                {
                    return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { AnalyticsReportingService.Scope.Analytics },
                        loginEmail,
                        CancellationToken.None,
                        new FileDataStore(fileDataStoreFolder));
                }
            }
        }
    }
}
