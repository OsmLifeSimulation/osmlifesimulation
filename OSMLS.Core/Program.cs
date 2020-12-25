﻿using System;
using System.IO;
using System.Runtime.Loader;
using System.Threading;

namespace OSMLS.Core
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Directory.CreateDirectory(Constants.SettingsDirectoryPath);
			Directory.CreateDirectory(Constants.ModulesDirectoryPath);

			//Events for systemd service
			AssemblyLoadContext.Default.Unloading += SigTermEventHandler; //register sigterm event handler. 
			Console.CancelKeyPress += CancelHandler; //register sigint event handler

			var presets = Constants.DeserializeXmlOrCreateNew<PresetsXml>(Constants.PresetsFilePath);

			var osm = new OsmLifeSimulator(presets.OsmFilePath);
			Console.WriteLine("Application successfully started.");
			new Timer(callback => { osm.Update(); }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));

			if (presets.RunWebSocketServer)
			{
				var webSocketServer = new WebSocketServer(presets.WebSocketServerUri, osm.MapObjects);

				var startTimeSpan = TimeSpan.Zero;
				var periodTimeSpan = TimeSpan.FromSeconds(1);
				new Timer(callback => { webSocketServer.Update(); }, null, startTimeSpan, periodTimeSpan);
			}

			Thread.Sleep(Timeout.Infinite);
		}

		private static void SigTermEventHandler(AssemblyLoadContext obj)
		{
			Console.WriteLine("Unloading...");
		}

		private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine("Exiting...");
		}
	}
}