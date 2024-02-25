using System;
using Soly.Utilities.ByteArray;
namespace ROIDForumServer
{
	public class MessageReader
	{
		private uint _currentLoc;
		private readonly ByteArray _byteData;
		private readonly uint _byteLength;

		public MessageReader(byte[] messageData)
		{
			this._byteData = new ByteArray(messageData);
			this._currentLoc = 0;
			this._byteLength = GetUint32();
			//Throw an error if the message is an incorrect length
			if (this._byteLength != messageData.Length)
			{
				throw new Exception("Message Incorrect Length");
			}
		}
		public bool IsAtEndOfData()
		{
			return _byteLength == _currentLoc;
		}

		public bool HasUint8()
		{
			return _currentLoc + 1 <= _byteLength;
		}

		public byte GetUint8()
		{
			var data = _byteData.ReadU8((int)_currentLoc);
			_currentLoc += 1;
			return data;
		}

		public bool HasInt8()
		{
			return _currentLoc + 1 <= _byteLength;
		}

		public sbyte GetInt8()
		{
			var data = _byteData.ReadI8((int)_currentLoc);

			_currentLoc += 1;
			return data;

		}

		public bool HasUint16()
		{
			return _currentLoc + 2 <= _byteLength;
		}

		public ushort GetUint16()
		{
			var data = _byteData.ReadU16((int)_currentLoc, Endianness.BigEndian);

			_currentLoc += 2;
			return data;

		}

		public bool HasInt16()
		{
			return _currentLoc + 2 <= _byteLength;
		}

		public short GetInt16()
		{
			var data = _byteData.ReadI16((int)_currentLoc, Endianness.BigEndian);

			_currentLoc += 2;
			return data;

		}

		public bool HasUint32()
		{
			return _currentLoc + 4 <= _byteLength;
		}

		public uint GetUint32()
		{
			var data = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
			return data;
		}

		public bool HasInt32()
		{
			return _currentLoc + 4 <= _byteLength;
		}

		public int GetInt32()
		{
			var data = _byteData.ReadI32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
			return data;
		}

		public double GetFloat64()
		{
			var data = _byteData.ReadF64((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 8;
			return data;
		}

		public bool HasFloat64()
		{
			return _currentLoc + 8 <= _byteLength;
		}

		public bool HasFloat32()
		{
			return _currentLoc + 4 <= _byteLength;
		}

		public float GetFloat32()
		{
			var data = _byteData.ReadF32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
			return data;
		}

		public bool HasString()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			return _currentLoc + length <= _byteLength;
		}

		public string GetString()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
			var innerLength = length - 4;

			var byteArray = new byte[innerLength];
			this._byteData.Read(byteArray, 0, (int)innerLength, (int)_currentLoc);         
			var returnString = System.Text.Encoding.Unicode.GetString(byteArray);
			_currentLoc += innerLength;
			return returnString;
		}

		public bool HasBinary()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			return (_currentLoc + length) <= _byteLength;
		}

		public byte[] GetBinary()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
			var innerLength = length - 4;
            var byteArray = new byte[innerLength];
            _byteData.Read(byteArray, 0, (int)innerLength, (int)_currentLoc);  
			_currentLoc += innerLength;
			return byteArray;
		}
        
		public uint GetLength()
		{
			return _byteLength;
		}
	}
}