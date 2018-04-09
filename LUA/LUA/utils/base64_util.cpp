//--------------------------------------------------------------------
// 文件名:		string_util.cpp
// 内  容:		字符串操作类
// 说  明:		
// 创建日期:	
// 创建人:		

//--------------------------------------------------------------------

#include "base64_util.h"

// BASE64块解码
void base64_decode_block(unsigned char* dst, const char* src)
{
	unsigned int x = 0;

	for (int i = 0; i < 4; ++i)
	{
		if (src[i] >= 'A' && src[i] <= 'Z')
		{
			x = (x << 6) + (unsigned int)(src[i] - 'A' + 0);
		}
		else if (src[i] >= 'a' && src[i] <= 'z')
		{
			x = (x << 6) + (unsigned int)(src[i] - 'a' + 26);
		}
		else if (src[i] >= '0' && src[i] <= '9')
		{
			x = (x << 6) + (unsigned int)(src[i] - '0' + 52);
		}
		else if (src[i] == '+')
		{
			x = (x << 6) + 62;
		}
		else if (src[i] == '/')
		{
			x = (x << 6) + 63;
		}
		else if (src[i] == '=')
		{
			x = (x << 6);
		}
	}

	dst[2] = (unsigned char)(x & 255);
	x >>= 8;
	dst[1] = (unsigned char)(x & 255);
	x >>= 8;
	dst[0] = (unsigned char)(x & 255);
}

// BASE64解码
int base64_decode(unsigned char* dst, const char* src, int inlen)
{
	int length = 0;

	while ((length < inlen) && (src[length] != '='))
	{
		length++;
	}

	int equal_num = 0;

	while (((length + equal_num) < inlen) && (src[length + equal_num] == '='))
	{
		equal_num++;
	}

	int block_num = (length + equal_num) / 4;

	for (int i = 0; i < (block_num - 1); ++i)
	{
		base64_decode_block(dst, src);

		dst += 3;
		src += 4;
	}

	unsigned char last_block[3];

	base64_decode_block(last_block, src);

	for (int k = 0; k < (3 - equal_num); ++k)
	{
		dst[k] = last_block[k];
	}

	return (block_num * 3) - equal_num;
}



int base64_encode(char * dest, const char * data, int len) {
	static char alphabet[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
	//char[] out = new char[((data.length + 2) / 3) * 4];
	int index = 0;
	for (int i = 0; i < len; i += 3, index += 4) {
		bool quad = false;
		bool trip = false;
		int val = (0xFF & (int)data[i]);
		val <<= 8;
		if ((i + 1) < len) {
			val |= (0xFF & (int)data[i + 1]);
			trip = true;
		}
		val <<= 8;
		if ((i + 2) < len) {
			val |= (0xFF & (int)data[i + 2]);
			quad = true;
		}
		dest[index + 3] = alphabet[(quad ? (val & 0x3F) : 64)];
		val >>= 6;
		dest[index + 2] = alphabet[(trip ? (val & 0x3F) : 64)];
		val >>= 6;
		dest[index + 1] = alphabet[val & 0x3F];
		val >>= 6;
		dest[index + 0] = alphabet[val & 0x3F];
	}
	return index;
}

std::string Base64Util::Base64Decode(const std::string & msg)
{
	//char * dst = new char[msg.size()];
	if (msg.size() > 2000)
	{
		return "base64decode-fail-msg-size-must-less-2000";
	}
	char dst[2048] = { 0 };
	int len = base64_decode((unsigned char *)dst, msg.c_str(), msg.size());
	std::string rt = std::string(dst, len);
	//delete[] dst;
	return rt;
}


std::string Base64Util::Base64Encode(const std::string & msg)
{
	char * dst = new char[msg.size()*2+1];
	int len = base64_encode(dst, (char *)msg.c_str(), msg.size());
	std::string rt = std::string(dst, len);
	delete[] dst;
	return rt;
}

int Base64Util::Base64EncodeQuick(char * dest, const unsigned char * data, unsigned int len)
{
	static char alphabet[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
	//char[] out = new char[((data.length + 2) / 3) * 4];
	unsigned int index = 0;
	for (unsigned int i = 0; i < len; i += 3, index += 4) {
		bool quad = false;
		bool trip = false;
		int val = (0xFF & (int)data[i]);
		val <<= 8;
		if ((i + 1) < len) {
			val |= (0xFF & (int)data[i + 1]);
			trip = true;
		}
		val <<= 8;
		if ((i + 2) < len) {
			val |= (0xFF & (int)data[i + 2]);
			quad = true;
		}
		dest[index + 3] = alphabet[(quad ? (val & 0x3F) : 64)];
		val >>= 6;
		dest[index + 2] = alphabet[(trip ? (val & 0x3F) : 64)];
		val >>= 6;
		dest[index + 1] = alphabet[val & 0x3F];
		val >>= 6;
		dest[index + 0] = alphabet[val & 0x3F];
	}
	return index;
}

int Base64Util::Base64DecodeQuick(unsigned char * dst, const unsigned char * msg, unsigned int msg_len)
{
	return base64_decode(dst, (char *)msg, msg_len);
}