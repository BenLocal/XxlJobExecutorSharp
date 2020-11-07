using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp
{
    public class XxlJobMiddleware
    {
        private readonly ILogger<XxlJobMiddleware> _logger;
        private readonly XxlJobOptions _options;
        private readonly XxlJobRouteTable _routeTable;
        private readonly RequestDelegate _next;

        public XxlJobMiddleware(ILogger<XxlJobMiddleware> logger,
            XxlJobOptions options,
            XxlJobRouteTable routeTable,
            RequestDelegate next)
        {
            _logger = logger;
            _options = options;
            _routeTable = routeTable;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsXxlJobRequest(context.Request, out PathString subPath))
            {
                // This is a request in the Dashboard path
                await HandleRequest(context, subPath);
                return;
            }

            await _next(context);
        }

        private async Task HandleRequest(HttpContext context, PathString subPath)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            // token 检查
            if (!await ValidToken(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(ReturnT.Failed("401 Unauthorized").SerializeObject());
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            var subUrl = subPath.Value.EnsureStartSlash();
            var route = _routeTable.FindByPattern(subUrl);
            if (route != null)
            {
                var controller = context.RequestServices.GetRequiredService(route.TypeController) as IJobController;
                if (controller != null)
                {
                    try
                    {
                        var response = await controller.ActionAsync(context);

                        if (response != null)
                        {
                            await context.Response.WriteAsync(response.ExecuteResult(context), Encoding.UTF8);
                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e, "请求异常");
                        await context.Response.WriteAsync(ReturnT.Failed("请求异常").SerializeObject());
                        return;
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync(ReturnT.Failed("404 NotFound").SerializeObject());
        }

        private async Task<bool> ValidToken(HttpContext context)
        {
            context.Request.Headers.TryGetValue(XxlJobConstant.Token, out StringValues token);
            if (!string.IsNullOrEmpty(_options.Token) && token != _options.Token)
            {
                await context.Response.WriteAsync(ReturnT.Failed("token验证失败").SerializeObject());
                _logger.LogError($"xxljob token验证失败:{context.Request.Path.Value}");
                return false;
            }

            return true;
        }

        private bool IsXxlJobRequest(HttpRequest request, out PathString subPath)
        {
            if (request.Path.StartsWithSegments("/api/xxljob", StringComparison.OrdinalIgnoreCase, out subPath))
            {
                return true;
            }

            return false;
        }
    }
}
