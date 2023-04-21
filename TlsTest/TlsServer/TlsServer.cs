using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
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
		//ServicePointManager.SecurityProtocol =
		//	0
		//	| SecurityProtocolType.Tls13
		//	| SecurityProtocolType.Tls12
		//	| SecurityProtocolType.Tls11
		//	| SecurityProtocolType.Tls
		//;
		var c = client.AcceptTcpClient();
		_stream = new SslStream(
			c.GetStream(),
			false,
			ValidateServerCertificate,
			null);
		X509Certificate2 certificate = X509Certificate2.CreateFromPemFile("certificate.pem", "privatekey.pem");
		certificate = new(certificate.Export(X509ContentType.Pfx));

		var t = certificate.GetRSAPrivateKey();
		_stream.AuthenticateAsServer(certificate, false, sp, false);

	}

	public bool Reading => _stream.CanRead;
	public Stream Stream => _stream;

	private static X509Certificate2 GenerateCertificate()
	{
		var keypairgen = new RsaKeyPairGenerator();
		keypairgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));

		var keypair = keypairgen.GenerateKeyPair();

		var gen = new X509V3CertificateGenerator();

		var CN = new X509Name("CN=" + "TempCert");
		BigInteger SN = BigInteger.ProbablePrime(120, new Random(DateTime.Now.Millisecond));

		gen.SetSerialNumber(SN);
		gen.SetSubjectDN(CN);
		gen.SetIssuerDN(CN);
		gen.SetNotAfter(DateTime.MaxValue);
		gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
		gen.SetSignatureAlgorithm("MD5WithRSA");
		gen.SetPublicKey(keypair.Public);
		

		var newCert = gen.Generate(keypair.Private);

		return new X509Certificate2(DotNetUtilities.ToX509Certificate(newCert));

	}

	private static bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
	{
		return true;
	}

}