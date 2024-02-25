using System;
using Soly.Utilities.ByteArray;
namespace ROIDForumServer
{
	public class MessageReader(byte[] messageData)
	{
		private uint _currentLoc = 0;
		private readonly ByteArray _byteData = new ByteArray(messageData);
		private readonly int _byteLength = messageData.Length;

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

		public byte PeekUint8()
		{
			return _byteData.ReadU8((int)_currentLoc);
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

		public sbyte PeekInt8()
		{
			return _byteData.ReadI8((int)_currentLoc);
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

		public ushort PeekUint16()
		{
			return _byteData.ReadU16((int)_currentLoc, Endianness.BigEndian);
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

		public short PeekInt16()
		{
			return _byteData.ReadI16((int)_currentLoc, Endianness.BigEndian);
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

		public uint PeekUint32()
		{
			return _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
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

		public int PeekInt32()
		{
			return _byteData.ReadI32((int)_currentLoc, Endianness.BigEndian);
		}

		public double GetFloat64()
		{
			var data = _byteData.ReadF64((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 8;
			return data;
		}

		public double PeekFloat64()
		{
			return _byteData.ReadF64((int)_currentLoc, Endianness.BigEndian);
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

		public float PeekFloat32()
		{
			return _byteData.ReadF32((int)_currentLoc, Endianness.BigEndian);
		}

		public bool HasString()
		{
			if (_currentLoc + 4 > _byteLength)
			{
				return false;
			}
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			return _currentLoc + 4 + length <= _byteLength;
		}

		public string GetString()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;

			var byteArray = new byte[length];
			this._byteData.Read(byteArray, 0, (int)length, (int)_currentLoc);         
			var returnString = System.Text.Encoding.Unicode.GetString(byteArray);
			_currentLoc += length;
			return returnString;
		}

		public string PeekString()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);

			var byteArray = new byte[length];
			this._byteData.Read(byteArray, 0, (int)length, (int)_currentLoc + 4);         
			return System.Text.Encoding.Unicode.GetString(byteArray);
		}

		public bool HasBinary()
		{
			if (_currentLoc + 4 > _byteLength)
			{
				return false;
			}
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			return (_currentLoc + 4 + length) <= _byteLength;
		}

		public byte[] GetBinary()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			_currentLoc += 4;
            var byteArray = new byte[length];
            _byteData.Read(byteArray, 0, (int)length, (int)_currentLoc);  
			_currentLoc += length;
			return byteArray;
		}

		public byte[] PeekBinary()
		{
			var length = _byteData.ReadU32((int)_currentLoc, Endianness.BigEndian);
			var byteArray = new byte[length];
			_byteData.Read(byteArray, 0, (int)length, (int)_currentLoc + 4);
			return byteArray;
		}
        
		public int GetLength()
		{
			return _byteLength;
		}
	}
}