using EasyVotes.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyVotes
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			CreateDbIfNotExists(host);

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		private static void CreateDbIfNotExists(IHost host)
		{
			try
			{
				VoteContext context = (VoteContext)host.Services.GetService(typeof(VoteContext));
				DbInitializer.Initialize(context);
			}
			catch (Exception ex)
			{
				ILogger<Program> logger = (ILogger<Program>)host.Services.GetService(typeof(ILogger<Program>));
				logger.LogError(ex, "An error occurred creating the DB.");
			}

		}
	}
}
