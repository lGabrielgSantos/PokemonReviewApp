using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // add a db set for models/tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set;}
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }

        //configure n - n
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PokemonCategory>()
                .HasKey(pc => new { pc.PokemonId, pc.CategoryId }); //configure primary key for n-n
            modelBuilder.Entity<PokemonCategory>()
                .HasOne(p => p.Pokemon) //for 1 pokemnon
                .WithMany(pc => pc.PokemonCategories) //is very pokemons categories
                .HasForeignKey(p => p.PokemonId); //and is my fk chain Pokemon id
            modelBuilder.Entity<PokemonCategory>()
                .HasOne(p => p.Category) //for 1 category
                .WithMany(pc => pc.PokemonCategories) //is very pokemon categories
                .HasForeignKey(c => c.CategoryId); // if link is categoryid

            modelBuilder.Entity<PokemonOwner>()
                .HasKey(po => new { po.PokemonId, po.OwnerId });
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Owner)
                .WithMany(po => po.PokemonOwners)
                .HasForeignKey(o => o.OwnerId);
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Pokemon)
                .WithMany(po => po.PokemonOwners)
                .HasForeignKey(p => p.PokemonId);

        }

    }
}
