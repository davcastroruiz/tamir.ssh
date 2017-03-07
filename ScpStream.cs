using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Tamir.SharpSsh.jsch;

namespace Tamir.SharpSsh
{
	/// <summary>
	/// Provides a stream to access files on a remote system using SCP
	/// </summary>
	public class ScpStream : Stream
	{
		private Channel channel;
		private Stream stream;
		bool writeStream;

		/// <summary>
		/// Creates a <see cref="ScpStream"/> from a given <see cref="Channel"/> and exisiting stream
		/// </summary>
		/// <param name="channel">The <see cref="Channel"/> that is used by the base stream</param>
		/// <param name="stream">The base stream of the <see cref="ScpStream"/></param>
		/// <param name="writeStream">A flag indicating whether this is a write stream or a read stream</param>
		internal ScpStream(Channel channel, Stream stream, bool writeStream)
		{
			this.channel = channel;
			this.stream = stream;
			this.writeStream = writeStream;
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get { return stream.CanRead && !writeStream; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get { return stream.CanSeek; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get { return stream.CanWrite && writeStream; }
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get { return stream.Length; }
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return stream.Position;
			}
			set
			{
				stream.Position = value;
			}
		}

		/// <summary>
		/// Closes the <see cref="ScpStream"/> and its associated <see cref="Channel"/>
		/// Also writes the file completion byte if the stream is a write stream
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (writeStream)
			{
				byte[] buf = Encoding.Default.GetBytes("\0");
				stream.Write(buf, 0, buf.Length);
				stream.Flush();
			}

			channel.disconnect();
			stream.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			stream.Flush();
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and ( offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream. </param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the origin parameter. </param>
		/// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position. </param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

	}
}
