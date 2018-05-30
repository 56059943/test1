
using System;
using System.Collections.Generic;
using System.IO;
using SysUtils;

namespace Fm_ClientNet
{
    class SocketRevMsgHandle
    {
        public delegate void MsgEevent(int id, object args);
        private Dictionary<int, MsgEevent> m_handlers = new Dictionary<int, MsgEevent>();

        RecvPacket mRecvPacket = null;

        public SocketRevMsgHandle()
        {
            mRecvPacket = new RecvPacket();

            //add event

            //SERVER_SET_VERIFY (1)
            AddEvent(GlobalServerMsg.SERVER_SET_VERIFY, mRecvPacket.recvSetVerify);
            //SERVER_SET_ENCODE (2)
            AddEvent(GlobalServerMsg.SERVER_SET_ENCODE, mRecvPacket.recvSetEncode);
            //SERVER_ERROR_CODE (3)
            AddEvent(GlobalServerMsg.SERVER_ERROR_CODE, mRecvPacket.recvErrorCode);
            //AddEvent(GlobalServerMsg.SERVER_IDLE, mRecvPacket.recvIdle);
            //SERVER_LOGIN_SUCCEED(4)
            AddEvent(GlobalServerMsg.SERVER_LOGIN_SUCCEED, mRecvPacket.recvLoginSuccess);
            //SERVER_WORLD_INFO (5)
            AddEvent(GlobalServerMsg.SERVER_WORLD_INFO, mRecvPacket.recvWorldInfo);
            //SERVER_IDLE (6)
            AddEvent(GlobalServerMsg.SERVER_IDLE, mRecvPacket.recvIdle);
            //SERVER_QUEUE (7)
            AddEvent(GlobalServerMsg.SERVER_QUEUE, mRecvPacket.recvQueue);
            //SERVER_TERMINATE (8)
            AddEvent(GlobalServerMsg.SERVER_TERMINATE, mRecvPacket.recvTerminate);
            //SERVER_PROPERTY_TABLE(9)
            AddEvent(GlobalServerMsg.SERVER_PROPERTY_TABLE, mRecvPacket.recvPropertyTable);
            //SERVER_RECORD_TABLE (10)
            AddEvent(GlobalServerMsg.SERVER_RECORD_TABLE, mRecvPacket.recvRecordTable);
            //SERVER_ENTRY_SCENE (11)
            AddEvent(GlobalServerMsg.SERVER_ENTRY_SCENE, mRecvPacket.recvEntryScene);
            //SERVER_EXIT_SCENE (12)
            AddEvent(GlobalServerMsg.SERVER_EXIT_SCENE, mRecvPacket.recvExitScene);
            //SERVER_ADD_OBJECT (13)
            AddEvent(GlobalServerMsg.SERVER_ADD_OBJECT, mRecvPacket.recvAddObj);
            //SERVER_REMOVE_OBJECT (14)
            AddEvent(GlobalServerMsg.SERVER_REMOVE_OBJECT, mRecvPacket.recvRemoveObj);
            //SERVER_SCENE_PROPERTY (15)
            AddEvent(GlobalServerMsg.SERVER_SCENE_PROPERTY, mRecvPacket.recvSceneProperty);
            //SERVER_OBJECT_PROPERTY (16)
            AddEvent(GlobalServerMsg.SERVER_OBJECT_PROPERTY, mRecvPacket.recvObjectProperty);
            //SERVER_RECORD_ADDROW (17)
            AddEvent(GlobalServerMsg.SERVER_RECORD_ADDROW, mRecvPacket.recvRecordAddRow);
            //SERVER_RECORD_DELROW (18)
            AddEvent(GlobalServerMsg.SERVER_RECORD_DELROW, mRecvPacket.recvRecordDelRow);
            //SERVER_RECORD_GRID(19)
            AddEvent(GlobalServerMsg.SERVER_RECORD_GRID, mRecvPacket.recvRecordGrid);
            //SERVER_RECORD_CLEAR (20)
            AddEvent(GlobalServerMsg.SERVER_RECORD_CLEAR, mRecvPacket.recvRecordClear);
            //SERVER_CREATE_VIEW (21)
            AddEvent(GlobalServerMsg.SERVER_CREATE_VIEW, mRecvPacket.recvCreateView);
            //SERVER_DELETE_VIEW (22)
            AddEvent(GlobalServerMsg.SERVER_DELETE_VIEW, mRecvPacket.recvDeleteView);
            //SERVER_VIEW_PROPERTY (23)
            AddEvent(GlobalServerMsg.SERVER_VIEW_PROPERTY, mRecvPacket.recvViewProperty);
            //SERVER_VIEW_ADD (24)
            AddEvent(GlobalServerMsg.SERVER_VIEW_ADD, mRecvPacket.recvViewAdd);
            //SERVER_VIEW_REMOVE (25)
            AddEvent(GlobalServerMsg.SERVER_VIEW_REMOVE, mRecvPacket.recvViewRemove);
            //SERVER_SPEECH (26)
            AddEvent(GlobalServerMsg.SERVER_SPEECH, mRecvPacket.recvSpeech);
            //SERVER_SYSTEM_INFO (27)
            AddEvent(GlobalServerMsg.SERVER_SYSTEM_INFO, mRecvPacket.recvSystemInfo);
            //SERVER_MENU (28)
            AddEvent(GlobalServerMsg.SERVER_MENU, mRecvPacket.recvMenu);
            //SERVER_CLEAR_MENU (29)
            AddEvent(GlobalServerMsg.SERVER_CLEAR_MENU, mRecvPacket.recvClearMenu);
            //SERVER_CUSTOM (30) (OK)
            AddEvent(GlobalServerMsg.SERVER_CUSTOM, mRecvPacket.recvCustom);
            //SERVER_LOCATION (31) (OK)
            AddEvent(GlobalServerMsg.SERVER_LOCATION, mRecvPacket.recvOnLocation);
            //SERVER_MOVING (32)
            AddEvent(GlobalServerMsg.SERVER_MOVING, mRecvPacket.recvOnMoving);
            //SERVER_ALL_DEST (33) (OK)
            AddEvent(GlobalServerMsg.SERVER_ALL_DEST, mRecvPacket.recvAllDest);
            //SERVER_WARNING (34)
            AddEvent(GlobalServerMsg.SERVER_WARNING, mRecvPacket.recvWarning);
            //SERVER_FROM_GMCC (35)
            AddEvent(GlobalServerMsg.SERVER_FROM_GMCC, mRecvPacket.recvFromGmcc);
            //SERVER_LINK_TO (36)
            AddEvent(GlobalServerMsg.SERVER_LINK_TO, mRecvPacket.recvLinkTo);
            //SERVER_UNLINK(37) 
            AddEvent(GlobalServerMsg.SERVER_UNLINK, mRecvPacket.recvUnLink);
            //SERVER_LINK_MOVE (38)
            AddEvent(GlobalServerMsg.SERVER_LINK_MOVE, mRecvPacket.recvLinkMove);
            //SERVER_CP_CUSTOM (39)
            AddEvent(GlobalServerMsg.SERVER_CP_CUSTOM, mRecvPacket.recvCpCustom);
            //SERVER_CP_ADD_OBJECT (40)
            AddEvent(GlobalServerMsg.SERVER_CP_ADD_OBJECT, mRecvPacket.recvCpAddObject);
            //SERVER_CP_RECORD_ADDROW (41)
            AddEvent(GlobalServerMsg.SERVER_CP_RECORD_ADDROW, mRecvPacket.recvCpRecordAddRow);
            //SERVER_CP_VIEW_ADD (42)
            AddEvent(GlobalServerMsg.SERVER_CP_VIEW_ADD, mRecvPacket.recvCpViewAdd);
            //SERVER_VIEW_CHANGE (43)
            AddEvent(GlobalServerMsg.SERVER_VIEW_CHANGE, mRecvPacket.recvViewChange);
            //SERVER_CP_ALL_DEST (44)
            AddEvent(GlobalServerMsg.SERVER_CP_ALL_DEST, mRecvPacket.recvCpAllDest);
            //SERVER_ALL_PROP (45)
            AddEvent(GlobalServerMsg.SERVER_ALL_PROP, mRecvPacket.recvAllProp);
            //SERVER_CP_ALL_PROP (46)
            AddEvent(GlobalServerMsg.SERVER_CP_ALL_PROP, mRecvPacket.recvCpAllProp);
            //SERVER_ADD_MORE_OBJECT (47)
            AddEvent(GlobalServerMsg.SERVER_ADD_MORE_OBJECT, mRecvPacket.recvAddMoreObject);
            //SERVER_CP_ADD_MORE_OBJECT (48)
            AddEvent(GlobalServerMsg.SERVER_CP_ADD_MORE_OBJECT, mRecvPacket.recvCpAddMoreObject);
            //SERVER_REMOVE_MORE_OBJECT (49)
            AddEvent(GlobalServerMsg.SERVER_REMOVE_MORE_OBJECT, mRecvPacket.recvRemoveMoreObject);
            //SERVER_CHARGE_VALIDSTRING (50)
            //AddEvent(GlobalServerMsg.SERVER_CHARGE_VALIDSTRING, mRecvPacket.recvChargeValidString);
            //SERVER_VERSION(50)
            AddEvent(GlobalServerMsg.SERVER_VERSION, mRecvPacket.recvVersion);

            AddEvent(GlobalServerMsg.SERVER_PLATFROM_LOGIN, mRecvPacket.recvPlatformLogin);


        }
        public RecvPacket GetRecvPacket()
        {
            return mRecvPacket;
        }

