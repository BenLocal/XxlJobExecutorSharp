using Newtonsoft.Json;

namespace XxlJobExecutorSharp.Entity
{
    public class ReturnT
    {
        public ReturnT() { }

        public ReturnT(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("content")]
        public object Content { get; set; }

        public static ReturnT Failed(string msg)
        {
            return new ReturnT(XxlJobConstant.HTTP_FAIL_CODE, msg);
        }
        public static ReturnT Success(string msg = "success")
        {
            return new ReturnT(XxlJobConstant.HTTP_SUCCESS_CODE, msg);
        }
    }
}
