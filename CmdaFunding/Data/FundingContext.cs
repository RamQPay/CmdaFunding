using CmdaFunding.Models;
using Microsoft.EntityFrameworkCore;

namespace CmdaFunding.Data
{
    public class FundingContext : DbContext
    {
        public FundingContext(DbContextOptions<FundingContext> options) : base(options)
        {
            
        }
        public DbSet<UserMaster> UserMaster { get; set; }
        public DbSet<MenuMaster> MenuMaster { get; set; }
        public DbSet<UserMenuMapping> UserMenuMapping { get; set; }
        public DbSet<PaymentGatewayMaster> PaymentGatewayMaster { get; set; }
        public DbSet<PaymentGatewayAccount> PaymPaymentGatewayAccount { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<HeaderMaster> HeaderMaster {  get; set; }

    }
}
