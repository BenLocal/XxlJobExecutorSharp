using System;
using Microsoft.Extensions.DependencyInjection;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp
{
    public class XxlJobOptions
    {
        /// <summary>
        /// 最小30秒
        /// </summary>
        public int HeartbeatIntervalSecond { get; set; }

        /// <summary>
        /// 访问token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 执行器名称 对应xxljob执行器配置的appName
        /// </summary>
        public string ExecutorAppName { get; set; }

        /// <summary>
        /// xxljob调度中心根地址 如：http://localhost:8080/xxl-job-admin/
        /// </summary>
        public string AdminUrl { get; set; }

        /// <summary>
        /// 当前执行器节点地址 如：http://localhost:55860/api/xxljob/
        /// </summary>
        public string ExecutorUrl { get; set; }

        /// <summary>
        /// 最少3次
        /// </summary>
        public int CallBackRetryCount { get; set; }

        public Action<FailedInfo> FailedCallback { get; set; }

        public ServiceLifetime HandlerServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
