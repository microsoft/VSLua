using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Errors
{
    internal static class TaskExtensions
    {
        internal static Task WithoutCancellation(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        tcs.SetException(t.Exception);
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
