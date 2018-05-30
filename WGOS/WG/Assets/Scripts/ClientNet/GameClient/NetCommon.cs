using System;
using SysUtils;
using System.Collections.Generic;

namespace Fm_ClientNet
{
    public class PropData
    {
        public string strName;
        public int nType;
        public uint nCount;
        public PropData()
        {
            strName = "";
            nType = 0;
            nCount = 0;
        }

    }

    public class RoleData
    {
        public RoleData()
        {
            nRoleIndex = 0;
            nSysFlags = 0;
            name = "";
            para = "";
            nDeleted = 0;
            dDeleteTime = 0.0f;
            parameters = new VarList();
        }

        int nRoleIndex;
        int nSysFlags;
        string name;
        string para;
        int nDeleted;
        double dDeleteTime;
        VarList parameters;

        public int RoleIndex
        {
            get { return nRoleIndex; }
            set { nRoleIndex = value; }
        }

        public int getRoleIndex()
        {
            return this.nRoleIndex;
        }

        public void setRoleIndex(int value)
        {
            this.nRoleIndex = value;
        }

        public int SysFlags
        {
            get { return nSysFlags; }
            set { nSysFlags = value; }
        }

        public int getSysFlags()
        {
            return this.nSysFlags;
        }

        public void setSysFlags(int value)
        {
            this.nSysFlags = value;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Para
        {
            get { return para; }
            set { para = value; }
        }

        public int Deleted
        {
			get { return nDeleted; }
			set { nDeleted = value; }
        }

        public void SetDeleted(int nDeleted)
        {
            this.nDeleted = nDeleted;
        }

        public double DeleteTime
        {
            get { return dDeleteTime; }
            set { dDeleteTime = value; }
        }



        public VarList paraList
        {
            get { return parameters; }
        }

    }

    public class RecData
    {
        public string strName;
        public int nCols;
        public Dictionary<int, int> ColTypes = null;
        //uint nRecordAddRowCount;
        //uint nRecordClearCount;
        //uint nRecordGridCount;
        //uint nRecordDelRowCount;
        //double dRecordAddRowTime;
        //double dRecordDelRowTime;
        //double dRecrodGridTime;
        //double dRecordClearTime;


        public RecData()
        {
            strName = "";
            nCols = 0;

            //nRecordAddRowCount = 0;
            //nRecordClearCount = 0;
            //nRecordGridCount = 0;
            //nRecordDelRowCount = 0;

            //dRecordAddRowTime = 0.0f;
            //dRecordDelRowTime = 0.0f;
            //dRecrodGridTime = 0.0f;
            //dRecordClearTime = 0.0f;
            ColTypes = new Dictionary<int, int>();
        }

        public bool AddRecColType(int index, int nType)
        {
            if (ColTypes == null)
            {
                return false;
            }

            try
            {
                if (ColTypes.ContainsKey(index))
                {
                    return false;
                }
                ColTypes.Add(index, nType);
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }

            return true;
        }
    }

    public class MenuData
    {
        public int nType;
        public int nMark;
        public string info;
        public MenuData()
        {
            nType = 0;
            nMark = 0;
            info = "";
        }

    }

    public class MsgStat
    {
        //uint nMsgCount;
        //uint nMsgBytes;
        //uint nMaxSize;
        //uint nCPMaxSize;
        //double dProcessTime;
        //public MsgStat()
        //{
        //    nMsgCount = 0;
        //    nMsgBytes = 0;
        //    nMaxSize = 0;
        //    nCPMaxSize = 0;
        //    dProcessTime = 0.0f;
        //}

    }
}


