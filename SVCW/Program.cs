using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SVCWContext>(option =>
{
    option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    //option.UseLazyLoadingProxies(true).UseSqlServer("Data Source=LAPTOP-8LC85HGU\\SQLEXPRESS;Initial Catalog=SVCW;Persist Security Info=True;User ID=sa;Password=12");
});
builder.Services.AddControllers().AddNewtonsoftJson(op => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRole, RoleService>();
builder.Services.AddScoped<IAchivement, AchivementService>();
builder.Services.AddScoped<IReportType, ReportTypeService>();
builder.Services.AddScoped<IProcessType, ProcessTypeService>();
builder.Services.AddScoped<IVote, VoteService>();
builder.Services.AddScoped<IDonation, DonationService>();
builder.Services.AddScoped<IActivity, ActivityService>();
builder.Services.AddScoped<IActivityResult, ActivityResultService>();
builder.Services.AddScoped<IBankAccount, BankAccountService>();
builder.Services.AddScoped<IFanpage, FanpageService>();
builder.Services.AddScoped<IComment, CommentService>();
builder.Services.AddScoped<ILike, LikeService>();
builder.Services.AddScoped<INotification, NotificationService>();
builder.Services.AddScoped<IReport, ReportService>();
builder.Services.AddScoped<IConfig, ConfigService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IProcess, ProcessService>();
builder.Services.AddScoped<IAdmin, AdminService>();
builder.Services.AddScoped<IModerator, ModeratorService>();
builder.Services.AddScoped<ISearchContent, UserSearchService>();






builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("SVCW", new OpenApiInfo() { Title = "SVCW", Version = "v1" });
    //setup comment in swagger UI
    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

    option.IncludeXmlComments(xmlCommentFileFullPath);
});

var app = builder.Build();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/SVCW/swagger.json", "SVCWApi v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseCors(x => x.AllowAnyOrigin()
                 .AllowAnyHeader()
                 .AllowAnyMethod());


app.UseAuthorization();

app.MapControllers();

app.Run();