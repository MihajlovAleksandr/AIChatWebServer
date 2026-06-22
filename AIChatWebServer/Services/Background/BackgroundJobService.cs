using AIChatWebServer.Models.Background;
using System.Collections.Concurrent;

namespace AIChatWebServer.Services.Background
{
    public class BackgroundJobService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundJobService> logger) : BackgroundService, IBackgroundJobService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<BackgroundJobService> _logger = logger;
        private readonly ConcurrentQueue<BackgroundJobInfo> _jobQueue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void FireAndForget(
            Func<IServiceProvider, CancellationToken, Task> action,
            string jobType,
            Action<Exception> onError = null)
        {
            var job = new BackgroundJobInfo
            {
                JobId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                JobType = jobType,
                Action = async (sp, ct) =>
                {
                    try
                    {
                        await action(sp, ct);
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke(ex);
                        throw;
                    }
                }
            };

            _jobQueue.Enqueue(job);
            _signal.Release();

            _logger.LogDebug("Job {JobType} ({JobId}) queued at {Time}",
                jobType, job.JobId, DateTime.UtcNow);
        }

        public void FireAndForgetDelayed(
            Func<IServiceProvider, CancellationToken, Task> action,
            string jobType,
            TimeSpan delay,
            Action<Exception> onError = null)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(delay);
                FireAndForget(action, $"{jobType}_Delayed", onError);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundJobService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(stoppingToken);

                    if (_jobQueue.TryDequeue(out var job))
                    {
                        using var scope = _serviceProvider.CreateScope();

                        _logger.LogInformation("Executing job {JobType} ({JobId})",
                            job.JobType, job.JobId);

                        try
                        {
                            await job.Action(scope.ServiceProvider, stoppingToken);
                            _logger.LogInformation("Job {JobType} ({JobId}) completed successfully",
                                job.JobType, job.JobId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Job {JobType} ({JobId}) failed after {Duration}s",
                                job.JobType, job.JobId, (DateTime.UtcNow - job.CreatedAt).TotalSeconds);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in BackgroundJobService main loop");
                }
            }

            _logger.LogInformation("BackgroundJobService stopped");
        }
    }
}