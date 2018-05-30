using System;
using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;

using SysUtils;
using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameSock : IGameSock
    {

        //sock 接口回调相关
        public class SockCall : IUserSockCallee
        {
            private GameSock m_gamesock = null;
            //private bool m_bCallfun;    //是否使用回调函数 true 回调函数 false 接口方法

            public void SetGameSock(GameSock sock)
            {
                m_gamesock = sock;
            }

            public int OnSockConnected(UserSock sock, string addr, int port)
            {
                Log.Trace("OnSockConnected");
                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_connected(addr, port);
                    return 1;
                }
                else
                {
                    VarList args = new VarList();
                    if (m_gamesock != null)
                    {
                        args.AddString(addr);
                        args.AddInt(port);
                        m_gamesock.Excute_CallBack("on_connected", args);
                        return 1;
                    }
                    else
                    {
                        //Log.TraceError("Error, OnSockConnected gamesock is null");
                    }
                    return 0;
                }
            }


            public int OnSockConnectFail(UserSock sock, string addr, int port)
            {
                //Log.Trace("OnSockConnectFail");

                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_connect_failed(addr, port);
                    return 1;
                }
                else
                {
                    if (m_gamesock != null)
                    {
                        VarList args = new VarList();
                        args.AddString(addr);
                        args.AddInt(port);
                        m_gamesock.Excute_CallBack("on_connect_fail", args);
                        return 1;
                    }
                    else
                    {
                        //Log.TraceError("Error, OnSockConnectFail gamesock is null");
                    }
                    return 0;
                }
            }


            public int OnSockClose(UserSock sock)
            {
                //Log.Trace("OnSockClose");

                if (null != m_gamesock.m_lgsockcall)
                {
                    m_gamesock.m_lgsockcall.on_close();
                    return 1;
                }
                else
                {
                    if (m_gamesock != null)
                    {
                        m_gamesock.Excute_CallBack("on_close", new VarList());
                        return 1;

                    }
                    else
                    {
                        //Log.TraceError("Error, OnSockClose gamesock is null");
                    }
                    return 0;
                }
            }

            public int OnSockReceive(UserSock sock, byte[] data, int size)
            {
                m_gamesock.ProcessMsg(data, size);
                return 1;
            }

            public int OnSockSend(UserSock sock, int size)
            {
                return 1;
            }

            public int OnSockLog(UserSock sock, string msg)
            {
                //Log.Trace("OnSockLog " + msg);
                return 1;
            }

            public int OnSockError(UserSock sock, int error_type, string error_msg)
            {
                Log.TraceError("Error, OnSockError error_type: " + error_type.ToString() + " error_msg:" + error_msg);
                return 1;
            }
        }


        //消息事件回调
        //public delegate void CallBack(VarList args);
        private Dictionary<string, CallBack> m_CallBacks = new Dictionary<string, CallBack>();

        private GameReceiver m_gameReceiver = null;
        private GameSender m_gameSender = null;
        private UserSock m_scoket = null;
        private SockCall m_sockcall = null;
        private ISockCallee m_lgsockcall = null;
        private Socket m_sock = null;

        public bool Init(ISockCallee sockcall)
        {
            m_lgsockcall = sockcall;
            m_sockcall = new SockCall();
            m_sockcall.SetGameSock(this);
            if (SysUtil.IsIos)
            {
                m_sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            m_scoket = new UserSock(m_sockcall, m_sock);

            m_gameSender = new GameSender();
            m_gameSender.SetSocket(ref m_scoket);

            m_gameReceiver = new GameReceiver();

            m_gameSender.SetReceiver(m_gameReceiver);
            return true;
        }

        //获取发送对象
        public IGameSender GetGameSender()
        {
            return m_gameSender;
        }

        //获取接受对象
        public IGameReceiver GetGameReceiver()
        {
            return m_gameReceiver;
        }


        //连接服务器
        public bool Connect(string addr, int port,string key)
        {
            return m_scoket.Open(addr, port, key);
        }

        //是否连接成功 
        public bool Connected()
        {
            return m_scoket.Connected();
        }

        public bool Disconnect()
        {
            return m_scoket.Close();
        }

        //连接状态 
        public SockState GetState()
        {
            return m_scoket.GetSockState();
        }

        public void SetState(SockState state)
        {
            m_scoket.SetSockState(state);
        }

        /// <summary>
        /// 初始化平台设置
        /// </summary>
        public void InitSet(bool block)
        {
            m_scoket.GetSocket().Blocking = block;
        }

        //回调事件注册
        public bool RegistCallBack(string funcName, CallBack callBack)
        {
            //
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
                //Log.TraceError("AgentEx RegistCallBack exception =[" + ex.ToString() + "]");
                return false;
            }
            return true;
        }


        public bool RemoveCallBack(string funcName)
        {
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
                }
                else
                {
                    //Log.TraceError("can find call_function " + fun_name);
                }
            }
            catch (Exception ex)
            {
                //Log.TraceError("AgentEx Excute_CallBack exception =[" + ex.ToString() + "]");
                return false;
            }
            return false;
        }

        public void ProcessMsg(byte[] data, int size)
        {       
            m_gameReceiver.ProcessMsg(data, size);
        }

        public void ProcessMessage()
        {
            if (m_scoket != null)
            {
                m_scoket.ProcessMessage();
            }
        }
    }
  }
