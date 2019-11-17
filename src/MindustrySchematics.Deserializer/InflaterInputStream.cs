using System;
using System.IO;
using System.Text;

namespace MindustrySchematics.Deserializer
{
	internal class InflaterInputStream
	{
		private readonly Stream _stream;

		public InflaterInputStream(Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new Exception("A stream that allows seeking is required.");
			}

			_stream = stream;
		}

		public byte[] ReadBytes(int count)
		{
			var buffer = new byte[count];
			_stream.Read(buffer, 0, buffer.Length);

			return buffer;
		}

		public short ReadShort()
		{
			var ch1 = (uint)_stream.ReadByte();
			var ch2 = (uint)_stream.ReadByte();

			return (short) ((ch1 << 8) + (ch2 << 0));
		}

		public byte ReadByte()
		{
			return (byte) _stream.ReadByte();
		}

		public int ReadInt()
		{
			var ch1 = (uint) _stream.ReadByte();
			var ch2 = (uint) _stream.ReadByte();
			var ch3 = (uint) _stream.ReadByte();
			var ch4 = (uint) _stream.ReadByte();

			return (int) ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
		}

		public string ReadUTF()
		{
			var stringLength = ReadShort();
			var stringBytes = ReadBytes(stringLength);

			return Encoding.UTF8.GetString(stringBytes);
		}
	}
}
