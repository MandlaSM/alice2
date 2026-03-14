namespace AliceTrainingSystem.ViewModels;

public class CourseListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string HeroText { get; set; } = string.Empty;
    public int LessonCount { get; set; }
    public int CompletedLessons { get; set; }
    public int ProgressPercent { get; set; }
    public bool IsEnrolled { get; set; }
}
