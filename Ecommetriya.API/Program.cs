using Ecommetriya.API.Jobs;
using Ecommetriya.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("7154240714:AAGr9-Fcxm0ZF_Ohx8OOWWhXyR6raA82rgA"));
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    #region Çàãðóçêà äàííûõ ïî ïîñòàâêàì êàæäûå 30 ìèíóò

    var jobKeyLoadIncomes = new JobKey("LoadIncomes");
    q.AddJob<LoadIncomes>(opts => opts.WithIdentity(jobKeyLoadIncomes));

    q.AddTrigger(opts => opts
    .ForJob(jobKeyLoadIncomes)
    .WithIdentity($"{jobKeyLoadIncomes}-trigger")
    .StartNow()
    .WithSimpleSchedule(x => x
    .WithIntervalInMinutes(30)
    .RepeatForever()
    .Build())
    );
    #endregion

    #region Çàãðóçêà äàííûõ ïî ñêëàäó êàæäûå 30 ìèíóò

    //var jobKeyLoadStocks = new JobKey("LoadStocks");
    //q.AddJob<LoadStocks>(opts => opts.WithIdentity(jobKeyLoadStocks));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadStocks)
    //.WithIdentity($"{jobKeyLoadStocks}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    #endregion

    #region Çàãðóçêà äàííûõ ïî çàêàçàì êàæäûå 30 ìèíóò

    //var jobKeyLoadOrders = new JobKey("LoadOrders");
    //q.AddJob<LoadOrders>(opts => opts.WithIdentity(jobKeyLoadOrders));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadOrders)
    //.WithIdentity($"{jobKeyLoadOrders}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    #endregion

    #region Çàãðóçêà äàííûõ ïî ïðîäàæàì êàæäûå 30 ìèíóò

    //var jobKeyLoadSales = new JobKey("LoadSales");
    //q.AddJob<LoadSales>(opts => opts.WithIdentity(jobKeyLoadSales));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadSales)
    //.WithIdentity($"{jobKeyLoadSales}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    #endregion

    #region Çàãðóçêà ïîäðîáíîãî îò÷åòà êàæäûé ïîíåäåëüíèê â 17:00

    // var jobKeyLoadReportDetails = new JobKey("LoadReportDetails");
    // q.AddJob<LoadReportDetails>(opts => opts.WithIdentity(jobKeyLoadReportDetails));

    // q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadReportDetails)
    //.WithIdentity($"{jobKeyLoadReportDetails}-trigger-now")
    //.StartNow()
    //.WithSimpleSchedule(x => x.Build())
    //);

    // q.AddTrigger(opts => opts
    // .ForJob(jobKeyLoadReportDetails)
    // .WithIdentity($"{jobKeyLoadReportDetails}-trigger")
    // .WithCronSchedule("0 0 17 ? * MON")
    // );
    #endregion

    #region Çàãðóçêà ëåíòû òîâàðîâ êàæäûå 30 ìèíóò

    var jobKeyExecuteCardsFeeds = new JobKey("ExecuteCardsFeeds");
    q.AddJob<ExecuteCardsFeeds>(opts => opts.WithIdentity(jobKeyExecuteCardsFeeds));

    q.AddTrigger(opts => opts
    .ForJob(jobKeyExecuteCardsFeeds)
    .WithIdentity($"{jobKeyExecuteCardsFeeds}-trigger")
    .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Minute))
    .WithSimpleSchedule(x => x
    .WithIntervalInMinutes(25)
    .RepeatForever()
    .Build())
    );
    #endregion

    #region Çàãðóçêà êàðòî÷åê òîâàðà êàæäûé äåíü â 00:01

    //var jobKeyLoadCardsWildberries = new JobKey("LoadCardsWildberries");
    //q.AddJob<LoadCards>(opts => opts.WithIdentity(jobKeyLoadCardsWildberries));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadCardsWildberries)
    //.WithIdentity($"{jobKeyLoadCardsWildberries}-trigger")
    //.WithCronSchedule("0 1 0 * * ?") // Çàïóñê êàæäûé äåíü â 00:01
    //.StartNow()
    // );
    #endregion

    #region Çàãðóçêà êîíêóðåíòîâ êàæäûé äåíü â 00:01

    //var jobKeyLoadCompetitors = new JobKey("LoadCompetitors");
    //q.AddJob<LoadCompetitors>(opts => opts.WithIdentity(jobKeyLoadCompetitors));

    ////Êàæäûé äåíü â 01:00
    //q.AddTrigger(opts => opts
    //    .ForJob(jobKeyLoadCompetitors)
    //    .WithIdentity($"{jobKeyLoadCompetitors}-trigger")
    //    .WithCronSchedule("0 1 0 * * ?") // Çàïóñê êàæäûé äåíü â 00:01
    //    );
    #endregion

    #region Çàãðóçêà ðåêëàìíûõ êàìïàíèé â 03:03 êàæäûé äåíü

    //var jobKeyLoadAdverts = new JobKey("LoadAdverts");
    //q.AddJob<LoadAdverts>(opts => opts.WithIdentity(jobKeyLoadAdverts));

    ////Êàæäûé äåíü â 03:03
    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadAdverts)
    //.WithIdentity($"{jobKeyLoadAdverts}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    #endregion
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddScoped<IDataRepository, DataRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
builder.Services.AddCors();
app.UseHttpsRedirection();
app.UseCors(builder => builder
    .WithOrigins("http://localhost:5126")  //запрос из клиента порт 5126
    .AllowAnyHeader()
    .AllowAnyMethod()
);
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
