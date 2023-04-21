using System;
using System.IO;
using System.Net.Sockets;

namespace Common
{
	public static class Util
	{
		public static uint ReadUint(Stream stream)
		{
			byte[] uintBuf = new byte[4];
			ReadBytes(stream, uintBuf);
			return BitConverter.ToUInt32(uintBuf, 0);
		}

		public static void WriteUint(Socket socket, uint val)
		{
			byte[] buf = new byte[4];
			int offset = 0;
			AppendUint(buf, val, ref offset);
			socket.Send(buf);
		}


		public static void WriteUint(Stream stream, uint val)
		{
			stream.Write(BitConverter.GetBytes(val), 0, 4);
		}
		public static void AppendUint(byte[] message, uint val, ref int offset)
		{
			message[offset + 3] = (byte)(val & 0xff);
			val >>= 8;
			message[offset + 2] = (byte)(val & 0xff);
			val >>= 8;
			message[offset + 1] = (byte)(val & 0xff);
			val >>= 8;
			message[offset] = (byte)(val & 0xff);
			val >>= 8;

			offset += 4;
		}


		public static void ReadBytes(Stream stream, byte[] buf, ref int offset, int size)
		{
			int bytesRead = 0;
			while (bytesRead < size)
			{
				int numBytes = stream.Read(buf, offset, size - bytesRead);
				if (numBytes == 0)
					throw new IOException("No data available");

				offset += numBytes;
				bytesRead += numBytes;
			}
		}

		public static void ReadBytes(Stream stream, byte[] buf)
		{
			int offset = 0;
			ReadBytes(stream, buf, ref offset, buf.Length);
		}

	}
}