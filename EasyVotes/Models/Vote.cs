using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyVotes.Models
{
	public class Vote
	{
		[Key]
		public int IdVote { get; set; }
		
		public SessionVote Session { get; set; }

		public string IntituleVote { get; set; }
		public bool Anonyme { get; set; }
		public bool VoteOuvert { get; set; }

		public List<Choix
			> ReponsesPossibles { get; set; }

		public List<AVoté> SuffragesExprimes { get; set; }
	}
}
