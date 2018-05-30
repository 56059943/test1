using System.Collections.Generic;
using System.Collections;
using SysUtils;
using System;

public class FreeAlloc<T> where T:new()
{
    private int m_Lengh; // 初始长度7365
    private int m_TotalSize;
    private Stack<T> m_FreeList = new Stack<T>();


    public FreeAlloc(int lengh)
    {
        m_Lengh = lengh;
        m_TotalSize = lengh;
        for (int i = 0; i <= lengh; ++i)
        {
            T ptr = new T();
            m_FreeList.Push(ptr);
        }
    }

    public T GetT()
    {
        if (m_FreeList.Count > 0)
        {
            return m_FreeList.Pop();
        }

        // 申请新的空间e6bd98e5bf97e8b1aa
        for (int i = 0; i <= m_Lengh; ++i)
        {
             T ptr = new T();
             m_FreeList.Push(ptr);
        }

        m_TotalSize += m_Lengh;
        return m_FreeList.Pop();
    }
   
    public void SetT(T ptr)
    {
        m_FreeList.Push(ptr);
    }

    public int GetFreeCount()
    {
        return m_FreeList.Count;
    }
}

public class FreeAllocManager
{
    private FreeAlloc<ObjectID> free_ObjectID = new FreeAlloc<ObjectID>(256);
    private FreeAlloc<VarList> free_VarList = new FreeAlloc<VarList>(512);


    // 根据分类分别建立
    private FreeAlloc<VarList.VarData> free_VarInt = new FreeAlloc<VarList.VarData>(512);
    private FreeAlloc<VarList.VarData> free_VarBool = new FreeAlloc<VarList.VarData>(128);
    private FreeAlloc<VarList.VarData> free_VarInt64 = new FreeAlloc<VarList.VarData>(128);
    private FreeAlloc<VarList.VarData> free_VarString = new FreeAlloc<VarList.VarData>(512);
    private FreeAlloc<VarList.VarData> free_Float = new FreeAlloc<VarList.VarData>(256);
    private FreeAlloc<VarList.VarData> free_Double = new FreeAlloc<VarList.VarData>(64);
    private FreeAlloc<VarList.VarData> free_WString = new FreeAlloc<VarList.VarData>(128);
    private FreeAlloc<VarList.VarData> free_Object = new FreeAlloc<VarList.VarData>(512);
    private FreeAlloc<VarList.VarData> free_UserData = new FreeAlloc<VarList.VarData>(64);

    private static FreeAllocManager m_pInstance = null;
    public static FreeAllocManager GetInstance
    {
        get
        {
            if (m_pInstance == null)
            {
                m_pInstance = new FreeAllocManager();
            }
            return m_pInstance;
        }
    }

    public void Init() {
    }

    FreeAllocManager()
    {
    }

    public ObjectID GetObjectID()
    {
        return free_ObjectID.GetT();
    }

    public VarList GetVarList()
    {
        return free_VarList.GetT();
    }

    public void SetObjectID(ObjectID id)
    {
        id.m_nIdent = id.m_nSerial = 0;
        free_ObjectID.SetT(id);
    }

    public void SetVarList(VarList var)
    {
        var.Clear();
        free_VarList.SetT(var);
    }

    public void SetVarData(VarList.VarData varData)
    {
        switch(varData.nType)
        {
            case VarType.Bool:
                free_VarBool.SetT(varData);
                break;
            case VarType.Int:
                free_VarInt.SetT(varData);
                break;
            case VarType.Int64:
                free_VarInt64.SetT(varData);
                break;
            case VarType.Float:
                free_Float.SetT(varData);
                break;
            case VarType.Double:
                free_Double.SetT(varData);
                break;
            case VarType.String:
                free_VarString.SetT(varData);
                break;
            case VarType.WideStr:
                free_WString.SetT(varData);
                break;
            case VarType.Object:
                free_Object.SetT(varData);
                break;
            case VarType.UserData:
                free_UserData.SetT(varData);
                break;
        }
    }

    //
    public VarList.VarData GetVarBool()
    {
        return free_VarBool.GetT();
    }

    public VarList.VarData GetVarInt()
    {
        return free_VarInt.GetT();
    }

    public VarList.VarData GetVarInt64()
    {
        return free_VarInt64.GetT();
    }

    public VarList.VarData GetVarFloat()
    {
        return free_Float.GetT();
    }

    public VarList.VarData GetVarDouble()
    {
        return free_Double.GetT();
    }

    public VarList.VarData GetVarString()
    {
        return free_VarString.GetT();
    }

    public VarList.VarData GetVarWString()
    {
        return free_WString.GetT();
    }

    public VarList.VarData GetVarObject()
    {
        return free_Object.GetT();
    }

    public VarList.VarData GetVarUserData()
    {
        return free_UserData.GetT(); 
    }
}