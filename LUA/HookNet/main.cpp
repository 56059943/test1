#include <iostream>
#include <string>
#include <time.h>  
#include <stdio.h>

#include  <winsock2.h> 
#pragma comment(lib, "WS2_32")

#include "my.h"

int main(int argc, char* argv[]) {
	//批处理命令中加上 pause 暂停看运行效果

	WORD wVersion;
	WSADATA wsadata;
	int err;
	wVersion = MAKEWORD(2, 2);
	// WSAStartup() initiates the winsock,if successful,the function returns zero
	err = ::WSAStartup(wVersion, &wsadata);
	if (err != 0)
	{
		printf("Couldn't initiate the winsock!\n");
	}
	else
	{
		// create a socket
		SOCKET ServerSock = socket(AF_INET, SOCK_RAW, IPPROTO_IP);
		int sockfd = socket(AF_INET, SOCK_STREAM, 0);  //获得fd

		char mname[128];
		struct hostent* pHostent;
		sockaddr_in myaddr;
		//Get the hostname of the local machine
		if (-1 == gethostname(mname, sizeof(mname)))
		{
			closesocket(ServerSock);
			printf("%d", WSAGetLastError());
			exit(-1);
		}
		else
		{
			//Get the IP adress according the hostname and save it in pHostent  
			pHostent = gethostbyname((char*)mname);
			//填充sockaddr_in结构
			myaddr.sin_addr = *(in_addr *)pHostent->h_addr_list[0];
			myaddr.sin_family = AF_INET;
			myaddr.sin_port = htons(88881);//对于IP层可随意填

			myaddr.sin_addr.s_addr = inet_addr("127.0.0.1");
			myaddr.sin_family = AF_INET;
			myaddr.sin_port = htons(8884);//对于IP层可随意填
			//bind函数创建的套接字句柄绑定到本地地址
			if (SOCKET_ERROR == bind(sockfd, (struct sockaddr *)&myaddr, sizeof(myaddr)))
			{
				closesocket(sockfd);
				std::cout << WSAGetLastError << std::endl;
				printf("..............................Error……");
				getchar();
				exit(-1);
			}

			//设置该SOCKET为接收所有流经绑定的IP的网卡的所有数据，包括接收和发送的数据包
			u_long sioarg = 1;
			DWORD dwValue = 0;
			DWORD SIO_RCVALL = 0xFFFFFFFFFFFFFFFF;
			if (SOCKET_ERROR == WSAIoctl(sockfd, SIO_FIND_ROUTE, &sioarg, sizeof(sioarg), NULL, 0, &dwValue, NULL, NULL))
			{
				closesocket(sockfd);
				std::cout << WSAGetLastError();
				getchar();
				exit(-1);
			}
			//获取分析数据报文
			char buf[65535];
			int len = 0;
			listen(ServerSock, 5);
			do
			{
				len = recv(ServerSock, buf, sizeof(buf), 0);
				if (len > 0)
				{
					//报文处理
				}
			} while (len > 0);
		}
	}
	::WSACleanup();

	//system("pause");
	return 0;
}
