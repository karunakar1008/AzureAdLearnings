namespace ProcessExcelDemo.Models
{
    public class ExcelProcessStatus
    {
        public Guid ProcessId { get; set; }
        public string? FileName { get; set; }
        public string Status { get; set; } = "Pending";  // Pending, Processing, Completed, Cancelled, Failed
        public int Progress { get; set; } = 0; // 0-100%
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
