using System.Collections.Concurrent;

namespace ProcessExcelDemo.Services
{
    public class ProcessManagerService
    {
        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _runningProcesses = new();

        public bool TryAddProcess(Guid processId, CancellationTokenSource cts)
        {
            return _runningProcesses.TryAdd(processId, cts);
        }

        public bool TryCancelProcess(Guid processId)
        {
            if (_runningProcesses.TryGetValue(processId, out var cts))
            {
                cts.Cancel();
                return true;
            }
            return false;
        }

        public void RemoveProcess(Guid processId)
        {
            _runningProcesses.TryRemove(processId, out _);
        }

        public bool IsProcessing(Guid processId) => _runningProcesses.ContainsKey(processId);
    }

}
