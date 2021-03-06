﻿using System.Collections;
using System.Collections.Generic;
using SysUtils;


//游戏中和公司 C++引擎的一些规范
//包括propety table 、record table

namespace Fm_ClientNet
{
    public class GameRule
    {
        private static GameRule gInstance = new GameRule();

        //玩家属性查询表 <索引,属性类型>
        private Dictionary<int, Propetry> m_propertyTable = new Dictionary<int, Propetry>();

        //玩家表格规则查询表 <索引，表格结构信息>
        private Dictionary<int, Record> m_recordInfo = new Dictionary<int, Record>();

        private GameRule()
        {

        }

        public static GameRule getInstance()
        {
            return gInstance;
        }

        //在玩家属性表条目中增加一条 (索引，属性)
        public void addPropTableItem(int id, Propetry type)
        {
            if (m_propertyTable.ContainsKey(id))
            {
                //Log.TraceError("Repeat protperty item is " + id);
                m_propertyTable.Remove(id);
            }
            m_propertyTable.Add(id, type);
        }

        //玩家属性表条目中获取对应索引的属性信息
        public bool getPropTableItem(int id, ref Propetry type)
        {
            if (!m_propertyTable.ContainsKey(id))
            {
                //Log.Trace("has not found prop index " + id);
                return false;
            }
            type = m_propertyTable[id];
            return true;
        }

        public Dictionary<int, Propetry> getPropTable()
        {
            return m_propertyTable;
        }

        public Dictionary<int, Record> getRecordInfo()
        {
            return this.m_recordInfo;
        }

        //清空玩家属性表
        public void clearPropertyTab()
        {
            m_propertyTable.Clear();
        }

        //在玩家表格条目中增加一条 (索引，表格结构信息)
        public void addRecordItem(int id, Record type)
        {
            if (m_recordInfo.ContainsKey(id))
            {
                //Log.TraceError("Repeat protperty item is " + id);
                m_recordInfo.Remove(id);
            }
            m_recordInfo.Add(id, type);
        }

        //获取玩家表格条目中对应索引号的表格信息
        public bool getRecordItem(int id, ref Record type)
        {
            if (!m_recordInfo.ContainsKey(id))
            {
                //Log.TraceError("Repeat protperty item is " + id);
                return false;
            }
            type = m_recordInfo[id];
            return true;
        }

        //清空玩家属性表
        public void clearRecord()
        {
            m_recordInfo.Clear();
        }
    }


    //游戏对象位置
    public class OuterPost
    {
        public float x;
        public float y;
        public float z;
        public float orient;

        public OuterPost()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.orient = 0;
        }



        //用 初始化类各成员
        public bool initOuterPost(ref LoadArchive loadAr)
        {
            float xTemp = 0.0f;
            float yTemp = 0.0f;
            float zTemp = 0.0f;
            float orientTemp = 0.0f;

            if (!loadAr.ReadFloat(ref xTemp))
            {
                return false;
            }
            this.x = xTemp;

            if (!loadAr.ReadFloat(ref yTemp))
            {
                return false;
            }
            this.y = yTemp;

            if (!loadAr.ReadFloat(ref zTemp))
            {
                return false;
            }
            this.z = zTemp;

            if (!loadAr.ReadFloat(ref orientTemp))
            {
                return false;
            }
            this.orient = orientTemp;

            return true;
        }

        public void addVarList(ref VarList argList)
        {
            argList.AddFloat(this.x);
            argList.AddFloat(this.y);
            argList.AddFloat(this.z);
            argList.AddFloat(this.orient);
        }
    }

    // 移动目标数据
    public class OuterDest
    {
        public float x;
        public float z;
        public float orient;
        public float MoveSpeed;
        public float RotateSpeed;
        public float JumpSpeed;
        public float Gravity;
        public int Mode;

        public float getX()
        {
            return this.x;
        }

        public float getZ()
        {
            return this.z;
        }

        public float getOrient()
        {
            return this.orient;
        }

        public float getMoveSpeed()
        {
            return this.MoveSpeed;
        }

        public float getRotateSpeed()
        {
            return this.RotateSpeed;
        }


        public float getJumpSpeed()
        {
            return this.JumpSpeed;
        }

        public float getGravity()
        {
            return this.Gravity;
        }

        public int getMode()
        {
            return this.Mode;
        }

        public OuterDest()
        {
            this.x = 0.0f;
            this.z = 0.0f;
            this.orient = 0.0f;
            this.MoveSpeed = 0.0f;
            this.RotateSpeed = 0.0f;
            this.JumpSpeed = 0.0f;
            this.Gravity = 0.0f;
            this.Mode = 0;
        }

        //用 初始化类各成员
        public bool initOuterDest(ref LoadArchive loadAr)
        {
            float xTemp = 0.0f;
            float yTemp = 0.0f;
            float zTemp = 0.0f;
            float orientTemp = 0.0f;
            float MoveSpeedTemp = 0.0f;
            float RotateSpeedTemp = 0.0f;
            float JumpSpeedTemp = 0.0f;
            float GravityTemp = 0.0f;
            int ModeTemp = 0;

            if (!loadAr.ReadFloat(ref xTemp))
            {
                return false;
            }
            this.x = xTemp;

            //if (!loadAr.ReadFloat(ref yTemp))
            //{
            //    return false;
            //}

            if (!loadAr.ReadFloat(ref zTemp))
            {
                return false;
            }
            this.z = zTemp;

            if (!loadAr.ReadFloat(ref orientTemp))
            {
                return false;
            }
            this.orient = orientTemp;


            //if (!loadAr.ReadFloat(ref MoveSpeedTemp))
            //{
            //    return false;
            //}
            //this.MoveSpeed = MoveSpeedTemp;

            //if (!loadAr.ReadFloat(ref RotateSpeedTemp))
            //{
            //    return false;
            //}
            //this.RotateSpeed = RotateSpeedTemp;

            //if (!loadAr.ReadFloat(ref JumpSpeedTemp))
            //{
            //    return false;
            //}
            //this.JumpSpeed = JumpSpeedTemp;

            //if (!loadAr.ReadFloat(ref GravityTemp))
            //{
            //    return false;
            //}
            //this.Gravity = GravityTemp;

            if (!loadAr.ReadInt32(ref ModeTemp))
            {
                return false;
            }
            this.Mode = ModeTemp;

            return true;
        }

        public void addVarList(ref VarList argList)
        {
            argList.AddFloat(this.x);
            //argList.AddFloat(this.y);
            argList.AddFloat(this.z);

            argList.AddFloat(this.orient);
            argList.AddFloat(this.MoveSpeed);
            argList.AddFloat(this.RotateSpeed);


            argList.AddFloat(this.JumpSpeed);
            argList.AddFloat(this.Gravity);
            argList.AddInt(this.Mode);
        }

    }
}


