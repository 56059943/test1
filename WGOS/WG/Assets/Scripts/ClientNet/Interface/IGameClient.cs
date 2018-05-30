using System;
using System.Collections.Generic;
using SysUtils;

namespace Fm_ClientNet.Interface
{
    //基类
    public interface IGameObj
    {
        GameRecord GetGameRecordByName(string name);
        bool UpdateProperty(ref string name, VarList.VarData val);
        int GetRecordCols(string name);
        int GetRecordRows(string name);
        bool FindRecord(string name);
        int GetPropType(string name);
        bool FindProp(string name);
        int GetRecordColType(string name, int col);
        int FindRecordRowCI(string name, int col, VarList.VarData key, int begRow);
        int FindRecordRow(string name, int col, VarList.VarData key, int begRow);

        VarList.VarData QueryRecord(string name, int row, int col);
        bool QueryRecordBool(string name, int row, int col, ref bool bResult);
        bool QueryRecordInt(string name, int row, int col, ref int iResult);
        bool QueryRecordInt64(string name, int row, int col, ref long iResult);
        bool QueryRecordFloat(string name, int row, int col, ref float fResult);
        bool QueryRecordDouble(string name, int row, int col, ref double dResult);
        bool QueryRecordString(string name, int row, int col, ref string strResult);
        bool QueryRecordStringW(string name, int row, int col, ref string strResult);
        bool QueryRecordObject(string name, int row, int col, ref ObjectID oResult);

        VarList.VarData QueryProp(string name);
        bool QueryPropBool(string strPropName, ref bool bResult);
        bool QueryPropInt(string strPropName, ref int iResult);
        bool QueryPropFloat(string strPropName, ref float fResult);
        bool QueryPropDouble(string strPropName, ref double dResult);
        bool QueryPropString(string strPropName, ref string strResult);
        bool QueryPropStringW(string strPropName, ref string strResult);
        bool QueryPropObject(string strPropName, ref ObjectID oResult);
        void GetPropList(ref VarList result);
        void GetRecordList(ref VarList result);
        ObjectID GetIdent();
        void SetIdent(ObjectID value);
        ObjectID GetObjId();
        void SetObjId(ObjectID id);
        bool AddRecord2Set(string name, ref GameRecord record);

        Dictionary<string, GameProperty> PropSets();

        bool QueryPropInt64(string strPropName, ref long lResult);
    }

    public interface IGameSceneObj : IGameObj
    {
        // 位置
        float GetPosiX();
        float GetPosiY();
        float GetPosiZ();
        float GetOrient();

        // 移动目标
        float GetDestX();
        float GetDestY();
        float GetDestZ();
        float GetDestOrient();
        float GetMoveSpeed();
        float GetRotateSpeed();
        float GetJumpSpeed();
        float GetGravity();

        // 移动模式
        void SetMode(int value);
        int GetMode();


        // 设置位置
        bool SetLocation(float x, float y, float z, float orient);
        // 设置移动目标
        bool SetDestination(float x, float y, float z, float orient,
            float move_speed, float rotate_speed, float jump_speed, float gravity);

        // 连接对象
        void SetLinkIdent(string value);
        string GetLinkIdent();

        // 连接位置
        float GetLinkX();
        float GetLinkY();
        float GetLinkZ();
        float GetLinkOrient();

        // 设置连接位置
        bool SetLinkPos(float x, float y, float z, float orient);
    }

    public interface IGameScene : IGameObj
    {
        GameSceneObj AddSceneObj (ObjectID object_ident);
        bool RemoveSceneObj(ObjectID object_ident);
        // 获得场景中对象号
        IGameSceneObj GetSceneObj(ObjectID object_ident);

        // 获得场景中对象列表
        void GetSceneObjList(ref VarList args, ref VarList result);

        // 获得场景中对象数量
        int GetSceneObjCount();
    }

    public interface IGameView : IGameObj
    {
        int GetCapacity();
        IGameViewObj GetViewObjByIdent(ObjectID item_ident);
        int GetViewObjCount();
        void GetViewObjList(ref VarList args, ref VarList result);
        Dictionary<ObjectID, GameObj> GetViewObjDictionary();
    }

    public interface IGameViewObj : IGameObj
    {

    }

    public interface IGameClient
    {
        GameScene CreateScene(ObjectID ident);

        // 获得当前场景
        IGameScene GetCurrentScene();

        ObjectID GetPlayerIdent();

        void SetPlayerIdent(ObjectID value);

        // 获得当前主角对象
        IGameSceneObj GetCurrentPlayer();

        // 是否玩家对象
        bool IsPlayer(ObjectID ident);

        // 获得场景内对象
        IGameSceneObj GetSceneObj(ObjectID ident);
        /// <summary>
        /// 获取对象
        /// </summary>
        IGameObj GetObj(ObjectID ident);

        // 获得视窗对象
        IGameView GetView(ObjectID view_ident);

        // 获得视窗内对象
        IGameViewObj GetViewObj(ObjectID view_ident, ObjectID item_ident);

        // 获得视窗数量
        int GetViewCount();

        // 获得视窗列表
        void GetViewList(ref VarList args, ref VarList result);

        // 清除所有数据
        bool ClearAll();

        void CloneObj(ref IGameObj obj, ref IGameObj cloneObj);

    }
}

