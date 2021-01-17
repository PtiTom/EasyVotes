using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyVotes.Data
{
	public static class DbInitializer
	{
		public static void Initialize(VoteContext context)
		{
			context.Database.EnsureCreated();

			// Look for any students.
			if (context.Sessions.Any())
			{
				return;   // DB has been seeded
			}
		}
	}
}