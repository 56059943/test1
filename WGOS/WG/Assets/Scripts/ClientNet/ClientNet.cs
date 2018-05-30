using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
	public class ClientNet
    {
        private IGameSock m_gameSock = null;
        private IGameClient m_gameClient = null;
        private IGameClient m_localClient = null;

        private static ClientNet _Instance = null;

		//初始化句柄
        public static ClientNet Instance()
		{
			if ( null == _Instance )
			{
				_Instance = new ClientNet();
			}

			return _Instance;
		}

        //初始化
        public bool Init()
        {
            return Init(null, null);
        }

        public bool Init(Log.on_log on_log, ISockCallee callee)
        {
            Log.SetLogCallBack(on_log);
            m_gameClient = new GameClient();
            if (m_localClient == null)
            {
                m_localClient = new GameClient();
            }
            m_gameSock = new GameSock();
            if (!m_gameSock.Init(callee))
            {
                return false;
            }

            ((GameReceiver)m_gameSock.GetGameReceiver()).SetGameClient((GameClient)m_gameClient);
            return true;
        }

        //结束
        public bool UnInit()
        {
			//_Instance = null;
            //关闭文件句柄
            Log.CloseFile();
            return true;
        }

        public IGameClient GetGameClient()
        {
            return m_gameClient;
        }

        public IGameClient GetLocalClient()
        {
            return m_localClient;
        }

        public IGameSock GetGameSock()
        {
            return m_gameSock;
        }

        //帧循环
        public void Excutue()
        {
            if ( m_gameSock != null )
            {
                ((GameSock)m_gameSock).ProcessMessage();
            }
        }
    }
}
