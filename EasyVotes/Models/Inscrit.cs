using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyVotes.Models
{
	public class Inscrit
	{
		public string LoginInscrit { get; set; }

		public SessionVote SessionVote { get; set; }
	}
}
