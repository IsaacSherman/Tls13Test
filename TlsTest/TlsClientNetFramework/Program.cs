// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Common;
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
				port = int.Parse(args[2]);
			

			TlsClient client = new TlsClient(port, ip);
			try
			{
				while (true)
				{
					string input = Console.ReadLine()??"Nothing";
					Util.WriteUint(client.Stream, (uint)input.Length);
					client.Stream.Write(Encoding.ASCII.GetBytes(input),0, input.Length);
					uint chars = Util.ReadUint(client.Stream);
					Console.WriteLine($"Chars received: {chars}");
					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				Debug.Print("Exception killed TlsClient.Read: " + ex);
				string msg1 = "Exception killed TlsClient.Read: " + ex;
				Console.WriteLine(msg1);
			}
		}
	}

	public class TlsClient
	{
		public TlsClient(int port, string ip)
		{
			TcpClient client = new TcpClient(ip, port);

			Stream = new SslStream(client.GetStream(), true,UserCertificateValidationCallback);
			Ssl.AuthenticateAsClient(ip, null,  SslProtocols.Tls12, false);
		}

		private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}

		public bool Writing => Stream.CanWrite;
		public Stream Stream { get; set; }
		private SslStream Ssl => (SslStream)Stream;
	}
}