using System;
using System.IO;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.Service.BitcoinGold.API.Middleware
{
    public class CustomGlobalErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly string _componentName;
        private readonly RequestDelegate _next;

        public CustomGlobalErrorHandlerMiddleware(RequestDelegate next, ILog log, string componentName)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _componentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);


            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }

            //await _log.WriteInfoAsync("OkRequests", context.Request.GetUri().AbsoluteUri, ReadBody(context));
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            await _log.WriteWarningAsync(_componentName, context.Request.GetUri().AbsoluteUri, ReadBody(context),
                ex);
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            

            ctx.Response.StatusCode = IsValidationError(ex) ? 400 : 500;

            var response = ErrorResponse.Create(ex.ToString());

            var responseJson = JsonConvert.SerializeObject(response);

            await ctx.Response.WriteAsync(responseJson);
        }

        private bool IsValidationError(Exception ex)
        {
            return ex is BusinessException businessException && businessException.Code == ErrorCode.BadInputParameter;
        }

        private string ReadBody(HttpContext context)
        {
            var body = string.Empty;

            if (context.Request.Body.CanSeek &&
                +context.Request.Body.Length > 0)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                body = new StreamReader(context.Request.Body).ReadToEnd();
            }

            return body;
        }
    }
}
