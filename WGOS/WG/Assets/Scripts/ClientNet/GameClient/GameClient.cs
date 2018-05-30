using SysUtils;
using System;
using Fm_ClientNet.Interface;
using System.Collections.Generic;

namespace Fm_ClientNet
{
    public class GameClient : IGameClient
    {
        private GameScene mScene = null;
        private GameObjectSet mViews = null;
        private ObjectID mStrPlayerIdent = null;

        public GameClient()
        {
            mScene = null;
            mViews = new GameObjectSet();
        }

        public void SetPlayerIdent(ObjectID value)
        {
            this.mStrPlayerIdent = value;
        }

        public ObjectID GetPlayerIdent()
        {
            return this.mStrPlayerIdent;
        }

        /// <summary>
        /// 创建场景对象
        /// </summary>
        /// <param name="player_ident">主角ObjID</param>
        public GameScene CreateScene(ObjectID player_ident)
        {
            mScene = new GameScene();
            mStrPlayerIdent = player_ident;
            return mScene;
        }

        public bool DeleteScene()
        {
            mScene = null;
            return true;
        }

        /// <summary>
        /// 获取场景对象
        /// </summary>
        public IGameScene GetCurrentScene()
        {
            return mScene;
        }

        /// <summary>
        /// 获取主角对象
        /// </summary>
        public IGameSceneObj GetCurrentPlayer()
        {
            if (null == mScene)
            {
                return null;
            }
            return mScene.GetSceneObjByIdent(mStrPlayerIdent);
        }

        public GameView CreateView(ObjectID view_ident, int capacity)
        {
            GameView view = new GameView();
            view.SetCapaticty(capacity);
            mViews.Add(view_ident, ref view);
            return view;
        }

        public bool DeleteView(ObjectID view_ident)
        {
            return mViews.Remove(view_ident);
        }

        public GameView GetViewByIdent(ObjectID view_ident)
        {
            return (GameView)mViews.GetObjectByIdent(view_ident);
        }

        public bool IsPlayer(ObjectID ident)
        {
            if (null == mScene)
            {
                return false;
            }

            return (ident == mStrPlayerIdent);
        }

        public IGameSceneObj GetSceneObj(ObjectID ident)
        {
            if (null == mScene)
            {
                return null;
            }

            GameSceneObj obj = mScene.GetSceneObjByIdent(ident);
            if (null == obj)
            {
                return null;
            }

            return obj;
        }

        public IGameObj GetObj(ObjectID ident)
        {
            if (null == mScene)
            {
                return null;
            }

            GameSceneObj obj = mScene.GetSceneObjByIdent(ident);
            if (null != obj)
            {
                return obj;
            }
            GameView gameView = GetViewByIdent(ident);
            if (null != gameView)
            {
                return gameView;
            }

            return null;
        }


        public IGameView GetView(ObjectID ident)
        {
            GameView view = (GameView)mViews.GetObjectByIdent(ident);
            if (null == view)
            {
                return null;
            }

            return view;
        }

        public IGameViewObj GetViewObj(ObjectID view_ident, ObjectID item_ident)
        {
            GameView view = (GameView)mViews.GetObjectByIdent(view_ident);
            if (null == view)
            {
                return null;
            }

            IGameViewObj obj = view.GetViewObjByIdent(item_ident);
            if (obj == null)
            {
                return null;
            }
            return obj;
        }


        public int GetViewCount()
        {
            return mViews.GetObjectCount();
        }

        public void GetViewList(ref VarList args, ref VarList result)
        {
            mViews.GetObjectList(ref result);
            return;
        }

        public bool ClearAll()
        {
            try
            {
                if (null != mViews)
                {
                    mViews.Clear();
                }
            }
            catch (Exception ex)
            {
                //Log.TraceError("Error, exception " + ex.ToString());
                return false;
            }
            return true;
        }

        public void CloneObj(ref IGameObj obj, ref IGameObj cloneObj)
        {
            Dictionary<string, GameProperty> mPropSet = cloneObj.PropSets();
            foreach (KeyValuePair<string, GameProperty> kvp in mPropSet)
            {
                string key = kvp.Key;
                obj.UpdateProperty(ref key, kvp.Value.propValue.Clone());
            }


            VarList recordList = new VarList();
            cloneObj.GetRecordList(ref recordList);
            int count = recordList.GetCount();
            for (int i = 0; i < count; i++ )
            {
                string recordName = recordList.GetString(i);
                GameRecord record = cloneObj.GetGameRecordByName(recordName).Clone();
                obj.AddRecord2Set(recordName, ref record);
            }
        }


    }
}

