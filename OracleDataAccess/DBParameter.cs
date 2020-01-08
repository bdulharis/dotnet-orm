using System;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;

namespace AHK.DataAccess.OracleDataAccess
{
    public class DBParameter
    {
        private string parameterName;
        private readonly OracleDbType oracleDbtype;
        private readonly Int64 dbsize;
        private readonly ParameterDirection parameterDirection;
        private Object value;
        public string ParameterName
        { get { return parameterName; } }
        public OracleDbType AHKDbtype
        { get { return oracleDbtype; } }
        public Int64 Dbsize
        { get { return dbsize; } }
        public ParameterDirection ParameterDirection
        { get { return parameterDirection; } }
        public Object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public DBParameter() { }
        public DBParameter(string parametername, AHKDbType datatype, ParameterDirection direction, object value)
        {
            this.parameterName = parametername;
            this.oracleDbtype = getEquivalentOracleDbType(datatype);
            this.parameterDirection = direction;
            this.value = (value != null) ? value : DBNull.Value;
        }
        public DBParameter(string parametername, AHKDbType datatype, Int64 dbsize, ParameterDirection direction, object value)
        {
            this.parameterName = parametername;
            this.oracleDbtype = getEquivalentOracleDbType(datatype);
            this.dbsize = dbsize;
            this.parameterDirection = direction;
            this.value = (value != null) ? value : DBNull.Value;
        }
        private OracleDbType getEquivalentOracleDbType(AHKDbType type)
        {
            switch (type)
            {
                case AHKDbType.BFile:
                    return OracleDbType.BFile;
                case AHKDbType.Blob:
                    return OracleDbType.Blob;
                case AHKDbType.Byte:
                    return OracleDbType.Byte;
                case AHKDbType.Char:
                    return OracleDbType.Char;
                case AHKDbType.Clob:
                    return OracleDbType.Clob;
                case AHKDbType.Date:
                    return OracleDbType.Date;
                case AHKDbType.Decimal:
                    return OracleDbType.Decimal;
                case AHKDbType.Double:
                    return OracleDbType.Double;
                case AHKDbType.Int16:
                    return OracleDbType.Int16;
                case AHKDbType.Int32:
                    return OracleDbType.Int32;
                case AHKDbType.Int64:
                    return OracleDbType.Int64;
                case AHKDbType.IntervalDS:
                    return OracleDbType.IntervalDS;
                case AHKDbType.IntervalYM:
                    return OracleDbType.IntervalYM;
                case AHKDbType.Long:
                    return OracleDbType.Long;
                case AHKDbType.LongRaw:
                    return OracleDbType.LongRaw;
                case AHKDbType.NChar:
                    return OracleDbType.NChar;
                case AHKDbType.NClob:
                    return OracleDbType.NClob;
                case AHKDbType.NVarchar2:
                    return OracleDbType.NVarchar2;
                case AHKDbType.Raw:
                    return OracleDbType.Raw;
                case AHKDbType.RefCursor:
                    return OracleDbType.RefCursor;
                case AHKDbType.Single:
                    return OracleDbType.Single;
                case AHKDbType.TimeStamp:
                    return OracleDbType.TimeStamp;
                case AHKDbType.TimeStampLTZ:
                    return OracleDbType.TimeStampLTZ;
                case AHKDbType.TimeStampTZ:
                    return OracleDbType.TimeStampTZ;
                case AHKDbType.Varchar2:
                    return OracleDbType.Varchar2;
                case AHKDbType.XmlType:
                    return OracleDbType.XmlType;
                default:
                    return OracleDbType.Varchar2;

            }
        }
    }
}
