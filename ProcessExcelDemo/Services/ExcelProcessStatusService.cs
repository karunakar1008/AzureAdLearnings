using ProcessExcelDemo.Models;
using System.Collections.Concurrent;

namespace ProcessExcelDemo.Services
{
    public class ExcelProcessStatusService
    {
        private readonly ConcurrentDictionary<Guid, ExcelProcessStatus> _processes = new();

        public IEnumerable<ExcelProcessStatus> GetAllProcesses() => _processes.Values;

        public ExcelProcessStatus? GetProcess(Guid processId)
        {
            _processes.TryGetValue(processId, out var status);
            return status;
        }

        public void AddProcess(ExcelProcessStatus status)
        {
            _processes[status.ProcessId] = status;
        }

        public void UpdateProcessStatus(Guid processId, string status, int progress = 0, string? error = null, DateTime? endTime = null)
        {
            if (_processes.TryGetValue(processId, out var process))
            {
                process.Status = status;
                process.Progress = progress;
                process.ErrorMessage = error;
                if (endTime.HasValue)
                    process.EndTime = endTime;
            }
        }

        public void RemoveProcess(Guid processId)
        {
            _processes.TryRemove(processId, out _);
        }
    }

}
