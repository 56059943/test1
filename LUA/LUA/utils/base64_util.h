//--------------------------------------------------------------------
// 文件名:		string_util.h
// 内  容:		字符串操作类
// 说  明:		
// 创建日期:	
// 创建人:		

//--------------------------------------------------------------------
#ifndef _Base64Util_H
#define _Base64Util_H

#include <string>

#ifndef OUT
#define  OUT
#endif // !OUT



class Base64Util
{
public:
	static std::string Base64Decode(const std::string & msg);
	static std::string Base64Encode(const std::string & msg);

	///
	/// 快速base64
	/// @param dest 目标地址
	/// @param data 源数据
	/// @param data_len 源数据长度
	/// @return dest_len 目标数据长度
	///
	static int Base64Util::Base64EncodeQuick(char * dest, const unsigned char * data, unsigned int data_len);
	static int Base64Util::Base64DecodeQuick(unsigned char * dest, const unsigned char * data, unsigned int data_len);
};


#endif// _STRINGUTIL_H
