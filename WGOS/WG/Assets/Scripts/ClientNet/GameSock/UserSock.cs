using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Fm_ClientNet
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum SockState
    {
        /// <summary>
        /// 初始化状态
        /// </summary>
        Idle,
        /// <summary>
        /// 正在连接
        /// </summary>
        Connecting,
        /// <summary>
        /// 已连上
        /// </summary>
        Connected,
        /// <summary>
        /// 正在监听
        /// </summary>
        Listening,
        /// <summary>
        /// 连接失败、监听失败、发送数据失败或接收数据失败
        /// </summary>
        Failed
    };

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum SockEventType
    {
        /// <summary>
        /// 接收
        /// </summary>
        Accept,
        /// <summary>
        /// 连接成功
        /// </summary>
        Connected,
        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFail = 10061,
        /// <summary>
        /// 异常错误
        /// </summary>
        Error,
        /// <summary>
        /// 连接被关闭
        /// </summary>
        Closed,
        /// <summary>
        /// 发送消息
        /// </summary>
        Send,
        /// <summary>
        /// 接收消息
        /// </summary>
        Receive,
        /// <summary>
        /// 输出日志
        /// </summary>
        PutInfo,
        /// <summary>
        /// 服务器主动断开
        /// </summary>
        ServerDisconnect = 10054,
        /// <summary>
        /// 保存二进制日志
        /// </summary>
        SaveByteLog,
    };

    /// <summary>
    /// 连接事件
    /// </summary>
    public class SockEvent
    {
        public SockEventType m_Type;
        public Object m_Sock;
        public Object m_Data;

        public SockEvent(SockEventType type, Object sock)
        {
            m_Type = type;
            m_Sock = sock;
            m_Data = null;
        }

        public SockEvent(SockEventType type, Object sock, Object data)
        {
            m_Type = type;
            m_Sock = sock;
            m_Data = data;
        }

        protected SockEvent()
        {
        }

        public SockEventType GetEventType()
        {
            return m_Type;
        }

        public Object GetEventSock()
        {
            return m_Sock;
        }

        public Object GetEventData()
        {
            return m_Data;
        }
    }

    /// <summary>
    /// 通讯错误
    /// </summary>
    public class SockError
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        private int m_nType;
        /// <summary>
        /// 消息
        /// </summary>
        private string m_sMsg;

        public SockError(int type, string msg)
        {
            m_nType = type;
            m_sMsg = msg;
        }

        protected SockError()
        {
        }

        public int GetErrorType()
        {
            return m_nType;
        }

        public string GetErrorMsg()
        {
            return m_sMsg;
        }
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    public class SockMsg
    {
        private byte[] m_Data;
        private int m_nSize;

        public SockMsg(byte[] data, int size)
        {
            m_Data = data;
            m_nSize = size;
        }

        protected SockMsg()
        {
        }

        public int GetMsgSize()
        {
            return m_nSize;
        }

        public byte[] GetMsgData()
        {
            return m_Data;
        }
    }

    /// <summary>
    /// 事件回调接口
    /// </summary>
    public interface IUserSockCallee
    {
        int OnSockConnected(UserSock sock, string addr, int port);
        int OnSockConnectFail(UserSock sock, string addr, int port);
        int OnSockReceive(UserSock sock, byte[] data, int size);
        int OnSockSend(UserSock sock, int size);
        int OnSockClose(UserSock sock);
        int OnSockLog(UserSock sock, string msg);
        int OnSockError(UserSock sock, int error_type, string error_msg);
    }

    /// <summary>
    /// 通讯客户端
    /// </summary>
    public class UserSock
    {
        /// <summary>
        /// 异步辅助类
        /// </summary>
        private class SockAsync
        {
            /// <summary>
            /// 连接
            /// </summary>
            /// <param name="ar"></param>
            public static void Connect(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    //Log.Trace("Idle");
                    return;
                }
                try
                {
                    self.m_Socket.EndConnect(ar);
                }
                catch (SocketException e)
                {
                    //Log.Trace("SocketException e");
                    self.SetSockError(e.ErrorCode, e.Message);
                    self.ConnectFail();
                    return;
                }
                catch (Exception e)
                {
                    //Log.Trace("Exception e" + e.Message);
                    return;
                }
                self.StopConnect();
            }

            /// <summary>
            /// 断开
            /// </summary>
            /// <param name="ar"></param>
            public static void Disconnect(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                try
                {
                    self.m_Socket.EndDisconnect(ar);
                }
                catch (SocketException e)
                {
                    self.SetSockError(e.ErrorCode, e.Message);
                    return;
                }
                catch (Exception)
                {
                    return;
                }
                self.StopDisconnect();
            }

            /// <summary>
            /// 接收
            /// </summary>
            /// <param name="ar"></param>
            public static void Receive(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                int size;
                try
                {
                    size = self.m_Socket.EndReceive(ar);
                }
                catch (SocketException e)
                {
                    if (self.m_Socket.Connected)
					{
                    	self.SetSockError(e.ErrorCode, e.Message);
					}
					else
					{
                        self.Close();
					}
                    return;
                }
                catch (Exception)
                {
                    return;
                }
                self.StopReceive(size);
            }

            /// <summary>
            /// 发送
            /// </summary>
            /// <param name="ar"></param>
            public static void Send(IAsyncResult ar)
            {
                UserSock self = (UserSock)ar.AsyncState;
                Log.Trace(self.m_State.ToString());
                if (self.m_State == SockState.Idle)
                {
                    return;
                }
                int size;
                try
                {
                    size = self.m_Socket.EndSend(ar);
                    Log.Trace("size " + size);
                }
                catch (SocketException e)
                {
                    self.SetSockError(e.ErrorCode, e.Message);
                    return;
                }
                catch (Exception)
                {
                    return;
                }
                self.StopSend(size);
            }
        }
        /// <summary>
        /// 接口
        /// </summary>
        private IUserSockCallee m_Callee;
        /// <summary>
        /// socket
        /// </summary>
        private Socket m_Socket;
        /// <summary>
        /// 服务器地址
        /// </summary>
        private string m_sAddr;
        /// <summary>
        /// 服务器端口号
        /// </summary>
        private int m_nPort;
        /// <summary>
        /// 错误类型
        /// </summary>
        private int m_nError = 0;
        /// <summary>
        /// 接受个数
        /// </summary>
        private int m_nRecvCount = 0;
        /// <summary>
        /// 前一个接收的byte
        /// </summary>
        private byte m_nRecvPrior = 0;
        /// <summary>
        /// 接收的BYTE大小
        /// </summary>
        private byte[] m_RecvBuf = new byte[0x1280];//2800 Mark
        /// <summary>
        /// 异步接收数据接口
        /// </summary>
        AsyncEvent m_AsyncEvent = new AsyncEvent();
        AsyncEvent.RecvBufferData m_RecvBufferData = null;
        /// <summary>
        /// 消息发送队列
        /// </summary>
        private Queue m_SendQueue = new Queue();
        /// <summary>
        /// 状态
        /// </summary>
        private SockState m_State = SockState.Idle;
        /// <summary>
        /// 消息发送锁
        /// </summary>
        private static Object s_SendinLock = new Object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callee">接口</param>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        public UserSock(IUserSockCallee callee, string addr, int port)
        {
            m_Callee = callee;
            m_sAddr = addr;
            m_nPort = port;

            InitSock(null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callee">地址</param>
        /// <param name="sock">端口号</param>
        public UserSock(IUserSockCallee callee, Socket sock)
        {
            m_Callee = callee;
            m_sAddr = "";
            m_nPort = 0;

            InitSock(sock);
        }

        protected UserSock()
        {
        }


        /// <summary>
        /// 初始化SOCKET
        /// </summary>
        /// <param name="sock"></param>
        private void InitSock(Socket sock)
        {
            if (sock == null)
            {
                if (m_Socket != null) m_Socket.Close();
                if (SysUtils.SysUtil.IsIos)
                {
                    m_Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }
                else
                {
                    m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            }
            else
            {
                m_Socket = sock;
            }
            //m_Socket.SendBufferSize = 64*1024;
            m_Socket.ReceiveBufferSize = 64 * 1024;

            m_Socket.Blocking = false;
            m_Socket.NoDelay = true;

            MsgEncode.Instance.Init();
        }

        /// <summary>
        /// 初始化平台设置
        /// </summary>
        public void InitSet(bool block)
        {
            m_Socket.Blocking = block;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            m_SendQueue.Clear();
            m_nRecvCount = 0;
            m_nRecvPrior = 0;
            SetSockState(SockState.Idle);

        }

        /// <summary>
        /// 获得socket
        /// </summary>
        /// <returns></returns>
        public Socket GetSocket()
        {
            return m_Socket;
        }

        /// <summary>
        /// 目标地址
        /// </summary>
        /// <returns></returns>
        public string GetDestAddr()
        {
            return m_sAddr;
        }

        /// <summary>
        /// 目标端口
        /// </summary>
        /// <returns></returns>
        public int GetDestPort()
        {
            return m_nPort;
        }

        /// <summary>
        /// 获得本地地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalAddr()
        {
            IPEndPoint ep = (IPEndPoint)m_Socket.LocalEndPoint;
            if (null == ep)
            {
                return "";
            }
            return ep.Address.ToString();
        }

        /// <summary>
        /// 获得本地端口号
        /// </summary>
        /// <returns></returns>
        public int GetLocalPort()
        {
            IPEndPoint ep = (IPEndPoint)m_Socket.LocalEndPoint;
            if (null == ep)
            {
                return 0;
            }
            return ep.Port;
        }

        /// <summary>
        /// 获得目标地址
        /// </summary>
        /// <returns></returns>
        public string GetRemoteAddr()
        {
            IPEndPoint ep = (IPEndPoint)m_Socket.RemoteEndPoint;
            if (null == ep)
            {
                return "";
            }
            return ep.Address.ToString();
        }

        /// <summary>
        /// 获得目标端口号
        /// </summary>
        /// <returns></returns>
        public int GetRemotePort()
        {
            IPEndPoint ep = (IPEndPoint)m_Socket.RemoteEndPoint;
            if (null == ep)
            {
                return 0;
            }
            return ep.Port;
        }

        /// <summary>
        /// 是否连接成功
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            return m_Socket.Connected;
        }

        /// <summary>
        /// 验证连接状态是否正确
        /// </summary>
        /// <returns></returns>
        public bool VerifyConnect()
        {
            return (m_State == SockState.Connected) && m_Socket.Connected;
        }

        /// <summary>
        /// 回调接口
        /// </summary>
        /// <returns></returns>
        public IUserSockCallee GetSockCallee()
        {
            return m_Callee;
        }

        /// <summary>
        /// 获得当前状态
        /// </summary>
        /// <returns></returns>
        public SockState GetSockState()
        {
            return m_State;
        }

        /// <summary>
        /// 获得错误码
        /// </summary>
        /// <returns></returns>
        public int GetSockError()
        {
            return m_nError;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        public void SetSockState(SockState state)
        {
            //lock (m_StateLock)
            {
                m_State = state;
                //Log.Trace("state " + m_State.ToString());
            }
        }

        /// <summary>
        /// 设置错误
        /// </summary>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        private void SetSockError(int error, string msg)
        {
            m_nError = error;
            m_Callee.OnSockError(this, error, msg);
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="msg">消息</param>
        private void SetSockLog(string msg)
        {
            m_Callee.OnSockLog(this, msg);
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <returns></returns>
        private bool ConnectFail()
        {
            SetSockState(SockState.Failed);
            m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTFAILED, m_sAddr, m_nPort);
            //m_Callee.OnSockConnectFail(this, m_sAddr, m_nPort);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool StopConnect()
        {
            if (m_Socket.Connected)
            {
                //Log.Trace("OnSockConnected(this, m_sAddr, m_nPort)");
                SetSockState(SockState.Connected);
                m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTED, m_sAddr, m_nPort);
                //m_Callee.OnSockConnected(this, m_sAddr, m_nPort);
                byte[] abySendMsg = null;
                lock (s_SendinLock)
                {
                    if (m_SendQueue.Count > 0)
                    {
                        abySendMsg = (byte[])m_SendQueue.Dequeue();
                    }
                }

                if (abySendMsg != null)
                {
                    SendData(abySendMsg, abySendMsg.Length);
                }
                return StartReceive();
            }
            else
            {
                //Log.Trace("ConnectFail()");
                return ConnectFail();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        private bool StopDisconnect()
        {
            //m_Callee.OnSockClose(this);
            //m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED);
            /*Close();
            return true;*/
			return Close();
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        private bool StartReceive()
        {
            try
            {
                m_RecvBufferData = m_AsyncEvent.GetBufferData();
                m_Socket.BeginReceive(m_RecvBufferData.m_abyRecvBuffer, 0, AsyncEvent.ASYNCRECVBUF_LENGTH,
                            SocketFlags.None, new AsyncCallback(SockAsync.Receive), this);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.Message);
                SetSockState(SockState.Failed);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool StopReceive(int size)
        {
            if (size > 0)
            {
                m_RecvBufferData.m_iRecvSize = size;
                m_AsyncEvent.PushData(m_RecvBufferData);
                return StartReceive();
            }
            else
            {
                //m_Callee.OnSockClose(this);
                //m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED);
                return Close();
            }
        }

        public void ProcessMessage()
        {
            m_AsyncEvent.Swap();

            AsyncEvent.RecvBufferData Data = null;
            AsyncEvent.EventData Event = null;

            while (m_AsyncEvent.PeekData(ref Data, ref Event))
            {
                if (Data != null)
                {
                    ReceiveData(Data.m_abyRecvBuffer, Data.m_iRecvSize);
                }
                else
                {
                    switch (Event.m_eEvent)
                    {
                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTED:
                            m_Callee.OnSockConnected(this, Event.m_strIP, Event.m_iPort);
                            break;

                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CONNECTFAILED:
                            m_Callee.OnSockConnectFail(this, Event.m_strIP, Event.m_iPort);
                            break;

                        case AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED:
                            m_Callee.OnSockClose(this);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 发送下一个消息
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool StopSend(int size)
        {
            m_Callee.OnSockSend(this, size);

            byte[] abySendMsg = null;
            lock (s_SendinLock)
            {
                if (m_SendQueue.Count > 0)
                {
                    abySendMsg = (byte[])m_SendQueue.Dequeue();
                }
            }

            if (abySendMsg != null)
            {
                return SendData(abySendMsg, abySendMsg.Length);
            }

            return true;
        }

        /// <summary>
        ///  接受连接后(Accept)初始化连接
        /// </summary>
        /// <returns></returns>
        public bool Attach()
        {
            return StopConnect();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            return Open("", 0,"");
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool Open(string addr, int port, string key)
        {
            if (m_State == SockState.Failed || m_State == SockState.Connecting)
            {
                InitSock(null);
                Init();
            }

            //if (m_State != SockState.Idle)
            //{
            //    return false;
            //}

            m_nError = 0;
            if (addr.Length > 0)
            {
                m_sAddr = addr;
                m_nPort = port;
            }
            try
            {
                IPAddress ipad = IPAddress.Parse(m_sAddr);
                IPEndPoint ipe = new IPEndPoint(ipad, m_nPort);

                //IPHostEntry ipHost = Dns.GetHostEntry(m_sAddr);
                //IPEndPoint ipe = new IPEndPoint(ipHost.AddressList[0], m_nPort);

                SetSockState(SockState.Connecting);
                m_Socket.BeginConnect(ipe, new AsyncCallback(SockAsync.Connect), this);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.Message);
                SetSockState(SockState.Failed);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (m_State == SockState.Idle)
            {
                return true;
            }
            try
            {
                if (m_Socket.Connected)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                m_Socket.Close();
                //直接给逻辑回调
                //m_Callee.OnSockClose(this);
                m_AsyncEvent.PushEvent(AsyncEvent.e_SocketEvent.E_SOCKETEVENT_CLOSED);
                InitSock(null);
            }
            catch (SocketException e)
            {
                m_State = SockState.Idle;
                SetSockError(e.ErrorCode, e.Message);
            }
            Init();
            return true;
        }

        /// <summary>
        /// 停止连接
        /// </summary>
        /// <returns></returns>
        public bool Shut()
        {
            try
            {
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                SetSockError(e.ErrorCode, e.Message);
                SetSockState(SockState.Failed);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public bool Send(byte[] data, int size)
        {
            Log.Trace("Send 1");
            // 正在连接
            if (m_State == SockState.Connecting)
            {
                lock (s_SendinLock)
                {
                    AddSendQueue(data, size);
                }
                return true;
            }

            Log.Trace("Send 2");

            if (m_State != SockState.Connected)
            {
                return false;
            }

            Log.Trace("Send 3");

            if (!m_Socket.Connected)
            {
                return false;
            }

            Log.Trace("Send 4");

            lock (s_SendinLock)
            {
                if (m_SendQueue.Count > 0)
                {
                    AddSendQueue(data, size);
                    return true;
                }
            }
            Log.Trace("SendData");
                    // 保证发送的顺序
            return SendData(data, size);
        }


        /// <summary>
        /// 添加消息到队列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private void AddSendQueue(byte[] data, int size)
        {
            byte[] msg = new byte[size];
            Array.Copy(data, msg, size);
            m_SendQueue.Enqueue(msg);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        private bool SendData(byte[] data, int size)
        {
            byte[] buf = new byte[size * 2 + 2];
            int nIndex = 0;

            if (!MsgEncode.Instance.Ready)
            {
                //不编码                
                for (int i = 0; i < size; i++)
                {
                    buf[nIndex++] = data[i];
                    if (data[i] == 0xEE)
                    {
                        buf[nIndex++] = 0;
                    }
                }
            }
            else
            {
                //编码
                nIndex = MsgEncode.Instance.Encode(buf, data, size);              
            }

            buf[nIndex++] = 0xEE;
            buf[nIndex++] = 0xEE;

            try
            {
                /*
                if (buf.Length > 24 && buf[24].Equals(211))
                {
                    Log.Trace("send 211");
                }
                */
                Log.Trace("BeginSend " + buf.Length + " " + m_sAddr + " " + m_nPort);
                m_Socket.BeginSend(buf, 0, nIndex,
                    SocketFlags.None, new AsyncCallback(SockAsync.Send), this);
            }
            catch (SocketException e)
            {
                Log.Trace("SocketException " + e.Message);
                SetSockError(e.ErrorCode, e.Message);
                SetSockState(SockState.Failed);
                return false;
            }
            buf = null;
            return true;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        private bool ReceiveData(byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                // IsEnd
                if ((0xEE == data[i]) && (0xEE == m_nRecvPrior))
                {
                    m_nRecvCount--;
                    if (m_nRecvCount > 0)
                    {
                        //解码
                        byte[] byteTemp = new byte[m_nRecvCount];
                        System.Array.Copy(m_RecvBuf, 0, byteTemp, 0, m_nRecvCount);

                        if (MsgEncode.Instance.Ready)
                        {
                            MsgEncode.Instance.Decode(byteTemp, m_nRecvCount);
                        }

                        //m_Callee.OnSockReceive(this, m_RecvBuf, m_nRecvCount);
                        m_Callee.OnSockReceive(this, byteTemp, m_nRecvCount);

                    }
                    m_nRecvPrior = 0;
                    m_nRecvCount = 0;
                    continue;
                }
                else if ((0 == data[i]) && (0xEE == m_nRecvPrior))
                {
                }
                else
                {
                    if (m_nRecvCount < m_RecvBuf.Length)
                    {
                        m_RecvBuf[m_nRecvCount++] = data[i];
                    }
                    else
                    {
                        //m_nRecvCount = 0;
                        byte[] mRecvBuf = new byte[m_RecvBuf.Length + 1000];
                        string info = string.Format(" - ReceiveData : Add Receive Buff Length ,Src = {0},Des = {1}", m_RecvBuf.Length, mRecvBuf.Length);
                        Log.Trace(info);
                        Array.Copy(m_RecvBuf, mRecvBuf, m_RecvBuf.Length);
                        m_RecvBuf = mRecvBuf;
                        m_RecvBuf[m_nRecvCount++] = data[i];
                        mRecvBuf = null;
                    }
                }
                m_nRecvPrior = data[i];
            }
            return true;
        }
    }
}
