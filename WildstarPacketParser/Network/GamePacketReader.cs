using System.Text;

using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Network;

public class Packet : IDisposable
{
    public uint BytePosition
    {
        get => (uint)(_stream?.Position ?? 0u);
    }

    public StringBuilder Writer { get; private set; } = new();

    public uint BytesRemaining => _stream?.Remaining() ?? 0u;

    private byte _currentBitPosition;
    private byte _currentBitValue;
    private readonly MemoryStream _stream;

    public Packet(MemoryStream input)
    {
        _stream = input;
        ResetBits();
    }

    public void ResetBits()
    {
        if (_currentBitPosition > 7)
            return;

        _currentBitPosition = 8;
        _currentBitValue = 0;
    }

    public bool ReadBit()
    {
        _currentBitPosition++;
        if (_currentBitPosition > 7)
        {
            _currentBitPosition = 0;
            _currentBitValue = (byte)_stream.ReadByte();
        }

        return ((_currentBitValue >> _currentBitPosition) & 1) != 0;
    }

    #region Binary Readers
    public ulong ReadBits(uint bits)
    {
        ulong value = 0ul;
        for (uint i = 0u; i < bits; i++)
            if (ReadBit())
                value |= 1ul << (int)i;

        return value;
    }

    public byte ReadByte(uint bits = 8u)
    {
        if (bits > sizeof(byte) * 8)
            throw new ArgumentException();

        return (byte)ReadBits(bits);
    }

    public ushort ReadUShort(uint bits = 16u)
    {
        if (bits > sizeof(ushort) * 8)
            throw new ArgumentException();

        return (ushort)ReadBits(bits);
    }

    public short ReadShort(uint bits = 16u)
    {
        if (bits > sizeof(short) * 8)
            throw new ArgumentException();

        return (short)ReadBits(bits);
    }

    public uint ReadUInt(uint bits = 32u)
    {
        if (bits > sizeof(uint) * 8)
            throw new ArgumentException();

        return (uint)ReadBits(bits);
    }

    public int ReadInt(uint bits = 32u)
    {
        if (bits > sizeof(int) * 8)
            throw new ArgumentException();

        return (int)ReadBits(bits);
    }

    public float ReadSingle(uint bits = 32u)
    {
        if (bits > sizeof(float) * 8)
            throw new ArgumentException();

        int value = (int)ReadBits(bits);
        return BitConverter.Int32BitsToSingle(value);
    }

    public double ReadDouble(uint bits = 64u)
    {
        if (bits > sizeof(double) * 8)
            throw new ArgumentException();

        long value = (long)ReadBits(bits);
        return BitConverter.Int64BitsToDouble(value);
    }

    public ulong ReadULong(uint bits = 64u)
    {
        if (bits > sizeof(ulong) * 8)
            throw new ArgumentException();

        return ReadBits(bits);
    }

    public T ReadEnum<T>(uint bits = 64u) where T : Enum
    {
        if (bits > sizeof(ulong) * 8)
            throw new ArgumentException();

        return (T)Enum.ToObject(typeof(T), ReadBits(bits));
    }

    public byte[] ReadBytes(uint length)
    {
        byte[] data = new byte[length];
        for (uint i = 0u; i < length; i++)
            data[i] = ReadByte();

        return data;
    }

    public string ReadWideStringFixed()
    {
        ushort length = ReadUShort();
        byte[] data = ReadBytes(length * 2u);
        return Encoding.Unicode.GetString(data, 0, data.Length - 2);
    }

    public string ReadWideString()
    {
        bool extended = ReadBit();
        ushort length = (ushort)(ReadUShort(extended ? 15u : 7u) << 1);

        byte[] data = ReadBytes(length);
        return Encoding.Unicode.GetString(data);
    }

