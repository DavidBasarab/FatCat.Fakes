using System;
using System.Linq;

namespace OneOff;

public struct DummyMongoObjectId
    : IComparable<DummyMongoObjectId>,
        IEquatable<DummyMongoObjectId>,
        IConvertible
{
    public DummyMongoObjectId(byte[] id)
    {
        Id = id;
    }

    public DummyMongoObjectId(string something) { }

    public DummyMongoObjectId(int a, int b, int c) { }

    public byte[] Id { get; }

    public override string ToString()
    {
        return string.Join("", Id.Select(b => b.ToString("x2")));
    }

    public int CompareTo(DummyMongoObjectId other) => throw new NotImplementedException();

    public bool Equals(DummyMongoObjectId other) => throw new NotImplementedException();

    public override bool Equals(object obj)
    {
        if (obj is DummyMongoObjectId other)
        {
            return Id.SequenceEqual(other.Id);
        }

        return false;
    }

    public override int GetHashCode() => BitConverter.ToInt32(Id, 0);

    public static DummyMongoObjectId NewObjectId()
    {
        var randomBytes = new byte[12];
        var rng = new Random();
        rng.NextBytes(randomBytes);

        return new DummyMongoObjectId(randomBytes);
    }

    public TypeCode GetTypeCode() => throw new NotImplementedException();

    public bool ToBoolean(IFormatProvider provider) => throw new NotImplementedException();

    public byte ToByte(IFormatProvider provider) => throw new NotImplementedException();

    public char ToChar(IFormatProvider provider) => throw new NotImplementedException();

    public DateTime ToDateTime(IFormatProvider provider) => throw new NotImplementedException();

    public decimal ToDecimal(IFormatProvider provider) => throw new NotImplementedException();

    public double ToDouble(IFormatProvider provider) => throw new NotImplementedException();

    public short ToInt16(IFormatProvider provider) => throw new NotImplementedException();

    public int ToInt32(IFormatProvider provider) => throw new NotImplementedException();

    public long ToInt64(IFormatProvider provider) => throw new NotImplementedException();

    public sbyte ToSByte(IFormatProvider provider) => throw new NotImplementedException();

    public float ToSingle(IFormatProvider provider) => throw new NotImplementedException();

    public string ToString(IFormatProvider provider) => throw new NotImplementedException();

    public object ToType(Type conversionType, IFormatProvider provider) =>
        throw new NotImplementedException();

    public ushort ToUInt16(IFormatProvider provider) => throw new NotImplementedException();

    public uint ToUInt32(IFormatProvider provider) => throw new NotImplementedException();

    public ulong ToUInt64(IFormatProvider provider) => throw new NotImplementedException();
}
