using System;
using System.Collections.Generic;
using System.Collections;
using SysUtils;

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameObj : IGameObj
    {
        private ObjectID mIdent;
        private ObjectID objId;
        private uint mHash;
        private Dictionary<string, GameRecord> mRecordSet = null;
        private Dictionary<string, GameProperty> mPropSet = null;


        public Dictionary<string, GameProperty> PropSets()
        {
            return mPropSet;
        }

        public GameObj()
        {
            mPropSet = new Dictionary<string, GameProperty>();
            mRecordSet = new Dictionary<string, GameRecord>();
        }

        public GameRecord GetGameRecordByName(string name)
        {
            try
            {
                if (name == null || name.Length == 0)
                {
                    return null;
                }

                if (!mRecordSet.ContainsKey(name))
                {
                    return null;
                }
                return mRecordSet[name];
            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return null;
            }
        }

        public bool UpdateProperty(ref string name, VarList.VarData val)
        {
            try
            {
                if (mPropSet == null)
                {
                    return false;
                }

                if (!mPropSet.ContainsKey(name))
                {
                    mPropSet.Add(name, new GameProperty());
                }

                GameProperty prop = mPropSet[name];
                //prop.Name = name;
                prop.setPropValue(val);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObject.UpdateProperty Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        public int GetRecordCols(string name)
        {
            int nCols = 0;
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.GetRecordCols name para is emtpy");
                return nCols;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GameObject.GetRecordCols does not exist record:" + name);
                    return nCols;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    nCols = record.GetColcount();
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.GetRecordCols Exception :" + ex.ToString());
                return nCols;
            }
            return nCols;
        }

        public int GetRecordRows(string name)
        {
            int nRows = 0;
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.GetRecordRows name para is emtpy");
                return nRows;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GetRecordRows does not exist record:" + name);
                    return nRows;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    nRows = record.GetRowCount();
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.GetRecordRows Exception :" + ex.ToString());
                return nRows;
            }
            return nRows;
        }

        public bool FindRecord(string name)
        {
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.FindRecord name para is emtpy");
                return false;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GameObject.GetRecordRows does not exist record:" + name);
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.FindRecord Exception :" + ex.ToString());
                return false;
            }
        }

        public int GetPropType(string name)
        {
            int type = 0;
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.GetPropType name para is emtpy");
                return type;
            }

            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GameObject.GetPropType does not exist property:" + name);
                    return type;
                }

                GameProperty property = mPropSet[name];
                if (null != property)
                {
                    type = property.ValueType;
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.GetPropType Exception :" + ex.ToString());
                return type;
            }
            return type;
        }

        public bool FindProp(string name)
        {
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.FindProp name para is emtpy");
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GameObject.FindProp does not exist record:" + name);
                    return false;
                }

                GameProperty property = mPropSet[name];
                if (property != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.FindRecord Exception :" + ex.ToString());
                return false;
            }
        }

        public int GetRecordColType(string name, int col)
        {
            int nColType = 0;
            if (name == null || name.Length == 0)
            {
                //Log.Trace("Error:GameObject.GetRecordColType name para is emtpy");
                return nColType;
            }

            if (col < 0)
            {
                //Log.Trace("Error:GameObject.GetRecordColType col para must  > 0");
                return nColType;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    //Log.Trace("Error:GameObject.GetRecordColType does not have rercord :" + name);
                    return nColType;
                }

                GameRecord record = mRecordSet[name];
                if (null == record)
                {
                    //Log.Trace("Error:GameObject.GetRecordColType GameRecord is null");
                    return nColType;
                }

                if (col >= record.GetColcount())
                {
                    //Log.Trace("Error:GameObject.GetRecordColType GameRecord col para is too larger");
                    return nColType;
                }

                nColType = record.GetColType(col);
                return nColType;
            }
            catch (Exception ex)
            {
                //Log.Trace("GameObject.GetRecordColType Exception :" + ex.ToString());
                return 0;
            }
        }

        public int FindRecordRowCI(string name, int col, VarList.VarData key, int begRow)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return -1;
                }
                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return -1;
                }

                return record.FindRow(col, key, begRow, true);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.FindRecordRowCI Exception :" + ex.ToString());
                return -1;
            }
        }

        public int FindRecordRow(string name, int col, VarList.VarData key, int begRow)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return -1;
                }
                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return -1;
                }

                return record.FindRow(col, key, begRow, false);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.FindRecordRow Exception :" + ex.ToString());
                return -1;
            }
        }

        public VarList.VarData QueryRecord(string name, int row, int col)
        {
            VarList.VarData result = new VarList.VarData();
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return result;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return result;
                }

                if (!record.GetValue(row, col, ref result))
                {
                    //Log.Trace("Error,GameObject.QueryRecord GetValue Failed");
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }

            return result;
        }

        public bool QueryRecordBool(string name, int row, int col, ref bool bResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarBool();
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                bResult = (bool)result.Data;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordInt(string name, int row, int col, ref int iResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarInt();
                result.nType = VarType.Int;

                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                iResult = (int)result.Data;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordInt64(string name, int row, int col, ref long lResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarInt64();
                result.nType = VarType.Int64;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                lResult = (long)result.Data;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordFloat(string name, int row, int col, ref float fResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarFloat();
                result.nType = VarType.Float;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }
                fResult = (float)result.Data;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordDouble(string name, int row, int col, ref double dResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarDouble();
                result.nType = VarType.Double;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                dResult = (double)result.Data;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordString(string name, int row, int col, ref string strResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarString();
                result.nType = VarType.String;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                strResult = result.Data as string;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public bool QueryRecordStringW(string name, int row, int col, ref string strResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarWString();
                result.nType = VarType.WideStr;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                strResult = result.Data as string;
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }

        public bool QueryRecordObject(string name, int row, int col, ref ObjectID oResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }

                VarList.VarData result = FreeAllocManager.GetInstance.GetVarObject();
                result.nType = VarType.Object;
                if (!record.GetValue(row, col, ref result))
                {
                    FreeAllocManager.GetInstance.SetVarData(result);
                    return false;
                }

                oResult = (result.Data as ObjectID).Clone();
                FreeAllocManager.GetInstance.SetVarData(result);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error:GameObject.QueryRecord Exception :" + ex.ToString());
            }
            return true;
        }
        public VarList.VarData QueryProp(string name)
        {
            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    return new VarList.VarData();
                }

                GameProperty prop = mPropSet[name];
                if (null == prop)
                {
                    return new VarList.VarData();
                }

                //防止值被逻辑重写，此处拷贝复制
                return prop.PropValue.Clone();
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return new VarList.VarData();
            }
        }

        public bool QueryPropBool(string strPropName, ref bool bResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.Bool)
                    return false;

                bResult = (bool)propValue.Data;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropInt(string strPropName, ref int iResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.Int)
                    return false;

                iResult = (int)propValue.Data;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropInt64(string strPropName, ref long lResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.Int64)
                    return false;

                lResult = (long)propValue.Data;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropFloat(string strPropName, ref float fResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.Float)
                    return false;

                fResult = (float)propValue.Data;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropDouble(string strPropName, ref double dResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.Double)
                    return false;

                dResult = (double)propValue.Data;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropString(string strPropName, ref string strResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }
                if (propValue.nType != VarType.String)
                    return false;

                strResult = propValue.Data as string;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropStringW(string strPropName, ref string strResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }

                if (propValue.nType != VarType.WideStr)
                    return false;

                strResult = propValue.Data as string;
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public bool QueryPropObject(string strPropName, ref ObjectID oResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                if (null == prop)
                {
                    return false;
                }

                VarList.VarData propValue = prop.getPropValue();
                if (propValue == null)
                {
                    return false;
                }

                if (propValue.nType != VarType.Object)
                    return false;

                oResult = (propValue.Data as ObjectID).Clone();
                return true;
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
        }
        public void GetPropList(ref VarList result)
        {
            if (result == null)
            {
                return;
            }

            foreach (KeyValuePair<string, GameProperty> kvp in mPropSet)
            {
                string name = kvp.Key;
                if ((name != null) &&
                    (name.Length != 0))
                {
                    result.AddString(name);
                }
            }
        }

        public void GetRecordList(ref VarList result)
        {
            if (result == null)
            {
                return;
            }

            foreach (KeyValuePair<string, GameRecord> kvp in mRecordSet)
            {
                string name = kvp.Key;
                if ((name != null) &&
                    (name.Length != 0))
                {
                    result.AddString(name);
                }
            }
        }

        public ObjectID Ident
        {
            get { return this.mIdent; }
        }

        public ObjectID GetIdent()
        {
            return this.mIdent;
        }

        public void SetIdent(ObjectID value)
        {
            if (!value.IsNull())
            {
                this.mIdent = value;
            }
        }

        public void SetHash(uint value)
        {
            this.mHash = value;
        }

        public uint GetHash()
        {
            return mHash;
        }

        public ObjectID GetObjId()
        {
            return this.objId;
        }

        public void SetObjId(ObjectID id)
        {
            this.objId = id;
        }

        public bool AddRecord2Set(string name, ref GameRecord record)
        {
            try
            {
                if (name == null || name.Length == 0)
                {
                    //Log.Trace("name is null");
                    return false;
                }

                if (record == null)
                {
                    //Log.Trace("record is null");
                    return false;
                }

                if (mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                mRecordSet.Add(name, record);
                //test begin
                //test end

            }
            catch (Exception ex)
            {
                //Log.Trace("Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        #region 支持本地服接口
        public Dictionary<string, GameRecord> RecordSets()
        {
            return mRecordSet;
        }
        #endregion
    }
}



