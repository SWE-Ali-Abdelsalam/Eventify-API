﻿using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Eventify.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500) // Log if request takes longer than 500ms
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds)",
                    requestName, elapsedMilliseconds);
            }

            return response;
        }
    }
}
