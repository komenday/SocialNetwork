using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using SQRS.Core.Consumers;
using EventHandler = Post.Query.Infrastructure.Handlers.EventHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Action<DbContextOptionsBuilder> contextConfig = 
    o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
builder.Services.AddDbContext<DatabaseContext>(contextConfig);
builder.Services.AddSingleton(new DatabaseContextFactory(contextConfig));

var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, EventHandler>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
