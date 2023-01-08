namespace EcoflowMqtt.Service.Extensions;

public static class ThreadingExtensions
{
    public static async Task WaitAsync(this CancellationToken cancellation)
    {
        var waitForStop = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        cancellation.Register(obj =>
        {
            var tcs = (TaskCompletionSource)obj!;
            tcs.TrySetResult();
        }, waitForStop);
        await waitForStop.Task.ConfigureAwait(false);
    }
}