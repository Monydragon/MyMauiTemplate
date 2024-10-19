using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using MyMauiTemplate.Configuration.Constants;
using MyMauiTemplate.Models;

namespace MyMauiTemplate.Data.Data_Context;

public class MyMauiTemplateAppContext(string connectionString = AppConstants.SqlLiteConnectionString) : DbContext
{
    public DbSet<User>? Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(connectionString);
    }
}