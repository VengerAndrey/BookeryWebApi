using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WebApi.Services
{
    public class ExpiredTokenCleaner : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IJwtService _jwtService;

        public ExpiredTokenCleaner(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer((o) =>
            {
                _jwtService.ClearExpiredRefreshTokens(DateTime.UtcNow);
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
