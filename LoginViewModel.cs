namespace AliceTrainingSystem.Models;

public class Lesson
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int SortOrder { get; set; }

    public Module? Module { get; set; }
    public ICollection<LessonProgress> ProgressItems { get; set; } = new List<LessonProgress>();
}
