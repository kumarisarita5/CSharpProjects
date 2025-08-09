using Azure.Core;
using Azure.Identity;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text.Json;
using System.Threading;

namespace SailfishCallPOC
{

    public class AzureArmService
    {
        private readonly HttpClient _httpClient;
        private readonly AzureCliCredential _credential;
        private readonly string _subscriptionId;
        private const string AzureArmBaseUrl = "https://management.azure.com";
        private const string AzureScope = "https://management.azure.com/.default";
        private const string apiVersion = "2021-04-01";

        public AzureArmService(string subscriptionId)
        {
            _subscriptionId = subscriptionId;
            _httpClient = new HttpClient();
            _credential = new AzureCliCredential(); // Uses 'az login' credentials
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
            var token = await _credential.GetTokenAsync(tokenRequestContext);
            return token.Token;
        }

        public async Task<JsonDocument> GetVirtualMachinesAsync(string resourceGroupName)
        {
            var requestUri = $"{AzureArmBaseUrl}/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/virtualMachines?api-version=2023-03-01";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonContent);
        }

        public async Task<JsonDocument> GetStorageAccountsAsync()
        {
            var requestUri = $"{AzureArmBaseUrl}/subscriptions/{_subscriptionId}/providers/Microsoft.Storage/storageAccounts?api-version=2023-01-01";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonContent);
        }

        private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(HttpMethod method, string requestUri)
        {
            var accessToken = await GetAccessTokenAsync();
            var request = new HttpRequestMessage(method, requestUri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("Accept", "application/json");

            return request;
        }

        public async Task<JsonDocument> GetResourceGroupsAsync()
        {
            var requestUri = $"{AzureArmBaseUrl}/subscriptions/{_subscriptionId}/resourcegroups?api-version=2021-04-01";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonContent);
        }

        public async Task<HttpResponseMessage> GetResourceFromSailfish()
        {
            var requestUri = $"{AzureArmBaseUrl}/providers/Microsoft.ResourceGraph/resources?api-version={apiVersion}";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetResourceFromSailfish_Extended()
        {
            var requestUri = $"{AzureArmBaseUrl}/providers/Microsoft.ResourceGraph/resources?api-version={apiVersion}";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            return response;

        //    startTimeStamp = Stopwatch.GetTimestamp();

        //    var requestUri = $"/providers/Microsoft.ResourceGraph/resources?api-version={apiVersion}";
        //    monitor.Activity[SolutionConstants.RequestURI] = requestUri;

        //    string? skipToken = null;
        //    HttpResponseMessage? response = null;
        //    int iterationCount = 0;
        //    JArray? aggregatedData = null;
        //    do
        //    {
        //        requestBody.Options = requestBody.Options ?? new QueryRequestOptions(null, 0, 0, skipToken);
        //        response = await _restClient.CallRestApiAsync(
        //            endPointSelector: _clientOptions.EndPointSelector,
        //            requestUri: requestUri,
        //            httpMethod: HttpMethod.Post,
        //            accessToken: null, // accessToken will be inserted through DstsV2TokenGenerationHandler
        //            headers: null,
        //            jsonRequestContent: requestBody,
        //            clientRequestId: clientRequestId,
        //            skipUriPathLogging: false,
        //            cancellationToken: cancellationToken).ConfigureAwait(false);

        //        (skipToken, aggregatedData) = ParseSailfishResponse(
        //            responseContent: await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false),
        //            aggregatedData: aggregatedData,
        //            monitor: monitor);

        //        // Add the back off logic if quota is exceeded.
        //        await this.ApplySailfishQuotaBackoff(response, monitor, cancellationToken);
        //        iterationCount++;
        //    }
        //    while (!string.IsNullOrWhiteSpace(skipToken));

        //    monitor.Activity[SolutionConstants.SailfishIterationCount] = iterationCount;

        //    // Replace the aggregated data in the response if we have collected any.
        //    response = await this.UpdateAggregatedDataInSailfishResponseAsync(
        //        aggregatedData,
        //        response,
        //        cancellationToken);

        //    long endTimestamp = Stopwatch.GetTimestamp();
        //    var restClientDuration = (long)Stopwatch.GetElapsedTime(startTimeStamp, endTimestamp).TotalMilliseconds;

        //    TagList tagList = default;
        //    tagList.Add(SolutionConstants.HttpStatusCode, response.StatusCode.FastEnumToString());
        //    tagList.Add(SolutionConstants.ResourceType, resourceType);
        //    tagList.Add(MonitoringConstants.GetSuccessDimension(true));
        //    QFDGetSailfishResourcesMetricDuration.Record(restClientDuration, tagList);

        //    monitor.OnCompleted();
        //    return response;
        //}
        //    catch (Exception ex)
        //    {
        //        monitor.OnError(ex);

        //        if (startTimeStamp > 0)
        //        {
        //            long endTimestamp = Stopwatch.GetTimestamp();
        //var restClientDuration = (long)Stopwatch.GetElapsedTime(startTimeStamp, endTimestamp).TotalMilliseconds;

        //TagList tagList = default;
        //tagList.Add(SolutionConstants.HttpStatusCode, SolutionUtils.GetExceptionTypeSimpleName(ex));
        //            tagList.Add(SolutionConstants.ResourceType, resourceType);
        //            tagList.Add(MonitoringConstants.GetSuccessDimension(false));
        //            QFDGetSailfishResourcesMetricDuration.Record(restClientDuration, tagList);
        //        }

        //        throw;
        //    }
        //
        }

        public async Task<HttpResponseMessage> GetResourceFromSailfish(string query)
        {
            var requestUri = $"{AzureArmBaseUrl}/providers/Microsoft.ResourceGraph/resources?api-version={apiVersion}";

            var requestBody = new
            {
                query = query,
                subscriptions = new[] { _subscriptionId }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            
            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, requestUri);
            request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            return response;
        }

        public async Task<JsonDocument> GetResourceByIdAsync(string resourceId, string apiVersion)
        {
            var requestUri = $"{AzureArmBaseUrl}{resourceId}?api-version={apiVersion}";

            using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, requestUri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Azure ARM API call failed: {response.StatusCode} - {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonContent);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}