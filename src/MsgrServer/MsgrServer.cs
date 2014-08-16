// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Msgr.Network.Handlers;
using Aura.Msgr.Util;
using Aura.Shared;
using Aura.Shared.Util;
using System;

namespace Aura.Msgr
{
	public class MsgrServer : ServerMain
	{
		public static readonly MsgrServer Instance = new MsgrServer();

		private bool _running = false;

		/// <summary>
		/// Configuration
		/// </summary>
		public MsgrConf Conf { get; private set; }

		/// <summary>
		/// Instance of the actual server component.
		/// </summary>
		private Aura.Msgr.Network.MsgrServer Server { get; set; }

		private MsgrServer()
		{
			this.Server = new Aura.Msgr.Network.MsgrServer();
			this.Server.Handlers = new MsgrServerHandlers();
			this.Server.Handlers.AutoLoad();
		}

		/// <summary>
		/// Loads all necessary components and starts the server.
		/// </summary>
		public void Run()
		{
			if (_running)
				throw new Exception("Server is already running.");

			CliUtil.WriteHeader("Msgr Server", ConsoleColor.DarkCyan);
			CliUtil.LoadingTitle();

			this.NavigateToRoot();

			// Conf
			this.LoadConf(this.Conf = new MsgrConf());

			// Database
			this.InitDatabase(this.Conf);

			// Data
			//this.LoadData(DataLoad.LoginServer, false);

			// Localization
			this.LoadLocalization(this.Conf);

			// Start
			this.Server.Start(8002);

			CliUtil.RunningTitle();
			_running = true;

			Console.ReadLine();
		}
	}
}
