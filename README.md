# XxlJobExecutorSharp
 Apsnet core for xxlJob Executor

Startup.cs
```csharp
 public void ConfigureServices(IServiceCollection services)
 {
	 services.AddXxlJob(options =>
						{
							options.XxlJobAdminUrl = "http://localhost:8080/xxl-job-admin";
							options.Token = "token";
							options.HeartbeatIntervalSecond = 10;
							options.ExecutorAppName = "test";
							options.ExecutorUrl = "http://localhost:19187/api/xxljob/";
						});
 }

...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	app.UseXxlJob();
}
```

Create job handler
```csharp
[XxlJobHandler("first")]
public class FirstJobHandler : IXxlJobExecutorHandler
{
	private readonly ILogger<FirstJobHandler> _logger;

	public FirstJobHandler(ILogger<FirstJobHandler> logger)
	{
		_logger = logger;
	}

	public Task<ReturnT> Execute(JobExecuteContext context)
	{
		_logger.LogInformation($"Hello, {context.LogId}, {context.JobParameter}");

		return Task.FromResult(ReturnT.Success());
	}
}
```

