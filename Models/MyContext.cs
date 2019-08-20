using Microsoft.EntityFrameworkCore;
 
namespace bank_accounts.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet <User> Users {get;set;}
        public DbSet <Transaction> Transactions {get;set;}
    }
}