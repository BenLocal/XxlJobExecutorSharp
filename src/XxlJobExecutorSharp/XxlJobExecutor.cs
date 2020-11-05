using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestSharp;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp
{
    public class XxlJobExecutor : IXxlJobExecutor
    {
        private readonly XxlJobOptions _options;

        private readonly IRestClient _httpClient;

        private readonly ILogger _logger;

        public XxlJobExecutor(XxlJobOptions options,
            IRestClient httpClient,
            ILogger<XxlJobExecutor> logger)
        {
            _options = options;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> CallBackExecutor(List<CallBackInfo> callBackInfos)
        {
            var registryBody = callBackInfos.Select(x => new
            {
                logId = x.LogId,
                logDateTim = DateTime.Now.GetTimeStamp(),
                executeResult = new
                {
                    code = x.Code,
                    msg = x.Message,
                }
            });

            return await InnnerAdminHttpClient(CreateRestRequest("api/callback", registryBody), "执行器回调");
        }

        public async Task<bool> RegistryExecutor()
        {
            var registryBody = new
            {
                registryGroup = "EXECUTOR",//固定值
                registryKey = _options.ExecutorAppName, //执行器AppName
                registryValue = _options.ExecutorUrl //执行器地址
            };

            return await InnnerAdminHttpClient(CreateRestRequest("api/registry", registryBody),
                "注册执行器");
        }

        public async Task<bool> RemoveRegistryExecutor()
        {
            var registryBody = new
            {
                registryGroup = "EXECUTOR",
                registryKey = _options.ExecutorAppName,
                registryValue = _options.ExecutorUrl
            };
            return await InnnerAdminHttpClient(CreateRestRequest("api/registryRemove", registryBody),
                "移除执行器");
        }

        private RestRequest CreateRestRequest(string url, object registryBody)
        {
            var request = new RestRequest(url, Method.POST);
            if (!string.IsNullOrEmpty(_options.Token))
            {
                request.AddHeader(XxlJobConstant.Token, _options.Token);
            }
            request.AddJsonBody(registryBody);

            return request;
        }

        private async Task<bool> InnnerAdminHttpClient(RestRequest request, string logActionName)
        {
            var response = await _httpClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var result = response.Content.DeserializeObject<ReturnT>();
                if (result?.Code == XxlJobConstant.HTTP_SUCCESS_CODE)
                {
                    _logger.LogInformation($"xxljob{logActionName}成功");
                    return true;
                }

                _logger.LogError($"xxljob{logActionName}失败,{result?.Code},{result?.Msg}");
                return false;
            }

            _logger.LogError(response.ErrorException, $"xxljob{logActionName}Http请求失败,{response.StatusCode},{response.ErrorMessage}");
            return false;
        }
    }
}
