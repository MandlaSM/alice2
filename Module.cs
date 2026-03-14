using AliceTrainingSystem.Models;

namespace AliceTrainingSystem.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Courses.Any())
        {
            return;
        }

        var admin = new AppUser
        {
            FullName = "Admin User",
            Email = "admin@alice.local",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            IsAdmin = true
        };

        var learner = new AppUser
        {
            FullName = "Demo Learner",
            Email = "learner@alice.local",
            PasswordHash = PasswordHasher.Hash("Learner123!"),
            IsAdmin = false
        };

        var course1 = new Course
        {
            Title = "Introduction to ModCo Reinsurance",
            Slug = "introduction-to-modco-reinsurance",
            Summary = "A foundation course introducing ModCo structures, commercial rationale, and key supervision points.",
            HeroText = "Start here for the core ModCo journey.",
            SortOrder = 1
        };

        var course2 = new Course
        {
            Title = "Collateral Fundamentals",
            Slug = "collateral-fundamentals",
            Summary = "Understand why collateral matters, how it supports obligations, and where the operational risks appear.",
            HeroText = "A practical collateral primer.",
            SortOrder = 2
        };

        var m11 = new Module { Course = course1, Title = "Module 1: ModCo Basics", Summary = "Commercial overview and structure.", SortOrder = 1 };
        var m12 = new Module { Course = course1, Title = "Module 2: Risk Transfer", Summary = "What to look for when assessing substance.", SortOrder = 2 };
        var m21 = new Module { Course = course2, Title = "Module 1: Why Collateral?", Summary = "Purpose, mechanics, and governance.", SortOrder = 1 };

        var lessons = new[]
        {
            new Lesson
            {
                Module = m11,
                Title = "Lesson 1: What is ModCo?",
                Summary = "A gentle introduction to the structure.",
                ContentHtml = "<p>ModCo allows insurers and reinsurers to transfer blocks of business while retaining assets on the ceding company's balance sheet. In practice, this means the legal form and economic substance both need careful review.</p><p>This C# milestone keeps the familiar training feel of your PHP system: large headings, simple cards, and a clean lesson flow.</p>",
                VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                SortOrder = 1
            },
            new Lesson
            {
                Module = m11,
                Title = "Lesson 2: Commercial Rationale",
                Summary = "Why the transaction exists and what benefits it seeks.",
                ContentHtml = "<p>Common objectives include capital efficiency, access to specialist platforms, and reallocation of risk. Reviewers should connect the transaction to the firm's strategy, governance, and operational readiness.</p>",
                SortOrder = 2
            },
            new Lesson
            {
                Module = m12,
                Title = "Lesson 3: Risk Transfer Indicators",
                Summary = "A practical lens for reviewing substance.",
                ContentHtml = "<p>Look at investment flexibility, recapture rights, collateral controls, treaty clauses, and whether downside scenarios genuinely shift risk. Documentation should align with reported economics.</p>",
                SortOrder = 1
            },
            new Lesson
            {
                Module = m21,
                Title = "Lesson 1: Why Collateral is Required",
                Summary = "Operational and prudential foundations.",
                ContentHtml = "<p>Collateral protects counterparties and supports timely performance. The reviewer should understand legal enforceability, eligible asset rules, valuation mechanics, and dispute handling.</p>",
                VideoUrl = "https://www.youtube.com/embed/ysz5S6PUM-U",
                SortOrder = 1
            },
            new Lesson
            {
                Module = m21,
                Title = "Lesson 2: Governance Around Collateral",
                Summary = "Ownership, escalation, and controls.",
                ContentHtml = "<p>Strong collateral governance covers threshold monitoring, margin calls, dispute resolution, independent oversight, and clear management information for decision makers.</p>",
                SortOrder = 2
            }
        };

        db.Users.AddRange(admin, learner);
        db.Courses.AddRange(course1, course2);
        db.Modules.AddRange(m11, m12, m21);
        db.Lessons.AddRange(lessons);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new Enrollment { UserId = learner.Id, CourseId = course1.Id },
            new Enrollment { UserId = learner.Id, CourseId = course2.Id });

        db.LessonProgressItems.Add(
            new LessonProgress
            {
                UserId = learner.Id,
                LessonId = lessons[0].Id,
                Completed = true,
                CompletedAtUtc = DateTime.UtcNow.AddDays(-1)
            });

        db.SaveChanges();
    }
}
