namespace TraineeManagement.SharedData.Models;

public enum JobStatus
{
    Queued,
    Processing,
    Completed,
    Failed
}

public class ProcessingJob
{

    public int Id { get; set; }
    public int Attempts { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid MessageId { get; set; }
    public JobStatus Status { get; set; }
    public string? ErrorSummary { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

}