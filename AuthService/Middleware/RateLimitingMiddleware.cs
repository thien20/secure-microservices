using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        private static readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
        private static readonly int _maxRequests = 3;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (IsRateLimited(ipAddress))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            await _next(context);
        }

        private bool IsRateLimited(string ipAddress)
        {
            var cacheKey = $"RateLimit_{ipAddress}";

            if (_memoryCache.TryGetValue(cacheKey, out int requestCount))
            {
                if (requestCount >= _maxRequests)
                {
                    return true;
                }

                _memoryCache.Set(cacheKey, ++requestCount, _timeWindow);
            }
            else
            {
                _memoryCache.Set(cacheKey, 1, _timeWindow);
            }

            return false;
        }
    }

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
