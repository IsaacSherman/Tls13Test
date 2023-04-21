// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using Common;
using Microsoft.VisualBasic.CompilerServices;

namespace TlsTest
{

	public static class Program
	{
		public const int DefaultPort = 5678;
		public const string DefaultIp = "127.0.0.1";
		public static void Main()
		{
			var args = Environment.GetCommandLineArgs();
			string ip = DefaultIp;
			if (args.Length > 1)
			{
				ip = args[1];
			}

			int port = DefaultPort;
			if (args.Length > 2)
			{
				port = int.Parse(args[2]);
			}

			TlsServer server = new TlsServer(port, ip);
			try
			{
				while (server.Reading)
				{
					uint len = Util.ReadUint(server.Stream);
					byte[] buf = new byte[len];
					Util.ReadBytes(server.Stream, buf);

					string incoming = Encoding.ASCII.GetString(buf, 0, (int)len);

					Console.WriteLine($"Incoming: {incoming}");
					Util.WriteUint(server.Stream, (uint)incoming.Length);
					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				Debug.Print("Exception killed TlsServer.Read: " + ex);
				string msg1 = "Exception killed TlsServer.Read: " + ex;
				Console.WriteLine(msg1);
			}
		}
	}
}