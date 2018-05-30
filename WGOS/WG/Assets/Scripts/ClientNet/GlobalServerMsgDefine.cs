
namespace Fm_ClientNet
{
    //服务器((引擎端))->客户端消息ID定义
    public class GlobalServerMsg
    {
        public static int SERVER_SERVER_INFO = 0;
        public static int SERVER_SET_VERIFY = 1;
        public static int SERVER_SET_ENCODE = 2;
        public static int SERVER_ERROR_CODE = 3;
        public static int SERVER_LOGIN_SUCCEED = 4;
        public static int SERVER_WORLD_INFO = 5;
        public static int SERVER_IDLE = 6;
        public static int SERVER_QUEUE = 7;
        public static int SERVER_TERMINATE = 8;
        public static int SERVER_PROPERTY_TABLE = 9;
        public static int SERVER_RECORD_TABLE = 10;
        public static int SERVER_ENTRY_SCENE = 11;
        public static int SERVER_EXIT_SCENE = 12;
        public static int SERVER_ADD_OBJECT = 13;
        public static int SERVER_REMOVE_OBJECT = 14;
        public static int SERVER_SCENE_PROPERTY = 15;
        public static int SERVER_OBJECT_PROPERTY = 16;
        public static int SERVER_RECORD_ADDROW = 17;
        public static int SERVER_RECORD_DELROW = 18;
        public static int SERVER_RECORD_GRID = 19;
        public static int SERVER_RECORD_CLEAR = 20;
        public static int SERVER_CREATE_VIEW = 21;
        public static int SERVER_DELETE_VIEW = 22;
        public static int SERVER_VIEW_PROPERTY = 23;
        public static int SERVER_VIEW_ADD = 24;
        public static int SERVER_VIEW_REMOVE = 25;
        public static int SERVER_SPEECH = 26;
        public static int SERVER_SYSTEM_INFO = 27;
        public static int SERVER_MENU = 28;
        public static int SERVER_CLEAR_MENU = 29;
        public static int SERVER_CUSTOM = 30;
        public static int SERVER_LOCATION = 31;
        public static int SERVER_MOVING = 32;
        public static int SERVER_ALL_DEST = 33;
        public static int SERVER_WARNING = 34;
        public static int SERVER_FROM_GMCC = 35;
        public static int SERVER_LINK_TO = 36;
        public static int SERVER_UNLINK = 37;
        public static int SERVER_LINK_MOVE = 38;
        public static int SERVER_CP_CUSTOM = 39;			// 压缩的自定义消息
        public static int SERVER_CP_ADD_OBJECT = 40;		// 压缩的添加可见对象消息
        public static int SERVER_CP_RECORD_ADDROW = 41;// 压缩的表格添加行消息
        public static int SERVER_CP_VIEW_ADD = 42;// 压缩的容器添加对象消息
        public static int SERVER_VIEW_CHANGE = 43;// 容器改变
        public static int SERVER_CP_ALL_DEST = 44;// 压缩的对象移动消息
        public static int SERVER_ALL_PROP = 45;// 多个对象的属性改变信息
        public static int SERVER_CP_ALL_PROP = 46;// 压缩的多个对象的属性改变信息
        public static int SERVER_ADD_MORE_OBJECT = 47;// 增加多个对象
        public static int SERVER_CP_ADD_MORE_OBJECT = 48;// 压缩的增加多个对象
        public static int SERVER_REMOVE_MORE_OBJECT = 49;	// 移除多个对象
        //public static int SERVER_CHARGE_VALIDSTRING = 50; // 将计费服务器返回的验证串发给客户端
        public static int SERVER_VERSION = 50;              // 服务器下发版本
        public static int SERVER_PLATFROM_LOGIN = 51;       // 平台登陆
     }
}

