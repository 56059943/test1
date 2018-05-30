
using System.Collections;
using SysUtils;
using System.IO;
using System;
using Fm_ClientNet.Interface;
using System.Security.Cryptography;

namespace Fm_ClientNet
{
    public class RecvPacket
    {
        private  GameReceiver m_receiver = null;
        private  IGameMsgHander m_gameMsgHandle = null;
        private  ICustomHandler m_cusMsgHandle = null;
        private  GameClient m_gameClient = null;
        private  VarList m_argList = new VarList();

        public  void SetGameReceiver(GameReceiver receiver)
        {
            m_receiver = receiver;
        }

        public  void SetGameClient(GameClient client)
        {
            m_gameClient = client;
        }

        public  void SetMsgHandle(IGameMsgHander msgHandle)
        {
            m_gameMsgHandle = msgHandle;
        }

        public  IGameMsgHander GetMsgHandle()
        {
            return m_gameMsgHandle;
        }

        public  void SetCustomHandle(ICustomHandler customHandle)
        {
            m_cusMsgHandle = customHandle;
        }

        public  ICustomHandler GetCustomHandle()
        {
            return m_cusMsgHandle;
        }

         LoadArchive loadAr;
        //SERVER_SET_VERIFY(1)
         public void recvSetVerify(int id, object args)
        {
            //Log.Trace("Receive SERVER_SET_VERIFY packet");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            int nWidth = 0;
            int nHeight = 0;
            if (!loadAr.ReadInt32(ref nWidth))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nHeight))
            {
                return;
            }


