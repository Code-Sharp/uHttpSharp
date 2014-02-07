using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace uhttpsharp.Utilities
{
    public class HttpStreamReader : Stream
    {
        Stream _stream;
        int _bufferSize;
        Encoding _encoding;

        /// <summary>
        /// Creates a HttpStreamReader that mimics a StreamReader (but does not buffer ahead, so we do not affect binary data that may be in a stream).
        /// This class is not thread-safe, just like a normal StreamReader.
        /// </summary>
        /// <param name="stream"></param>
        public HttpStreamReader (Stream stream, Encoding encoding) : base()
        {
            _stream = stream;
            _bufferSize = 4096;
            _encoding = encoding;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        public Stream BaseStream
        {
            get { return _stream; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// This function will only return a string for lines ending in \n or \r\n. Lines ending in \r will not be recognized.
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadLineAsync()
        {
            Task<string> task = new Task<string>(() =>
            {
                long position = 0;
                bool found = false;
                bool foundR = false;
                List<byte[]> buffers = new List<byte[]>();
                byte[] buffer = new byte[_bufferSize];

                while (!found)
                {
                    if (position > 0)
                    {
                        position = 0;
                        buffers.Add(buffer);
                        buffer = new byte[_bufferSize];
                    }

                    while (position < buffer.Length)
                    {
                        int s = _stream.ReadByte();

                        if (s < 0)
                        {
                            found = true;
                            break;
                        }
                        else if (s == '\n')
                        {
                            found = true;
                            break;
                        }
                        else if (s == '\r')
                        {
                            foundR = true;
                        }
                        else
                        {
                            foundR = false;
                        }

                        buffer[position++] = (byte)s;
                    }
                }

                if (position > 0)
                {
                    buffers.Add(buffer);
                }
                else
                {
                    position = buffer.Length;
                }

                var size = (buffers.Count - 1) * _bufferSize + (foundR ? position - 1 : position);

                if (size < 1)
                {
                    return "";
                }

                byte[] finalArray = new byte[size];

                long finalArrayposition = 0;

                for (int i = 0; i < buffers.Count - 1; i++)
                {
                    Array.Copy(buffers[i], 0, finalArray, finalArrayposition, buffers[i].Length);
                    finalArrayposition += buffers[i].Length;
                }

                Array.Copy(buffers[buffers.Count - 1], 0, finalArray, finalArrayposition, foundR ? position - 1 : position);

                return _encoding.GetString(finalArray);
            });

            task.Start();

            return task;
        }
    }
}
