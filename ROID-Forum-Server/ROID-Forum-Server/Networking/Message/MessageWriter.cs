using System;
using System.Collections.Generic;
using Soly.Utilities.ByteArray;

namespace ROIDForumServer
{
	public class MessageWriter
	{
		private readonly List<MessageData> _dataArray = new List<MessageData>();
		private uint _byteLength = 0;

		public byte[] ToBuffer()
		{
			ByteArray byteData = new ByteArray((int)_byteLength);
			uint loc = 0;
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
			_byteLength += data.GetLength();
		}

		public void AddInt8(sbyte value)
		{
			var data = new MessageDataInt8(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddUint16(ushort value)
		{
			var data = new MessageDataUint16(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddInt16(short value)
		{
			var data = new MessageDataInt16(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddUint32(uint value)
		{
			var data = new MessageDataUint32(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddInt32(int value)
		{
			var data = new MessageDataInt32(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddFloat64(double value)
		{
			var data = new MessageDataFloat64(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddFloat32(float value)
		{
			var data = new MessageDataFloat32(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public void AddString(string value)
		{
			var data = new MessageDataString(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}
        
		public void AddBinary(byte[] value)
		{
			var data = new MessageDataBinary(value);
			_dataArray.Add(data);
			_byteLength += data.GetLength();
		}

		public uint GetLength()
		{
			return _byteLength;
		}
	}
}