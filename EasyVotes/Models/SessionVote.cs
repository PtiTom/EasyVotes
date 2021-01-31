using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;

namespace EasyVotes.Models
{
	public class SessionVote
	{
		[Key]
		public int IdSessionVote { get; set; }

		public string NomSessionVote { get; set; }

		public DateTime DebutSession { get; set; }

		public DateTime FinSession { get; set; }
		
		public string InitiateurSession { get; set; }

		public IEnumerable<Vote> Questions { get; set; }
		
		[Display(Name="Nombre de questions")]
		public int NombreQuestions { get; set; }

		public IEnumerable<string> Inscrits { get; set; }
	}
}