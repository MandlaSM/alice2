using AliceTrainingSystem.Data;
using AliceTrainingSystem.Models;
using AliceTrainingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliceTrainingSystem.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly AppDbContext _db;

    public CoursesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var enrolledCourseIds = await _db.Enrollments
            .Where(x => x.UserId == userId.Value)
            .Select(x => x.CourseId)
            .ToListAsync();

        var completedLessonIds = await _db.LessonProgressItems
            .Where(x => x.UserId == userId.Value && x.Completed)
            .Select(x => x.LessonId)
            .ToListAsync();

        var courses = await _db.Courses
            .Where(x => x.IsPublished)
            .OrderBy(x => x.SortOrder)
            .Select(course => new
            {
                course.Id,
                course.Title,
                course.Summary,
                course.HeroText,
                LessonIds = course.Modules.SelectMany(m => m.Lessons).Select(l => l.Id).ToList()
            })
            .ToListAsync();

        var model = courses.Select(course =>
        {
            var lessonCount = course.LessonIds.Count;
            var completed = course.LessonIds.Count(id => completedLessonIds.Contains(id));
            var progressPercent = lessonCount == 0 ? 0 : (int)Math.Round((double)completed * 100 / lessonCount);

            return new CourseListItemViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Summary = course.Summary,
                HeroText = course.HeroText,
                LessonCount = lessonCount,
                CompletedLessons = completed,
                ProgressPercent = progressPercent,
                IsEnrolled = enrolledCourseIds.Contains(course.Id)
            };
        }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var course = await _db.Courses
            .Include(x => x.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsPublished);

        if (course == null)
        {
            return NotFound();
        }

        var enrollment = await _db.Enrollments.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.CourseId == course.Id);
        if (enrollment == null)
        {
            _db.Enrollments.Add(new Enrollment { UserId = userId.Value, CourseId = course.Id });
            await _db.SaveChangesAsync();
        }

        var completedLessonIds = await _db.LessonProgressItems
            .Where(x => x.UserId == userId.Value && x.Completed)
            .Select(x => x.LessonId)
            .ToListAsync();

        var allLessons = course.Modules.SelectMany(m => m.Lessons).ToList();
        var completedCount = allLessons.Count(l => completedLessonIds.Contains(l.Id));
        var progressPercent = allLessons.Count == 0 ? 0 : (int)Math.Round((double)completedCount * 100 / allLessons.Count);

        var model = new CourseDetailsViewModel
        {
            Id = course.Id,
            Title = course.Title,
            Summary = course.Summary,
            HeroText = course.HeroText,
            TotalLessons = allLessons.Count,
            CompletedLessons = completedCount,
            ProgressPercent = progressPercent,
            IsEnrolled = true,
            Modules = course.Modules
                .OrderBy(m => m.SortOrder)
                .Select(module => new ModuleViewModel
                {
                    Id = module.Id,
                    Title = module.Title,
                    Summary = module.Summary,
                    Lessons = module.Lessons
                        .OrderBy(l => l.SortOrder)
                        .Select(lesson => new LessonListItemViewModel
                        {
                            Id = lesson.Id,
                            Title = lesson.Title,
                            Summary = lesson.Summary,
                            Completed = completedLessonIds.Contains(lesson.Id)
                        }).ToList()
                }).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Lesson(int id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var lesson = await _db.Lessons
            .Include(x => x.Module!)
                .ThenInclude(m => m.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (lesson?.Module?.Course == null)
        {
            return NotFound();
        }

        var enrollment = await _db.Enrollments.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.CourseId == lesson.Module.CourseId);
        if (enrollment == null)
        {
            _db.Enrollments.Add(new Enrollment { UserId = userId.Value, CourseId = lesson.Module.CourseId });
            await _db.SaveChangesAsync();
        }

        var progress = await _db.LessonProgressItems.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.LessonId == lesson.Id);
        var courseLessonIds = await _db.Lessons
            .Where(x => x.Module!.CourseId == lesson.Module.CourseId)
            .OrderBy(x => x.Module!.SortOrder)
            .ThenBy(x => x.SortOrder)
            .Select(x => x.Id)
            .ToListAsync();

        var completedLessonIds = await _db.LessonProgressItems
            .Where(x => x.UserId == userId.Value && x.Completed && courseLessonIds.Contains(x.LessonId))
            .Select(x => x.LessonId)
            .ToListAsync();

        var currentIndex = courseLessonIds.FindIndex(x => x == lesson.Id);
        var progressPercent = courseLessonIds.Count == 0 ? 0 : (int)Math.Round((double)completedLessonIds.Count * 100 / courseLessonIds.Count);

        var model = new LessonViewModel
        {
            LessonId = lesson.Id,
            CourseId = lesson.Module.CourseId,
            CourseTitle = lesson.Module.Course.Title,
            ModuleTitle = lesson.Module.Title,
            LessonTitle = lesson.Title,
            Summary = lesson.Summary,
            ContentHtml = lesson.ContentHtml,
            VideoUrl = lesson.VideoUrl,
            Completed = progress?.Completed == true,
            ProgressPercent = progressPercent,
            PreviousLessonId = currentIndex > 0 ? courseLessonIds[currentIndex - 1] : null,
            NextLessonId = currentIndex >= 0 && currentIndex < courseLessonIds.Count - 1 ? courseLessonIds[currentIndex + 1] : null
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteLesson(int id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var lesson = await _db.Lessons
            .Include(x => x.Module)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        var progress = await _db.LessonProgressItems.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.LessonId == id);
        if (progress == null)
        {
            progress = new LessonProgress
            {
                UserId = userId.Value,
                LessonId = id,
                Completed = true,
                CompletedAtUtc = DateTime.UtcNow
            };
            _db.LessonProgressItems.Add(progress);
        }
        else
        {
            progress.Completed = true;
            progress.CompletedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Lesson marked as complete.";
        return RedirectToAction(nameof(Lesson), new { id });
    }
}
