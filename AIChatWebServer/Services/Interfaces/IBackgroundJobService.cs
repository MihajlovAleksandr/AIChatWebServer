namespace AIChatWebServer.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        void FireAndForget(
            Func<IServiceProvider, CancellationToken, Task> action,
            string jobType,
            Action<Exception> onError = null);

        void FireAndForgetDelayed(
            Func<IServiceProvider, CancellationToken, Task> action,
            string jobType,
            TimeSpan delay,
            Action<Exception> onError = null);
    }
}