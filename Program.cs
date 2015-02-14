// Originally from http://forum.xda-developers.com/windows-phone-8/development/howto-install-windows-10-phones-t3030105

using Fiddler;
using System;
using System.IO;
using System.Threading;

namespace WPInsiderHacks
{
	class Program
	{
		public static void WriteCommandResponse(string s)
		{
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(s);
			Console.ForegroundColor = oldColor;
		}

		public static void DoQuit()
		{
			WriteCommandResponse("Shutting down...");
			Fiddler.FiddlerApplication.Shutdown();
			Thread.Sleep(500);
		}

		static void Main(string[] args)
		{
			// Personalize for your Application, 64 chars or fewer
			Fiddler.FiddlerApplication.SetAppDisplayName("WP Insider Hacks");


			#region AttachEventListeners
			// Simply echo notifications to the console.  Because Fiddler.CONFIG.QuietMode=true 
			// by default, we must handle notifying the user ourselves.
			Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA) { Console.WriteLine("** NotifyUser: " + oNEA.NotifyString); };
			Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA) { Console.WriteLine("** LogString: " + oLEA.LogString); };

			Fiddler.FiddlerApplication.BeforeRequest += OnBeforeRequest;

			// Tell the system console to handle CTRL+C by calling our method that
			// gracefully shuts down the FiddlerCore.
			//
			// Note, this doesn't handle the case where the user closes the window with the close button.
			// See http://geekswithblogs.net/mrnat/archive/2004/09/23/11594.aspx for info on that...
			//
			Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
			#endregion AttachEventListeners

			string sSAZInfo = "NoSAZ";

			Console.WriteLine(String.Format("Starting {0} ({1})...", Fiddler.FiddlerApplication.GetVersionString(), sSAZInfo));

			// For the purposes of this demo, we'll forbid connections to HTTPS 
			// sites that use invalid certificates. Change this from the default only
			// if you know EXACTLY what that implies.
			Fiddler.CONFIG.IgnoreServerCertErrors = true;

			// ... but you can allow a specific (even invalid) certificate by implementing and assigning a callback...
			// FiddlerApplication.OnValidateServerCertificate += new System.EventHandler<ValidateServerCertificateEventArgs>(CheckCert);

			FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

			// For forward-compatibility with updated FiddlerCore libraries, it is strongly recommended that you 
			// start with the DEFAULT options and manually disable specific unwanted options.
			FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.DecryptSSL
				| FiddlerCoreStartupFlags.AllowRemoteClients | FiddlerCoreStartupFlags.ChainToUpstreamGateway
				| FiddlerCoreStartupFlags.OptimizeThreadPool;

			// NOTE: In the next line, you can pass 0 for the port (instead of 8877) to have FiddlerCore auto-select an available port
			int iPort = 8877;
			Fiddler.FiddlerApplication.Startup(iPort, oFCSF);

			FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);

			FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
			FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());


			Console.WriteLine("Hit CTRL+C to end session.");


			bool bDone = false;
			do
			{
				Console.WriteLine("\nEnter a command [Q=Quit]:");
				Console.Write(">");
				ConsoleKeyInfo cki = Console.ReadKey();
				Console.WriteLine();
				switch (Char.ToLower(cki.KeyChar))
				{

					case 'd':
						FiddlerApplication.Log.LogString("FiddlerApplication::Shutdown.");
						FiddlerApplication.Shutdown();
						break;

					case 'g':
						Console.WriteLine("Working Set:\t" + Environment.WorkingSet.ToString("n0"));
						Console.WriteLine("Begin GC...");
						GC.Collect();
						Console.WriteLine("GC Done.\nWorking Set:\t" + Environment.WorkingSet.ToString("n0"));
						break;

					case 'q':
						bDone = true;
						DoQuit();
						break;

					case 't':
						try
						{
							WriteCommandResponse("Result: " + Fiddler.CertMaker.trustRootCert().ToString());
						}
						catch (Exception eX)
						{
							WriteCommandResponse("Failed: " + eX.ToString());
						}
						break;

					// Forgetful streaming
					case 's':
						bool bForgetful = !FiddlerApplication.Prefs.GetBoolPref("fiddler.network.streaming.ForgetStreamedData", false);
						FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.ForgetStreamedData", bForgetful);
						Console.WriteLine(bForgetful ? "FiddlerCore will immediately dump streaming response data." : "FiddlerCore will keep a copy of streamed response data.");
						break;

				}
			} while (!bDone);
		}

		/// <summary>
		///	This is where the hack happens
		/// </summary>
		/// <param name="oS"></param>
		static void OnBeforeRequest(Fiddler.Session oS)
		{
			// Console.WriteLine("Before request for:\t" + oS.fullUrl);
			// In order to enable response tampering, buffering mode MUST
			// be enabled; this allows FiddlerCore to permit modification of
			// the response in the BeforeResponse handler rather than streaming
			// the response to the client as the response comes in.
			oS.bBufferResponse = false;

			if (oS.fullUrl.StartsWith("https://wpflights.trafficmanager.net/RestUpdateProvisioningService.svc/UpdateChoices?"))
			{
				oS.utilCreateResponseAndBypassServer();
				oS.oResponse.headers.SetStatus(200, "Ok");
				oS.oResponse["Content-Type"] = "application/xml; charset=utf-8";
				oS.oResponse["Cache-Control"] = "private, max-age=0";
				// Read the XML config.
				oS.utilSetResponseBody(File.ReadAllText("WPFlights.xml"));
				FiddlerApplication.Log.LogFormat("Sending custom Flighting Response");
			}


		}

		/// <summary>
		/// When the user hits CTRL+C, this event fires.  We use this to shut down and unregister our FiddlerCore.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			DoQuit();
		}
	}
}

