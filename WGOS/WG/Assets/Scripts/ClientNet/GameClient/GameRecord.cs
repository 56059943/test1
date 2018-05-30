using System;
using System.Collections.Generic;
using SysUtils;


namespace Fm_ClientNet
{
    public class GameRecord
    {
        private string recName;
        //private int nCols;
        private Dictionary<int, int> colTypes;

        private List<VarList> rowSet;

        //空ID，用于返回
        public static readonly ObjectID EmptyID = new ObjectID();

        //空VarList，用于返回
        public static readonly VarList EmptyVarList = new VarList();

        public GameRecord()
        {
            rowSet = new List<VarList>();
            colTypes = new Dictionary<int, int>();
        }

        public GameRecord Clone()
        {
            int count = GetColcount();
            GameRecord record = new GameRecord(count);
            record.SetName(recName);
            for (int i = 0; i < count; i++ )
            {
                record.SetColType(i , GetColType(i));
            }

            for (int j = 0; j < GetRowCount(); j++ )
            {
                record.GetRowSet().Insert(j, rowSet[j].Clone());
            }

            return record;
        }

        public List<VarList> GetRowSet()
        {
            return this.rowSet;
        }

        public GameRecord(int nCols)
        {
            colTypes = new Dictionary<int, int>();
            for (int i = 0; i < nCols; i++)
            {
                colTypes.Add(i, 0);
            }
            this.rowSet = new List<VarList>();
        }

        public void SetName(string name)
        {
            this.recName = name;
        }

        public string GetName()
        {
            return this.recName;
        }

        public void SetColCount(int nCount)
        {
            //this.nCols = nCount;
            try
            {
                if (colTypes.Count != 0)
                {
                    colTypes.Clear();
                }

                for (int i = 0; i < nCount; i++)
                {
                    colTypes.Add(i, 0);
                }
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
            }
        }

        public int GetColcount()
        {
            return colTypes.Count;
        }

        public bool SetColType(int index, int type)
        {
            if (index >= colTypes.Count)
            {
                return false;
            }

            colTypes[index] = type;
            return true;
        }

        public int GetColType(int col)
        {
            if (col >= colTypes.Count)
            {
                return 0;
            }

            if (!colTypes.ContainsKey(col))
            {
                return 0;
            }

            return colTypes[col];
        }

        public int GetRowCount()
        {
            return rowSet.Count;
        }

        public bool InsertRow(int row)
        {
            try
            {
                int col_num = colTypes.Count;
                VarList newRow = new VarList();

                if (row > col_num)
                {
                    rowSet.Add(newRow);
                }
                else
                {
                    rowSet.Insert(row, newRow);
                }

            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
            return true;
        }

        public bool DeleteRow(int row)
        {
            try
            {
                if (row >= rowSet.Count)
                {
                    return false;
                }
                rowSet.RemoveAt(row);
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
            return true;
        }

        public void ClearRow()
        {
            try
            {
                rowSet.Clear();
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return;
            }
            return;
        }

        public int FindRow(int col, VarList.VarData key, int begRow, bool ignoreCase)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            if (null == key)
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (0 == nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }

				switch (nColType)
                {
                    case VarType.Bool:
                        if (rowValueItem.GetBool(col) == (bool)key.Data)
                        {
                            return i;
                        }
                        break;
                    case VarType.Int:
                        if (rowValueItem.GetInt(col) == (int)key.Data)
                        {
                            return i;
                        }
                        break;
                    case VarType.Int64:
                        if (rowValueItem.GetInt64(col) == (long)key.Data)
                        {
                            return i;
                        }
                        break;
                    case VarType.Float:
                        if (Tools.FloatEqual(rowValueItem.GetFloat(col), (float)key.Data))
                        {
                            return i;
                        }
                        break;
                    case VarType.Double:
                        if (Tools.DoubleEqual(rowValueItem.GetDouble(col), (double)key.Data))
                        {
                            return i;
                        }
                        break;
                    case VarType.String:
                        if (0 == String.Compare(rowValueItem.GetString(col), key.Data as string, ignoreCase))
                        {
                            return i;
                        }
                        break;
                    case VarType.WideStr:
                        if (0 == String.Compare(rowValueItem.GetWideStr(col), key.Data as string, ignoreCase))
                        {
                            return i;
                        }
                        break;
                    case VarType.Object:
                        if (rowValueItem.GetObject(col).Equals(key.Data as ObjectID))
                        {
                            return i;
                        }
                        break;
                    default:
                        break;
                }
            }
            return -1;
        }

