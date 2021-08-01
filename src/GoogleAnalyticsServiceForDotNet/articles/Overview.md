## Overview

**Table of Contents**
- [Google Analytics Service](#google-analytics-service)
- [Links](#links)

### Google Analytics Service

This class serves as a wrapper around the Google Analytics v4 Reporting library found [here](https://www.nuget.org/packages/Google.Apis.AnalyticsReporting.v4). It uses service account credentials for access. You must provide these credentials yourself. This class will then accept a view id, start date, end date, dimensions, metrics, and return a C# [DataTable](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable) with the columns being the unique dimension combinations and the rows reflecting that.

### Links

Please see [Examples](Examples.html) for how to use.