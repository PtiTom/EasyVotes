using EasyVotes.Models;
using EF = Microsoft.EntityFrameworkCore;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace EasyVotes.Data
{
	public class VoteContext : EF.DbContext
	{
		private string easyVoteConnectionString;
		public VoteContext(EF.DbContextOptions<VoteContext> options) : base(options)
		{
			// TODO : Modifier l'injection.
			this.easyVoteConnectionString = options.FindExtension<EF.SqlServer.Infrastructure.Internal.SqlServerOptionsExtension>().ConnectionString;
		}

		public VoteContext(string connectionString)
		{
			this.easyVoteConnectionString = connectionString;
		}

		//public DbSet<SessionVote> Sessions { get; set; }
		//public DbSet<Vote> Votes { get; set; }
		//public DbSet<Choix> Reponses { get; set; }
		//public DbSet<Inscrit> Inscrits { get; set; }

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//    modelBuilder.Entity<SessionVote>().ToTable("SessionVote", "Vote").HasMany(s => s.Questions);
		//    var voteRef = modelBuilder.Entity<Vote>().ToTable("Vote", "Vote");
		//    voteRef.HasOne(v => v.Session).WithMany(v => v.Questions);
		//    voteRef.HasMany(v => v.SuffragesExprimes);
		//    voteRef.HasMany(v => v.ReponsesPossibles);
		//    modelBuilder.Entity<Inscrit>().ToTable("Inscrit", "Vote").HasNoKey().HasOne(i => i.SessionVote);
		//    modelBuilder.Entity<AVoté>().ToTable("AVoté", "Vote").HasNoKey().HasOne(s => s.Vote);
		//    modelBuilder.Entity<Choix>().ToTable("Choix", "Vote").HasOne(c => c.Vote);
		//}

		public async Task<IEnumerable<SessionVote>> GetSessions(string userLogin)
		{
			string query = "SELECT IdSessionVote, NomSessionVote, DebutSession, FinSession, InitiateurSession, (SELECT COUNT(*) FROM Vote.Vote V WHERE V.IdSessionVote = S.IdSessionVote) AS NombreQuestions FROM Vote.SessionVote S WHERE IdSessionVote IN (SELECT IdSessionVote FROM Vote.Inscrit WHERE LoginInscrit = @UserLogin) OR InitiateurSession = @UserLogin";


			using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
			{
				c.Open();
				return await c.QueryAsync<SessionVote>(query, new { UserLogin = userLogin });
			}
		}

		public async Task<SessionVote> GetSession(int sessionId)
		{
			string sessionQuery = "SELECT IdSessionVote, NomSessionVote, DebutSession, FinSession, InitiateurSession, (SELECT COUNT(*) FROM Vote.Vote V WHERE V.IdSessionVote = S.IdSessionVote) AS NombreQuestions FROM Vote.SessionVote S WHERE IdSessionVote = @IdSession";
			SessionVote returnedEntity;

			using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
			{
				c.Open();
				returnedEntity = await c.QueryFirstAsync<SessionVote>(sessionQuery, new { IdSession = sessionId });
				returnedEntity.Questions = await this.GetVotes(sessionId);
			}

			return returnedEntity;
		}

		public async Task<IEnumerable<Vote>> GetVotes(int sessionId)
		{
			string votesQuery = "SELECT IdVote, IntituleVote, Anonyme, VoteOuvert FROM Vote.Vote V WHERE V.IdSessionVote = @IdSession";
			string choixQuery = "SELECT IdChoix, Ordre, IntituleChoix FROM Vote.Choix WHERE IdVote = @IdVote";
			string aVoteQuery = "SELECT LoginInscrit, MAX(DateHeureVote) AS DateHeureVote FROM Vote.AVoté WHERE IdVote = @IdVote GROUP BY LoginInscrit";
			IEnumerable<Vote> returnedEntity;

			using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
			{
				c.Open();
				returnedEntity = await c.QueryAsync<Vote>(votesQuery, new { IdSession = sessionId });
				foreach (Vote v in returnedEntity)
				{
					v.ReponsesPossibles = await c.QueryAsync<Choix>(choixQuery, new { IdVote = v.IdVote });
					v.VotesEffectués = await c.QueryAsync<AVoté>(aVoteQuery, new { IdVote = v.IdVote });
				}
			}

			return returnedEntity;
		}

		public async Task<Vote> GetVote(int voteId, string userLogin = null)
		{
			string votesQuery = "SELECT IdVote, IdSessionVote, IntituleVote, Anonyme, VoteOuvert FROM Vote.Vote V WHERE V.IdVote = @IdVote";
			string choixQuery = "SELECT IdChoix, Ordre, IntituleChoix FROM Vote.Choix WHERE IdVote = @IdVote";
			string aVoteQuery = "SELECT LoginInscrit, DateHeureVote FROM Vote.AVoté WHERE IdVote = @IdVote";
			string reviewVoteQuery = "SELECT LoginInscrit, IdChoix, DateHeureModif, IdGroupeReponses, CodeReponse FROM Vote.SuffrageExprimé WHERE IdVote = @IdVote AND LoginInscrit = @LoginInscrit";
			Vote returnedEntity;

			using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
			{
				c.Open();
				returnedEntity = await c.QueryFirstAsync<Vote>(votesQuery, new { IdVote = voteId });
				returnedEntity.ReponsesPossibles = await c.QueryAsync<Choix>(choixQuery, new { IdVote = voteId });
				returnedEntity.VotesEffectués = await c.QueryAsync<AVoté>(aVoteQuery, new { IdVote = voteId });

				if (!string.IsNullOrEmpty(userLogin) && !returnedEntity.Anonyme) // Dans le cas des votes non anonymes, on permet d'éditer le vote.
				{
					returnedEntity.DejaExprime = await c.QueryFirstOrDefaultAsync<SuffrageExprimé>(reviewVoteQuery, new { IdVote = voteId, LoginInscrit = userLogin });
				}
			}

			return returnedEntity;
		}
		public async Task<Vote> GetVoteWithResultats(int voteId, string userLogin = null)
		{
			Vote returnedEntity = await GetVote(voteId, userLogin);

			if (returnedEntity != null)
			{
				using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
				{
					c.Open();
					string resultatsQuery = "SELECT LoginInscrit, IdChoix, DateHeureModif, IdGroupeReponses, CodeReponse FROM Vote.SuffrageExprimé WHERE IdVote = @IdVote";
					returnedEntity.Resultats = await c.QueryAsync<SuffrageExprimé>(resultatsQuery, new { IdVote = voteId });
				}
			}

			return returnedEntity;
		}

		public async Task RecordVote(int idVote, int idChoix, string userLogin)
		{
			Vote currentVote = await GetVote(idVote);
			if ((await this.GetSessions(userLogin)).Any(s => s.IdSessionVote == currentVote.IdSessionVote))
			{
				// Ajout du "à voté" :
				using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
				{
					c.Open();
					await c.ExecuteAsync("INSERT INTO Vote.AVoté SELECT @IdVote, @LoginInscrit, GETDATE()", new { IdVote = idVote, LoginInscrit = userLogin });

					if (currentVote.Anonyme)
					{
						await c.ExecuteAsync("INSERT INTO Vote.SuffrageExprimé SELECT @IdVote, @IdChoix, NULL, GETDATE(), NULL, NULL", new { IdVote = idVote, IdChoix = idChoix });
					}
					else
					{
						await c.ExecuteAsync("DELETE FROM Vote.SuffrageExprimé WHERE IdVote = @IdVote AND LoginInscrit = @LoginInscrit", new { IdVote = idVote, LoginInscrit = userLogin });
						await c.ExecuteAsync("INSERT INTO Vote.SuffrageExprimé SELECT @IdVote, @IdChoix, @LoginInscrit, GETDATE(), NULL, NULL", new { IdVote = idVote, IdChoix = idChoix, LoginInscrit = userLogin });
					}
				}
			}

			// TODO : Logguer car tentative de fraude.
		}

		public async Task<int> OpenVote(int idVote, string userLogin)
		{
			Vote currentVote = await GetVote(idVote);
			SessionVote currentSession = await GetSession(currentVote.IdSessionVote);

			if (userLogin == currentSession.InitiateurSession)
			{
				using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
				{
					c.Open();
					await c.ExecuteAsync("UPDATE Vote.Vote SET VoteOuvert = 1 WHERE IdVote = @IdVote", new { IdVote = idVote });
				}
			}

			// TODO : Logguer car tentative de fraude.

			return currentVote.IdSessionVote;
		}

		public async Task<int> CloseVote(int idVote, string userLogin)
		{
			Vote currentVote = await GetVote(idVote);
			SessionVote currentSession = await GetSession(currentVote.IdSessionVote);

			if (userLogin == currentSession.InitiateurSession)
			{
				using (SqlConnection c = new SqlConnection(this.easyVoteConnectionString))
				{
					c.Open();
					await c.ExecuteAsync("UPDATE Vote.Vote SET VoteOuvert = 0 WHERE IdVote = @IdVote", new { IdVote = idVote });
				}
			}

			// TODO : Logguer car tentative de fraude.

			return currentVote.IdSessionVote;
		}
	}
}
