using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Mock
{
    public sealed class MockNetworkStream : Stream
    {
        //private readonly byte[] _data;
        //private int _index;

        private readonly MemoryStream _stream;

        public MockNetworkStream(byte[] data)
        {
            _stream = new MemoryStream(data, false);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //// EOF
            //if( _index + 1 >= _data.Length )
            //    return 0;

            //// 长度不够，只能返回部分内容
            //if( _index + 1 + count > _data.Length ) {
            //    int count2 = _data.Length - _index;
            //    Array.Copy(_data, _index, buffer, offset, count2);
            //    _index += count2;
            //    return count2;
            //}

            //// 返回指定的长度
            //Array.Copy(_data, _index, buffer, offset, count);
            //_index += count;
            //return count;

            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
