/**************************************************************************************************************************
* ServerSystem.h
*
* Created By: John Simko
* Date of Creation: March 31st, 2015
* Date last modified: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Brandon Stanley
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Shahood Mirza
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Description: ServerSystem class is the core control class for our Database Server. It has the majority of the
* Server's methods and spawns threads for each client socket connection.
***************************************************************************************************************************/


#ifndef SERVERSYSTEM_H
#define SERVERSYSTEM_H

#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <thread>
#include <mutex>

#include "Resource.h"
#include "ServerSideConnectionToClient.h"
#include "Login.h"

class ServerSystem{
private:
	SOCKET listenSocket;
	std::vector<std::string> Attributes;
	std::vector<Resource> Resources;
	std::vector<Login> Logins;
	std::vector<ServerSideConnectionToClient> clientConnections;
	std::mutex resourceWriteLock;
	int currThreads;
	static int MAXXCONN;

	void loadLogins(std::string filename);
	void loadResources(std::string filename);
	void acceptConnection();
	std::vector<std::string> parseFileToRows(std::string fileName, char delim);

public:
	static int DEFAULT_BUFLEN;
	static std::string DEFAULT_PORT;
	static char LINE_DELIM;
	static char COL_DELIM;

	void export();
	int login(std::string loginStr);
	std::vector<Resource*> search(std::string attribute, std::string searchString);
	bool addNewResource(std::string newResource);
	bool modifyResource(std::string resourceData);		//make this an int and a string? (resource ID, new values)
	bool removeResource(std::string toRemoved);			//can't we change this to an int for resource ID?
	bool addAttribute(std::string newAttribute);
	int removeAttribute(std::string toRemove);
	void run();
	bool addNewLogin(std::string newLogin);
	bool removeLogin(std::string toRemove);
	std::string getListOfLogins();
	std::vector<std::string> getAttributeList();
	std::vector<std::string> parseStrToVector(std::string row, char delim);

};

#endif