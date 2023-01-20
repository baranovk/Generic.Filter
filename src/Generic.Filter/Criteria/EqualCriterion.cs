using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Filter.Criteria
{
    public readonly struct EqualCriterion : IFilterCriteria, IConvertible
    {
        #region Fields

        private readonly object? _value;

        #endregion

        #region Constructors

        public EqualCriterion(object value)
        {
            // TODO: add a descriptive exception message
            if (value is EqualCriterion)
                throw new ArgumentException(nameof(value));

            _value = value;
        }

        #endregion

        #region Properties

        public bool IsNull => _value == null;

        #endregion

        #region Public Methods

        public override string? ToString()
        {
            return _value?.ToString();
        }

        #endregion

        #region Interfaces

        #region IConvertible

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
        bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(_value);
        byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotSupportedException();
        char IConvertible.ToChar(IFormatProvider? provider) => throw new NotSupportedException();
        DateTime IConvertible.ToDateTime(IFormatProvider? provider) => null == _value ? default : DateTime.Parse(((string?)_value)!, provider);
        decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotSupportedException();
        double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotSupportedException();
        short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotSupportedException();
        int IConvertible.ToInt32(IFormatProvider? provider) => Convert.ToInt32(_value);
        long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotSupportedException();
        sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotSupportedException();
        float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotSupportedException();
        string IConvertible.ToString(IFormatProvider? provider) => null != _value ? (string)_value! : string.Empty;
        ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotSupportedException();
        uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotSupportedException();
        ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotSupportedException();

        object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
        {
            if (conversionType == null) throw new ArgumentNullException(nameof(conversionType));

            return Type.GetTypeCode(conversionType) switch
            {
                TypeCode.Boolean => (bool)(_value ?? default(bool)),
                TypeCode.DateTime => DateTime.Parse(((string?)_value)!, provider),
                TypeCode.Int32 => (int)this,
                TypeCode.String => ((string?)_value)!,
                TypeCode.Object => this,
                _ => throw new NotSupportedException(),
            };
        }

        #endregion

        #endregion

        #region Operators

        public static implicit operator EqualCriterion(int value) => new(value);

        public static implicit operator EqualCriterion(string value) => new(value);

        public static implicit operator string?(EqualCriterion value) => value.ToString();

        public static implicit operator int(EqualCriterion value) 
        {
            return int.TryParse((string?)value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var int32) ? int32 : default;
        }

        #endregion
    }
}
