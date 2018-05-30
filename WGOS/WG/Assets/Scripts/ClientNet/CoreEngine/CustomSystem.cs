using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;
using Fm_ClientNet;
using System;

/// <summary>
/// 自定义消息管理系统
/// </summary>
public class CustomSystem 
{
       //自定义消息单件
    private static CustomSystem _instance;
    public static CustomSystem Instance
    {
        get
        {
            return _instance;
        }  
    }
    public CustomSystem()
    {
        if( _instance == null )
        {
            _instance = this;
        }
    }

#region 服务器自定义

    public delegate void OnCustom(VarList args);
    Dictionary<int, List<OnCustom>> mCustoms = new Dictionary<int, List<OnCustom>>();
     
      /* @brief: 启动注册自定义消息监听
           @param: iGameRecv 消息注册接口
           @return void
      */
      public void RegistCallback(IGameReceiver iGameRecv)
      {
          iGameRecv.RegistCallBack("on_custom", on_custom);
      }
	  /* @brief: 接收服务器自定义消息，并按注册代理分发
           @param: args 服务器自定义消息参数列表
           @return void
      */
      private void on_custom(VarList args)
      {
		   int iCustomCmd = args.GetInt(1);
		   if( mCustoms.ContainsKey(iCustomCmd) )
		   {
				for( int i = 0 ;i < mCustoms[iCustomCmd].Count ;i++ )
				{
				    mCustoms[iCustomCmd][i](args);
			    }
		   }
      }
      /* @brief: 逻辑功能注册自定义消息接口
         @param: args 服务器自定义消息参数列表
         @return void
       */
      public void RegistCustomCallback(int iCustomCmd, OnCustom fnCallback)
	  {
	    if( mCustoms.ContainsKey(iCustomCmd) )
		{
            if (!mCustoms[iCustomCmd].Contains(fnCallback))
			{
                mCustoms[iCustomCmd].Add(fnCallback);
			}	
		}
		else 
		{
			mCustoms[iCustomCmd] = new List<OnCustom>();
            mCustoms[iCustomCmd].Add(fnCallback);
		}
	  }
      /* @brief: 逻辑功能取消注册自定义消息接口
           @param1: iCustomCmd 自定义消息编号
           @param2: callback  逻辑自定义消息回调接口
           @return void
         */
      public void UnRegistCustomCallback(int iCustomCmd, OnCustom fnCallback)
      {
          if (mCustoms.ContainsKey(iCustomCmd))
          {
              if (mCustoms[iCustomCmd].Contains(fnCallback))
              {
                  mCustoms[iCustomCmd].Remove(fnCallback);
                  if (mCustoms[iCustomCmd].Count == 0)
                  {
                      mCustoms.Remove(iCustomCmd);
                  }
              }
          }
      }

#endregion

#region 客户端自定义

      private Dictionary<int, List<DispatchEventHandler>> EventTable = new Dictionary<int, List<DispatchEventHandler>>();

      public delegate void DispatchEventHandler(System.Object sender, DispatchEventArgs e);
      public event DispatchEventHandler Dispatch;

      public void dispacthEvent(ValueType t)
      {
          int type = (int)t;
          if (EventTable.ContainsKey(type))
          {
              DispatchEventArgs e = new DispatchEventArgs();
              List<DispatchEventHandler> funcArr = (List<DispatchEventHandler>)EventTable[type];
              List<DispatchEventHandler> funcArrTemp = new List<DispatchEventHandler>(funcArr);
              int count = funcArrTemp.Count;
              for (int i = 0; i < count; i++ )
              {
                  this.Dispatch = funcArrTemp[i];
                  OnDispatch(e);
              }
          }
      }
      public void dispacthEvent(ValueType t, params object[] data)
      {
          int type = (int)t;
          if (EventTable.ContainsKey(type))
          {
              DispatchEventArgs e = new DispatchEventArgs(type, data);
              List<DispatchEventHandler> funcArr = (List<DispatchEventHandler>)EventTable[type];
              List<DispatchEventHandler> funcArrTemp = new List<DispatchEventHandler>(funcArr);
              int count = funcArrTemp.Count;
              for (int i = 0; i < count; i++)
              {
                  this.Dispatch = funcArrTemp[i];
                  OnDispatch(e);
              }
          }
      }
      protected virtual void OnDispatch(DispatchEventArgs e)
      {
          if (Dispatch != null)
          {
              Dispatch(this, e);
          }
      }

      public void RegistCustomCallback(ValueType t, DispatchEventHandler func)
      {
          int type = (int)t;
          if (!EventTable.ContainsKey(type))
          {
              List<DispatchEventHandler> FuncList = new List<DispatchEventHandler>();
              FuncList.Add(func);
              EventTable.Add(type, FuncList);
          }
          else
          {
              List<DispatchEventHandler> funclist = (List<DispatchEventHandler>)EventTable[type];
              funclist.Add(func);
          }
      }

      public void UnRegistCustomCallback(ValueType t, DispatchEventHandler func)
      {
          int type = (int)t;
          if (EventTable.ContainsKey(type))
          {
              List<DispatchEventHandler> funclist = (List<DispatchEventHandler>)EventTable[type];
              if (funclist.Contains(func))
              {
                  funclist.Remove(func);
              }
              if (funclist.Count == 0)
              {
                  EventTable.Remove(type);
              }
          }
      }
      public void UnRegistCustomCallback(ValueType t)
      {
          int type = (int)t;
          if (EventTable.ContainsKey(type))
          {
              EventTable.Remove(type);
          }
      }

    #endregion

#region 本地服务器

    Dictionary<string, CallBack> m_CallBacks = new Dictionary<string, CallBack>();

    /// <summary>
    /// 本地服回调事件注册
    /// </summary>
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
        catch (System.Exception ex)
        {
            //Log.Trace("Error, exception " + ex.ToString());
            return false;
        }
        return true;
    }

    /// <summary>
    /// 本地服移除消息
    /// </summary>
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
        catch (System.Exception ex)
        {
            //Log.Trace("AgentEx RemoveCallBack exception =[" + ex.ToString() + "]");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 本地服消息触发
    /// </summary>
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
                //Log.Trace("can not find call_function " + fun_name);
            }
        }
        catch (System.Exception ex)
        {
            //Log.Trace("AgentEx Excute_CallBack exception =[" + ex.ToString() + "]");
        }
        return false;
    }


#endregion


    /// <summary>
    /// 取消所有注册自定义消息
    /// </summary>
    public void UnRegistAllCustomCallback()
    {
        mCustoms.Clear();
        m_CallBacks.Clear();
    }

}
