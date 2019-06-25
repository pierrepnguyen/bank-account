using Microsoft.EntityFrameworkCore;
using bank.Models;

namespace bank.Models
{
  public class bankContext : DbContext
  {
    // base() calls the parent class' constructor passing the "options" parameter along
    public bankContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users {get;set;}
    public DbSet<Transaction> transactions {get;set;}
  }
}
