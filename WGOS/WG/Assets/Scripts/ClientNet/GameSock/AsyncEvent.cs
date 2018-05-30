//USRecvBuff.cs
//接收缓冲类

using System;
using System.Collections;
using System.Collections.Generic;
using SysUtils;

namespace Fm_ClientNet
{
    public class AsyncEvent
    {
        #region 数据接收缓冲结构定义

        public const int ASYNCRECVBUF_COUNT = 128; //缓冲数量64  Mark
        public const int ASYNCRECVBUF_LENGTH = 0x1280; //缓冲体大小1000

        //消息接收结构体
        public class RecvBufferData
        {
            public byte[] m_abyRecvBuffer = new byte[ASYNCRECVBUF_LENGTH];
            public int m_iRecvSize = 0;
        }

        public enum e_SocketEvent
        {
            E_SOCKETEVENT_CONNECTED,
            E_SOCKETEVENT_CONNECTFAILED,
            E_SOCKETEVENT_CLOSED,
        }

        //事件结构体
        public class EventData
        {
            public e_SocketEvent m_eEvent;
            public string m_strIP;
            public int m_iPort;

            public EventData(e_SocketEvent eEvent)
            {
                m_eEvent = eEvent;
            }

            public EventData(e_SocketEvent eEvent, string strIP, int iPort)
            {
                m_eEvent = eEvent;
                m_strIP = strIP;
                m_iPort = iPort;
            }
        }

        //空闲缓冲队列
        private Queue<RecvBufferData> m_queueRecvBufferFree = new Queue<RecvBufferData>();
        //读写缓冲队列
        private List<RecvBufferData>[] m_RecvDataList = new List<RecvBufferData>[2] { new List<RecvBufferData>(), new List<RecvBufferData>() };
        private List<EventData>[] m_RecvEventList = new List<EventData>[2] { new List<EventData>(), new List<EventData>() };

        private int m_iReadDataIdx = 0; //读缓冲队列索引
        private int m_iWriteDataIdx = 1; //写缓冲队列索引

        private int m_iCurReadDataIdx = 0; //当前数据包读缓冲位置
        private int m_iCurReadEventIdx = 0; //当前事件包读缓冲位置

        //互斥锁声明
        private static Object s_AsyncLock = new Object();

        #endregion

        public AsyncEvent()
        {
            //初始化缓冲队列
            for (int i = 0; i < ASYNCRECVBUF_COUNT; ++i)
            {
                m_queueRecvBufferFree.Enqueue(new RecvBufferData());
            }
        }

        //获得空闲数据缓冲
        public RecvBufferData GetBufferData()
        {
            RecvBufferData FreeData = null;

            System.Threading.Monitor.Enter(s_AsyncLock);
            try
            {
                while (m_queueRecvBufferFree.Count < 1)
                {
                    //无可用资源则等待其他线程释放资源后继续执行
                    System.Threading.Monitor.Exit(s_AsyncLock);
                    System.Threading.Thread.Sleep(10);
                    System.Threading.Monitor.Enter(s_AsyncLock);
                }

                FreeData = m_queueRecvBufferFree.Dequeue();
            }
            finally
            {
                System.Threading.Monitor.Exit(s_AsyncLock);

                FreeData = new RecvBufferData();
            }

            return FreeData;
        }

        //将网络数据压入队列
        public void PushData(RecvBufferData Data)
        {
            lock (s_AsyncLock)
            {
                m_RecvDataList[m_iWriteDataIdx].Add(Data);
            }
        }

        //将消息体压入链表
        public void PushEvent(e_SocketEvent eEvent, string strIP, int iPort)
        {
            lock (s_AsyncLock)
            {
                m_RecvEventList[m_iWriteDataIdx].Add(new EventData(eEvent, strIP, iPort));
            }
        }
        public void PushEvent(e_SocketEvent eEvent)
        {
            lock (s_AsyncLock)
            {
                m_RecvEventList[m_iWriteDataIdx].Add(new EventData(eEvent));
            }
        }

        bool InnerPeekData(ref RecvBufferData Data)
        {
            if (m_iCurReadDataIdx >= m_RecvDataList[m_iReadDataIdx].Count)
            {
                Data = null;
                return false;
            }

            Data = m_RecvDataList[m_iReadDataIdx][m_iCurReadDataIdx++];
            return true;
        }

        bool InnerPeekEvent(ref EventData Event)
        {
            if (m_iCurReadEventIdx >= m_RecvEventList[m_iReadDataIdx].Count)
            {
                Event = null;
                return false;
            }

            Event = m_RecvEventList[m_iReadDataIdx][m_iCurReadEventIdx++];
            return true;
        }

        //读取链表缓冲数据
        public bool PeekData(ref RecvBufferData Data, ref EventData Event)
        {
            if (InnerPeekData(ref Data))
            {
                return true;
            }

            return InnerPeekEvent(ref Event);
        }

        //交换前后端缓冲
        public void Swap()
        {
            lock (s_AsyncLock)
            {
                //释放事件缓冲
                m_RecvEventList[m_iReadDataIdx].Clear();
                //释放网络数据缓冲
                int iCount = m_RecvDataList[m_iReadDataIdx].Count;
                for (int i = 0; i < iCount; ++i)
                {
                    m_queueRecvBufferFree.Enqueue(m_RecvDataList[m_iReadDataIdx][i]);
                }
                m_RecvDataList[m_iReadDataIdx].Clear();

                m_iCurReadDataIdx = 0; //当前数据包读缓冲位置
                m_iCurReadEventIdx = 0; //当前事件包读缓冲位置

                if (m_iReadDataIdx == 0)
                {
                    m_iReadDataIdx = 1;
                    m_iWriteDataIdx = 0;
                }
                else
                {
                    m_iReadDataIdx = 0;
                    m_iWriteDataIdx = 1;
                }
            }
        }
    }
}