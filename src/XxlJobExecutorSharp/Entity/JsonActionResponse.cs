using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp.Entity
{
    public class JsonActionResponse : IActionResponse
    {
        public JsonActionResponse(HttpContext context, object value)
        {
            this.HttpContext = context;
            this.Result = value;
        }

        public object Result { get; set; }

        public HttpContext HttpContext { get; set; }

        public string ExecuteResult(HttpContext context)
        {
            if (this.Result == null)
            {
                return null;
            }

            return this.Result.SerializeObject();
        }
    }
}
