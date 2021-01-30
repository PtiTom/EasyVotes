using EasyVotes.Data;
using EasyVotes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public async Task<IActionResult> Index()
		{
			//return View(this.ctx.Inscrits.Where(i => i.LoginInscrit == User.Identity.Name).Select(i => i.SessionVote).ToList());
			return View(await this.ctx.GetSessions(User.Identity.Name));
		}

		[Authorize]
		public async Task<IActionResult> VoteSession(int IdSessionVote)
		{
			//var session = this.ctx.Sessions.SingleOrDefault(s => s.IdSessionVote == IdSessionVote);
			//if (session == null)
			//{
			//	throw new Exception("Session inexistante");
			//}

			////session.Questions = this.ctx.Votes.Where(v => v.IdSessionVote == session.IdSessionVote).ToList();
			//session.Questions = this.ctx.Votes.ToList();
			if ((await ctx.GetSessions(User.Identity.Name)).Any(s => s.IdSessionVote == IdSessionVote))
			{
				return View(await this.ctx.GetSession(IdSessionVote));
			}

			return View(null);
		}

		[Authorize]
		public async Task<IActionResult> Vote(int IdVote)
		{
			Vote voteDetails = await this.ctx.GetVote(IdVote, User.Identity.Name);

			if ((await ctx.GetSessions(User.Identity.Name)).Any(s => s.IdSessionVote == voteDetails.IdSessionVote))
			{
				return View(voteDetails);
			}

			return View(null);
		}

		[Authorize, HttpPost]
		public async Task<IActionResult> Vote(string IdVote, string IdChoix)
		{
			if (int.TryParse(IdVote, out int idVote) && int.TryParse(IdChoix, out int idChoix))
			{
				await ctx.RecordVote(idVote, idChoix, User.Identity.Name);
				Vote voteDetails = await ctx.GetVote(idVote);
				return this.RedirectToAction("VoteSession", "Vote", new { IdSessionVote = voteDetails.IdSessionVote });
			}

			return this.RedirectToAction("Index", "Vote");
		}

		[Authorize]
		public async Task<IActionResult> Open(int IdVote)
		{
			int idSession = await this.ctx.OpenVote(IdVote, User.Identity.Name);
			return this.RedirectToAction("VoteSession", "Vote", new { IdSessionVote = idSession });
		}


		[Authorize]
		public async Task<IActionResult> Close(int IdVote)
		{
			int idSession = await this.ctx.CloseVote(IdVote, User.Identity.Name);
			return this.RedirectToAction("VoteSession", "Vote", new { IdSessionVote = idSession });
		}


		[Authorize]
		public async Task<IActionResult> Resultats(int IdVote)
		{
			Vote voteDetails = await this.ctx.GetVoteWithResultats(IdVote, User.Identity.Name);

			if ((await ctx.GetSessions(User.Identity.Name)).Any(s => s.IdSessionVote == voteDetails.IdSessionVote))
			{
				return View(voteDetails);
			}

			return View(null);
		}
	}
}
