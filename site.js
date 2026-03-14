@model LoginViewModel
@{
    ViewData["Title"] = "Login";
}

<div class="auth-wrap">
    <section class="card auth-card">
        <h1>Welcome back</h1>
        <p class="lead">Sign in to continue your training journey.</p>

        <form asp-action="Login" method="post" class="form">
            @Html.AntiForgeryToken()
            <input asp-for="ReturnUrl" type="hidden" />
            <div asp-validation-summary="ModelOnly" class="flash error"></div>

            <label asp-for="Email"></label>
            <input asp-for="Email" />
            <span asp-validation-for="Email" class="field-error"></span>

            <label asp-for="Password"></label>
            <input asp-for="Password" />
            <span asp-validation-for="Password" class="field-error"></span>

            <div class="top-gap-sm">
                <button type="submit" class="btn">Login</button>
                <a asp-action="Register" class="btn ghost">Create account</a>
            </div>
        </form>
    </section>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
