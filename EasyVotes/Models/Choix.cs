using System.ComponentModel.DataAnnotations;

namespace EasyVotes.Models
{
	public class Choix
	{
		[Key]
		public int IdChoix { get; set; }

		public string IntituleChoix { get; set; }

		public int Ordre { get; set; }

		//public Vote Vote { get; set; }
	}
}
