using System;
using System.Collections.Generic;
using SysUtils;


namespace Fm_ClientNet
{
    public class GameObjectSet
    {
        private Dictionary<ObjectID, GameObj> mObjects = null;

        public GameObjectSet()
        {
            mObjects = new Dictionary<ObjectID, GameObj>();
        }

        public bool Find(ObjectID ident)
        {
            if (ident.IsNull())
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }

            return true;
        }

        public bool Add(ObjectID ident, ref GameObj obj)
        {
            if (obj == null
                || ident.IsNull())
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.Add Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(ObjectID ident, ref GameView obj)
        {
            if (obj == null
                || ident.IsNull())
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.Add Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(ObjectID ident, ref GameSceneObj obj)
        {
            if (obj == null
                || ident.IsNull())
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.Add Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(ObjectID ident, ref GameViewObj obj)
        {
            if (obj == null
                || ident.IsNull())
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.Add Exception:" + ex.ToString());
                return false;
            }
            return true;
        }

        public bool Remove(ObjectID ident)
        {
            if (ident.IsNull())
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return false;
                }
                mObjects.Remove(ident);
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.Remove Exception:" + ex.ToString());
            }
            return true;
        }

        public bool Change(ObjectID old_ident, ObjectID new_ident)
        {
            if (old_ident.IsNull() || new_ident.IsNull())
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(old_ident))
                {
                    return false;
                }

                if (!mObjects.ContainsKey(new_ident))
                {
                    return false;
                }

                GameObj oldObj = mObjects[old_ident];
                GameObj newObj = mObjects[new_ident];
                if (oldObj == null || newObj == null)
                {
                    return false;
                }

                newObj.SetIdent(old_ident);
                newObj.SetHash(Tools.GetHashValueCase(old_ident));

                oldObj.SetIdent(new_ident);
                oldObj.SetHash(Tools.GetHashValueCase(new_ident));
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
            return true;
        }

        public void Clear()
        {
            mObjects.Clear();
        }

        public GameObj GetObjectByIdent(ObjectID ident)
        {
            if (ident.IsNull())
            {
                return null;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return null;
                }
                return mObjects[ident];
            }
            catch (Exception ex)
            {
                //Log.Trace("Error,GameObjectSet.GetObjectByIdent Exception:" + ex.ToString());
                return null;
            }
        }

        public ObjectID GetObjectID(ObjectID ident)
        {
            GameObj obj = GetObjectByIdent(ident);
            if (obj == null)
            {
                return new ObjectID();
            }

            return obj.GetObjId();
        }

        public int GetObjectCount()
        {
            return this.mObjects.Count;
        }

        public void GetObjectList(ref VarList result)
        {
            if (result == null)
            {
                return;
            }

            foreach (KeyValuePair<ObjectID, GameObj> kvp in mObjects)
            {
                GameObj obj = kvp.Value;
                if (obj != null && (!obj.GetIdent().IsNull()))
                {
                    result.AddObject(obj.GetIdent());
                }
            }
        }

        public Dictionary<ObjectID,GameObj> GetObjectDictionary()
        {
            return mObjects;
        }

    }
}



