#include <iostream>
#include <string>
#include <time.h>  
#include <stdio.h>

#include  <winsock2.h> 
#pragma comment(lib, "WS2_32")

#include "my.h"

int main(int argc, char* argv[]) {
	//�����������м��� pause ��ͣ������Ч��

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
		int sockfd = socket(AF_INET, SOCK_STREAM, 0);  //���fd

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
			//���sockaddr_in�ṹ
			myaddr.sin_addr = *(in_addr *)pHostent->h_addr_list[0];
			myaddr.sin_family = AF_INET;
			myaddr.sin_port = htons(88881);//����IP���������

			myaddr.sin_addr.s_addr = inet_addr("127.0.0.1");
			myaddr.sin_family = AF_INET;
			myaddr.sin_port = htons(8884);//����IP���������
			//bind�����������׽��־���󶨵����ص�ַ
			if (SOCKET_ERROR == bind(sockfd, (struct sockaddr *)&myaddr, sizeof(myaddr)))
			{
				closesocket(sockfd);
				std::cout << WSAGetLastError << std::endl;
				printf("..............................Error����");
				getchar();
				exit(-1);
			}

			//���ø�SOCKETΪ�������������󶨵�IP���������������ݣ��������պͷ��͵����ݰ�
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
			//��ȡ�������ݱ���
			char buf[65535];
			int len = 0;
			listen(ServerSock, 5);
			do
			{
				len = recv(ServerSock, buf, sizeof(buf), 0);
				if (len > 0)
				{
					//���Ĵ���
				}
			} while (len > 0);
		}
	}
	::WSACleanup();

	//system("pause");
	return 0;
}
