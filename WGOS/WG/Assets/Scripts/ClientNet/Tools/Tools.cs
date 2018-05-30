using System.Collections;
using SysUtils;
using System.IO;
using System;
using System.Reflection;
using System.Diagnostics;


// 	 OUTER_TYPE_UNKNOWN  = 0;	// 未知
//	 OUTER_TYPE_BYTE     = 1;   // 一字节 //ReadInt8
//	 OUTER_TYPE_WORD     = 2;   // 二字节 //ReadInt16 或ReadUInt16
//	 OUTER_TYPE_DWORD    = 3;   // 四字节  //ReadUInt32 或ReadInt32
//	 OUTER_TYPE_QWORD    = 4;	// 八字节  //ReadUInt64 或ReadInt64
//	 OUTER_TYPE_FLOAT    = 5;	// 浮点四字节 //ReadFloat
//	 OUTER_TYPE_DOUBLE   = 6;	// 浮点八字节 // ReadDouble
//	 OUTER_TYPE_STRING   = 7;	// 字符串，前四个字节为长度 //ReadString
//	 OUTER_TYPE_WIDESTR  = 8;	// UNICODE宽字符串，前四个字节为长度 //ReadWideStr
//	 OUTER_TYPE_OBJECT   = 9;	// 对象号objectId


namespace Fm_ClientNet
{
    //工具类
    public class Tools
    {
        const float FLT_EPSILON = 1.192092896e-07F;
        const double DBL_EPSILON = 2.2204460492503131e-016;


        //读取数据
        public static bool ReadData(ref LoadArchive loadAr, int nDataType, ref object obj)
        {
            switch (nDataType)
            {
                case OuterDataType.OUTER_TYPE_UNKNOWN:
                    {
                        //Log.TraceError("Tools::ReadData nDataType error");
                        return false;
                    }
                // break;
                case OuterDataType.OUTER_TYPE_BYTE:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt8(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_BYTE error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt16(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_WORD error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DWORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt32(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_DWORD error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_QWORD:
                    {
                        long value = 0;
                        if (!loadAr.ReadInt64(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_QWORD error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_FLOAT:
                    {
                        float value = 0.0f;
                        if (!loadAr.ReadFloat(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_FLOAT error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DOUBLE:
                    {
                        double value = 0.0f;
                        if (!loadAr.ReadDouble(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_DOUBLE error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_STRING:
                    {
                        string value = "";
                        if (!loadAr.ReadString(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_STRING error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WIDESTR:
                    {
                        string value = "";
                        if (!loadAr.ReadWideStr(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_WIDESTR error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + value + "]");
                    }
                    break;
                case OuterDataType.OUTER_TYPE_OBJECT:
                    {
                        ObjectID value = new ObjectID();
                        if (!loadAr.ReadObject(ref value))
                        {
                            //Log.TraceError("Read OUTER_TYPE_WIDESTR error");
                        }
                        //				//Log.Trace("Read prop name = [" + propItem.getPropName() + "]"
                        //				+ "prop value = [" + objId.m_nIdent + "." + objId.m_nSerial+"]");
                    }
                    break;
                default:
                    //Log.TraceError("unkwon data type");
                    return false;
            }
            return true;
        }


        //public static ObjectID genObjectId(string name)
        //{
        //    if (name == null || name.Length == 0)
        //    {
        //        return new ObjectID();
        //    }

        //    uint hashValue = GetHashValueCase(name);

        //    ObjectID objId = new ObjectID(hashValue, hashValue);
        //    return objId;
        //}

        public static uint GetHashValueCase(ObjectID name)
        {
            if (name.IsNull())
            {
                return 0;
            }

            uint hashValue = 0;
            string strName = name.ToString();
            for (int i = 0; i < strName.Length; i++)
            {
                hashValue = hashValue * 5 + strName[i];
            }

            return hashValue;

        }


        public static bool FloatEqual(float v1, float v2)
        {
            return (v1 < (v2 + FLT_EPSILON)) && (v1 > (v2 - FLT_EPSILON));
        }

        public static bool DoubleEqual(double v1, double v2)
        {
            return (v1 < (v2 + DBL_EPSILON)) && (v1 > (v2 - DBL_EPSILON));
        }

        public static string GetIdent(ObjectID obj)
        {
            return obj.m_nIdent.ToString() + "-" + obj.m_nSerial.ToString();
        }

    }

    //日志类，供后期替换
    public class Log
    {
        public delegate void on_log(string str);
        private static on_log m_LogFunc = TraceFile;
        private static StreamWriter m_FileLog = null; // = new FileStream();

        public static void CloseFile()
        {
            if (m_FileLog != null)
            {
                try
                {
                    m_FileLog.Flush();
                    m_FileLog.Close();
                    m_FileLog = null;
                }
                catch (Exception)
                {
                }
            }
        }

        public static void SetLogCallBack(on_log funname)
        {
            m_LogFunc = funname;
        }

        private static void InnerTrace(string str, string func)
        {
            try
            {
                if (m_LogFunc == null) return;
                StackTrace trace = new StackTrace();
                string outStr = trace.GetFrame(1).GetMethod().ReflectedType.Name;
                outStr += trace.GetFrame(1).GetMethod().Name;
                outStr += func;
                outStr += str;

                m_LogFunc(outStr);
            }
            catch (Exception)
            {
                //没有设置回调
                //TraceFile("Error,Log.Trace Exception:" + ex.ToString());

                //此处屏蔽 防止函数再抛异常 将导致异常无法捕获
                /*if (null == m_LogFunc)
                {
                    TraceFile("Error,Log.Trace Exception:" + ex.ToString());
                }
                else
                {
                    m_LogFunc("Error,Log.Trace Exception:" + ex.ToString());
                }*/
            }
        }

        public static void Trace(string str)
        {
            InnerTrace(str, ":");
        }

        public static void TraceError(string str)
        {
            InnerTrace(str, "Error:");
        }

        public static void TraceWarning(string str)
        {
            InnerTrace(str, "Warning:");
        }

        public static void TraceExcep(ref Exception exin)
        {
            InnerTrace(exin.ToString(), "Exception:");
        }
        public static void TraceSelf(string str)
        {
            InnerTrace(str, "Self:");
        }

        public static void TraceFile(string str)
        {
            if (null == m_FileLog)
            {
                m_FileLog = new StreamWriter("Error.log", false);
            }
            //bool bres = m_FileLog.CanWrite;
            //StreamWriter sw = new StreamWriter(m_FileLog);
            m_FileLog.WriteLine(str);
            m_FileLog.Flush();
            //sw.Close();
            //sw.Dispose();
        }
    }

}
