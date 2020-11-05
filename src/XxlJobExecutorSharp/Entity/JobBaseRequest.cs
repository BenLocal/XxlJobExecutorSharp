using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace XxlJobExecutorSharp.Entity
{
    public class JobBaseRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [JsonProperty("jobId")]
        public int JobId { get; set; }
    }
}
