using System;
using System.Collections;
using System.Collections.Generic;
using SysUtils;

namespace SysUtils
{
    // 基本游戏数据类型
    public class VarType
    {
        public const int Bool = 1;
        public const int Int = 2;
        public const int Int64 = 3;
        public const int Float = 4;
        public const int Double = 5;
        public const int String = 6;
        public const int WideStr = 7;
        public const int Object = 8;
        public const int Pointer = 9;
        public const int UserData = 10;

        // 是否有效的数据类型
        public static bool IsValid(int type)
        {
            return GetString(type) != "";
        }

        // 获得类型名称
        public static string GetString(int type)
        {
            if (type == VarType.Bool)
            {
                return "boolean";
            }
            else if (type == VarType.Int)
            {
                return "integer";
            }
            else if (type == VarType.Int64)
            {
                return "int64";
            }
            else if (type == VarType.Float)
            {
                return "float";
            }
            else if (type == VarType.Double)
            {
                return "double";
            }
            else if (type == VarType.String)
            {
                return "string";
            }
            else if (type == VarType.WideStr)
            {
                return "widestr";
            }
            else if (type == VarType.Object)
            {
                return "object";
            }
            else if (type == VarType.Pointer)
            {
                return "pointer";
            }
            else if (type == VarType.UserData)
            {
                return "binary";
            }
            else
            {
                return "";
            }
        }
    }

    // 可变类型变量
    public class Var
    {
        private int nType;
        private object Data;

        public int Type
        {
            get { return nType; }
        }

        public Var Clone()
        {
            Var var = new Var();
            var.nType = this.nType;
            var.Data = this.Data;
            return var;
        }

        public void Copy(Var var)
        {
            this.nType = var.nType;
            this.Data = var.Data;
        }

