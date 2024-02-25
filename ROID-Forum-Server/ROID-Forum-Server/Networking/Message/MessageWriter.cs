using System;
using System.Collections.Generic;
using Soly.Utilities.ByteArray;

namespace ROIDForumServer
{
    /*
     * Writes a message and produces a binary buffer.
     * Format:
     * length (4 bytes) + binary data (any size)
     */
    /*
      This is currently very slow. Use ideas from:
      https://github.com/brianc/node-buffer-writer/blob/master/index.js
    */
	public class MessageWriter
	{
		private readonly List<MessageData> _dataArray;
		private uint _innerByteLength;
		public MessageWriter()
		{
			_dataArray = new List<MessageData>();
			//Only the length of bytes being stored
			_innerByteLength = 0;
		}

		public byte[] ToBuffer()
		{
			//Take length of all data and add the message length holder
			uint totalLength = _innerByteLength + 4;
			ByteArray byteData = new ByteArray((int)totalLength);
			uint loc = 0;
			//Append the message length
			byteData.Write(totalLength, (int)loc, Endianness.BigEndian);
			loc += 4;
			//Append the message
			foreach (MessageData data in _dataArray)
			{
				data.AddToByteData(byteData, (int)loc);
				loc += data.GetLength();
			}
			return byteData.Buffer;
		}
		public byte[] ToBufferWithoutLength()
		{
			//Take length of all data and add the message length holder
			uint totalLength = _innerByteLength;
			ByteArray byteData = new ByteArray((int)totalLength);
			uint loc = 0;
			//Append the message
			foreach (MessageData data in _dataArray)
			{
				data.AddToByteData(byteData, (int)loc);
				loc += data.GetLength();
			}
			return byteData.Buffer;
		}

		public void AddUint8(byte value)
		{
			var data = new MessageDataUint8(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddInt8(sbyte value)
		{
			var data = new MessageDataInt8(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddUint16(ushort value)
		{
			var data = new MessageDataUint16(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddInt16(short value)
		{
			var data = new MessageDataInt16(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddUint32(uint value)
		{
			var data = new MessageDataUint32(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddInt32(int value)
		{
			var data = new MessageDataInt32(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddFloat64(double value)
		{
			var data = new MessageDataFloat64(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddFloat32(float value)
		{
			var data = new MessageDataFloat32(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public void AddString(string value)
		{
			var data = new MessageDataString(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}
        
		public void AddBinary(byte[] value)
		{
			var data = new MessageDataBinary(value);
			_dataArray.Add(data);
			_innerByteLength += data.GetLength();
		}

		public uint GetLength()
		{
			return _innerByteLength + 4;
		}

	}
}