using System;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
