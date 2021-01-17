using EasyVotes.Models;
using Microsoft.EntityFrameworkCore;


namespace EasyVotes.Data
{
	public class VoteContext : DbContext
    {
        public VoteContext(DbContextOptions<VoteContext> options) : base(options)
        {
        }

        public DbSet<SessionVote> Sessions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Choix> Reponses { get; set; }
        public DbSet<Inscrit> Inscrits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionVote>().ToTable("SessionVote", "Vote").HasMany(s => s.Questions);
            var voteRef = modelBuilder.Entity<Vote>().ToTable("Vote", "Vote");
            voteRef.HasOne(v => v.Session).WithMany(v => v.Questions);
            voteRef.HasMany(v => v.SuffragesExprimes);
            voteRef.HasMany(v => v.ReponsesPossibles);
            modelBuilder.Entity<Inscrit>().ToTable("Inscrit", "Vote").HasNoKey().HasOne(i => i.SessionVote);
            modelBuilder.Entity<AVoté>().ToTable("AVoté", "Vote").HasNoKey().HasOne(s => s.Vote);
            modelBuilder.Entity<Choix>().ToTable("Choix", "Vote").HasOne(c => c.Vote);
        }
    }
}
