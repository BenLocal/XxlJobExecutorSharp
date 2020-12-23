using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace XxlJobExecutorSharp.Entity
{
    public class JobLogRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [JsonProperty("logId")]
        public int JobLogId { get; set; }

        /// <summary>
        /// 日志开始行号，滚动加载日志   // 从1开始
        /// </summary>
        [JsonProperty("fromLineNum")]
        public int FromLineNum { get; set; }

        /// <summary>
        /// 本次调度日志时间
        /// </summary>
        [JsonProperty("logDateTim")]
        public long LogDateTime { get; set; }
    }
}
