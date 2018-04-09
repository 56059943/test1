//--------------------------------------------------------------------
// �ļ���:		string_util.h
// ��  ��:		�ַ���������
// ˵  ��:		
// ��������:	
// ������:		

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
	/// ����base64
	/// @param dest Ŀ���ַ
	/// @param data Դ����
	/// @param data_len Դ���ݳ���
	/// @return dest_len Ŀ�����ݳ���
	///
	static int Base64Util::Base64EncodeQuick(char * dest, const unsigned char * data, unsigned int data_len);
	static int Base64Util::Base64DecodeQuick(unsigned char * dest, const unsigned char * data, unsigned int data_len);
};


#endif// _STRINGUTIL_H
