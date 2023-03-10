using Microsoft.EntityFrameworkCore;
using PokemonReviewApp;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Repository;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers().AddJsonOptions(x =>
                  x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Add auto mapper
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>(); //call my interface and repository in the scoped
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); //call my interface and repository in the scoped
builder.Services.AddScoped<ICountryRepository, CountryRepository>(); //call my interface and repository in the scoped
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>(); //call my interface and repository in the scoped
builder.Services.AddScoped<IReviewRepository, ReviewRepository>(); //call my interface and repository in the scoped
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>(); //call my interface and repository in the scoped





// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add we contex (Data > DataContext) and pass connection string for connect in the bank
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