    public float ReadPackedFloat()
    {
        float UnpackFloat(ushort packed)
        {
            uint v3 = packed & 0xFFFF7FFF;
            uint v4 = (packed & 0xFFFF8000) << 16;

            if ((v3 & 0x7C00) != 0)
                return BitConverter.Int32BitsToSingle((int)(v4 | ((v3 + 0x1C000) << 13)));
            if ((v3 & 0x3FF) == 0)
                return BitConverter.Int32BitsToSingle((int)(v4 | v3));

            uint v6 = (v3 & 0x3FF) << 13;
            uint i = 113;
            for (; v6 <= 0x7FFFFF; --i)
                v6 *= 2;
            return BitConverter.Int32BitsToSingle((int)(v4 | (i << 23) | v6 & 0x7FFFFF));
        }

        return UnpackFloat(ReadUShort());
    }
    #endregion
    #region StringBuilder
    public void Write(string value)
    {
        if (Writer == null)
            Writer = new StringBuilder();

        Writer.Append(value);
    }

    public void Write(string format, params object[] args)
    {
        if (Writer == null)
            Writer = new StringBuilder();

        Writer.AppendFormat(format, args);
    }

    public void WriteLine()
    {
        if (Writer == null)
            Writer = new StringBuilder();

        Writer.AppendLine();
    }

    public void WriteLine(string value)
    {
        if (Writer == null)
            Writer = new StringBuilder();

        Writer.AppendLine(value);
    }

    public void WriteLine(string format, params object[] args)
    {
        if (Writer == null)
            Writer = new StringBuilder();

        Writer.AppendLine(string.Format(format, args));
    }

    public T AddValue<T>(string name, T obj, params object[] indexes)
    {
        if (name != string.Empty || name != "")
            WriteLine($"{GetIndexString(indexes)}{name}: {obj}");

        return obj;
    }

    private static string GetIndexString(params object[] values)
    {
        var list = values.Flatten();

        return list.Where(value => value != null)
            .Aggregate(string.Empty, (current, value) =>
            {
                var s = value is string ? "()" : "[]";
                return current + (s[0] + value.ToString() + s[1] + ' ');
            });
    }

    public void AddHeader(string direction, uint opcode, int length, uint number)
    {
        WriteLine($"{Environment.NewLine}{direction}: {(Opcodes)opcode} (0x{opcode:X4}) Length: {length} Number: {number}");
    }
    #endregion
    #region Parser Readers
    public byte ReadByte(string name, uint bits = 8u, params object[] indexes)
    {
        var val = ReadByte(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public ushort ReadUShort(string name, uint bits = 16u, params object[] indexes)
    {
        var val = ReadUShort(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public short ReadShort(string name, uint bits = 16u, params object[] indexes)
    {
        var val = ReadShort(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public uint ReadUInt(string name, uint bits = 32u, params object[] indexes)
    {
        var val = ReadUInt(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public int ReadInt(string name, uint bits = 32u, params object[] indexes)
    {
        var val = ReadInt(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public float ReadSingle(string name, uint bits = 32u, params object[] indexes)
    {
        var val = ReadSingle(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public double ReadDouble(string name, uint bits = 64u, params object[] indexes)
    {
        var val = ReadDouble(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public ulong ReadULong(string name, uint bits = 64u, params object[] indexes)
    {
        var val = ReadULong(bits);
        AddValue(name, val, indexes);
        return val;
    }

    public T ReadEnum<T>(string name, uint bits = 64u, params object[] indexes) where T : Enum
    {
        var val = ReadEnum<T>(bits);
        AddValue(name, $"{Convert.ToInt64(val)} ({val})", indexes);
        return (T)Enum.ToObject(typeof(T), val);
    }

    public string ReadWideStringFixed(string name, params object[] indexes)
    {
        var val = ReadWideStringFixed();
        AddValue(name, val, indexes);
        return val;
    }

    public string ReadWideString(string name, params object[] indexes)
    {
        var val = ReadWideString();
        AddValue(name, val, indexes);
        return val;
    }

    public float ReadPackedFloat(string name, params object[] indexes)
    {
        var val = ReadPackedFloat();
        AddValue(name, val, indexes);
        return val;
    }

    public bool ReadBool(string name, params object[] indexes)
    {
        var val = ReadBit();
        AddValue(name, val, indexes);
        return val;
    }
    #endregion

    public void Dispose()
    {
        _stream?.Dispose();
    }
}

public struct PacketStruct
{
    public string Direction;
    public byte[] Data;
    public Opcodes Opcode;
}
