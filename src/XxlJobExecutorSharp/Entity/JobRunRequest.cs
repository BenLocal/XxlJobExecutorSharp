using Newtonsoft.Json;

namespace XxlJobExecutorSharp.Entity
{
    public class JobRunRequest : JobBaseRequest
    {
        /// <summary>
        /// 任务标识 JobHandler
        /// </summary>
        [JsonProperty("executorHandler")]
        public string ExecutorHandler { get; set; }

        /// <summary>
        /// 任务参数
        /// </summary>
        [JsonProperty("executorParams")]
        public string ExecutorParams { get; set; }

        /// <summary>
        /// 任务阻塞策略 SERIAL_EXECUTION=单机串行  DISCARD_LATER=丢弃后续调度  COVER_EARLY=覆盖之前调度 
        /// </summary>
        [JsonProperty("executorBlockStrategy")]
        public string ExecutorBlockStrategy { get; set; }

        /// <summary>
        /// 任务超时时间，单位秒，大于零时生效
        /// </summary>
        [JsonProperty("executorTimeout")]
        public int ExecutorTimeout { get; set; }

        /// <summary>
        /// 本次调度日志ID
        /// </summary>
        [JsonProperty("logId")]
        public int LogId { get; set; }

        /// <summary>
        /// 本次调度日志时间
        /// </summary>
        [JsonProperty("logDateTime")]
        public long LogDateTime { get; set; }

        /// <summary>
        /// 任务模式
        /// </summary>
        [JsonProperty("glueType")]
        public string GlueType { get; set; }

        /// <summary>
        /// GLUE脚本代码
        /// </summary>
        [JsonProperty("glueSource")]
        public string GlueSource { get; set; }

        /// <summary>
        /// GLUE脚本更新时间，用于判定脚本是否变更以及是否需要刷新
        /// </summary>
        [JsonProperty("glueUpdatetime")]
        public long GlueUpdatetime { get; set; }

        /// <summary>
        /// 分片参数：当前分片
        /// </summary>
        [JsonProperty("broadcastIndex")]
        public int BroadcastIndex { get; set; }

        /// <summary>
        /// 分片参数：总分片
        /// </summary>
        [JsonProperty("broadcastTotal")]
        public int BroadcastTotal { get; set; }
    }
}
