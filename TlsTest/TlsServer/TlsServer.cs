using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace TlsTest;

public class TlsServer
{
	private readonly SslStream _stream;

	public TlsServer(int port, string ipAddr)
	{
		TcpListener client = new(IPAddress.Parse(ipAddr), port);
		client.Start();
		SslProtocols sp = SslProtocols.Tls13;
		var c = client.AcceptTcpClient();
		_stream = new SslStream(
			c.GetStream(),
			false,
			ValidateServerCertificate,
			null);
		X509Certificate2 certificate = X509Certificate2.CreateFromPemFile("certificate.pem", "privatekey.pem");
		certificate = new(certificate.Export(X509ContentType.Pfx));
		_stream.AuthenticateAsServer(certificate, false, sp, false);

	}

	public bool Reading => _stream.CanRead;
	public Stream Stream => _stream;


	private static bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
	{
		return true;
	}

}