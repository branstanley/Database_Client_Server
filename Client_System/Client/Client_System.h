/**************************************************************************************************************************
* Client_System.h
*
* Created By: Shahood Mirza
* Date of Creation: March 31st, 2015
* Date last modified: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Brandon Stanley
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Approved By: John Simko
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Description:
* Header file for the Client system class
***************************************************************************************************************************/

#pragma once
//#include "stdafx.h"
#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdexcept>
#include <string>
#include <iostream>
#include <fstream>

// Need to link with Ws2_32.lib, Mswsock.lib, and Advapi32.lib
#pragma comment (lib, "Ws2_32.lib")
#pragma comment (lib, "Mswsock.lib")
#pragma comment (lib, "AdvApi32.lib")

#define DEFAULT_BUFLEN 1020
//#define DEFAULT_PORT "65222"

using namespace std;

class Client_System
{
private:
	int iResult;
	WSADATA wsaData;
	struct addrinfo *result = NULL, *ptr = NULL, hints;
	SOCKET ConnectSocket = INVALID_SOCKET;
	char recvbuf[DEFAULT_BUFLEN];
	int recvbuflen = DEFAULT_BUFLEN;

	const char* portnumber;

	const char *ip1;
	const char **ip3;

	string addr;
	int portnum;

	string sendstr;

public:
	Client_System();
	~Client_System();

	int login(string userName, string password, string ip, string port);

	string search(string columnName, string searchString);
	string filterByKeyword(string keyword, bool adding);
	string sortByAttribute(string attributeName, bool descending);

	bool addNewResource(string resource);
	bool modifyResource(string resource);
	bool removeResource(string resource);

	bool addAttribute(string attributeName, string defaultValue);
	bool removeAttribute(string attributeName);

	bool addNewLogin(string userName, string password, bool isAdmin);
	bool removeLogin(string userName);

	bool exportDatabase();
	string requestListOfLogins();
	string requestListOfAttributes();


	int sendData(const char *arg);
	char* getData();
	int open();
	char* close();
};
