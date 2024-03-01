using System;
using Soly.Utilities.ByteArray;
namespace ROIDForumServer
{
	public abstract class MessageData
	{
		public abstract void AddToByteData(ByteArray byteData, int loc);
		public abstract uint GetLength();
	}

	public class MessageDataUint8(byte value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
			byteData.Write(value, loc);
		}

		public override uint GetLength()
		{
			return 1;
		}
	}
	
	public class MessageDataInt8(sbyte value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc);
		}

		public override uint GetLength()
		{
			return 1;
		}
	}
    
	public class MessageDataUint16(ushort value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc, Endianness.BigEndian);
		}

		public override uint GetLength()
		{
			return 2;
		}
	}
    
	public class MessageDataInt16(short value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc, Endianness.BigEndian);
		}

		public override uint GetLength()
		{
			return 2;
		}
	}

	public class MessageDataUint32(uint value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc, Endianness.BigEndian);
		}

		public override uint GetLength()
		{
			return 4;
		}
	}

	public class MessageDataInt32(int value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc, Endianness.BigEndian);
		}

		public override uint GetLength()
		{
			return 4;
		}
	}

	public class MessageDataFloat32(float value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            
            byteData.Write(bytes, 0, (int)GetLength(), loc);
		}

		public override uint GetLength()
		{
			return 4;
		}
	}

	public class MessageDataFloat64(double value) : MessageData
	{
		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(value, loc, Endianness.BigEndian);
		}

		public override uint GetLength()
		{
			return 8;
		}
	}

	public class MessageDataString : MessageData
	{
		private readonly byte[] _value;
		private readonly uint _totalLength;

		public MessageDataString(string value)
		{
			this._value = System.Text.Encoding.Unicode.GetBytes(value);
			//Total length is buffer plus length of buffer
			this._totalLength = 4 + (uint)_value.Length;
		}

		public override void AddToByteData(ByteArray byteData, int loc)
		{
			byteData.Write(_totalLength - 4, loc, Endianness.BigEndian);
			if (_totalLength - 4 > 0)
			{
				byteData.Write(_value, 0, _value.Length, loc + 4);
			}
		}
        
		public override uint GetLength()
		{
			return _totalLength;
		}
	}

	public class MessageDataBinary : MessageData
	{
		private readonly byte[] _value;
		private readonly uint _totalLength;

		public MessageDataBinary(byte[] value)
		{
			_value = value;
			_totalLength = 4 + (uint)_value.Length;
		}

		public override void AddToByteData(ByteArray byteData, int loc)
		{
            byteData.Write(_totalLength - 4, loc, Endianness.BigEndian);
            if (_totalLength - 4 > 0)
            {
	            byteData.Write(_value, 0, _value.Length, loc + 4);
            }
		}

		public override uint GetLength()
		{
			return this._totalLength;
		}
	}
}