            if (nHeight == 1)
            {

            }
            else
            {

            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnSetVerify(nWidth, nHeight, "", null);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nWidth);
                m_argList.AddInt(nHeight);
                m_receiver.Excute_CallBack("on_set_verify", m_argList);
            }
        }

        //设置加密密码
         public void recvSetEncode(int id, object args)
        {
            //Log.Trace("Receive SERVER_SET_VERIFY packet");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }
                         
            uint nDynamicKey = 0;
            uint nEncodeId = 0;
            byte[] info = new byte[32];
            int nAddress = 0;


            if (!loadAr.ReadUInt16(ref nDynamicKey))
            {
                return;
            }

            if (!loadAr.ReadUInt16(ref nEncodeId))
            {
                return;
            }

            if (!loadAr.ReadUserDataNoLen(ref info,32))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nAddress))
            {
                return;
            }

            string strInfo = System.Text.Encoding.ASCII.GetString(info);

            //设置当前消息序号和解密组
            MsgEncode.Instance.SetEncodeId(nDynamicKey, nEncodeId, strInfo, nAddress);       
        }

         RoleData roleData = new RoleData();
         ObjectID value = new ObjectID();
        //读取登录成功后玩家获取到的玩家角色信息
         private bool readLoginRoleData(int nRoleNum, ref LoadArchive loadAr)
        {
            m_gameClient.ClearAll();
            for (int i = 0; i < nRoleNum; i++)
            {
                uint nArg_num = 0;
                roleData = new RoleData();
                if (!loadAr.ReadUInt16(ref nArg_num))
                {
                    return false;
                }

                for (uint index = 0; index < nArg_num; index++)
                {
                    int type = 0;
                    if (!loadAr.ReadInt8(ref type))
                    {
                        return false;
                    }

                    switch (type)
                    {
                        case VarType.Bool:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt8(ref value))
                                {
                                    return false;
                                }

                                if (value > 0)
                                {
                                    roleData.paraList.AddBool(true);
                                }
                                else
                                {
                                    roleData.paraList.AddBool(false);
                                }

                            }
                            break;
                        case VarType.Int:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt32(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddInt(value);
                            }
                            break;
                        case VarType.Int64:
                            {
                                long value = 0;
                                if (!loadAr.ReadInt64(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddInt64(value);
                            }
                            break;
                        case VarType.Float:
                            {
                                float value = 0.0f;
                                if (!loadAr.ReadFloat(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddFloat(value);
                            }
                            break;
                        case VarType.Double:
                            {
                                double value = 0.0f;
                                if (!loadAr.ReadDouble(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddDouble(value);
                            }
                            break;
                        case VarType.String:
                            {
                                string value = "";
                                if (!loadAr.ReadString(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddString(value);
                            }
                            break;
                        case VarType.WideStr:
                            {
                                string value = "";
                                if (!loadAr.ReadWideStr(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddWideStr(value);
                            }
                            break;
                        case VarType.Object:
                            {
                                if (!loadAr.ReadObject(ref value))
                                {
                                    return false;
                                }
                                roleData.paraList.AddObject(value);
                            }
                            break;
                        default:
                            //Log.Trace("readLoginRoleData role type error");
                            return false;
                    }
                }

                if (nArg_num >= 4)
                {
                    if (roleData.paraList.GetType(0) == VarType.Int)
                    {
                        roleData.RoleIndex = roleData.paraList.GetInt(0);
                    }

                    if (roleData.paraList.GetType(1) == VarType.Int)
                    {
                        roleData.SysFlags = roleData.paraList.GetInt(1);
                    }

                    if (roleData.paraList.GetType(2) == VarType.WideStr)
                    {
                        roleData.Name = roleData.paraList.GetWideStr(2);
                    }

                    if (roleData.paraList.GetType(3) == VarType.WideStr)
                    {
                        roleData.Para = roleData.paraList.GetWideStr(3);
                    }

                }

                if (nArg_num >= 6)
                {
                    if (roleData.paraList.GetType(4) == VarType.Int)
                    {
                        int nTest = roleData.paraList.GetInt(4);
                        roleData.SetDeleted(nTest);
                    }

                    if (roleData.paraList.GetType(5) == VarType.Double)
                    {
                        roleData.DeleteTime = roleData.paraList.GetDouble(5);
                    }
                }
                else
                {
                    roleData.Deleted = 0;
                    roleData.DeleteTime = 0.0;
                }
                m_receiver.AddRoleData(i, ref roleData);
            }

            return true;
        }

        //SERVER_LOGIN_SUCCEED(4) OK
         public void recvLoginSuccess(int id, object args)
        {
            //Log.Trace("Recv Login Success Packet");
            m_receiver.ClearRoles();
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            //跳过8位的协议Id
            //9个int 分别是指如下9个整型字段
            int nIsFree = 0;		// 是否免费
            int nPoints = 0;		// 剩余点数
            int nYear = 0;			// 包月截止日期
            int nMonth = 0;
            int nDay = 0;
            int nHour = 0;
            int nMinute = 0;
            int nSecond = 0;
            int nDynamicKey = 0;	// 动态密匙

            int roleNum = 0;  //此账号的现在角色数
            if (!loadAr.Seek(1))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nIsFree))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPoints))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nYear))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nMonth))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nDay))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nHour))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nMinute))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nSecond))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nDynamicKey))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref roleNum))
            {
                return;
            }

            if (!readLoginRoleData(roleNum, ref loadAr))
            {
                //Log.Trace("RecvPacket recvLoginSuccess readLoginRoleData failed");
                return;
            }


            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnLoginSucceed(nIsFree, nPoints, nYear, nMonth, nDay, nHour, nMinute, nSecond, roleNum);
            }
            else
            {

                m_argList.Clear();
                if (0 == nIsFree)
                {
                    m_argList.AddBool(false);
                }
                else
                {
                    m_argList.AddBool(true);
                }

                m_argList.AddInt(nPoints);
                m_argList.AddInt(nYear);
                m_argList.AddInt(nMonth);
                m_argList.AddInt(nDay);
                m_argList.AddInt(nHour);
                m_argList.AddInt(nMinute);
                m_argList.AddInt(nSecond);
                m_argList.AddInt(roleNum);
                m_receiver.Excute_CallBack("on_login_succeed", m_argList);
            }

        }

        //SERVER_WORLD_INFO (5)
         public void recvWorldInfo(int id, object args)
        {
            //Log.Trace("Recv World Info");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            int nInfoType = 0;
            if (!loadAr.ReadInt16(ref nInfoType))
            {
                return;
            }

            string info = "";
            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnWorldInfo(nInfoType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nInfoType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_world_info", m_argList);
            }
        }

        //SERVER_ERROR_CODE (3) OK
         public void recvErrorCode(int id, object args)
        {
            //Log.Trace("Recv Error Code");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            int nErrorCode = 0;

            if (!loadAr.Seek(1))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nErrorCode))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnErrorCode(nErrorCode);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nErrorCode);
                m_receiver.Excute_CallBack("on_error_code", m_argList);
            }
        }

         ObjectID objId;
         OuterPost outerPost = new OuterPost();
         OuterDest outerDest = new OuterDest();
        //SERVER_ADD_OBJECT(13) OK
         public void recvAddObj(int id, object args)
        {
            //Log.Trace("Recv Add Obj Packet");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);
            objId = new ObjectID();
            if (!loadAr.ReadObject(ref objId))
            {
                //Log.Trace("read object failed");
                return;
            }

            
            if (!outerPost.initOuterPost(ref loadAr))
            {
                //Log.Trace("initOuterPost failed");
                return;
            }

            
            if (!outerDest.initOuterDest(ref loadAr))
            {
                //Log.Trace("initOuterDest failed");
                return;
            }

            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nPropCount))
            {
                //Log.Trace("read nPropCount failed");
                return;
            }

            ObjectID ident = objId;
            GameClient client = m_gameClient;
            if (null == client)
            {
                //Log.Trace("client is null");
                return;
            }

            GameScene scene = (GameScene)client.GetCurrentScene();
            if (scene == null)
            {
                //Log.Trace("current scene is null");
                return;
            }

            GameSceneObj sceneObj = ((GameScene)client.GetCurrentScene()).AddSceneObj(ident);
            if (sceneObj == null)
            {
                //Log.Trace("sceneObj is null");
                return;
            }
            sceneObj.SetObjId(objId);

            if (nPropCount > 0)
            {
                if (!m_receiver.RecvProperty(ref sceneObj, ref loadAr, nPropCount, false))
                {
                    //Log.Trace("RecvProperty failed");
                    return;
                }
            }

            sceneObj.SetLocation(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
            sceneObj.SetDestination(outerDest.x, 0, outerDest.z, outerDest.orient,
                outerDest.MoveSpeed, outerDest.RotateSpeed, outerDest.JumpSpeed, outerDest.Gravity);
            sceneObj.SetMode(outerDest.Mode);

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnAddObject(ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(ident);
                //m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_add_object", m_argList);
            }

            return;
        }

         byte[] validStr = new byte[1];
        //SERVER_CHARGE_VALIDSTRING (50)(不用做)
         public void recvChargeValidString(int id, object args)
        {
            //Log.Trace("Recv Charge Valid String");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);
            
            if ( !loadAr.ReadUserDataNoLen( ref validStr, Const.VALIDATE_LEN_LENGTH + 1) )
            {
                return;
            }

            string strValid = System.Text.Encoding.Default.GetString(validStr);
            //Stream stream = new MemoryStream();
            //stream.Write(validStr, 0, validStr.Length);
            //string strValid = stream.ToString();
            //stream.Close();

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnServerChargeValidstring(strValid);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddString(strValid);
                m_receiver.Excute_CallBack("on_charge_validstring", m_argList);
            }
        }

        //SERVER_PROPERTY_TABLE (9) OK
         public void recvPropertyTable(int id, object args)
        {
            //Log.Trace("Recv Property Table");

            m_receiver.ClearPropertyTable();

            byte[] data = (byte[])args;
            m_receiver.SetPropertyTableMd5(data);

            loadAr = new LoadArchive(data, 0, data.Length);

            //属性数量
            int nPropertyNum = 0;
            loadAr.Seek(1);

            if (!loadAr.ReadInt16(ref nPropertyNum))
            {
                return;
            }

            for (int i = 0, count = 0; i < nPropertyNum * 2; i = i + 2, count++)
            {
                string strProp = "";
                if (!loadAr.ReadStringNoLen(ref strProp))
                {
                    return;
                }

                int ntype = 0;
                if (!loadAr.ReadInt8(ref ntype))
                {
                    return;
                }

                PropData propData = new PropData();
                if (propData == null)
                {
                    return;
                }
                propData.nCount = 0;
                propData.strName = strProp;
                propData.nType = ntype;
                //Propetry prop = new Propetry(strProp, ntype);
                m_receiver.AddPropData(count, ref propData);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnPropertyTable(nPropertyNum);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nPropertyNum);
                m_receiver.Excute_CallBack("on_property_table", m_argList);
            }

        }

        //SERVER_RECORD_TABLE (10)OK
         public void recvRecordTable(int id, object args)
        {
            //Log.Trace("Recv Record Table");

            m_receiver.ClearRecordTable();

            byte[] data = (byte[])args;
            m_receiver.SetRecordTableMd5( data );
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nRecordCount = 0;
            if (!loadAr.ReadInt16(ref nRecordCount))
            {
                return;
            }

            for (int i = 0; i < nRecordCount; i++)
            {
                string recordName = "";
                if (!loadAr.ReadStringNoLen(ref recordName))
                {
                    return;
                }

                ////Log.Trace("recordName" + recordName);
                int nCols = 0;
                if (!loadAr.ReadInt16(ref nCols))
                {
                    return;
                }

                RecData rec = new RecData();
                rec.strName = recordName;
                rec.nCols = nCols;

                //获取表格详情
                for (int index = 0; index < nCols; index++)
                {
                    int nColType = 0;
                    if (!loadAr.ReadInt8(ref nColType))
                    {
                        return;
                    }

                    if (!rec.AddRecColType(index, nColType))
                    {
                        return;
                    }
                }

                m_receiver.AddRecData(i, ref rec);
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnRecordTable(nRecordCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nRecordCount);
                m_receiver.Excute_CallBack("on_record_table", m_argList);
            }
        }

        //SERVER_ENTRY_SCENE (11) OK
         public void recvEntryScene(int id, object args)
        {
            //Log.Trace("Recv Entry Scene");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            //动态密匙
            int nDynamicKey = 0;
            if (!loadAr.ReadInt32(ref nDynamicKey))
            {
                return;
            }

            //主角玩家的对象标识
            ObjectID objId = new ObjectID();
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }


            int nSceneID = 0;
            if (!loadAr.ReadInt16(ref nSceneID))
            {
                return;
            }

            //后续场景属性数量
            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            ObjectID ident = objId;
            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            GameScene scene = m_gameClient.CreateScene(ident);
            if (scene == null)
            {
                //Log.Trace("CreateScene is null");
                return;
            }

            if (!m_receiver.RecvProperty(ref scene, ref loadAr, nPropCount, false))
            {
                //Log.Trace("RecvProperty Failed");
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnEntryScene(ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(ident);
                m_argList.AddInt(nSceneID);
                m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_entry_scene", m_argList);
            }
        }

        //SERVER_EXIT_SCENE (12)
         public void recvExitScene(int id, object args)
        {
            //Log.Trace("Recv Exit Scene");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnExitScene();
            }
            else
            {
                m_argList.Clear();
                m_receiver.Excute_CallBack("on_exit_scene", m_argList);
            }
        }

        //SERVER_RECORD_CLEAR (20) OK(obj =0,3,1,2)
         public void recvRecordClear(int id, object args)
        {
            //Log.Trace("Recv Record Clear");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = -1;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = new ObjectID();
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }

            int nIndex = 0;
            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            if (null == m_receiver)
            {
                return;
            }

            string recName = m_receiver.GetRecordName(nIndex);
            if (recName == null || recName.Length == 0)
            {
                //Log.Trace("recName is empty or is null");
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                //Log.Trace("GameReceiver is null");
                return;
            }




            if (nIsViewObj == 0)
            {
                ObjectID ident = objId;
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (null == sceneObj)
                {
                    //Log.Trace("scene obj is null");
                    return;
                }
                GameRecord record = sceneObj.GetGameRecordByName(recName);
                if (record != null)
                {
                    record.ClearRow();
                }
                else
                {
                    if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                    {
                        //Log.Trace("add failed record" + recName);
                    }
                }


                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRecordClear(ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_record_clear", m_argList);
                }


            }//end nIsViewObj == 0
            else if (nIsViewObj == 3)
            {
                ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                GameView viewObj = m_receiver.GetView(view_ident);
                if (viewObj == null)
                {
                    //Log.Trace("no object record name " + recName);
                }
                else
                {
                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();

                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            //Log.Trace("add failed record name " + recName);
                        }
                    }
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewRecordClear(view_ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(view_ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_view_record_clear", m_argList);
                }

            }//end nIsViewObj == 3
            else if (nIsViewObj == 1)
            {
                ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                ObjectID item_ident = new ObjectID(objId.m_nSerial,0);
                GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                if (viewObj != null)
                {
                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();
                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            //Log.Trace("add failed record name " + recName);
                        }
                    }

                }
                else
                {
                    //Log.Trace("no object record name " + recName);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewObjRecordClear(view_ident, item_ident, recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(view_ident);
                    m_argList.AddObject(item_ident);
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_viewobj_record_clear", m_argList);
                }

            }//end nIsViewObj == 1
            else if (nIsViewObj == 2)
            {
                GameScene sceneObj = (GameScene)(GameScene)m_gameClient.GetCurrentScene();
                if (sceneObj != null)
                {
                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        record.ClearRow();
                    }
                    else
                    {
                        if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                        {
                            //Log.Trace("add failed record name " + recName);
                        }
                    }

                }
                else
                {
                    //Log.Trace("no object record name = " + recName);
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSceneRecordClear(recName);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddString(recName);
                    m_receiver.Excute_CallBack("on_scene_record_clear", m_argList);
                }
            }//end nIsViewObj == 2
        }

        //SERVER_CREATE_VIEW (21) OK
         public void recvCreateView(int id, object args)
        {
            //Log.Trace("Recv Create View");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            //视图编号
            //容量
            //后续属性数量
            uint nViewId = 0;
            int nCapacity = 0;
            int nCount = 0;
            if (!loadAr.ReadUInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nCapacity))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nCount))
            {
                return;
            }

            ObjectID view_ident = new ObjectID(nViewId,0);
            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            try
            {
                GameView view = m_gameClient.CreateView(view_ident, nCapacity);

                if (view != null)
                {
                    if (false == m_receiver.RecvProperty(ref view, ref loadAr, nCount, false))
                    {
                        //Log.Trace("RecvProperty failed");
                    }
                }
                else
                {
                    //Log.Trace("Create View is null");
                    return;
                }

            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnCreateView(view_ident, nCapacity, nCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(view_ident);
                m_argList.AddInt(nCapacity);
                m_argList.AddInt(nCount);
                m_receiver.Excute_CallBack("on_create_view", m_argList);
            }
        }

        // SERVER_OBJECT_PROPERTY(16) (ok)
         public void recvObjectProperty(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = 0;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = new ObjectID();
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }

            int nCount = 0;
            if (!loadAr.ReadInt16(ref nCount))
            {
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                //Log.Trace("GameReceiver is null");
                return;
            }

            try
            {
                if (nIsViewObj == 0)
                {
                    ObjectID ident = objId;
                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (sceneObj == null)
                    {
                        //Log.Trace("scene obj is null");
                        return;
                    }

                    if (!m_receiver.RecvProperty(ref sceneObj,
                        ref loadAr, nCount, true))
                    {
                        //Log.Trace("property error");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnObjectProperty(ident, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_object_property", m_argList);
                    }

                }
                else
                {
                    ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                    ObjectID item_ident = new ObjectID(objId.m_nSerial,0);
                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj != null)
                    {
                        if (!m_receiver.RecvProperty(ref viewObj,
                            ref loadAr, nCount, true))
                        {
                            //Log.Trace("property error");
                        }

                    }
                    else
                    {
                        //Log.Trace("view obj is null");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjProperty(view_ident, item_ident, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddObject(item_ident);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_view_object_property", m_argList);
                    }
                }//end if (nIsViewObj == 0)

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }//end try catch
        }

        //SERVER_RECORD_ADDROW (17) OK (viewobj =0,3,1,2)
         public void recvRecordAddRow(int id, object args)
        {
            //Log.Trace("Recv Record AddRow");
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            //跳过协议Id
            loadAr.Seek(1);

            int nIsViewObj = 0;
            if (!loadAr.ReadInt8(ref nIsViewObj))
            {
                return;
            }

            ObjectID objId = new ObjectID();
            if (!loadAr.ReadObject(ref objId))
            {
                return;
            }


            int nIndex = 0;
            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            int nRow = 0;
            if (!loadAr.ReadInt16(ref nRow))
            {
                return;
            }

            int nRows = 0;
            if (!loadAr.ReadInt16(ref nRows))
            {
                return;
            }

            try
            {
                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                string recName = m_receiver.GetRecordName(nIndex);
                if (nIsViewObj == 0)
                {
                    ObjectID ident = objId;
                    if (null == m_gameClient)
                    {
                        //Log.Trace("GameClient is null");
                        return;
                    }

                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (sceneObj == null)
                    {
                        //Log.Trace("sceneObj is null");
                        return;
                    }
                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref sceneObj, nIndex))
                        {
                            //Log.Trace("add failed record name:" + recName);
                        }

                        record = sceneObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record, nIndex,
                            ref loadAr, nRow, nRows))
                        {
                            //Log.Trace("RecvRecordRow failed");
                            return;
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnRecordAddRow(ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 0
                else if (nIsViewObj == 3)
                {
                    ObjectID ident = new ObjectID(objId.m_nIdent,0);
                    if (null == m_gameClient)
                    {
                        //Log.Trace("GameClient is null");
                        return;
                    }

                    GameView viewObj = m_receiver.GetView(ident);
                    if (viewObj == null)
                    {
                        //Log.Trace("view obj is null");
                        return;
                    }

                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            //Log.Trace("AddRecord failed recrod" + record.GetName());
                        }

                        record = viewObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record, nIndex,
                            ref loadAr, nRow, nRows))
                        {
                            //Log.Trace("obj 3 RecvRecordRow record" + record.GetName());
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordAddRow(ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_view_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 3 
                else if (nIsViewObj == 1)
                {
                    ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                    ObjectID item_ident = new ObjectID(objId.m_nSerial,0);
                    if (m_gameClient == null)
                    {
                        //Log.Trace("GameClient is null");
                        return;
                    }

                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj == null)
                    {
                        //Log.Trace("obj 1 view obj is null ");
                        return;
                    }

                    GameRecord record = viewObj.GetGameRecordByName(recName);
                    if (null == record)
                    {
                        if (!m_receiver.AddRecord(ref viewObj, nIndex))
                        {
                            //Log.Trace("obj 1 AddRecord failed record" + record.GetName());
                        }

                        record = viewObj.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record,
                            nIndex, ref loadAr, nRow, nRows))
                        {
                            //Log.Trace("obj 1 RecvRecordRow failed record :" + record.GetName());
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordAddRow(view_ident, item_ident, recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddObject(item_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_viewobj_record_add_row", m_argList);
                    }

                }//end nIsViewObj == 1
                else if (nIsViewObj == 2)
                {
                    if (m_gameClient == null)
                    {
                        //Log.Trace("obj 2 GameClient is null");
                        return;
                    }

                    GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                    if (scene == null)
                    {
                        //Log.Trace("obj 2 scene is null");
                        return;
                    }

                    GameRecord record = scene.GetGameRecordByName(recName);
                    if (record == null)
                    {
                        if (!m_receiver.AddRecord(ref scene, nIndex))
                        {
                            //Log.Trace("obj 2 AddRecord failed record:" + recName);
                        }
                        record = scene.GetGameRecordByName(recName);
                    }

                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordRow(ref record,
                            nIndex, ref loadAr, nRow, nRows))
                        {
                            //Log.Trace("obj 2 RecvRecordRow failed");
                        }
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordAddRow(recName, nRow, nRows);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_argList.AddInt(nRows);
                        m_receiver.Excute_CallBack("on_scene_record_add_row", m_argList);
                    }
                }//end nIsViewObj == 2

            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
            }//end try catch

        }

        //SERVER_CP_ADD_OBJECT (40)
         public void recvCpAddObject(int id, object args)
        {
            //Log.Trace("Recv cp add obj");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvAddObj(GlobalServerMsg.SERVER_ADD_OBJECT, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
            return;
        }

        //SERVER_CP_RECORD_ADDROW (41)
         public void recvCpRecordAddRow(int id, object args)
        {
            //Log.Trace("Recv SERVER_CP_RECORD_ADDROW");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvRecordAddRow(GlobalServerMsg.SERVER_RECORD_ADDROW, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_CP_VIEW_ADD (42)
         public void recvCpViewAdd(int id, object args)
        {
            //Log.Trace("Recv SERVER_CP_VIEW_ADD");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvViewAdd(GlobalServerMsg.SERVER_VIEW_ADD, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_VIEW_CHANGE (43)
         public void recvViewChange(int id, object args)
        {
            //Log.Trace("Recv View Change");
            try
            {
                byte[] data = (byte[])args;
                loadAr = new LoadArchive(data, 0, data.Length);
				if (!loadAr.Seek(1))
				{
					return;
				}


                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nViewId = 0;
                int nOldObjectId = 0;
                int nNewOjbectId = 0;
                if (!loadAr.ReadInt16(ref nViewId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nOldObjectId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nNewOjbectId))
                {
                    return;
                }

                ObjectID view_ident = new ObjectID((uint)nViewId,0);
                ObjectID old_item_ident = new ObjectID((uint)nOldObjectId,0);
                ObjectID new_item_ident = new ObjectID((uint)nNewOjbectId, 0);

                GameView view = m_gameClient.GetViewByIdent(view_ident);
                if (view != null)
                {
                    if (!view.ChangeViewObj(old_item_ident, new_item_ident))
                    {
                        //Log.Trace("change failed");
                    }

                }
                else
                {
                    //Log.Trace("no view");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewChange(view_ident, old_item_ident, new_item_ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(view_ident);
                    m_argList.AddObject(old_item_ident);
                    m_argList.AddObject(new_item_ident);
                    m_receiver.Excute_CallBack("on_view_change", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        // SERVER_ALL_PROP(44)
         public void recvAllProp(int id, object args)
        {
            //Log.Trace("Recv All Prop");
            try
            {
                byte[] data = (byte[])args;
                loadAr = new LoadArchive(data, 0, data.Length);
				if (!loadAr.Seek(1))
				{
					return;
				}


                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)(GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    //Log.Trace("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID obj = new ObjectID();
                    if (!loadAr.ReadObject(ref obj))
                    {
                        return;
                    }

                    int prop_num = 0;
                    if (!loadAr.ReadInt16(ref prop_num))
                    {
                        return;
                    }
                    ObjectID ident = obj;
                    GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                    if (!m_receiver.RecvProperty(ref sceneObj, ref loadAr,
                        prop_num, true))
                    {
                        //Log.Trace("property error");
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnObjectProperty(ident, prop_num);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_argList.AddInt(prop_num);
                        m_receiver.Excute_CallBack("on_object_property", m_argList);
                    }
                }//end for

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAllProperty(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_all_prop", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_CP_ALL_PROP (46)
         public void recvCpAllProp(int id, object args)
        {
            //Log.Trace("Recv SERVER_CP_ALL_PROP");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvAllProp(GlobalServerMsg.SERVER_ALL_PROP, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_ADD_MORE_OBJECT (47)
         public void recvAddMoreObject(int id, object args)
        {
            //Log.Trace("Recv Add More Object");
            try
            {
                byte[] data = (byte[])args;
                loadAr = new LoadArchive(data, 0, data.Length);

                loadAr.Seek(1);

                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    //Log.Trace("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID objId = new ObjectID();
                    if (!loadAr.ReadObject(ref objId))
                    {
                        return;
                    }

                    OuterPost outerPost = new OuterPost();
                    if (!outerPost.initOuterPost(ref loadAr))
                    {
                        return;
                    }

                    OuterDest outerDest = new OuterDest();
                    if (!outerDest.initOuterDest(ref loadAr))
                    {
                        return;
                    }
                    int prop_count = 0;
                    if (!loadAr.ReadInt16(ref prop_count))
                    {
                        return;
                    }

                    ObjectID ident = objId;
                    GameSceneObj sceneObj = scene.AddSceneObj(ident);
                    if (prop_count > 0)
                    {
                        if (!m_receiver.RecvProperty(ref sceneObj,
                            ref loadAr, prop_count, false))
                        {
                            //Log.Trace("property error");
                        }
                    }

                    sceneObj.SetLocation(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                    sceneObj.SetDestination(outerDest.x, 0, outerDest.z,
                        outerDest.orient, outerDest.MoveSpeed, outerDest.RotateSpeed,
                        outerDest.JumpSpeed, outerDest.Gravity);
                    sceneObj.SetMode(outerDest.Mode);

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnAddObject(ident, prop_count);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_argList.AddInt(prop_count);
                        m_receiver.Excute_CallBack("on_add_object", m_argList);
                    }

                }//end for
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAddMoreObject(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_add_more_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_CP_ADD_MORE_OBJECT (48)
         public void recvCpAddMoreObject(int id, object args)
        {
            //Log.Trace("Recv SERVER_CP_ADD_MORE_OBJECT");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvAddMoreObject(GlobalServerMsg.SERVER_ADD_MORE_OBJECT, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_REMOVE_MORE_OBJECT (49) (OK)
         public void recvRemoveMoreObject(int id, object args)
        {
            //Log.Trace("Recv SERVER_REMOVE_MORE_OBJECT");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameCLient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene == null)
                {
                    //Log.Trace("no scene");
                    return;
                }

                for (int i = 0; i < nCount; i++)
                {
                    ObjectID objId = new ObjectID();
                    if (!loadAr.ReadObject(ref objId))
                    {
                        return;
                    }

                    ObjectID ident = objId;

                    GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                    if (sceneObj == null)
                    {
                        continue;
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnBeforeRemoveObject(ident);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_receiver.Excute_CallBack("on_before_remove_object", m_argList);
                    }

                    scene.RemoveSceneObj(ident);
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnRemoveObject(ident);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(ident);
                        m_receiver.Excute_CallBack("on_remove_object", m_argList);
                    }
                }//end for

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRemoveMoreObject(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_remove_more_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_CP_ALL_DEST (42)
         public void recvCpAllDest(int id, object args)
        {
            //Log.Trace("Recv SERVER_CP_ALL_DEST");

            try
            {
                byte[] data = (byte[])args;
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

                //调用非压缩的解析方式
                recvAllDest(GlobalServerMsg.SERVER_ALL_DEST, dataTemp);
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //CLIENT_SPEECH (26) (OK)
         public void recvSpeech(int id, object args)
        {
            //Log.Trace("Recv Speech");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);

            ObjectID obj = new ObjectID();
            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            string content = "";
            if (!loadAr.ReadWideStrNoLen(ref content))
            {
                return;
            }

            ObjectID ident = obj;
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnSpeech(ident, content);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(ident);
                m_argList.AddWideStr(content);
                m_receiver.Excute_CallBack("on_speech", m_argList);
            }
        }

        //SERVER_SYSTEM_INFO (27) (OK)
         public void recvSystemInfo(int id, object args)
        {
            //Log.Trace("Recv System Info");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nInfoType = 0;
            if (!loadAr.ReadInt16(ref nInfoType))
            {
                return;
            }

            string info = "";
            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnSystemInfo(nInfoType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nInfoType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_system_info", m_argList);
            }

        }

        //SERVER_MENU (28) 
         public void recvMenu(int id, object args)
        {
            //Log.Trace("Recv Menu");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);
            try
            {
                ObjectID objId = new ObjectID();
                int nCount = 0;
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                m_receiver.ClearMunus();

                for (int i = 0; i < nCount; i++)
                {
                    MenuData menuData = new MenuData();
                    int nType = 0;
                    if (!loadAr.ReadInt8(ref nType))
                    {
                        return;
                    }

                    int nMark = 0;
                    if (!loadAr.ReadInt16(ref nMark))
                    {
                        return;
                    }

                    string info = "";
                    if (!loadAr.ReadWideStr(ref info))
                    {
                        return;
                    }

                    if (!m_receiver.AddMenuData(i, ref menuData))
                    {
                        //Log.Trace("AddMenuData failed");
                        return;
                    }

                }
                ObjectID ident = objId;
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnMenu(ident, nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_menu", m_argList);
                }

            }//end try
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }//end catch
        }

        //SERVER_CLEAR_MENU (29)
         public void recvClearMenu(int id, object args)
        {
            //Log.Trace("Recv Clear Menu");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                m_receiver.ClearMunus();
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnClearMenu();
                }
                else
                {
                    m_argList.Clear();
                    m_receiver.Excute_CallBack("on_clear_menu", m_argList);
                }

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return;
            }
        }

        //SERVER_CUSTOM
         public void recvCustom(int id, object args)
        {
            //Log.Trace("Recv Custom");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nArgNum = 0;
            if (!loadAr.ReadInt16(ref nArgNum))
            {
                return;
            }

            VarList argList = new VarList();
            argList.AddInt(nArgNum);
            int temp = nArgNum;
            try
            {
                for (int i = 0; i < nArgNum; i++)
                {
                    int nType = 0;
                    if (!loadAr.ReadInt8(ref nType))
                    {
                        return;
                    }
                    

                    switch (nType)
                    {
                        case VarType.Int:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt32(ref value))
                                {
                                    return;
                                }
                                if(i == 0 && value > 0xffff)
                                {
                                    argList.AddInt(value & 0xffff);

                                    int high = value >> 16;

                                    argList.AddInt(high & 0xff);

                                    int low = high >> 8;

                                    argList.AddInt(low);

                                    temp += 2;

                                    argList.SetInt(0, temp);
                                }
                                else
                                {
                                    argList.AddInt(value);
                                }
                            }
                            break;
                        case VarType.Int64:
                            {
                                long value = 0;
                                if (!loadAr.ReadInt64(ref value))
                                {
                                    return;
                                }
                                argList.AddInt64(value);
                            }
                            break;
                        case VarType.Float:
                            {
                                float value = 0.0f;
                                if (!loadAr.ReadFloat(ref value))
                                {
                                    return;
                                }
                                argList.AddFloat(value);
                            }
                            break;
                        case VarType.Double:
                            {
                                double value = 0.0;
                                if (!loadAr.ReadDouble(ref value))
                                {
                                    return;
                                }
                                argList.AddDouble(value);
                            }
                            break;
                        case VarType.String:
                            {
                                string value = "";
                                if (!loadAr.ReadString(ref value))
                                {
                                    return;
                                }
                                argList.AddString(value);
                            }
                            break;
                        case VarType.WideStr:
                            {
                                string value = "";
                                if (!loadAr.ReadWideStr(ref value))
                                {
                                    return;
                                }
                                argList.AddWideStr(value);
                            }
                            break;
                        case VarType.Object:
                            {
                                ObjectID value = new ObjectID();
                                if (!loadAr.ReadObject(ref value))
                                {
                                    return;
                                }
                                argList.AddObject(value);
                            }
                            break;
                        case VarType.UserData:
                            {
                                byte[] udata = null;
                                if (!loadAr.ReadUserData(ref udata))
                                {
                                    return;
                                }
                                argList.AddUserData(udata);
                            }
                            break;
                        default:
                            //Log.Trace("error arg type");
                            break;
                    }

                }//end for

                if (null != m_cusMsgHandle)
                {
                    m_cusMsgHandle.Process(argList);
                }
                else
                {
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnCustom(ref argList);
                    }
                    else
                    {
                        m_receiver.Excute_CallBack("on_custom", argList);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }

        }

        //SERVER_VIEW_PROPERTY(ok)
         public void recvViewProperty(int id, object args)
        {
            //Log.Trace("Recv ViewProperty");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int nPropCount = 0;
            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                ObjectID view_ident = new ObjectID((uint)nViewId,0);
                GameView view = m_gameClient.GetViewByIdent(view_ident);
                if (view != null)
                {
                    if (!m_receiver.RecvProperty(ref view,
                        ref loadAr, nPropCount, true))
                    {
                        //Log.Trace("property error");
                    }
                }
                else
                {
                    //Log.Trace("no view");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnViewProperty(view_ident, nPropCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(view_ident);
                    m_argList.AddInt(nPropCount);
                    m_receiver.Excute_CallBack("on_view_property", m_argList);
                }
            }//end try
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }

        }

        //SERVER_VIEW_ADD (24) (ok)
         public void recvViewAdd(int id, object args)
        {
            //Log.Trace("Recv View Add");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int objId = 0;
            int nPropCount = 0;

            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref objId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nPropCount))
            {
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                //Log.Trace("GameReceiver is null");
                return;
            }

            ObjectID view_ident = new ObjectID((uint)nViewId,0);
            ObjectID item_ident = new ObjectID((uint)objId,0);

            GameView view = m_gameClient.GetViewByIdent(view_ident);
            if (view != null)
            {
                GameViewObj viewObj = view.AddViewObj(item_ident);

                if (viewObj != null)
                {
                    if (!m_receiver.RecvProperty(ref viewObj,
                    ref loadAr, nPropCount, false))
                    {
                        //Log.Trace("property error");
                    }
                }
                else
                {
                    //Log.Trace("view obj is null");
                }
            }
            else
            {
                //Log.Trace("no view ");
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnViewAdd(view_ident, item_ident, nPropCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(view_ident);
                m_argList.AddObject(item_ident);
                m_argList.AddInt(nPropCount);
                m_receiver.Excute_CallBack("on_view_add", m_argList);
            }
        }

        //SERVER_VIEW_REMOVE (25) (OK)
         public void recvViewRemove(int id, object args)
        {
            //Log.Trace("Recv View Remove");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nViewId = 0;
            int objId = 0;


            if (!loadAr.ReadInt16(ref nViewId))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref objId))
            {
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            if (m_receiver == null)
            {
                //Log.Trace("GameReceiver is null");
                return;
            }

            ObjectID view_ident = new ObjectID((uint)nViewId, 0);
            ObjectID item_ident = new ObjectID((uint)objId, 0);

            GameView view = m_gameClient.GetViewByIdent(view_ident);
            if (view != null)
            {
                if (!view.RemoveViewObj(item_ident))
                {
                    //Log.Trace("remove failed");
                }

            }
            else
            {
                //Log.Trace("no view");
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnViewRemove(view_ident, item_ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(view_ident);
                m_argList.AddObject(item_ident);
                m_receiver.Excute_CallBack("on_view_remove", m_argList);
            }
        }

        //SERVER_REMOVE_OBJECT (14)
         public void recvRemoveObj(int id, object args)
        {
            //Log.Trace("Recv  Remove obj");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);


            ObjectID obj = new ObjectID();

            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                ObjectID ident = obj;
                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    scene.RemoveSceneObj(ident);
                }
                else
                {
                    //Log.Trace("no scene");
                }
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnRemoveObject(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_receiver.Excute_CallBack("on_remove_object", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_SCENE_PROPERTY (15) (OK)
         public void recvSceneProperty(int id, object args)
        {
            //Log.Trace("Recv Scene Property");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            if (!loadAr.Seek(1))
            {
                return;
            }

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    if (!m_receiver.RecvProperty(ref scene,
                        ref loadAr, nCount, true))
                    {
                        //Log.Trace("property error");
                    }
                }
                else
                {
                    //Log.Trace("no scene");
                }

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnSceneProperty(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_scene_property", m_argList);
                }
            }//end try
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }//end catch
        }

        //SERVER_RECORD_DELROW (ok viewobj=0 obj=3,obj=1,obj=2)
         public void recvRecordDelRow(int id, object args)
        {
            //Log.Trace("Recv  Remove Row");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nViewObj = 0;
            ObjectID obj = new ObjectID();

            int nIndex = 0;
            int nRow = 0;



            if (!loadAr.ReadInt8(ref nViewObj))
            {
                return;
            }

            if (!loadAr.ReadObject(ref obj))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nIndex))
            {
                return;
            }

            if (!loadAr.ReadInt16(ref nRow))
            {
                return;
            }

            if (null == m_receiver)
            {
                //Log.Trace("GameReceiver is null");
                return;
            }

            if (null == m_gameClient)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            string recName = m_receiver.GetRecordName(nIndex);
            if (recName == null || recName.Length == 0)
            {
                //Log.Trace("recName is null");
                return;
            }

            try
            {
                if (nViewObj == 0)
                {
                    ObjectID ident = obj;
                    if (ident.IsNull())
                    {
                        //Log.Trace("viewObj is 0 ident is null ");
                        return;
                    }

                    if (m_gameClient != null)
                    {
                        GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                        if (sceneObj == null)
                        {
                            //Log.Trace("sceneObj is null");
                            return;
                        }

                        GameRecord record = sceneObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            bool bRet = record.DeleteRow(nRow);
                            if (!bRet)
                            {
                                //Log.Trace("delete row failed record name" + recName.ToString());
                            }
                        }
                        else
                        {
                            //Log.Trace("no record record name =" + recName.ToString());
                        }

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnRecordRemoveRow(ident, recName, nRow);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddObject(ident);
                            m_argList.AddString(recName);
                            m_argList.AddInt(nRow);
                            m_receiver.Excute_CallBack("on_record_remove_row", m_argList);
                        }
                    }

                }//end  if (nViewObj == 0)
                else if (nViewObj == 3)
                {
                    ObjectID view_ident = new ObjectID(obj.m_nIdent,0);

                    if (m_gameClient != null)
                    {
                        GameView viewObj = m_receiver.GetView(view_ident);
                        if (viewObj != null)
                        {
                            GameRecord record = viewObj.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    //Log.Trace("delete row failed record name" + recName);
                                }
                            }
                            else
                            {
                                //Log.Trace("no record record name =" + recName);
                            }
                        }
                    }
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordRemoveRow(view_ident, recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_view_record_remove_row", m_argList);
                    }

                }//end if (nViewObj == 3)
                else if (nViewObj == 1)
                {
                    ObjectID view_ident = new ObjectID(obj.m_nIdent,0);
                    ObjectID item_idnet = new ObjectID(obj.m_nSerial,0);
                    if (m_gameClient != null)
                    {
                        GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_idnet);
                        if (viewObj != null)
                        {
                            GameRecord record = viewObj.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    //Log.Trace("delete row failed record name =" + recName);
                                }

                            }
                            else
                            {
                                //Log.Trace("no record record name " + recName);
                            }

                        }
                        else
                        {
                            //Log.Trace("no object record name " + recName);
                        }

                    }
                    //end m_gameClient != null
                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordRemoveRow(view_ident, item_idnet, recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddObject(item_idnet);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_viewobj_record_remove_row", m_argList);
                    }

                }//end if (nViewObj == 1)
                else if (nViewObj == 2)
                {
                    if (m_gameClient != null)
                    {
                        GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                        if (scene != null)
                        {
                            GameRecord record = scene.GetGameRecordByName(recName);
                            if (record != null)
                            {
                                if (!record.DeleteRow(nRow))
                                {
                                    //Log.Trace("delete row failed record name " + recName);
                                }

                            }
                            else
                            {
                                //Log.Trace("no record record name " + recName);
                            }

                        }
                        else
                        {
                            //Log.Trace("no object record name " + recName);
                        }


                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordRemoveRow(recName, nRow);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nRow);
                        m_receiver.Excute_CallBack("on_scene_record_remove_row", m_argList);
                    }
                }//end if (nViewObj == 2)
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
                return;
            }
        }

        //SERVER_DELETE_VIEW OK
         public void recvDeleteView(int id, object args)
        {
            //Log.Trace("Recv  Delete View");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);

            uint nViewId = 0;

            if (!loadAr.ReadUInt16(ref nViewId))
            {
                return;
            }

            if (m_gameClient == null)
            {
                //Log.Trace("GameClient is null");
                return;
            }

            ObjectID view_ident = new ObjectID(nViewId,0);
            if (!m_gameClient.DeleteView(view_ident))
            {
                //Log.Trace("delete failed view id = " + nViewId.ToString());
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnDeleteView(view_ident);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddObject(view_ident);
                m_receiver.Excute_CallBack("on_delete_view", m_argList);
            }
        }

        //SERVER_QUEUE
         public void recvQueue(int id, object args)
        {
            //Log.Trace("Recv  Queue");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);

            int nQueue = 0;//队列编号
            int nPosition = 0;//在队列中的位置（为0 表示结束排队）
            int nPriorCount = 0;//绿色通道人数

            if (!loadAr.ReadInt32(ref nQueue))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPosition))
            {
                return;
            }

            if (!loadAr.ReadInt32(ref nPriorCount))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnQueue(nQueue, nPosition, nPriorCount);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nQueue);
                m_argList.AddInt(nPosition);
                m_argList.AddInt(nPriorCount);
                m_receiver.Excute_CallBack("on_queue", m_argList);
            }
        }

        //SERVER_TERMINATE
         public void recvTerminate(int id, object args)
        {
            //Log.Trace("Recv  Terminate");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);

            loadAr.Seek(1);

            int nResult = 0;

            if (!loadAr.ReadInt32(ref nResult))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnTerminate(nResult);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nResult);
                m_receiver.Excute_CallBack("on_terminate", m_argList);
            }
        }
        //SERVER_FROM_GMCC
         public void recvFromGmcc(int id, object args)
        {
            //Log.Trace("Recv  From Gmcc");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);
            int nId = 0;//GM编号
            string name = "";//GM名称
            string info = "";//信息内容

            if (!loadAr.ReadInt32(ref nId))
            {
                return;
            }

            if (!loadAr.ReadWideStrNoLen(ref name))
            {
                return;
            }

            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }

            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnFromGmcc(nId, name, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nId);
                m_argList.AddWideStr(name);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_from_gmcc", m_argList);
            }
        }

        //SERVER_LINK_TO
         public void recvLinkTo(int id, object args)
        {
            //Log.Trace("Recv  From Link To");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                ObjectID objId = new ObjectID();
                ObjectID linkId = new ObjectID();
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                if (!loadAr.ReadObject(ref linkId))
                {
                    return;
                }

                OuterPost outerPost = new OuterPost();
                if (!outerPost.initOuterPost(ref loadAr))
                {
                    return;
                }

                ObjectID ident = objId;
                string link_itent = Tools.GetIdent(linkId);
                if (null == m_gameClient)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent(link_itent);
                sceneObj.SetLinkPos(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLinkTo(ident, link_itent, outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_argList.AddString(link_itent);
                    m_argList.AddFloat(outerPost.x);
                    m_argList.AddFloat(outerPost.y);
                    m_argList.AddFloat(outerPost.z);
                    m_argList.AddFloat(outerPost.orient);
                    m_receiver.Excute_CallBack("on_link_to", m_argList);
                }

            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_UNLINK
         public void recvUnLink(int id, object args)
        {
            //Log.Trace("Recv  From Un Link");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                ObjectID objId = new ObjectID();

                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                ObjectID ident = objId;
                if (null == m_gameClient)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent("");

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnUnlink(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_receiver.Excute_CallBack("on_un_link", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_LINK_MOVE
         public void recvLinkMove(int id, object args)
        {
            //Log.Trace("Recv  From  Link Move");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                if (null == m_gameClient)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                ObjectID objId = new ObjectID();
                ObjectID linkId = new ObjectID();
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }
                if (!loadAr.ReadObject(ref linkId))
                {
                    return;
                }

                OuterPost outerPost = new OuterPost();
                if (!outerPost.initOuterPost(ref loadAr))
                {
                    return;
                }

                ObjectID ident = objId;
                string link_ident = Tools.GetIdent(linkId);


                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLinkIdent(link_ident);
                sceneObj.SetLinkPos(outerPost.x, outerPost.y, outerPost.z, outerPost.orient);
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLinkMove(ident, link_ident, outerPost.x,
                        outerPost.y, outerPost.z, outerPost.orient);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_argList.AddString(link_ident);
                    m_argList.AddFloat(outerPost.x);
                    m_argList.AddFloat(outerPost.y);
                    m_argList.AddFloat(outerPost.z);
                    m_argList.AddFloat(outerPost.orient);
                    m_receiver.Excute_CallBack("on_link_move", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //SERVER_LINK_MOVE
         public void recvCpCustom(int id, object args)
        {
            //Log.Trace("Recv Cp Custom");

            byte[] data = (byte[])args;
            byte[] content = new byte[data.Length - 1];
            System.Array.Copy(data, 1, content, 0, content.Length);
            byte[] contentBytes = QuickLZ.decompress(content);

            byte[] dataTemp = new byte[contentBytes.Length + 1];
            //System.Array.Copy(temp, 0, stateObject.tempBuffer, 0, temp.Length);
            System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
            System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)

            //调用非压缩的解析方式
            recvCustom(GlobalServerMsg.SERVER_CUSTOM, dataTemp);
        }

        //SERVER_WARNING (34)
         public void recvWarning(int id, object args)
        {
            //Log.Trace("Recv  Warning");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);


            int nType = 0;//告警类型
            string info = "";//告警内容

            if (!loadAr.ReadInt16(ref nType))
            {
                return;
            }

            if (!loadAr.ReadWideStrNoLen(ref info))
            {
                return;
            }
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnWarning(nType, info);
            }
            else
            {
                m_argList.Clear();
                m_argList.AddInt(nType);
                m_argList.AddWideStr(info);
                m_receiver.Excute_CallBack("on_warning", m_argList);
            }
        }

        //SERVER_IDLE
         public void recvIdle(int id, object args)
        {
            //Log.Trace("Recv  Idle");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);
            if (null != m_gameMsgHandle)
            {
                m_gameMsgHandle.OnIdle();
            }
            else
            {
                m_argList.Clear();
                m_receiver.Excute_CallBack("on_idle", m_argList);
            }
        }

        //SERVER_ALL_DEST (OK)
         public void recvAllDest(int id, object args)
        {
            //Log.Trace("Recv  All Dest");

            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                //VarList arg = new VarList();
                GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                if (scene != null)
                {
                    for (int i = 0; i < nCount; i++)
                    {
                        ObjectID obj = new ObjectID();
                        if (!loadAr.ReadObject(ref obj))
                        {
                            //Log.Trace("read object failed");
                            return;
                        }

                        OuterDest dest = new OuterDest();
                        if (!dest.initOuterDest(ref loadAr))
                        {
                            //Log.Trace("initOuterDest failed");
                            return;
                        }

                        ObjectID ident = obj;
                        GameSceneObj sceneObj = scene.GetSceneObjByIdent(ident);
                        if (sceneObj == null)
                        {
                            continue;
                        }

                        sceneObj.SetDestination(dest.x, 0,
                            dest.z, dest.orient, dest.MoveSpeed, dest.RotateSpeed,
                            dest.JumpSpeed, dest.Gravity);

                        sceneObj.SetMode(dest.Mode);

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnMoving(ident);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddObject(ident);
                            m_receiver.Excute_CallBack("on_moving", m_argList);
                        }
                    }//end for(int i= 0)

                }
                else
                {
                    //Log.Trace("no scene");
                }
                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnAllDestination(nCount);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddInt(nCount);
                    m_receiver.Excute_CallBack("on_all_dest", m_argList);
                }

            }//end try
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }//end catch


        }

        //SERVER_MOVING
         public void recvOnMoving(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            try
            {
                if (m_gameClient == null)
                {
                    //Log.Trace("GameClient is null");
                    return;
                }

                if (m_receiver == null)
                {
                    //Log.Trace("GameReceiver is null");
                    return;
                }

                ObjectID obj = new ObjectID();
                OuterDest dest = new OuterDest();
                if (!loadAr.ReadObject(ref obj))
                {
                    return;
                }

                if (!dest.initOuterDest(ref loadAr))
                {
                    //Log.Trace("initOuterDest failed");
                    return;
                }

                ObjectID ident = obj;
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }
                sceneObj.SetDestination(dest.x, 0, dest.z,
                    dest.orient, dest.MoveSpeed, dest.RotateSpeed,
                    dest.JumpSpeed, dest.Gravity);
                sceneObj.SetMode(dest.Mode);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnMoving(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_receiver.Excute_CallBack("on_moving", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }



        }

        //SERVER_LOCATION (31) (OK)
         public void recvOnLocation(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);
            try
            {
                if (m_gameClient == null)
                {
                    return;
                }

                if (m_receiver == null)
                {
                    return;
                }

                ObjectID obj = new ObjectID();
                OuterPost post = new OuterPost();

                if (!loadAr.ReadObject(ref obj))
                {
                    //Log.Trace("read object failed");
                    return;
                }

                if (!post.initOuterPost(ref loadAr))
                {
                    //Log.Trace("init outer post failed");
                    return;
                }

                ObjectID ident = obj;
                GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                if (sceneObj == null)
                {
                    return;
                }

                sceneObj.SetLocation(post.x, post.y, post.z, post.orient);
                sceneObj.SetDestination(post.x, post.y, post.z, post.orient, 0.0f, 0.0f, 0.0f, 0.0f);

                if (null != m_gameMsgHandle)
                {
                    m_gameMsgHandle.OnLocation(ident);
                }
                else
                {
                    m_argList.Clear();
                    m_argList.AddObject(ident);
                    m_receiver.Excute_CallBack("on_location", m_argList);
                }
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }



        }

        //SERVER_RECORD_GRID
         public void recvRecordGrid(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);
            try
            {
                if (m_gameClient == null)
                {
                    return;
                }

                if (m_receiver == null)
                {
                    return;
                }

                int nIsViewObj = 0;
                if (!loadAr.ReadInt8(ref nIsViewObj))
                {
                    return;
                }

                ObjectID objId = new ObjectID();
                if (!loadAr.ReadObject(ref objId))
                {
                    return;
                }

                int nIndex = 0;
                if (!loadAr.ReadInt16(ref nIndex))
                {
                    return;
                }

                int nCount = 0;
                if (!loadAr.ReadInt16(ref nCount))
                {
                    return;
                }

                string recName = m_receiver.GetRecordName(nIndex);
                if (nIsViewObj == 0)
                {
                    ObjectID ident = objId;
                    GameSceneObj sceneObj = m_receiver.GetSceneObj(ident);
                    if (null == sceneObj)
                    {
                        return;
                    }

                    GameRecord record = sceneObj.GetGameRecordByName(recName);
                    if (record != null)
                    {
                        if (!m_receiver.RecvRecordGrid(ref record,
                            nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial, nIndex,
                            ref loadAr, nCount))
                        {
                            //Log.Trace("recv failed");
                        }

                        if (null != m_gameMsgHandle)
                        {
                            m_gameMsgHandle.OnRecordGrid(ident, recName, nCount);
                        }
                        else
                        {
                            m_argList.Clear();
                            m_argList.AddObject(ident);
                            m_argList.AddString(recName);
                            m_argList.AddInt(nCount);
                            m_receiver.Excute_CallBack("on_record_grid", m_argList);
                        }
                    }
                    else
                    {
                        //Log.Trace("no record record name " + recName);
                    }
                }//end if (nIsViewObj == 0)
                else if (nIsViewObj == 3)
                {
                    ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                    GameView viewObj = m_receiver.GetView(view_ident);
                    if (viewObj != null)
                    {
                        GameRecord record = viewObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record,
                                nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial,
                                nIndex, ref loadAr, nCount))
                            {
                                //Log.Trace("recv failed record name " + recName);
                            }
                        }
                        else
                        {
                            //Log.Trace("no record record name " + recName);
                        }

                    }
                    else
                    {
                        //Log.Trace("no object record name " + recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewRecordGrid(view_ident, recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_view_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 3)
                else if (nIsViewObj == 1)
                {
                    ObjectID view_ident = new ObjectID(objId.m_nIdent,0);
                    ObjectID item_ident = new ObjectID(objId.m_nSerial,0);

                    GameViewObj viewObj = m_receiver.GetViewObj(view_ident, item_ident);
                    if (viewObj != null)
                    {
                        GameRecord record = viewObj.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record,
                                nIsViewObj, (int)objId.m_nIdent, (int)objId.m_nSerial,
                                nIndex, ref loadAr, nCount))
                            {
                                //Log.Trace("recv failed record name " + recName);
                            }
                        }
                        else
                        {
                            //Log.Trace("no record record name " + recName);
                        }
                    }
                    else
                    {
                        //Log.Trace("no object record name " + recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnViewObjRecordGrid(view_ident, item_ident, recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddObject(view_ident);
                        m_argList.AddObject(item_ident);
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_viewobj_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 1)
                else if (nIsViewObj == 2)
                {
                    GameScene scene = (GameScene)m_gameClient.GetCurrentScene();
                    if (scene != null)
                    {
                        GameRecord record = scene.GetGameRecordByName(recName);
                        if (record != null)
                        {
                            if (!m_receiver.RecvRecordGrid(ref record, nIsViewObj, (int)objId.m_nIdent,
                                (int)objId.m_nSerial, nIndex, ref loadAr, nCount))
                            {
                                //Log.Trace("recv failed record name " + recName);
                            }

                        }
                        else
                        {
                            //Log.Trace("no reocrd record name " + recName);
                        }

                    }
                    else
                    {
                        //Log.Trace("no obejct record name " + recName);
                    }

                    if (null != m_gameMsgHandle)
                    {
                        m_gameMsgHandle.OnSceneRecordGrid(recName, nCount);
                    }
                    else
                    {
                        m_argList.Clear();
                        m_argList.AddString(recName);
                        m_argList.AddInt(nCount);
                        m_receiver.Excute_CallBack("on_scene_record_grid", m_argList);
                    }

                }//end if (nIsViewObj == 2)
            }
            catch (System.Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        //收到版本号
         public void recvVersion(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            m_argList.Clear();
            int nVersion = 0;
            if (!loadAr.ReadInt16(ref nVersion))
            {
                return;
            }

            m_argList.AddInt(nVersion);
            m_receiver.Excute_CallBack("on_version", m_argList);
        }       

        //平台登陆
         public void recvPlatformLogin(int id, object args)
        {
            byte[] data = (byte[])args;
            loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            m_argList.Clear();
            int accounttype = 0;

            if (!loadAr.ReadInt16(ref accounttype))
            {
                return;
            }
            m_argList.AddInt(accounttype);

            string value = "";
            if (!loadAr.ReadStringNoLen(ref value))
            {
                return ;
            }
            m_argList.AddString(value);

           
            m_receiver.Excute_CallBack("on_platformlogin", m_argList);
        }        
    }
}