        private bool AddEvent(int id, MsgEevent msg)
        {
            if (m_handlers.ContainsKey(id))
            {
                m_handlers.Remove(id);
            }
            m_handlers.Add(id, msg);
            return true;
        }

        public void RemoveEvent(int id)
        {
            if (m_handlers.ContainsKey(id))
            {
                m_handlers.Remove(id);
            }
        }

        public void ExcuteEvent(int id, object args)
        {
            if (!m_handlers.ContainsKey(id))
            {
                Log.TraceWarning("DispatchNow: has not id matched for:" + id);
                return;
            }

            MsgEevent handler = m_handlers[id];
            if (handler != null)
            {
                handler(id, args);
            }
        }

        public void HandleMessage(byte[] data, int size)
        {
            if (data.Length == 0 || 0 == size)
            {
                Log.TraceError("Error, HandleMessage data is 0 ");
                return;
            }

            //byte[] byteTemp = new byte[size];
            //System.Array.Copy(data, 0, byteTemp, 0, size);

            //init LoadArchive
            //LoadArchive loadAr = new LoadArchive(byteTemp, 0, byteTemp.Length);

			//LoadArchive loadAr = new LoadArchive(data, 0, size);

			int protocolId = data[0];
			/*int protocolId = 0;
            bool bRet = loadAr.ReadInt8(ref protocolId);
            if (!bRet)
            {
                //Log.Trace("SocketReceiveHandle::HandleMessage parse get protocol id failed!");
                return;
            }*/

            //if (protocolId == GlobalServerMsg.SERVER_IDLE)
            //{
            //    //不处理
            //    return;
            //}

            /*
            LoadArchive loadAr = new LoadArchive(data, 0, data.Length);
            loadAr.Seek(1);

            int nArgNum = 0;
            loadAr.ReadInt16(ref nArgNum);
            int nType = 0;
            loadAr.ReadInt8(ref nType);
            int value = 0;
            loadAr.ReadInt32(ref value);

            if (value.Equals(1401))
            {
                loadAr.ReadInt8(ref nType);
                int type = 0;
                loadAr.ReadInt32(ref type);
                if (type.Equals(1))
                {
                    Log.Trace("Receive msg id = " + value);
                }
            }
            */

            ExcuteEvent(protocolId, data);

            if (protocolId.Equals(GlobalServerMsg.SERVER_CP_CUSTOM))
            {
                byte[] content = new byte[data.Length - 1];
                System.Array.Copy(data, 1, content, 0, content.Length);
                byte[] contentBytes = QuickLZ.decompress(content);

                byte[] dataTemp = new byte[contentBytes.Length + 1];
                System.Array.Copy(data, 0, dataTemp, 0, 1);//拷贝消息id
                System.Array.Copy(contentBytes, 0, dataTemp, 1, contentBytes.Length);//拷贝解压后消息体(除去消息头)
                data = dataTemp;
            }

            if (protocolId.Equals(GlobalServerMsg.SERVER_CUSTOM) || protocolId.Equals(GlobalServerMsg.SERVER_CP_CUSTOM))
            {
                LoadArchive loadAr = new LoadArchive(data, 0, data.Length);
                loadAr.Seek(1);
                int nArgNum = 0;
                loadAr.ReadInt16(ref nArgNum);
                int nType = 0;
                loadAr.ReadInt8(ref nType);
                loadAr.ReadInt32(ref protocolId);
            }
            CustomSystem.Instance.dispacthEvent(0, protocolId);
        }
    }
}
