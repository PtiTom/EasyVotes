using System;

namespace EasyVotes.Models
{
	public class AVoté
	{
		public Vote Vote { get; set; }

		public string LoginInscrit { get; set; }

		public DateTime DateHeureVote { get; set; }
	}
}
