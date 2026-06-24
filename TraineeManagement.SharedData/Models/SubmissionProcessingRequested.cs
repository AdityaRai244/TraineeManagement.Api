using System.Text.Json.Serialization;

namespace TraineeManagement.SharedData.Models;

public class SubmissionProcessingRequested
{

    public Guid MessageId { get; set; }
    public Guid CorrelationId { get; set; }
    public int SubmissionId { get; set; }
    public int FileId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string ContractVersion = "v1.0";


}