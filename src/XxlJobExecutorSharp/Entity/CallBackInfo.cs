using System;
using System.Collections.Generic;
using System.Text;

namespace XxlJobExecutorSharp.Entity
{
    public class CallBackInfo
    {
        /// <summary>
        /// 本次调度日志ID
        /// </summary>
        public int LogId { get; set; }

        /// <summary>
        /// 200 表示任务执行正常，500表示失败
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }
    }
}
