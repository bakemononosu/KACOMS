using ElsWebApp.Areas.Identity.Models;
using ElsWebApp.Data;
using ElsWebApp.Models;
using ElsWebApp.Models.Entitiy;
using ElsWebApp.Services;
using ElsWebApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ElsConnection") ?? throw new InvalidOperationException("Connection string 'ElsConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
var connectionStringEls = builder.Configuration.GetConnectionString("ElsConnection") ?? throw new InvalidOperationException("Connection string 'ElsConnection' not found.");
builder.Services.AddDbContext<ElsWebAppDbContext>(options =>
    options.UseSqlServer(connectionStringEls));

// 業務ロジックDI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IMovieContentsService, MovieContentsService>();
builder.Services.AddScoped<ITestContentsService, TestContentsService>();
builder.Services.AddScoped<IQuestionCatalogService, QuestionCatalogService>();
builder.Services.AddScoped<IAnswerGroupService, AnswerGroupService>();
builder.Services.AddScoped<IExamListService, ExamListService>();
builder.Services.AddScoped<IUserChapterService, UserChapterService>();
builder.Services.AddScoped<IUserExamService, UserExamService>();
builder.Services.AddScoped<IUserScoreService, UserScoreService>();
builder.Services.AddScoped<ICreateExamService, CreateExamService>();
builder.Services.AddScoped<IElsService, ElsService>();
builder.Services.AddScoped<IAccountInfoService, AccountInfoService>();
builder.Services.AddScoped<IAdminStudentsService, AdminStudentsService>();
builder.Services.AddScoped<IAdminTaskStatusService, AdminTaskStatusService>();
builder.Services.AddScoped<IAdminTestStatusService, AdminTestStatusService>();
builder.Services.AddScoped<IAdminCourseService, AdminCourseService>();
builder.Services.AddScoped<ISysCodeService, SysCodeService>();
builder.Services.AddScoped<IStudentMyCourseService, StudentMyCourseService>();
builder.Services.AddScoped<IStudentCoursesService, StudentCoursesService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<IdentityErrorDescriberJP>();

builder.Services.AddControllersWithViews();

// セッション設定
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// NLog設定
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    //app.UseExceptionHandler("/Base/Error");
}
else
{
    app.UseExceptionHandler("/Base/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".m3u8"] = "application/x-mpegURL";
provider.Mappings[".ts"] = "video/MP2T";

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    ContentTypeProvider = provider,
});

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Student}");
app.MapRazorPages();

app.Run();
