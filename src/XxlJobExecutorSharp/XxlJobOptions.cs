using System;
using System.Collections.Generic;
using System.Text;

namespace XxlJobExecutorSharp
{
    public class XxlJobOptions
    {
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
        public string XxlJobAdminUrl { get; set; }

        /// <summary>
        /// 当前执行器节点地址 如：http://localhost:55860/api/xxljob/
        /// </summary>
        public string ExecutorUrl { get; set; }

        /// <summary>
        /// 线程执行器线程数 默认为Environment.ProcessorCount * 2
        /// </summary>
        public int TaskExecutorThreadCount { get; set; }

        /// <summary>
        /// httpJobHandler的url根地址
        /// </summary>
        public string HttpJobhandlerUrl { get; set; }

        /// <summary>
        /// 最少3次
        /// </summary>
        public int CallBackRetryCount { get; set; }
    }
}
