namespace AliceTrainingSystem.Models;

public class LessonProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public AppUser? User { get; set; }
    public Lesson? Lesson { get; set; }
}
