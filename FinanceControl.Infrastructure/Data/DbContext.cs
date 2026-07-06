using System.Data.Entity;
using FinanceControl.Core.Entities;

namespace FinanceControl.Infrastructure.Data
{
    public class FinanceControlDbContext : System.Data.Entity.DbContext
    {
        public FinanceControlDbContext()
            : base("FinanceControlConnection")
        {
            Database.SetInitializer<FinanceControlDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().Property(x => x.CPF).HasColumnName("CPF").HasMaxLength(11).IsFixedLength().IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Senha).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Role).HasColumnType("tinyint").IsRequired();
            modelBuilder.Entity<User>().HasMany(x => x.Financas).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.TokensRecuperacaoSenha).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Finance>().ToTable("Finances");
            modelBuilder.Entity<Finance>().HasKey(x => x.Id);
            modelBuilder.Entity<Finance>().Property(x => x.Tipo).HasColumnType("tinyint").IsRequired();
            modelBuilder.Entity<Finance>().Property(x => x.Valor).HasPrecision(18, 2).IsRequired();
            modelBuilder.Entity<Finance>().Property(x => x.Descricao).HasMaxLength(255).IsRequired();

            modelBuilder.Entity<PasswordResetToken>().ToTable("PasswordResetTokens");
            modelBuilder.Entity<PasswordResetToken>().HasKey(x => x.Id);
            modelBuilder.Entity<PasswordResetToken>().Property(x => x.Token).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<PasswordResetToken>().Property(x => x.Expiracao).IsRequired();
            modelBuilder.Entity<PasswordResetToken>().Property(x => x.Usado).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
