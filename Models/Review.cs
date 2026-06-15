namespace TraineeManagement.Api.Models;

public enum ReviewStatus
{
    Accepted,
    ChangesRequired,
    Rejected
}

public class Review
{

    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public int MentorId { get; set; }

    public string Feedback { get; set; } = string.Empty;
    public int? Score { get; set; }


    public ReviewStatus ReviewStatus { get; set; }
    public DateTime ReviewedDate { get; set; }
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

}