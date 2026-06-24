namespace TraineeManagement.SharedData.Models;

public class SubmissionFile
{
    
    public int Id {get; set;}
    public int SubmissionId { get; set; } 

    public  string OriginalFileName {get; set;} = string.Empty;
    public string StorageName {get; set;} = string.Empty;
    public string ContentType {get; set;} = string.Empty;
    public long Size {get; set;}
    public string CheckSum {get; set;} = string.Empty;
    public int UploadedBy {get; set;}
    public Submission Submission {get; set;} = null!;

    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}


}