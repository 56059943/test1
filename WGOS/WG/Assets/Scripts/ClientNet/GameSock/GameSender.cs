using System;
using System.Collections;
using SysUtils;

using Fm_ClientNet.Interface;
using System.Text;

namespace Fm_ClientNet
{
    public class GameSender : IGameSender
    {
        private UserSock m_sender = null;
        private byte[] m_buffer = new byte[512];
        private IGameReceiver mRecver = null ;
       
        public void SetSocket(ref UserSock sender)
        {
            m_sender = sender;
        }

        public void SetReceiver(IGameReceiver gamerecver)
        {
            mRecver = gamerecver;
        }
        public bool ClientReady()
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_READY);//消息ID
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }


        public bool GetVerify()
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_GET_VERIFY);//消息ID
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool Speech(string info)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_SPEECH);//消息ID
            ar.WriteWideStrNoLen(info);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool GetWorldInfo(int type)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_WORLD_INFO);//消息ID
            ar.WriteInt32(type);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        public bool ChooseRole(string role_name)
        {
            if (role_name == null)
            {
                //Log.TraceError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                //Log.Trace("sendChooseRole packet role name is empty!");
                return false;
            }

            //byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            //Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_CHOOSE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称
            //unsigned char nVerify[4];							// 校验码
            //char strInfo[1];	

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名

            //byte[] verify = new byte[4];
            //校验码4个字节,对消息加密获取验证码

            byte[] data = new byte[256];
            int index = 0;

            //序号，名字加入校验
            MsgEncode.WriteInt32(ref data, ref index, (uint)MsgEncode.Instance.Serial);
            MsgEncode.WriteUnicodeLen(ref data, ref index, role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);

            byte[] verify = MsgEncode.Instance.Encode(data, index, 4);   

            ar.WriteUserDataNoLen(verify);//检验码

            ar.WriteInt8(0);//附加信息

            MsgEncode.Instance.MsgSerial++;

            return m_sender.Send(ar.GetData(), ar.GetLength());

        }

        public bool ReviveRole(string role_name)
        {
            if (role_name == null)
            {
                //Log.TraceError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                //Log.Trace("ReviveRole  role name is empty!");
                return false;
            }


            //byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            //Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_REVIVE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool DeleteRole(string role_name)
        {
            if (role_name == null)
            {
                //Log.TraceError("Role Name Is Empty!");
                return false;
            }

            if (0 == role_name.Length)
            {
                //Log.Trace("DeleteRole  role name is empty!");
                return false;
            }


            //byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
            //Array.Copy(System.Text.Encoding.Default.GetBytes(role_name), name, role_name.Length);

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_DELETE_ROLE);//消息ID

            //unsigned short wsName[OUTER_OBJNAME_LENGTH + 1];	// 名称

            ar.WriteUnicodeLen(role_name, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);//玩家名
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool Select(string object_ident, int func_id)
        {
            if (object_ident == null)
            {
                //Log.TraceError("GameSender Select object_ident is null");
                return false;
            }

            ObjectID objID = ObjectID.FromString(object_ident);

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_SELECT);//消息ID
            //unsigned char nVerify[4];	// 校验码
            //int nSerial;				// 消息序列号	
            //outer_object_t ObjectId;	// 对象标识
            //int nFunctionId;			// 功能号为0表示执行缺省功能
            //byte[] message = TypeConvert.encode(m_buffer,ar.GetLength());

            byte[] data = new byte[256];
            int index = 0;

            //序号，名字加入校验
            MsgEncode.WriteInt16(ref data, ref index, (uint)MsgEncode.Instance.Serial);
            MsgEncode.WriteInt16(ref data, ref index, (uint)MsgEncode.Instance.MsgSerial);
            MsgEncode.WriteInt32(ref data, ref index, (uint)func_id);
            MsgEncode.WriteObject(ref data, ref index, objID);

            byte[] verify = MsgEncode.Instance.Encode(data, index, 4);   

            ar.WriteUserDataNoLen(verify);//检验码
            ar.WriteInt32(MsgEncode.Instance.MsgSerial);//消息序列号
            ar.WriteObject(objID);//对象标识
            ar.WriteInt32(func_id);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool GetWorldInfo2(int type, string info)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_WORLD_INFO);//消息ID
            ar.WriteInt32(type);
            ar.WriteWideStr(info);
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool LoginByString(string account, string login_string)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(MsgEncode.Instance.Version);//版本号
            byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);//验证码
            ar.WriteString(account);//账号
            ar.WriteString("");//密码
            ar.WriteString(login_string);//登录串
            ar.WriteInt32(2);//登录类型
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        public bool Login(string account, string password, string device_uid)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID

            ar.WriteInt32(MsgEncode.Instance.Version);//版本号
            byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);

            ar.WriteUserData(MsgEncode.Instance.EncodeString(account));//账号
            ar.WriteUserData(MsgEncode.Instance.EncodeString(password));//密码

            ar.WriteString(device_uid);//设备uiid
            ar.WriteString("0");//登录串
            ar.WriteInt32(1);//登录类型
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        /// <summary>
        /// 断线重连接
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="pswd">密码</param>
        /// <param name="validate_string">计费串</param>
        /// <param name="device_uid">设备号</param>
        /// <param name="in_stub">是否是本地副本（false）</param>
        /// <returns></returns>
        public bool LoginReconnect(string account, string pswd, string validate_string, string device_uid, bool in_stub = false)
        {
            if (account == null || pswd == null || validate_string == null || device_uid == null)
                return false;

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(MsgEncode.Instance.Version);//版本号

            byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);//验证码
            ar.WriteUserData(MsgEncode.Instance.EncodeString(account));           //账号
            ar.WriteUserData(MsgEncode.Instance.EncodeString(pswd));               //密码
            ar.WriteString(validate_string); //登录串
            ar.WriteInt32(100);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            ar.WriteString(device_uid);

            string strPropMd5 = null;
            string strRecdMd5 = null;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }

            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString("");
            }

            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString("");
            }

            if (in_stub)
            {
                ar.WriteInt8(1);
            }
            else
            {
                ar.WriteInt8(0);
            }
    
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }
        public bool CreateRole(ref VarList args)
        {
            try
            {
                if ((args.GetCount() < 2) || (args.GetType(1) != VarType.WideStr))
                {
                    //Log.Trace("arguments error");
                    return false;
                }

                string roleName = args.GetWideStr(1);
                //role name
                //byte[] name = new byte[ServerInfo.ROLENAME_MAX_LENGTH * 2 + 1];
                //Array.Copy(System.Text.Encoding.Default.GetBytes(roleName), name, roleName.Length);

                //verify
                byte[] verify = new byte[4];

                //校验码4个字节,对消息加密获取验证码
                byte[] varify = MsgEncode.Instance.Encode(ref args, 4);              
                
                StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
                ar.WriteInt8(GlobalClineMsgId.CLIENT_CREATE_ROLE);
                ar.WriteInt32(args.GetInt(0));
                ar.WriteUnicodeLen(roleName, (ServerInfo.ROLENAME_MAX_LENGTH + 1) * 2);
                ar.WriteInt16(1);
                ar.WriteInt8(2);
                ar.WriteInt32(args.GetInt(2));
                //ar.WriteUserDataNoLen(verify);
                ar.WriteUserDataNoLen(varify);
                ar.WriteInt8(0);
                return m_sender.Send(ar.GetData(), ar.GetLength());
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
        }

        public bool LoginByShield(string account, string password, string login_string, string device_uid)
        {
            if (account == null || password == null || login_string == null || device_uid == null)
                return false;

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(MsgEncode.Instance.Version);//版本号

            byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);//验证码
            ar.WriteString(account);           //账号
            ar.WriteString(password);               //密码
            ar.WriteString(login_string); //登录串
            ar.WriteInt32(100);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            ar.WriteString(device_uid);

            string strPropMd5 = null;
            string strRecdMd5 = null;
            if (mRecver != null)
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }
            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString("");
            }
            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString("");
            }

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        //发送带验证码的登录消息
        public bool LoginVerify(string account, string password, string verify, string device_uid)
        {
            if( account == null || password == null || verify == null || device_uid == null )
                 return false;

            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_LOGIN);//消息ID
            ar.WriteInt32(MsgEncode.Instance.Version);//版本号

            byte[] varify = Encoding.Default.GetBytes(verify);
            ar.WriteUserDataNoLen(varify);//验证码
            ar.WriteString(account);           //账号
            ar.WriteString(password);               //密码
            ar.WriteString(""); //登录串
            ar.WriteInt32(100);                     //登录类型 LOGIN_TYPE_RECONNECT 100
            ar.WriteString(device_uid);

            string strPropMd5 = null;
            string strRecdMd5 = null;
            if ( mRecver != null )
            {
                strPropMd5 = mRecver.GetPropertyTableMd5();
                strRecdMd5 = mRecver.GetRecordTableMd5();
            }
            if (strPropMd5 != null)
            {
                ar.WriteString(strPropMd5);
            }
            else
            {
                ar.WriteString("");
            }
            if (strRecdMd5 != null)
            {
                ar.WriteString(strRecdMd5);
            }
            else
            {
                ar.WriteString("");
            }
   
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        //发送请求移动消息
        //public bool RequestMove(int mode, int arg_num, float[] args, string info)
        public bool RequestMove(ref VarList args, ref VarList ret)
        {
            try
            {
                if (args.GetCount() < 3)
                {
                    //Log.Trace(" arguments count must be > 3");
                    ret.AddBool(false);
                    return false;
                }

                int mode = args.GetInt(0);
                int arg_num = args.GetInt(1);
                if (arg_num > 256)
                {
                    //Log.Trace("more arguments");
                    ret.AddBool(false);
                    return false;
                }

                if (args.GetCount() < (arg_num + 2))
                {
                    //Log.Trace("too few arguments");
                    ret.AddBool(false);
                    return false;
                }

                StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
                ar.WriteInt8(GlobalClineMsgId.CLIENT_REQUEST_MOVE);//消息ID
                ar.WriteInt8(mode);
                ar.WriteInt16(arg_num);

                for (int i = 0; i < arg_num; i++)
                {
                    float value = args.GetFloat(2 + i);
                    ar.WriteFloat(value);
                }

                string info = "";
                if (args.GetCount() > (arg_num + 2))
                {
                    info = args.GetString(arg_num + 2);
                }

                ar.WriteStringNoLen(info);
                return m_sender.Send(ar.GetData(), ar.GetLength());
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
        }

        public bool Custom(ref VarList args)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_CUSTOM);//消息ID
            
            //校验码4个字节,对消息加密获取验证码
            byte[] varify = MsgEncode.Instance.Encode(ref args, 4);
            ar.WriteUserDataNoLen(varify);
            ar.WriteInt16(MsgEncode.Instance.MsgSerial);
            ar.WriteInt16(args.GetCount());
            Log.Trace("Custom");
            if (!AddMsgVarList(ref ar, ref args, 0, args.GetCount()))
            {
                Log.Trace("add para error");
                return false;
            }
            Log.Trace("Send");

            MsgEncode.Instance.MsgSerial++;

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }


        public bool  CheckVersion()
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_CHECK_VERSION);//消息ID
         
            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        //确认加密码改变
        public bool RetEncode(int serial,string info, int address)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_RET_ENCODE);//消息ID

            byte[] b = new byte[32];
            b = System.Text.Encoding.ASCII.GetBytes(info);

            ar.WriteUserDataNoLen(b);
            ar.WriteInt32(address);

            byte[] data = new byte[256];
            int index = 0;

            MsgEncode.WriteInt32(ref data, ref index, (uint)serial);
            MsgEncode.WriteInt32(ref data, ref index, (uint)address);

            //32位固定长度
            Array.Copy(b,0,data,index,b.Length);
            index+=b.Length;
            
            //校验码4个字节,对消息加密获取验证码
            byte[] varify = MsgEncode.Instance.Encode(data,index,4);

            //byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);

            MsgEncode.Instance.Serial = (uint)serial;
         
            return m_sender.Send(ar.GetData(), ar.GetLength());
            
        }

        public bool LoginPlatform(int type,string json)
        {
            StoreArchive ar = new StoreArchive(m_buffer, m_buffer.Length);
            ar.WriteInt8(GlobalClineMsgId.CLIENT_SEND_TO_LOGIN_MSG);//消息ID
            ar.WriteInt32(MsgEncode.Instance.Version);//版本号
            byte[] varify = new byte[4];
            ar.WriteUserDataNoLen(varify);//验证码
            ar.WriteInt8(type);           //消息类型
            ar.WriteString(json);

            return m_sender.Send(ar.GetData(), ar.GetLength());
        }

        bool AddMsgVarList(ref StoreArchive storeAr, ref VarList args,
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
                                storeAr.WriteInt8(VarType.Int);
                                storeAr.WriteInt32(args.GetInt(i));
                            }
                            break;
                        case VarType.Int64:
                            {
                                storeAr.WriteInt8(VarType.Int64);
                                storeAr.WriteInt64(args.GetInt64(i));
                            }
                            break;
                        case VarType.Float:
                            {
                                storeAr.WriteInt8(VarType.Float);
                                storeAr.WriteFloat(args.GetFloat(i));
                            }
                            break;
                        case VarType.Double:
                            {
                                storeAr.WriteInt8(VarType.Double);
                                storeAr.WriteDouble(args.GetDouble(i));
                            }
                            break;
                        case VarType.String:
                            {
                                storeAr.WriteInt8(VarType.String);
                                storeAr.WriteString(args.GetString(i));
                            }
                            break;
                        case VarType.WideStr:
                            {
                                storeAr.WriteInt8(VarType.WideStr);
                                storeAr.WriteWideStr(args.GetWideStr(i));
                            }
                            break;
                        case VarType.Object:
                            {
                                storeAr.WriteInt8(VarType.Object);
                                storeAr.WriteObject(args.GetObject(i));
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
    }
}

