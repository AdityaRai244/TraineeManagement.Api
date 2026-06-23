using System.Text.Json.Serialization;

namespace TraineeManagement.Api.Models;

public class SubmissionProcessingRequested
{

    public string MessageId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public int SubmissionId { get; set; }
    public string FileId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string ContractVersion = "v1.0";


}