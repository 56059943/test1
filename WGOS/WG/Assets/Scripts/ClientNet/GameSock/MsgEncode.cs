using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SysUtils;

namespace Fm_ClientNet
{
    public class MsgEncode
    {
        //连接标示号
        private UInt32 nSerial = 0;
        //序号给两个个字节，轮回
        private UInt16 nMsgSerial = 0;

        //当前的加密分组
        private UInt16 nEncode = 0;
        //加密包版本
        private Int32 nVersion = 0;
        //密码本是否就绪
        private bool nReady = false;

        const int MAX_CODE = 2048;
        const int CODE_LEN = 4;

        private byte[] mEncodeArr = null;

        public byte[] MEncodeArr
        {
            get { return mEncodeArr; }
            set
            {
                mEncodeArr = value;

                nReady = true;

                //前4个字节是加密包版本库
                nVersion = System.BitConverter.ToInt32(mEncodeArr, 0); 

                //设置加密码和解密码，客户端 第2个是解密码 第1个是加密码
                m_decodeKey = GetEncodeKey(1, sizeof(Int32));
                m_encodeKey = GetEncodeKey(0, sizeof(Int32));  
            }
        }

        private byte[] m_buffer = new byte[512];

        private MD5 md5 = null;

        //加密key
        byte[]  m_encodeKey     = new byte[4];       

        //解密key
        byte[]  m_decodeKey     = new byte[4];

        //静态指针
        static MsgEncode instance = null;

