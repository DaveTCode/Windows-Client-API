using System.Diagnostics;
using System.IO;

namespace Sendsafely.Api.Objects
{
    internal class ProgressStream : Stream
    {
        private readonly Stream _inner;
        private readonly ISendSafelyProgress _progress;
        private readonly string _prefix;
        private readonly long _fileSize;
        private readonly long _offset;
        private long _readSoFar;
        private const int UpdateFrequency = 250;
        private readonly Stopwatch _stopwatch;

        public ProgressStream(Stream inner, ISendSafelyProgress progress, string prefix, long size, long offset)
        {
            _inner = inner;
            _progress = progress;
            _prefix = prefix;
            _fileSize = size;
            _readSoFar = 0;
            _offset = offset;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);

            _readSoFar += buffer.Length;

            UpdateProgress();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = _inner.Read(buffer, offset, count);

            _readSoFar += result;

            UpdateProgress();
            return result;
        }

        public override bool CanRead => _inner.CanRead;

        public override bool CanSeek => _inner.CanSeek;

        public override bool CanWrite => _inner.CanWrite;

        public override void Close()
        {
            _inner.Close();
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _readSoFar = offset;
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        private void UpdateProgress()
        {
            if (_stopwatch.ElapsedMilliseconds > UpdateFrequency)
            {
                _stopwatch.Reset();
                _stopwatch.Start();

                _progress.UpdateProgress(_prefix, (_readSoFar + _offset) / (double)_fileSize * 100);
            }
        }
    }
}
