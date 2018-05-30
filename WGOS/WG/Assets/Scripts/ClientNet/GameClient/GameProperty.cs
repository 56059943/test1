
using System;
using SysUtils;

namespace Fm_ClientNet
{
    public class GameProperty
    {
        //private string name = "";
        //private int index = 0;
        public VarList.VarData propValue;

        public GameProperty()
        {
            propValue = new VarList.VarData();
        }

        /*public string Name
        {
            get { return name; }
            set { name = value; }
        }*/

        /*public int Index
        {
            get { return index; }
            set { index = value; }
        }*/

        public int ValueType
        {
            get { return this.propValue.nType; }
        }

        public int getPropType()
        {
            return this.propValue.nType;
        }

        public VarList.VarData PropValue
        {
            get { return propValue; }
            set { propValue = value; }
        }

        public void setPropValue(VarList.VarData val)
        {
            propValue.nType = val.nType;
            propValue.Data = val.Data;
        }

        public VarList.VarData getPropValue()
        {
            return this.propValue;
        }

        public bool getPropValueBool()
        {
            if (this.propValue.nType == VarType.Bool)
            {
                return (bool)propValue.Data;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueBool propType is not bool!");
                return false;
            }
        }

        public void setPropValueBool(bool value)
        {
            this.propValue.nType = VarType.Bool;
            this.propValue.Data = value;
        }

        public int getPropValueInt()
        {
            if (this.propValue.nType == VarType.Int)
            {
                return (int)propValue.Data;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueInt propType is not int!");
                return 0;
            }
        }

        public void setPropValueInt(int value)
        {
            this.propValue.nType = VarType.Int;
            this.propValue.Data = value;
        }

        public long getPropValueInt64()
        {
            if (this.propValue.nType == VarType.Int64)
            {
                return (long)propValue.Data;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueInt64 propType is not int64!");
                return 0;
            }
        }

        public void setPropValueInt64(long value)
        {
            this.propValue.nType = VarType.Int64;
            this.propValue.Data = value;
        }

        public float getPropValueFloat()
        {
            if (this.propValue.nType == VarType.Float)
            {
                return (float)propValue.Data;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueFloat propType is not float!");
                return 0.0f;
            }
        }

        public void setPropValueFloat(float value)
        {
            this.propValue.nType = VarType.Float;
            this.propValue.Data = value;
        }

        public double getPropValueDouble()
        {
            if (this.propValue.nType == VarType.Double)
            {
                return (double)propValue.Data;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueDouble propType is not double!");
                return 0.0;
            }
        }

        public void setPropValueDouble(double value)
        {
            this.propValue.nType = VarType.Double;
            this.propValue.Data = value;
        }

        public string getPropValueString()
        {
            if (this.propValue.nType == VarType.String)
            {
                return propValue.Data as string;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueString propType is not string!");
                return "";
            }
        }

        public void setPropValueString(string value)
        {
            this.propValue.nType = VarType.String;
            this.propValue.Data = value;
        }

        public string getPropValueWideStr()
        {
            if (this.propValue.nType == VarType.WideStr)
            {
                return propValue.Data as string;
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueWideStr propType is not wstring!");
                return "";
            }
        }

        public byte[] getPropValueUserData()
        {
            if (this.propValue.nType == VarType.UserData)
            {
                return propValue.Data as byte[];
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueWideStr propType is not wstring!");
                return new byte[1];
            }
        }

        public void setPropValueWideStr(string value)
        {
            this.propValue.nType = VarType.WideStr;
            this.propValue.Data = value;
        }

        public ObjectID getPropValueObject()
        {
            if (this.propValue.nType == VarType.Object)
            {
                return (propValue.Data as ObjectID).Clone();
            }
            else
            {
                //Log.Trace("Error,GameProperty.getPropValueObject propType is not object!");
                return new ObjectID();
            }
        }

        public void setPropValueObject(ObjectID value)
        {
            this.propValue.nType = VarType.Object;
            this.propValue.Data = value;
        }


        public void setPropValueUserData(object val)
        {
            this.propValue.nType = VarType.UserData;
            this.propValue.Data = (byte[])val;
        }
    }
}



