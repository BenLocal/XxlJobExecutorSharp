﻿namespace XxlJobExecutorSharp
{
    public static class XxlJobConstant
    {
        /// <summary>
        /// token标识key
        /// </summary>
        public const string Token = "XXL-JOB-ACCESS-TOKEN";

        public const int HTTP_SUCCESS_CODE = 200;

        public const int HTTP_FAIL_CODE = 500;

        /// <summary>
        /// SERIAL_EXECUTION=单机串行
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_SERIAL_EXECUTION = "SERIAL_EXECUTION";

        /// <summary>
        /// DISCARD_LATER=丢弃后续调度
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_DISCARD_LATER = "DISCARD_LATER";

        /// <summary>
        /// COVER_EARLY=覆盖之前调度 
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_COVER_EARLY = "COVER_EARLY";

        public const string LOGGER_SCOPE_JOBID_KEY = "id";

        public const string LOGGER_SCOPE_JOBLOGID_KEY = "logId";

        public const string LOGGER_SCOPE_JOBAREA_KEY = "area";

        public const string LOGGER_SCOPE_JOBAREA_VALUE = "job_handler";
    }
}