        public bool SetValue(int row, int col, Var result)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount()
                    || row < 0 || col < 0)
                {
                    return false;
                }

                if (null == result)
                {
                    return false;
                }

                VarList rowItem = rowSet[row];
                if (rowItem == null)
                {
                    return false;
                }

				if (col > rowItem.GetCount())
                {
					//Log.Trace("col error col=" + col.ToString());
                    return false;
                }

                switch (result.Type)
                {
                    case VarType.Int:
                        {
							if (col < rowItem.GetCount())
							{
                                rowItem.SetInt(col, result.GetInt());
							}
							else
							{
                            	rowItem.AddInt(result.GetInt());
							}
                        }
                        break;
                    case VarType.Int64:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetInt64(col, result.GetInt64());
                            }
                            else
                            {
                                rowItem.AddInt64(result.GetInt64());
                            }
                        }
                        break;
                    case VarType.Float:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetFloat(col, result.GetFloat());
                            }
                            else
                            {
                                rowItem.AddFloat(result.GetFloat());
                            }
                        }
                        break;
                    case VarType.Double:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetDouble(col, result.GetDouble());
                            }
                            else
                            {
                                rowItem.AddDouble(result.GetDouble());
                            }
                        }
                        break;
                    case VarType.String:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetString(col, result.GetString());
                            }
                            else
                            {
                                rowItem.AddString(result.GetString());
                            }
                        }
                        break;
                    case VarType.WideStr:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetWideStr(col, result.GetWideStr());
                            }
                            else
                            {
                                rowItem.AddWideStr(result.GetWideStr());
                            }
                        }
                        break;
                    case VarType.Object:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetObject(col, result.GetObject());
                            }
                            else
                            {
                                rowItem.AddObject(result.GetObject());
                            }
                        }
                        break;
                    default:
                        //Log.Trace("typer error type=" + result.Type.ToString());
                        return false;
                }//end switch

                //rowSet[row] = rowItem;
                //int nCount = rowItem.GetCount();
				return true;
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return false;
            }
        }

        public bool GetValue(int row, int col, ref VarList.VarData result)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                //Log.Trace("Error,GameRecord.GetValue row or col out of range:");
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
                if (rowItem == null)
                {
                    return false;
                }

                if (rowItem.GetCount() != GetColcount())
                {
                    //Log.Trace("Error,GameRecord.GetValue Col Error:");
                    return false;
                }

                switch (colTypes[col])
                {
                    case VarType.Bool:
                        {
                            bool value = rowItem.GetBool(col);
                            result.Data = value;
                        }

                        break;

                    case VarType.Int:
                        {
                            int value = rowItem.GetInt(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.Int64:
                        {
                            long value = rowItem.GetInt64(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.Float:
                        {
                            float value = rowItem.GetFloat(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.Double:
                        {
                            double value = rowItem.GetDouble(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.String:
                        {
                            string value = rowItem.GetString(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.WideStr:
                        {
                            string value = rowItem.GetWideStr(col);
                            result.Data = value;
                        }
                        break;

                    case VarType.Object:
                        {
                            ObjectID value = rowItem.GetObject(col);
                            if (value.IsNull())
                            {
                                //Log.Trace("Error,GameRecord.GetValue objid is null:");
                                return false;
                            }
                            result.Data = value;
                        }
                        break;

                    default:
                        //Log.Trace("Error,GameRecord.GetValue type error:");
                        return false;
                }
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameRecord.GetValue Exception:" + ex.ToString());
                return false;
            }

            return true;
        }

        public string GetString(int row, int col)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount())
                {
                    return "";
                }

                VarList row_value = (VarList)rowSet[row];
                if (row_value.GetType(col) != VarType.String)
                {
                    return "";
                }
                return row_value.GetString(col);
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return "";
            }
        }

        public string GetWideStr(int row, int col)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount())
                {
                    return "";
                }

                VarList row_value = (VarList)rowSet[row];
                if (row_value.GetType(col) != VarType.WideStr)
                {
                    return "";
                }
                return row_value.GetWideStr(col);
            }
            catch (Exception ex)
            {
                Log.TraceExcep(ref ex);
                return "";
            }
        }

        public void ReleaseAll()
        {
        }

        // bool
        public bool SetValue(int row, int col, bool result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetBool(col, result);
            }
            else
            {
                rowItem.AddBool(result);
            }

            return true;
        }

        // int
        public bool SetValue(int row, int col, int result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetInt(col, result);
            }
            else
            {
                rowItem.AddInt(result);
            }

            return true;
        }

        // int64
        public bool SetValue(int row, int col, long result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetInt64(col, result);
            }
            else
            {
                rowItem.AddInt64(result);
            }

            return true;
        }

        // double
        public bool SetValue(int row, int col, double result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetDouble(col, result);
            }
            else
            {
                rowItem.AddDouble(result);
            }

            return true;
        }

        // float
        public bool SetValue(int row, int col, float result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetFloat(col, result);
            }
            else
            {
                rowItem.AddFloat(result);
            }

            return true;
        }

        // string
        public bool SetValue(int row, int col, string result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetString(col, result);
            }
            else
            {
                rowItem.AddString(result);
            }

            return true;
        }

        // wstring
        //public bool SetValue(int row, int col, string result)
        //{
        //    if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
        //    {
        //        return false;
        //    }

        //    VarList rowItem = rowSet[row];
        //    if (rowItem == null)
        //    {
        //        return false;
        //    }

        //    if (col < rowItem.GetCount())
        //    {
        //        rowItem.SetString(col, result);
        //    }
        //    else
        //    {
        //        rowItem.AddString(result);
        //    }

        //    return true;
        //}

        // OBJECITID
        public bool SetValue(int row, int col, ObjectID result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetObject(col, result);
            }
            else
            {
                rowItem.AddObject(result);
            }

            return true;
        }

        #region 新接口
        int maxRow = -1;

        public GameRecord(string name, int _maxRow, List<int> cols)
        {
            recName = name;

            colTypes = new Dictionary<int, int>();
            for (int i = 0; i < cols.Count; i++)
            {
                colTypes.Add(i, cols[i]);
            }
            this.rowSet = new List<VarList>();

            this.maxRow = _maxRow;
        }

        public int GetRows()
        {
            return GetRowCount();
        }

        public int GetRowMax()
        {
            return maxRow;
        }

        public int GetColCount()
        {
            return GetColcount();
        }

        public void Clear()
        {
            rowSet.Clear();
        }
        #endregion

        #region 查询
        public int QueryInt(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return 0;
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.Int)
            {
                return 0;
            }
            return row_value.GetInt(col);
        }

        public Int64 QueryInt64(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return 0;
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.Int64)
            {
                return 0;
            }
            return row_value.GetInt64(col);
        }

        public float QueryFloat(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return 0.0f;
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.Float)
            {
                return 0.0f;
            }
            return row_value.GetFloat(col);
        }

        public double QueryDouble(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return 0.0;
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.Double)
            {
                return 0.0;
            }
            return row_value.GetDouble(col);
        }

        public string QueryString(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return "";
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.String)
            {
                return "";
            }
            return row_value.GetString(col);
        }

        public string QueryWideStr(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return "";
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.WideStr)
            {
                return "";
            }
            return row_value.GetWideStr(col);
        }

        public ObjectID QueryObject(int row, int col)
        {
            if (row >= GetRows() || col >= GetColCount())
            {
                return EmptyID;
            }

            VarList row_value = (VarList)rowSet[row];
            if (row_value.GetType(col) != VarType.Object)
            {
                return EmptyID;
            }
            return row_value.GetObject(col).Clone();
        }

        public bool QueryRowValue(int row, ref VarList rowValue)
        {
            if (row >= GetRows())
            {
                return false;
            }

            rowValue = (VarList)rowSet[row].CloneList();

            return rowValue.GetCount() > 0;
        }        

        #endregion

        #region 设置
        public bool SetInt(int row, int col, int result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetInt(col, result);
            }
            else
            {
                rowItem.AddInt(result);
            }

            return true;
        }
        public bool SetInt64(int row, int col, Int64 result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetInt64(col, result);
            }
            else
            {
                rowItem.AddInt64(result);
            }

            return true;
        }
        public bool SetFloat(int row, int col, float result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetFloat(col, result);
            }
            else
            {
                rowItem.AddFloat(result);
            }

            return true;
        }
        public bool SetDouble(int row, int col, double result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetDouble(col, result);
            }
            else
            {
                rowItem.AddDouble(result);
            }

            return true;
        }
        public bool SetString(int row, int col, string result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetString(col, result);
            }
            else
            {
                rowItem.AddString(result);
            }

            return true;
        }
        public bool SetWideStr(int row, int col, string result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetWideStr(col, result);
            }
            else
            {
                rowItem.AddWideStr(result);
            }

            return true;
        }
        public bool SetObject(int row, int col, ObjectID result)
        {
            if (row >= GetRowCount() || col >= GetColcount() || row < 0 || col < 0)
            {
                return false;
            }

            VarList rowItem = rowSet[row];
            if (rowItem == null)
            {
                return false;
            }

            if (col < rowItem.GetCount())
            {
                rowItem.SetObject(col, result);
            }
            else
            {
                rowItem.AddObject(result);
            }

            return true;
        }
        #endregion

        #region 查找
        public int FindInt(int col, int key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.Int != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetInt(col) == key)
                {
                    return i;
                }             
            }

            return -1;
        }

        public int FindInt64(int col, Int64 key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.Int64 != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetInt64(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindFloat(int col, float key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.Float != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetFloat(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }
        public int FindDouble(int col, double key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.Double != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetDouble(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }
        public int FindString(int col, string key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.String != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetString(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindWideStr(int col, string key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.WideStr != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetWideStr(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindObject(int col, ObjectID key, int begRow = 0, bool ignoreCase = true)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (VarType.Object != nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if (null == rowValueItem || 0 == rowValueItem.GetCount())
                {
                    return -1;
                }
                if (rowValueItem.GetObject(col) == key)
                {
                    return i;
                }
            }

            return -1;
        }

        public int Find(int col, VarList.VarData key, int begRow, bool ignoreCase)
        {
            return FindRow(col, key, begRow, ignoreCase);
        }

        public void RemoveRow(int row)
        {
            if(row >= rowSet.Count)
            {
                Log.Trace("Error,RemoveRow error:" + recName);
                return;
            }
            rowSet.RemoveAt(row);
        }

        //添加一行
        public int AddRow(int row)
        {
            if (row == -1 || row >= rowSet.Count)
            {
                row = rowSet.Count;
                rowSet.Add(new VarList());
            }
            else
            {
                rowSet.Insert(row, new VarList());
            }

            return row;
        }

        //增加一行数据到客户端
        public int AddRowValue(int row, VarList rowValue)
        {          
            int newRow = AddRow(row);

            for (int i = 0; i < colTypes.Count; i++)
            {
                int nColType = colTypes[i];
                switch (nColType)
                {
                    case VarType.Bool:
                        {
                            SetValue(newRow, i, rowValue.GetBool(i));
                        }
                        break;
                    case VarType.Int:
                        {
                            SetValue(newRow, i, rowValue.GetInt(i));
                        }
                        break;
                    case VarType.Int64:
                        {
                            SetValue(newRow, i, rowValue.GetInt64(i));
                        }
                        break;
                    case VarType.Float:
                        {
                            SetValue(newRow, i, rowValue.GetFloat(i));
                        }
                        break;
                    case VarType.Double:
                        {
                            SetValue(newRow, i, rowValue.GetDouble(i));
                        }
                        break;
                    case VarType.String:
                        {
                            SetValue(newRow, i, rowValue.GetString(i));
                        }
                        break;
                    case VarType.WideStr:
                        {
                            SetValue(newRow, i, rowValue.GetWideStr(i));
                        }
                        break;
                    case VarType.Object:
                        {
                            SetValue(newRow, i, rowValue.GetObject(i));
                        }
                        break;
                    default:
                        break;
                }
            }
            return newRow;
        }
        #endregion
    }
}



