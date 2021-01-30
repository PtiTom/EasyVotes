using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyVotes.Models
{
	public class Vote
	{
		[Key]
		public int IdVote { get; set; }
		
		public int IdSessionVote { get; set; }
		
		//public SessionVote Session { get; set; }

		public string IntituleVote { get; set; }
		public bool Anonyme { get; set; }
		public bool VoteOuvert { get; set; }

		public IEnumerable<Choix> ReponsesPossibles { get; set; }

		public IEnumerable<AVoté> VotesEffectués { get; set; }

		public SuffrageExprimé DejaExprime { get; set; }
		
		public IEnumerable<SuffrageExprimé> Resultats { get; set; }
	}
}
