using System;
using System.Text;

namespace SysUtils
{
    // 对象ID
    public class ObjectID
    {
        public uint m_nIdent = 0;
        public uint m_nSerial = 0;

        public ObjectID()
        {
        }

        public ObjectID(uint ident, uint serial)
        {
            m_nIdent = ident;
            m_nSerial = serial;
        }

        public ObjectID Clone()
        {
            return new ObjectID(m_nIdent, m_nSerial);
        }

        public bool IsNull()
        {
            if (m_nIdent != 0)
            {
                return false;
            }

            if (m_nSerial != 0)
            {
                return false;
            }
            return true;
        }

        // 判断相等
        public bool EqualTo(ObjectID other)
        {
            return (m_nIdent == other.m_nIdent)
                && (m_nSerial == other.m_nSerial);
        }

        // 转换成字符串
        public override string ToString()
        {
            return string.Format("{0}-{1}", m_nIdent, m_nSerial);
        }

        // 从字符串转换
        public static ObjectID FromString(string val)
        {
            int index = val.IndexOf("-");

            if (index == -1)
            {
                return new ObjectID();
            }

            uint ident = Convert.ToUInt32(val.Substring(0, index));
            uint serial = Convert.ToUInt32(val.Substring(index + 1,
                val.Length - index - 1));

            return new ObjectID(ident, serial);
        }

        public override bool Equals(System.Object obj)
        {
            try
            {
                return EqualTo((ObjectID)obj);
            }
            catch (Exception)
            {
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)(m_nIdent + m_nSerial);
        }

        public static bool operator ==(ObjectID p1, ObjectID p2)
        {
            if(ReferenceEquals(null, p1) && ReferenceEquals(null, p2))
            {
                return true;
            }
            else if(ReferenceEquals(null, p1) || ReferenceEquals(null, p2))
            {
                return false;
            }
            else
            {
                return  (p1.m_nIdent == p2.m_nIdent) && (p1.m_nSerial == p2.m_nSerial);
            }
        }

        public static bool operator !=(ObjectID p1, ObjectID p2)
        {
            if (ReferenceEquals(null, p1) && ReferenceEquals(null, p2))
            {
                return false;
            }
            else if (ReferenceEquals(null, p1) || ReferenceEquals(null, p2))
            {
                return true;
            }
            else
            {
                return ((p1.m_nIdent != p2.m_nIdent) || (p1.m_nSerial != p2.m_nSerial));
            }
        }

        public static ObjectID val2ObjID(string objID)
        {
            return new ObjectID(uint.Parse(objID) , 0);
        }
        public static ObjectID val2ObjID(uint objID)
        {
            return new ObjectID(objID, 0);
        }
        public static ObjectID val2ObjID(int objID)
        {
            return new ObjectID((uint)objID, 0);
        }

    }

    /*public class PERSISTID
    {
        public uint m_nIdent;
        public uint m_nSerial;

        public PERSISTID()
        {
            m_nIdent = 0;
            m_nSerial = 0;
        }

        public bool IsNull()
        {
            if (m_nIdent != 0)
            {
                return false;
            }

            if (m_nSerial != 0)
            {
                return false;
            }
            return true;
        }

        public PERSISTID(uint ident, uint serial)
        {
            m_nIdent = ident;
            m_nSerial = serial;
        }

        // 判断相等
        public bool EqualTo(ObjectID other)
        {
            return (m_nIdent == other.m_nIdent)
                && (m_nSerial == other.m_nSerial);
        }

        // 转换成字符串
        public override string ToString()
        {
            return string.Format("{0}-{1}", m_nIdent, m_nSerial);
        }

        // 从字符串转换
        public static ObjectID FromString(string val)
        {
            int index = val.IndexOf("-");

            if (index == -1)
            {
                return new ObjectID();
            }

            uint ident = Convert.ToUInt32(val.Substring(0, index));
            uint serial = Convert.ToUInt32(val.Substring(index + 1,
                val.Length - index - 1));

            return new ObjectID(ident, serial);
        }
    }*/
}
