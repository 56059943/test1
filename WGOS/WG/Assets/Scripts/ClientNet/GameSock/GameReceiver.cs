using System.Collections.Generic;
using SysUtils;
using System;

using Fm_ClientNet.Interface;
using System.Security.Cryptography;

namespace Fm_ClientNet
{
    public partial class GameReceiver : IGameReceiver
    {
        public delegate int on_event(ObjectID ent_ident, VarList args);

        private Dictionary<int, RoleData> mRoles = null;
        private Dictionary<int, PropData> mPropertyTable = null;
        private Dictionary<int, RecData> mRecordTable = null;
        private Dictionary<int, MenuData> mMenus = null;
        private Dictionary<string, CallBack> m_CallBacks = new Dictionary<string, CallBack>();


        private SocketRevMsgHandle m_RecvHandle = new SocketRevMsgHandle();
        private GameClient m_client = null;

        private string m_strPropertyTableMd5;
        private string m_strRecordTableMd5;
        public void SetPropertyTableMd5(byte[] data)
        {
            MD5 md5 = MD5.Create();
            byte[] md5data = md5.ComputeHash(data);
            m_strPropertyTableMd5  = System.Text.Encoding.Default.GetString(md5data);
        }
        public string GetPropertyTableMd5()
        {
            return m_strPropertyTableMd5;
        }

        public void SetRecordTableMd5( byte[] data)
        {
            MD5 md5 = MD5.Create();
            byte[] md5data = md5.ComputeHash(data);
            m_strRecordTableMd5= System.Text.Encoding.Default.GetString(md5data);
        }

        public string GetRecordTableMd5()
        {
            return m_strRecordTableMd5;
        }
        private int m_nMaxCustomArguments = 64;

        public GameReceiver()
        {
            mRoles = new Dictionary<int, RoleData>();
            mPropertyTable = new Dictionary<int, PropData>();
            mRecordTable = new Dictionary<int, RecData>();
            mMenus = new Dictionary<int, MenuData>();
            m_RecvHandle.GetRecvPacket().SetGameReceiver(this);
        }

        public void SetGameClient(GameClient client)
        {
            m_client = client;
            m_RecvHandle.GetRecvPacket().SetGameClient(client);
        }

        public void SetGameMsgHander(IGameMsgHander msgHandle)
        {
            m_RecvHandle.GetRecvPacket().SetMsgHandle(msgHandle);
        }

        public IGameMsgHander GetGameMsgHandler()
        {
            return m_RecvHandle.GetRecvPacket().GetMsgHandle();
        }

        // 自定义消息处理器对象
        public void SetCustomHandler(ICustomHandler msgHandle)
        {
            m_RecvHandle.GetRecvPacket().SetCustomHandle(msgHandle);
        }

        public ICustomHandler GetCustomHandler()
        {
            return m_RecvHandle.GetRecvPacket().GetCustomHandle();
        }

        // 自定义消息的最大参数数量
        public void SetMaxCustomArguments(int value)
        {
            m_nMaxCustomArguments = value;
        }

        public int GetMaxCustomArguments()
        {
            return m_nMaxCustomArguments;
        }

        public void ClearRoles()
        {
            if (mRoles != null)
            {
                mRoles.Clear();
            }
        }

        public void ClearPropertyTable()
        {
            if (mPropertyTable != null)
            {
                mPropertyTable.Clear();
            }
        }

        public void ClearRecordTable()
        {
            if (mRecordTable != null)
            {
                mRecordTable.Clear();
            }
        }

        public void AddRoleData(int index, ref RoleData roleData)
        {
            if (mRoles == null)
            {
                //Log.Trace("GameReceiver.AddRoleData mRoles is null");
                return;
            }

            try
            {
                if (mRoles.ContainsKey(index))
                {
                    //Log.Trace("GameReceiver.AddRoleData Has Same Index:" + index);
                    return;
                }

                mRoles.Add(index, roleData);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.AddRoleData Exception:" + ex.ToString());
                return;
            }
        }