        static public MsgEncode Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MsgEncode();
                }
                return instance;
            }
        }


        //连接标示号
        public UInt32 Serial
        {
            set
            {
                nSerial = value;
                nMsgSerial = (UInt16)(nSerial + 1);
            }
            get
            {
                return nSerial;
            }
        }

        //返回当前消息序列
        public UInt16 MsgSerial
        {
            set
            {
                nMsgSerial = value;
            }
            get
            {
                return nMsgSerial;
            }
        }


        //密码本是否就绪(编码||不编码)
        public bool Ready
        {
            get
            {
                return nReady;
            }
        }

        //密码本版本号
        public Int32 Version
        {
            get
            {
                return nVersion;
            }
        }

        public void Init() { }
        //构造
        public MsgEncode()
        {
            md5 = MD5.Create();

            //申请加密码空间
            mEncodeArr = new byte[MAX_CODE * CODE_LEN];
        }       


        //加密消息，获得校验码
        public byte[] Encode(ref VarList args,int size)
        {
            if(!Ready)
            {
                return new byte[size];
            }
            byte[] data = new byte[256];
            int index = 0;
            //服务器序号
            WriteInt32(ref data, ref index, nSerial);
            //客户端序号   
            WriteInt16(ref data, ref index, nMsgSerial);              
            //参数数目
            WriteInt16(ref data, ref index, (uint)args.GetCount());

            int requireLen = MsgVarListLen(ref args, 0, args.GetCount());

            byte[] info = new byte[requireLen + 1];

            int arg_len = 0;
            if (!AddMsgVarList(ref info, ref arg_len, ref args, 0, args.GetCount()))
            {
                Log.Trace("add para error");
                return null;
            }

            if (arg_len <= 8)
            {
                //小于等于8个字节，全部参与计算
                Array.Copy(info, 0, data, index, arg_len);
                index += arg_len;
            }
            else
            {
                //用前4个和后4个字节
                Array.Copy(info, 0, data, index, 4);
                index += 4;
                Array.Copy(info, arg_len - 4,data, index, 4);
                index += 4;
            }

            return Encode(data,index, size);
        }

        //打印byte[]
        private void Output(byte[] data, string context, int size = 0)
        {
            //string info = "";
            //int len = data.Length;
            //if(size != 0)
            //{
            //    len = size;
            //}

            //for (int i = 0; i < len; i++)
            //{
            //    int v = data[i];
            //    info += " ";
            //    info += v.ToString("x");
            //}
            //LogSystem.LogError(context + info);
        }

        //加密消息，获得校验码
        public byte[] Encode(byte[] data, int data_size, int ret_size)
        {
            byte[] varify = new byte[ret_size];

            if (!nReady)
            {
                return varify;
            }

            Output(data, "before encode 1",data_size);
            
            //先MD5
            byte[] value = md5.ComputeHash(data, 0, data_size);

            Output(value, "after encode 1");

            //获取加密码
            byte[] key = GetEncodeKey(nEncode, CODE_LEN);

            Output(key, "encode key");

            //再按组异或
            int group = value.Length / key.Length;
            for (int i = 0; i < group; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    value[i * key.Length + j] ^= key[j];
                }
            }

            Output(value, "after xor");

            //再加密一次
            byte[] result = md5.ComputeHash(value, 0, value.Length);

            Output(result, "after encode 2");

            Buffer.BlockCopy(result, 0, varify, 0, ret_size);

            return varify;
        }


        //获取加密key
        public byte[] GetEncodeKey(int encodeID,int size)
        {
            byte[] key = new byte[size];

            Buffer.BlockCopy(mEncodeArr, CODE_LEN * encodeID + sizeof(Int32), key, 0, size);

            return key;
        }
        
        //写入int
        static public void WriteInt8(ref byte[] data, ref int index, uint value)
        {
            RequireLength(ref data,ref index,1);

            data[index] = (byte)(value & 0xFF);
            index++;
        }

        static public void WriteInt16(ref byte[] data, ref int index, uint value)
        {
            RequireLength(ref data,ref index,2);

            byte[] bValue = BitConverter.GetBytes((short)value);
            Array.Copy(bValue, 0, data, index, bValue.Length);
            index += bValue.Length;
        }

        static public void WriteInt32(ref byte[] data, ref int index, uint value)
        {
            RequireLength(ref data, ref index, 4);
            byte[] bValue = BitConverter.GetBytes(value);
            Array.Copy(bValue, 0, data, index, bValue.Length);
            index += bValue.Length;
        }

        static public void WriteInt64(ref byte[] data, ref int index, long value)
        {
            RequireLength(ref data, ref index, 8);
            byte[] bValue = BitConverter.GetBytes(value);
            Array.Copy(bValue, 0, data, index, bValue.Length);
            index += bValue.Length;
        }

        static public void WriteFloat(ref byte[] data, ref int index, float value)
        {
            RequireLength(ref data, ref index, 4);
            byte[] bValue = BitConverter.GetBytes(value);
            Array.Copy(bValue, 0, data, index, bValue.Length);
            index += bValue.Length;
        }

        static public void WriteDouble(ref byte[] data, ref int index, double value)
        {
            RequireLength(ref data, ref index, 4);
            byte[] bValue = BitConverter.GetBytes(value);
            Array.Copy(bValue, 0, data, index, bValue.Length);
            index += bValue.Length;
        }

        static public void WriteString(ref byte[] data, ref int index, string value)
        {
            // 包含结束符
            uint len = (uint)Encoding.Default.GetByteCount(value) + 1;

            RequireLength(ref data, ref index, (int)len);

            data[index] = (byte)(len & 0xFF);
            data[index + 1] = (byte)((len >> 8) & 0xFF);
            data[index + 2] = (byte)((len >> 16) & 0xFF);
            data[index + 3] = (byte)((len >> 24) & 0xFF);

            index += 4;

            try
            {
                Encoding.Default.GetBytes(value).CopyTo(data, index);
            }
            catch (Exception)
            {
                return;
            }

            // 结束符
            data[index + len - 1] = 0;

            index += (int)len;
        }

        static public void WriteWideStr(ref byte[] data, ref int index, string value)
        {
            // 包含结束符
            int len = (value.Length + 1) * 2;
            RequireLength(ref data, ref index, (int)len);

            data[index] = (byte)(len & 0xFF);
            data[index + 1] = (byte)((len >> 8) & 0xFF);
            data[index + 2] = (byte)((len >> 16) & 0xFF);
            data[index + 3] = (byte)((len >> 24) & 0xFF);

            index += 4;

            WriteUnicodeLen(ref data, ref index, value, len);      
        }

        static public void WriteObject(ref byte[] data, ref int index, ObjectID value)
        {
            WriteInt32(ref data, ref index, value.m_nIdent);
            WriteInt32(ref data, ref index, value.m_nSerial);
        }

        // 写入不超过指定长度的宽字符串
        static public bool WriteUnicodeLen(ref byte[] data, ref int index, string val, int len)
        {
            // 截断
            int val_len = val.Length;

            if (val_len >= (len / 2))
            {
                val_len = (len / 2) - 1;
            }

            for (int c = 0; c < len; c++)
            {
                data[index + c] = 0;
            }

            for (int c = 0; c < val_len; c++)
            {
                BitConverter.GetBytes(val[c]).CopyTo(data,
                    index + c * 2);
            }

            index += len;

            return true;
        }

        // 申请必要的写入空间
        static private bool RequireLength(ref byte[] data, ref int index,int length)
        {
            int nLength = data.Length;

            int need_len = index + length;

            if (need_len <= nLength)
            {
                return true;
            }

            int new_len = nLength * 2;

            if (new_len < need_len)
            {
                new_len = need_len;
            }

            byte[] new_buf = new byte[new_len];

            Array.Copy(data, new_buf, nLength);

            data = new_buf;

            return true;
        }


        bool AddMsgVarList(ref byte[] data, ref int index, ref VarList args,
           int beg, int end)
        {
            try
            {
                for (int i = beg; i < end; i++)
                {
                    switch (args.GetType(i))
                    {
                        case VarType.Int:
                            {
                                WriteInt8(ref data,ref index,VarType.Int);
                                WriteInt32(ref data, ref index, (uint)args.GetInt(i));
                            }
                            break;
                        case VarType.Int64:
                            {
                                WriteInt8(ref data,ref index,VarType.Int64);
                                WriteInt64(ref data, ref index, args.GetInt64(i));
                            }
                            break;
                        case VarType.Float:
                            {
                                WriteInt8(ref data, ref index, VarType.Float);
                                WriteFloat(ref data, ref index, args.GetFloat(i));
                            }
                            break;
                        case VarType.Double:
                            {
                                WriteInt8(ref data, ref index, VarType.Double);
                                WriteDouble(ref data, ref index, args.GetDouble(i));
                            }
                            break;
                        case VarType.String:
                            {
                                WriteInt8(ref data, ref index, VarType.String);
                                WriteString(ref data, ref index, args.GetString(i));
                            }
                            break;
                        case VarType.WideStr:
                            {
                                WriteInt8(ref data, ref index, VarType.WideStr);
                                WriteWideStr(ref data, ref index, args.GetWideStr(i));
                            }
                            break;
                        case VarType.Object:
                            {
                                WriteInt8(ref data, ref index, VarType.Object);
                                WriteObject(ref data, ref index, args.GetObject(i));
                            }
                            break;
                        default:
                            //Log.Trace("unkown data type");
                            break;
                    }//end switch
                }//end for

            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return false;
            }//end try catch
            return true;
        }
        
        int MsgVarListLen(ref VarList args,
         int beg, int end)
        {
            int len = 0;
            try
            {
                for (int i = beg; i < end; i++)
                {
                    switch (args.GetType(i))
                    {
                        case VarType.Int:
                            {
                                len += 1;
                                len += 4;
                            }
                            break;
                        case VarType.Int64:
                            {
                                len += 1;
                                len += 8;
                            }
                            break;
                        case VarType.Float:
                            {
                                len += 1;
                                len += 4;
                            }
                            break;
                        case VarType.Double:
                            {
                                len += 1;
                                len += 4;
                            }
                            break;
                        case VarType.String:
                            {
                                len += 1;
                                len += 4;
                                len += (Encoding.Default.GetByteCount(args.GetString(i)) + 1) * 2;
                            }
                            break;
                        case VarType.WideStr:
                            {
                                len += 1;
                                len += 4;
                                len += (Encoding.Default.GetByteCount(args.GetWideStr(i)) + 1) * 2;
                            }
                            break;
                        case VarType.Object:
                            {
                                len += 1;
                                len += 8;
                            }
                            break;
                        default:
                            //Log.Trace("unkown data type");
                            break;
                    }//end switch
                }//end for

            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return len;
            }//end try catch
            return len;
        }


        //解码
        public void Decode(byte[] src, int size)
        {
            int index = 0;
            for (int i = 0; i < size; ++i)
            {
                index = i % 4;
                src[i] ^= m_decodeKey[index];
            }
        }

        //编码
        public int Encode(byte[] dst, byte[] src, int size)
        {
            byte ch = 0;	      
            int dstIndex = 0;      
            int index = 0;

		    for (int i = 0; i < size; i++)
		    {
                index = i % 4;                        
			    ch = Convert.ToByte(src[i] ^ m_encodeKey[index]);
			    dst[dstIndex] = ch;
                dstIndex += (1 + Convert.ToInt32(Const.MESSAGE_END_SUFFIX == ch));
			}

            return dstIndex;  
        }

        //string转byte
        public byte[] Str2Byte(string str)
        {
            byte[] data = new byte[str.Length + 1];
            int iCount = str.Length;
            
            for(int i = 0;i < iCount; i++)
            {
                data[i] = (byte)str[i];
            }
            return data;
        }

        //加密字符串
        public byte[] EncodeString(string str)
        {
            byte[] byteArray = Str2Byte(str);
            byte[] outArray = new byte[byteArray.Length];
            Encode(outArray, byteArray, byteArray.Length);

            return outArray;
        }

        //设置序号和解密分组
        public void SetEncodeId(uint serial, uint encodeId,string info,int address)
        {
            nSerial = (UInt16)serial;
            nEncode = (UInt16)encodeId;

            //向服务器确认加密改变
            ClientNet.Instance().GetGameSock().GetGameSender().RetEncode((int)serial, info, address);
        }
    }
}
