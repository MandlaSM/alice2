namespace AliceTrainingSystem.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAtUtc { get; set; } = DateTime.UtcNow;

    public AppUser? User { get; set; }
    public Course? Course { get; set; }
}