        public void AddPropData(int index, ref PropData propData)
        {
            if (mPropertyTable == null)
            {
                //Log.Trace("GameReceiver.AddPropData mPropertyTable is null");
                return;
            }

            try
            {
                if (mPropertyTable.ContainsKey(index))
                {
                    //Log.Trace("GameReceiver.AddPropData Has Same Index:" + index);
                    return;
                }

                mPropertyTable.Add(index, propData);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.AddPropData Exception:" + ex.ToString());
                return;
            }
        }

        public void AddRecData(int index, ref RecData recData)
        {
            if (mRecordTable == null)
            {
                //Log.Trace("GameReceiver.AddRecData mPropertyTable is null");
                return;
            }

            try
            {
                if (mRecordTable.ContainsKey(index))
                {
                    //Log.Trace("GameReceiver.AddRecData Has Same Index:" + index);
                    return;
                }

                mRecordTable.Add(index, recData);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.AddRecData Exception:" + ex.ToString());
                return;
            }
        }


        //回调事件注册
        public bool RegistCallBack(string funcName, CallBack callBack)
        {
            if (CustomSystem.Instance != null)
            {
                CustomSystem.Instance.RegistCallBack(funcName, callBack);
            }

            if (funcName == null || funcName.Length == 0 || (callBack == null))
            {
                return false;
            }

            try
            {
                if (m_CallBacks.ContainsKey(funcName))
                {
                    m_CallBacks.Remove(funcName);
                }
                m_CallBacks.Add(funcName, callBack);
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
            return true;
        }


        public bool RemoveCallBack(string funcName)
        {
            CustomSystem.Instance.RemoveCallBack(funcName);

            if (funcName == null || funcName.Length == 0)
            {
                return false;
            }

            try
            {
                m_CallBacks.Remove(funcName);
            }
            catch (Exception ex)
            {
                //Log.TraceError("AgentEx RemoveCallBack exception =[" + ex.ToString() + "]");
                return false;
            }

            return true;
        }

        //回调处理
        public bool Excute_CallBack(string fun_name, VarList args)
        {
            try
            {
                if (m_CallBacks.ContainsKey(fun_name))
                {
                    m_CallBacks[fun_name](args);
					return true;
                }
                else
                {
                    //Log.TraceError("can not find call_function " + fun_name);
                }
            }
            catch (Exception ex)
            {
                //Log.TraceError("AgentEx Excute_CallBack exception =[" + ex.ToString() + "]");
            }
            return false;
        }

        public RecData GetRecDataByIndex(int index)
        {
            try
            {
                if (!mRecordTable.ContainsKey(index))
                {
                    return null;
                }

                RecData rec = mRecordTable[index];
                return rec;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRecDataByIndex Exception:" + ex.ToString());
                return null;
            }
        }

        public PropData GetPropDataByIndex(int index)
        {
            try
            {
                if (!mPropertyTable.ContainsKey(index))
                {
                    return null;
                }

                PropData prop = mPropertyTable[index];
                return prop;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetPropDataByIndex Exception:" + ex.ToString());
                return null;
            }
        }

        public int GetRoleCount()
        {
            if (mRoles == null)
            {
                return 0;
            }
            return mRoles.Count;
        }

        public int GetRoleInfoCount()
        {
            if (mRoles == null || mRoles.Count == 0)
            {
                return 0;
            }

            int nCount = 0;
            try
            {
                nCount = mRoles[0].paraList.GetCount();
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleInfoCount Exception:" + ex.ToString());
                return nCount;
            }
            return nCount;
        }

        public int GetRoleIndex(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].RoleIndex;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleIndex Exception:" + ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public int GetRoleSysFlags(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].SysFlags;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleSysFlags Exception:" + ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public string GetRoleName(int index)
        {
            if (mRoles == null)
            {
                return "";
            }

            if (index >= mRoles.Count)
            {
                return "";
            }

            try
            {
                return mRoles[index].Name;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleName Exception:" + ex.ToString());
                return "";
            }
        }

        public string GetRolePara(int index)
        {
            string strRet = "";
            if (mRoles == null)
            {
                return strRet;
            }

            if (index >= mRoles.Count)
            {
                return strRet;
            }

            try
            {
                strRet = mRoles[index].Para;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRolePara Exception:" + ex.ToString());
                return strRet;
            }
            return strRet;
        }

        public int GetRoleDeleted(int index)
        {
            int nRet = 0;
            if (mRoles == null)
            {
                return 0;
            }

            if (index >= mRoles.Count)
            {
                return 0;
            }

            try
            {
                nRet = mRoles[index].Deleted;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleDeleted Exception:" + ex.ToString());
                return nRet;
            }
            return nRet;
        }

        public double GetRoleDeleteTime(int index)
        {
            double dRet = 0.0;
            if (mRoles == null)
            {
                return dRet;
            }

            if (index >= mRoles.Count)
            {
                return dRet;
            }

            try
            {
                dRet = mRoles[index].DeleteTime;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleDeleteTime Exception:" + ex.ToString());
                return dRet;
            }
            return dRet;
        }

        public void GetRoleInfo(ref VarList args, ref VarList ret)
        {
            try
            {
                if (mRoles == null || mRoles.Count == 0)
                {
                    return;
                }

                if (args == null || ret == null)
                {
                    return;
                }

                if (args.GetCount() == 0)
                {
                    return;
                }

                if (args.GetType(0) != VarType.Int)
                {
                    return;
                }

                int nIndex = args.GetInt(0);
                if (nIndex >= mRoles.Count)
                {
                    return;
                }

                VarList paraList = mRoles[nIndex].paraList;
                for (int i = 0; i < paraList.GetCount(); i++)
                {
                    switch (paraList.GetType(i))
                    {
                        case VarType.Bool:
                            {
                                ret.AddBool(paraList.GetBool(i));
                            }
                            break;
                        case VarType.Int:
                            {
                                ret.AddInt(paraList.GetInt(i));
                            }
                            break;
                        case VarType.Int64:
                            {
                                ret.AddInt64(paraList.GetInt64(i));
                            }
                            break;
                        case VarType.Float:
                            {
                                ret.AddFloat(paraList.GetFloat(i));
                            }
                            break;
                        case VarType.Double:
                            {
                                ret.AddDouble(paraList.GetDouble(i));
                            }
                            break;
                        case VarType.String:
                            {
                                ret.AddString(paraList.GetString(i));
                            }
                            break;
                        case VarType.WideStr:
                            {
                                ret.AddWideStr(paraList.GetWideStr(i));
                            }
                            break;
                        case VarType.Object:
                            {
                                ret.AddObject(paraList.GetObject(i));
                            }
                            break;
                        default:
                            return;
                    }
                }

            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GetRoleInfo Exception:" + ex.ToString());
            }
            return;
        }

        public string GerPropertyName(int index)
        {
            if (index < 0 || index >= mPropertyTable.Count)
            {
                return "";
            }

            try
            {
                if (!mPropertyTable.ContainsKey(index))
                {
                    return "";
                }

                PropData propData = mPropertyTable[index];
                if (propData != null)
                {
                    return propData.strName;
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.GerPropertyName Exception:" + ex.ToString());
                return "";
            }
            return "";
        }

        private bool InnerParsePropValue(LoadArchive loadAr, int type, ref VarList.VarData key)
        {
            switch (type)
            {
                case OuterDataType.OUTER_TYPE_BYTE:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt8(ref value))
                        {
                            return false;
                        }
                        key.nType = VarType.Int;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt16(ref value))
                        {
                            return false;
                        }

                        key.nType = VarType.Int;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DWORD:
                    {
                        int value = 0;
                        if (!loadAr.ReadInt32(ref value))
                        {
                            return false;
                        }

                        key.nType = VarType.Int;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_QWORD:
                    {
                        long value = 0;
                        if (!loadAr.ReadInt64(ref value))
                        {
                            return false;
                        }
                        key.nType = VarType.Int64;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_FLOAT:
                    {
                        float value = 0.0f;
                        if (!loadAr.ReadFloat(ref value))
                        {
                            return false;
                        }
                        key.nType = VarType.Float;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_DOUBLE:
                    {
                        double value = 0.0;
                        if (!loadAr.ReadDouble(ref value))
                        {
                            return false;
                        }
                        key.nType = VarType.Double;
                        key.Data = value; ;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_STRING:
                    {
                        string value = "";
                        if (!loadAr.ReadString(ref value))
                        {
                            return false;
                        }
                        key.nType = VarType.String;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_WIDESTR:
                    {
                        string value = "";
                        if (!loadAr.ReadWideStr(ref value))
                        {
                            return false;
                        }

                        key.nType = VarType.WideStr;
                        key.Data = value;
                    }
                    break;
                case OuterDataType.OUTER_TYPE_OBJECT:
                    {
                        ObjectID value = new ObjectID();
                        if (!loadAr.ReadObject(ref value))
                        {
                            return false;
                        }

                        key.nType = VarType.Object;
                        key.Data = value;
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool InnerRecvProperty(GameObj obj, LoadArchive loadAr, int count, bool change)
        {
            VarList.VarData key = new VarList.VarData();
            for (int i = 0; i < count; i++)
            {
                int index = 0;
                if (!loadAr.ReadInt16(ref index))
                {
                    return false;
                }

                if (index >= mPropertyTable.Count)
                {
                    return false;
                }

                PropData propData = GetPropDataByIndex(index);
                if (propData == null)
                {
                    return false;
                }

                propData.nCount = propData.nCount + 1;

                if (!InnerParsePropValue(loadAr, propData.nType, ref key))
                {
                    return false;
                }

                if (!obj.UpdateProperty(ref propData.strName, key))
                {
                    return false;
                }

                if (change)
                {
                    VarList argList = new VarList();
                    argList.AddObject(obj.GetIdent());
                    argList.AddString(propData.strName);
					Excute_CallBack("on_object_property_change", argList);
                }
            }

            return true;
        }

        public bool RecvProperty(ref GameObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.RecvProperty(GameObj) Exception:" + ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameView obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.RecvProperty(GameView) Exception:" + ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameViewObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.RecvProperty(GameViewObj) Exception:" + ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameScene obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.RecvProperty(GameScene) Exception:" + ex.ToString());
                return false;
            }
        }

        public bool RecvProperty(ref GameSceneObj obj, ref LoadArchive loadAr,
            int count, bool change)
        {
            try
            {
                return InnerRecvProperty(obj, loadAr, count, change);
            }
            catch (Exception ex)
            {
                //Log.Trace("GameReceiver.RecvProperty Exception:" + ex.ToString());
                return false;
            }
        }

        public string GetRecordName(int index)
        {
            try
            {
                if (index < 0 || index >= mRecordTable.Count)
                {
                    //Log.Trace("index is not right");
                    return "";
                }
                return mRecordTable[index].strName;
            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
            }
            return "";
        }

        public GameSceneObj GetSceneObj(ObjectID ident)
        {
            try
            {
                if (ident.IsNull())
                {
                    return null;
                }

                if (m_client == null)
                {
                    return null;
                }

                GameScene scene = (GameScene)m_client.GetCurrentScene();
                if (scene == null)
                {
                    return null;
                }

                GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                return sceneObj;
            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return null;
            }
        }

        public void ClearMunus()
        {
            mMenus.Clear();
        }

        public int GetMenuCount()
        {
            return mMenus.Count;
        }

        public int GetMenuType(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return 0;
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return 0;
                }

                return mMenus[index].nType;

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return 0;
            }
        }

        public int GetMenuMark(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return 0;
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return 0;
                }

                return mMenus[index].nMark;

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return 0;
            }
        }

        public string GetMenuContent(int index)
        {
            if (index < 0 || index > mMenus.Count)
            {
                return "";
            }

            try
            {
                if (!mMenus.ContainsKey(index))
                {
                    return "";
                }

                return mMenus[index].info;

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return "";
            }
        }

        public bool AddMenuData(int index, ref MenuData menuData)
        {
            if (index < 0)
            {
                return false;
            }

            try
            {
                if (mMenus.ContainsKey(index))
                {
                    return false;
                }

                mMenus.Add(index, menuData);

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
            return true;
        }

        public bool ClearAll()
        {
            if (m_client != null)
            {
                m_client.ClearAll();
            }

            ClearPropertyTable();
            ClearRecordTable();
            ClearRoles();
            ClearMunus();
            return true;
        }

        // 消息处理
        public void ProcessMsg(byte[] data, int size)
        {
            m_RecvHandle.HandleMessage(data, size);
        }

        // 暂时没有实现
        public bool DumpMsgStat(string file_name)
        {
            return false;
        }
    }
}