        public bool GetBool()
        {
            try
            {
                return (bool)Data;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetInt()
        {
            try
            {
                if (nType == VarType.Int64)
                {
                    return (int)((long)Data);
                }

                return (int)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetInt64()
        {
            try
            {
                if (nType == VarType.Int)
                {
                    return (long)((int)Data);
                }

                return (long)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float GetFloat()
        {
            try
            {
                return (float)Data;
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }

        public double GetDouble()
        {
            try
            {
                return (double)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetString()
        {
            try
            {
                return Data as string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetWideStr()
        {
            try
            {
                return Data as string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public ObjectID GetObject()
        {
            if (nType != VarType.Object)
            {
                return null;
            }

            return ((ObjectID)Data).Clone();
        }

        public byte[] GetUserData()
        {
            if (nType != VarType.UserData)
            {
                return null;
            }

            return (byte[])((byte[])Data).Clone();
        }

        public void SetBool(bool value)
        {
            nType = VarType.Bool;
            Data = value;
        }

        public void SetInt(int value)
        {
            nType = VarType.Int;
            Data = value;
        }

        public void SetInt64(long value)
        {
             nType = VarType.Int64;
             Data = value;
        }

        public void SetFloat(float value)
        {
             nType = VarType.Float;
             Data = value;
        }

        public void SetDouble(double value)
        {
             nType = VarType.Double;
             Data = value;
        }

        public void SetString(string value)
        {
             nType = VarType.String;
             Data = value;
        }

        public void SetWideStr(string value)
        {
             nType = VarType.WideStr;
             Data = value;
        }

        public void SetObject(ObjectID value)
        {
            if (Data != null && nType == VarType.Object)
            {
                Data = value;
            }
            else
            {
                nType = VarType.Object;
                Data = value.Clone();
             }
        }

        public void SetUserData(byte[] value)
        {
            if (Data != null && nType == VarType.UserData)
            {
                Data = value;
            }
            else
            {
                nType = VarType.UserData;
                Data = value.Clone();
            }
        }
    }

    // 可变类型参数集合
    public class VarList
    {
        public class VarData
        {
            public int nType;
            public object Data;

            public VarData() { }

            public VarData(int type, object data)
            {
                nType = type;
                Data = data;
            }

            public VarData Clone()
            {
                VarData newData = new VarData(nType, null);

                switch(nType)
                {
                    case VarType.Object:
                        newData.Data = ((ObjectID)Data).Clone();
                        break;

                    case VarType.UserData:
                        newData.Data = ((byte[])Data).Clone();
                        break;

                    default:
                        newData.Data = Data;
                        break;
                }

                return newData;
            }
        }

        private List<VarData> m_Values = new List<VarData>();

        public VarList()
        {
        }

        public VarList Clone()
        {
            VarList varList = new VarList();
            varList.Copy(this);
            return varList;
        }

        public void Clear()
        {
            for (int i = 0; i < m_Values.Count; ++i)
            {
                FreeAllocManager.GetInstance.SetVarData(m_Values[i]);
            }
            m_Values.Clear();
        }

        public int GetCount()
        {
            return m_Values.Count;
        }

        public int GetType(int index)
        {
            return m_Values[index].nType;
        }

        public bool GetBool(int index)
        {
            try
            {
                return (bool)m_Values[index].Data;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetInt(int index)
        {
            int type = m_Values[index].nType;

            if (type == VarType.Int64)
            {
                long value = (long)m_Values[index].Data;

                return (int)value;
            }

            try
            {
                return (int)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetInt64(int index)
        {
            int type = m_Values[index].nType;

            if (type == VarType.Int)
            {
                int value = (int)m_Values[index].Data;

                return (long)value;
            }

            try
            {
                return (long)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float GetFloat(int index)
        {
            try
            {
                return (float)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }

        public double GetDouble(int index)
        {
            try
            {
                return (double)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetString(int index)
        {
            try
            {
                return m_Values[index].Data as string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetWideStr(int index)
        {
            try
            {
                return m_Values[index].Data as string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public ObjectID GetObject(int index)
        {
            if (m_Values[index].nType != VarType.Object)
            {
                return null;
            }

            return m_Values[index].Data as ObjectID;
        }

        public byte[] GetUserData(int index)
        {
            if (m_Values[index].nType != VarType.UserData)
            {
                return null;
            }

            return m_Values[index].Data as byte[];
        }

        public void SetBool(int index, bool value)
        {
            m_Values[index].nType = VarType.Bool;
            m_Values[index].Data = value;
        }

        public void SetInt(int index, int value)
        {
            m_Values[index].nType = VarType.Int;
            m_Values[index].Data = value;
        }

        public void SetInt64(int index, long value)
        {
            m_Values[index].nType = VarType.Int64;
            m_Values[index].Data = value;
        }

        public void SetFloat(int index, float value)
        {
            m_Values[index].nType = VarType.Float;
            m_Values[index].Data = value;
        }

        public void SetDouble(int index, double value)
        {
            m_Values[index].nType = VarType.Double;
            m_Values[index].Data = value;
        }

        public void SetString(int index, string value)
        {
            m_Values[index].nType = VarType.String;
            m_Values[index].Data = value;
        }

        public void SetWideStr(int index, string value)
        {
            m_Values[index].nType = VarType.WideStr;
            m_Values[index].Data = value;
        }

        public void SetObject(int index, ObjectID value)
        {
            m_Values[index].nType = VarType.Object;
            m_Values[index].Data = value.Clone();
        }

        public void SetUserData(int index, byte[] value)
        {
            m_Values[index].nType = VarType.UserData;
            m_Values[index].Data = value.Clone();
        }

        public void AddBool(bool value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarBool();
            data.nType = VarType.Bool;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddInt(int value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarInt();
            data.nType = VarType.Int;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddInt64(long value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarInt64();
            data.nType = VarType.Int64;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddFloat(float value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarFloat();
            data.nType = VarType.Float;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddDouble(double value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarDouble();
            data.nType = VarType.Double;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddString(string value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarString();
            data.nType = VarType.String;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddWideStr(string value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarWString();
            data.nType = VarType.WideStr;
            data.Data = value;

            m_Values.Add(data);
        }

        public void AddObject(ObjectID value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarObject();
            data.nType = VarType.Object;
            data.Data = value.Clone();

            m_Values.Add(data);
        }

        public void AddUserData(byte[] value)
        {
            VarData data = FreeAllocManager.GetInstance.GetVarUserData();
            data.nType = VarType.UserData;
            data.Data = value.Clone();

            m_Values.Add(data);
        }

        // 不支持优化，勿用
        public void AddUserData(byte[] value, int start, int size)
        {
            byte[] bytes = new byte[size];

            Array.Copy(value, start, bytes, 0, size);

            m_Values.Add(new VarData(VarType.UserData, bytes));
        }

        public bool Copy(VarList VarData)
        {
            return Copy(VarData, 0, VarData.GetCount());
        }
        public bool Copy(VarList VarData, int start, int Count)
        {
            if (VarData.m_Values == null)
            {
                return false;
            }

            int srcDataCount = VarData.m_Values.Count;

            if (start >= srcDataCount)
            {
                return false;
            }

            int endPos = srcDataCount;
            if (start + Count > srcDataCount)
            {
                endPos = srcDataCount;
            }
            else
            {
                endPos = start + Count;
            }
            

            for (int i = start; i < endPos; i++)
            {
                m_Values.Add(VarData.m_Values[i].Clone());
            }

            return true;
        }

        // 重载符号
        public static VarList operator +(VarList lhs,VarList rhs)
        {
            for (int i = 0; i < rhs.GetCount() && i < 1024; i++)
            {
                int type = rhs.GetType(i);
                switch (type)
                {
                    case VarType.Bool:
                        lhs.AddBool(rhs.GetBool(i));
                        break;
                    case VarType.Int:
                        lhs.AddInt(rhs.GetInt(i));
                        break;
                    case VarType.Int64:
                        lhs.AddInt64(rhs.GetInt64(i));
                        break;
                    case VarType.Float:
                        lhs.AddFloat(rhs.GetFloat(i));
                        break;
                    case VarType.Double:
                        lhs.AddDouble(rhs.GetDouble(i));
                        break;
                    case VarType.String:
                        lhs.AddString(rhs.GetString(i));
                        break;
                    case VarType.WideStr:
                        lhs.AddWideStr(rhs.GetWideStr(i));
                        break;
                    case VarType.Object:
                        lhs.AddObject(rhs.GetObject(i));
                        break;
                    default:
                        break;
                }
            }

            return lhs;
        }

        public static VarList operator +(VarList lhs, int rhs)
        {
            lhs.AddInt(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, bool rhs)
        {
            lhs.AddBool(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, Int64 rhs)
        {
            lhs.AddInt64(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, float rhs)
        {
            lhs.AddFloat(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, double rhs)
        {
            lhs.AddDouble(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, string rhs)
        {
            lhs.AddString(rhs);
            return lhs;
        }

        public static VarList operator +(VarList lhs, ObjectID rhs)
        {
            lhs.AddObject(rhs);
            return lhs;
        }

        public VarList CloneList()
        {
            VarList varList = new VarList();
            varList = (varList + this);
            return varList;
        }
    }
}