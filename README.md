@model IEnumerable<CourseListItemViewModel>
@{
    ViewData["Title"] = "Courses";
}

<section class="hero card compact">
    <span class="pill">Course catalogue</span>
    <h1>Courses</h1>
    <p class="lead">A clean course list that stays visually close to your PHP system.</p>
</section>

<div class="grid top-gap">
    @foreach (var course in Model)
    {
        <article class="card course-card">
            <span class="pill">@course.HeroText</span>
            <h3>@course.Title</h3>
            <p>@course.Summary</p>
            <p class="muted">@course.CompletedLessons of @course.LessonCount lessons complete</p>
            <div class="progress"><span style="width:@course.ProgressPercent%"></span></div>
            <div class="row top-gap-sm">
                <span class="muted">@course.ProgressPercent% complete</span>
                <a asp-action="Details" asp-route-id="@course.Id" class="btn small">Open course</a>
            </div>
        </article>
    }
</div>
