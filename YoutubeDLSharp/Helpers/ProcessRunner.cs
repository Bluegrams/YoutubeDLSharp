using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp.Options;

namespace YoutubeDLSharp.Helpers
{
    /// <summary>
    /// Provides methods for throttled execution of processes.
    /// </summary>
    public class ProcessRunner
    {
        private const int MaxCount = 100;
        private readonly SemaphoreSlim _semaphore;

        public byte TotalCount { get; private set; }

        public ProcessRunner(byte initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, MaxCount);
            TotalCount = initialCount;
        }

        public async Task<(int, string[])> RunThrottled(YoutubeDlProcess process, string[] urls, OptionSet options,
                                       CancellationToken ct, IProgress<DownloadProgress> progress = null)
        {
            var errors = new List<string>();
            process.ErrorReceived += (_, e) => errors.Add(e.Data);
            await _semaphore.WaitAsync(ct);
            try
            {
                var exitCode = await process.RunAsync(urls, options, ct, progress);
                return (exitCode, errors.ToArray());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void IncrementCount(byte incr)
        {
            _semaphore.Release(incr);
            TotalCount += incr;
        }

        private async Task DecrementCount(byte decr)
        {
            var decrs = new Task[decr];
            for (var i = 0; i < decr; i++)
                decrs[i] = _semaphore.WaitAsync();
            TotalCount -= decr;
            await Task.WhenAll(decrs);
        }

        public async Task SetTotalCount(byte count)
        {
            if (count is < 1 or > MaxCount)
                throw new ArgumentException($"Number of threads must be between 1 and {MaxCount}.");
            if (count > TotalCount)
                IncrementCount((byte)(count - TotalCount));
            else if (count < TotalCount)
                await DecrementCount((byte)(TotalCount - count));
        }
    }
}
