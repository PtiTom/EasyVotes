using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyVotes.Models
{
	public class SuffrageExprimé
	{
		public string LoginInscrit { get; set; }
		public DateTime DateHeureModif { get; set; }
		public int IdGroupeReponses { get; set; }
		public string CodeReponse { get; set; }

		public Vote Vote { get; set; }
		public Choix Choix { get; set; }
	}
}
