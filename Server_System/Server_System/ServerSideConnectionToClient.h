/**************************************************************************************************************************
* ServerSideConnectionToClient.h
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
* Description: ServerSideConnectionToClient handles the connections to individual Clients. It takes in control codes
* from its client, and runs the corresponding methods.
***************************************************************************************************************************/


#ifndef SERVERSIDECONNECTION_H
#define SERVERSIDECONNECTION_H

#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <vector>

#include "Resource.h"
//#include "ServerSystem.h"

class ServerSystem;

class ServerSideConnectionToClient{
private:
	SOCKET ClientSocket;
	std::vector<Resource*> searched;
	std::vector<Resource*>* modifiedSearch;
	ServerSystem* Server;
	std::string serializeResults(std::vector<Resource*>* inVector);
	std::string getData();

	static int DEFAULT_BUFLEN;
	static std::string DEFAULT_PORT;
	static char LINE_DELIM;
	static char COL_DELIM;

public:
	ServerSideConnectionToClient(SOCKET newSocket, ServerSystem* serverPtr, int DEFAULT_BUFLEN, std::string DEFAULT_PORT, char LINE_DELIM, char COL_DELIM);
	void filter(std::string term);
	std::vector<Resource*>* sort(std::string attribute, bool ascending);
	void run();		//marked on class diagram as having void* return and arg... why?
};

#endif