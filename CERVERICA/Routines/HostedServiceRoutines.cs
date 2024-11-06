using System;
using System.Threading;
using System.Threading.Tasks;
using CERVERICA.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CERVERICA.Routines
{
    /*
    public class HostedServiceRoutines : IHostedService, IDisposable
    {
        
        private readonly ILogger<HostedServiceRoutines> _logger;
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public HostedServiceRoutines(ILogger<HostedServiceRoutines> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted Service running.");

            // Ejecutar el método una vez al día
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<RoutineController>();
                await controller.AsignarLotesInsumoCaducados();
            }

            _logger.LogInformation("Hosted Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        
}
    */

}