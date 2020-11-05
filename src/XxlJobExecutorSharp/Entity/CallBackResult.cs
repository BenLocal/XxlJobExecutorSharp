using System;
using System.Collections.Generic;
using System.Text;

namespace XxlJobExecutorSharp.Entity
{
    public class CallBackResult
    {
        public DateTime? ExpiresAt { get; set; }

        public int Retries { get; set; }
    }
}
