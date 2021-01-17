using EasyVotes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyVotes.Controllers
{
	public class VoteController : Controller
	{
		private readonly VoteContext ctx;

		public VoteController(VoteContext context)
		{
			this.ctx = context;
		}

		[Authorize]
		public IActionResult Index()
		{
			return View(this.ctx.Inscrits.Where(i => i.LoginInscrit == User.Identity.Name).Select(i => i.SessionVote).ToList());
		}

		[Authorize]
		public IActionResult VoteSession(int IdSessionVote)
		{
			var session = this.ctx.Sessions.SingleOrDefault(s => s.IdSessionVote == IdSessionVote);
			if (session == null)
			{
				throw new Exception("Session inexistante");
			}

			//session.Questions = this.ctx.Votes.Where(v => v.IdSessionVote == session.IdSessionVote).ToList();
			session.Questions = this.ctx.Votes.ToList();

			return View(session);
		}
	}
}
