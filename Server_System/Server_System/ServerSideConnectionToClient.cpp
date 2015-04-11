/**************************************************************************************************************************
* ServerSideConnectionToClient.cpp
*
* Created By: John Simko
* Date of Creation: April 1st, 2015
* Date last modified: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Brandon Stanley
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Shahood Mirza
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Description: ServerSideConnectionToClient handles the connections to individual Clients. It takes in control codes
* from its client, and runs the corresponding methods. This is the implementation file for ServerSideConnectionToClient.
***************************************************************************************************************************/

#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <sstream>
#include <algorithm>

#include"ServerSideConnectionToClient.h"
#include"ServerSystem.h"

#define HEXCTRLCODE 1


int ServerSideConnectionToClient::DEFAULT_BUFLEN = 512;
std::string ServerSideConnectionToClient::DEFAULT_PORT = "65222";
char ServerSideConnectionToClient::LINE_DELIM = '\n';
char ServerSideConnectionToClient::COL_DELIM = ',';

/// class constructor. takes in several constants set by 
/// the server system class, as well as the socket connected 
/// to the client and a pointer to the server system object
ServerSideConnectionToClient::ServerSideConnectionToClient(SOCKET newSocket, ServerSystem* serverPtr, int BUFLEN, std::string PORT, char LINE, char COL){
	ClientSocket = newSocket;
	Server = serverPtr;
	DEFAULT_BUFLEN = BUFLEN;
	DEFAULT_PORT = PORT;
	LINE_DELIM = LINE;
	COL_DELIM = COL;
}

/// takes in a string containing the term to filter by.
/// checks if an already filtered list exists, and if so
/// applies to the new filter to that list. Otherwise, 
/// applies filter to base search results.
void ServerSideConnectionToClient::filter(std::string term){
	std::vector<Resource*>* temp;

	if ((modifiedSearch != NULL) && (modifiedSearch->size() > 0)){
		temp = modifiedSearch;
	}
	else{
		temp = &searched;
	}

	// Construct a new vector for our modified search
	modifiedSearch = new std::vector<Resource*>;

	for (size_t i = 0; i < temp->size(); i++){
		// Get the keywords string for this resource
		std::string value = temp->at(i)->getAttributeValue("Keywords");
		// Get the individual key words
		std::vector<std::string> keys = Server->parseStrToVector(value, ',');

		for (size_t j = 0; j < keys.size(); j++){
			if (keys[j].compare(term) == 0){
				modifiedSearch->push_back(temp->at(i));
				break;
			} // End if
		} // End inner for
	} // End outer for
} // End filter

/// takes in string containing attribute to sort by, and bool 
/// representing ascending or descending sort order.
/// returns pointer to sorted list for serializing output.
std::vector<Resource*>* ServerSideConnectionToClient::sort(std::string attribute, bool ascending){
	
	std::vector<Resource*>* toSort;
	Resource* bubble;
	// check if we are sorting filtered search vector or searched vector.
	if ((modifiedSearch != NULL) && (modifiedSearch->size() > 0)){
		toSort = modifiedSearch;
	}
	else{
		toSort = &searched;
	}

	int j = 2;
	for (size_t i = 0; i < toSort->size(); i++){
		bubble = toSort->at(toSort->size() - 1);
		for (j = toSort->size() - 2; j > ((int)i) - 1; j--){
			if (ascending){
				if (bubble->getAttributeValue(attribute) < toSort->at(j)->getAttributeValue(attribute))
					std::iter_swap(bubble, toSort->at(j));
				bubble = toSort->at(j);
			}
			else{
				if (bubble->getAttributeValue(attribute) > toSort->at(j)->getAttributeValue(attribute))
					std::iter_swap(bubble, toSort->at(j));
				bubble = toSort->at(j);
			}
		}
	}
	return toSort;
}

/// takes in pointer to vector of Resource pointers, and appends 
/// all values of each Resource in vector into a single string for
/// socket transmission. delimits attribute values and resources
/// based on given constants in constructor.
std::string ServerSideConnectionToClient::serializeResults(std::vector<Resource*>* inVector){

	std::string results = "";
	for (size_t i = 0; i < inVector->size(); i++){
		if (i > 0) results.append("\n");
		results.append(
			inVector->at(i)->serializeValues(
			Server->getAttributeList(), COL_DELIM));

	}
	return results;
}

/// Reads from socket repeatedly and appends to single string, 
/// until no more data is in buffer.
/// returns string containing entire send.
std::string ServerSideConnectionToClient::getData()
{
	// Receive until the peer closes the connection

	std::stringstream toBuild;
	int iResult;
	char* recvbuf;
	recvbuf = (char*)malloc(sizeof(char) * DEFAULT_BUFLEN);
	int recvbuflen = DEFAULT_BUFLEN;
	u_long iMode = 0;
	ioctlsocket(ClientSocket, FIONBIO, &iMode);

	do {
		iResult = recv(ClientSocket, recvbuf, recvbuflen - 1, 0);

		if (iMode == 0){
			iMode = 1;
			ioctlsocket(ClientSocket, FIONBIO, &iMode);
		}

		if (iResult == -1)
		{
			Sleep(50);
			iResult = recv(ClientSocket, recvbuf, recvbuflen - 1, 0);
			if (iResult == -1){

				std::string output = toBuild.str();
				printf("Bytes received: %d\n", iResult);
				std::cout << "received: " << output << std::endl;
				iMode = 0;
				ioctlsocket(ClientSocket, FIONBIO, &iMode);
				return output;
			}
		}

		recvbuf[iResult] = '\0';
		toBuild << recvbuf;
		printf("Bytes received: %d\n", iResult);
		Sleep(10);

	} while (iResult > 0);
}

/// main method for ClientConnection to server. listens 
/// for control codes from client, and calls corresponding 
/// methods based on code received.
void ServerSideConnectionToClient::run(){
	char *recvbuf;
	recvbuf = (char*)malloc(sizeof(char) * DEFAULT_BUFLEN);
	int iResult, iSendResult;
	int recvbuflen = DEFAULT_BUFLEN;

	bool isAdmin = false;
	bool dataChanged = false;

	// Receive until the peer shuts down the connection
	do {

		iResult = recv(ClientSocket, recvbuf, HEXCTRLCODE, 0);

#pragma region Switch case
		if (iResult > 0) {			
			if (recvbuf[0] == '\x0E') std::cout << "it is a login" << std::endl;

			switch (recvbuf[0]){
			case '\x01':{			//request list of attributes
				std::vector<std::string> attributes = Server->getAttributeList();
				std::string serialized = "";
				for (size_t i = 0; i < attributes.size(); i++){
					if (i > 0) serialized.append("\t");
					serialized.append(attributes[i]);
				}
				iSendResult = send(ClientSocket, serialized.c_str(), (int)strlen(serialized.c_str()), 0);
				break;
			}

			case '\x02':{			//add filter
				std::string inStr = getData();
				if (searched.size() < 1)
					break;
				filter(inStr);
				std::string toSend = serializeResults(modifiedSearch);

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				std::cout << "filter results: " << modifiedSearch->size() << std::endl;
				break;
			}

			case '\x03':{			//Search
				std::string inStr = getData();
				std::vector<std::string> values = Server->parseStrToVector(inStr, COL_DELIM);
				if (values.size() != 2){
					if (values.size() == 1){
						searched = Server->search(values[0], "");
					}
					else{
						break;
					}
				}
				else{
					searched = Server->search(values[0], values[1]);
				}
				std::string toSend = serializeResults(&searched);
				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				std::cout << "search results: " << searched.size() << std::endl;
				break;
			}

			case '\x04':{			//Sort
				std::string inStr = getData();
				std::vector<std::string> split = Server->parseStrToVector(inStr, COL_DELIM);
				bool asc = 1;
				if (split[1] == "false") asc = 0;
				std::string toSend;
				toSend = serializeResults(sort(split[0], asc));
				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				break;
			}

			case '\x05':{			//Remove Filter	
				std::string inStr = getData();
				delete modifiedSearch; // Need to clean up the pointer
				modifiedSearch = NULL;
				if (inStr.compare("0") != 0){
					std::vector<std::string> current = Server->parseStrToVector(inStr, COL_DELIM);
					for (size_t i = 0; i < current.size(); i++){
						filter(current[i]);
					}
				}
				std::string toSend;
				if (modifiedSearch == NULL)
					toSend = serializeResults(&searched);
				else 
					toSend = serializeResults(modifiedSearch);

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				break;
			}

			case '\x06':{			//Add Attribute
				if (!isAdmin) break;
				std::string inStr = getData();
				std::stringstream ss;
				bool success = Server->addAttribute(inStr);
				ss << success;
				std::string toSend(ss.str());

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x07':{			//Remove Attribute
				if (!isAdmin) break;
				std::string inStr = getData();
				std::stringstream ss;
				int result = Server->removeAttribute(inStr);
				ss << result;
				std::string toSend = ss.str();
				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x08':{			//add resource
				if (!isAdmin) break;
				std::string inStr = getData();
				bool result = Server->addNewResource(inStr);
				//string stream result to send back;
				std::stringstream ss;
				ss << result;
				std::string toSend = ss.str();

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x09':{			//modify resource
				if (!isAdmin) break;
				std::string inStr = getData();
				bool result = Server->modifyResource(inStr);
				std::stringstream ss;
				ss << result;
				std::string toSend = ss.str();

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x0A':{			//remove resource
				if (!isAdmin) break;
				std::string inStr = getData();
				bool result = Server->removeResource(inStr);
				std::stringstream ss;
				ss << result;
				std::string toSend = ss.str();

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x0B':{			//request users
				if (!isAdmin) break;
				std::string logins = Server->getListOfLogins();
				iSendResult = send(ClientSocket, logins.c_str(), (int)strlen(logins.c_str()), 0);
				break;
			}

			case '\x0C':{			//add new user
				if (!isAdmin) break;
				std::string inStr = getData();
				bool result = Server->addNewLogin(inStr);
				std::stringstream ss;
				ss << result;
				std::string toSend = ss.str();

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x0D':{			//remove user
				if (!isAdmin) break;
				std::string inStr = getData();
				bool result = Server->removeLogin(inStr);
				std::stringstream ss;
				ss << result;
				std::string toSend = ss.str();

				iSendResult = send(ClientSocket, toSend.c_str(), (int)strlen(toSend.c_str()), 0);
				dataChanged = true;
				break;
			}

			case '\x0E':{			//Login
				iResult = recv(ClientSocket, recvbuf, DEFAULT_BUFLEN, 0);
				recvbuf[iResult] = '\0';
				std::string loginStr(recvbuf);
				std::vector<std::string> loginParse = Server->parseStrToVector(loginStr, '\n');
				int result = Server->login(loginParse[0]);
				std::stringstream ss;
				ss << result;
				std::string Res = ss.str();
				if (result == 3) isAdmin = true;
				iSendResult = send(ClientSocket, Res.c_str(), (int)strlen(Res.c_str()), 0);
				break;
			}

			default:{
				iSendResult = SOCKET_ERROR;
				break;
			}
			}
#pragma endregion
			recvbuf[1] = '\0';

			if (iSendResult == SOCKET_ERROR) {
				std::cout << "send failed: " << WSAGetLastError() << std::endl;
				closesocket(ClientSocket);
				WSACleanup();
				return;
			}
			if (iSendResult == 0){
				std::string toSend = "0";
				iSendResult = send(ClientSocket, toSend.c_str(), 1, 0);
			}
			std::cout << "Bytes sent: " << iSendResult << std::endl;
			iSendResult = 2;
		}
		else if (iResult == 0){
			std::cout << "Connection closing..." << std::endl;
			if (dataChanged) Server->export();
		}
		else {
			std::cout << "recv failed: " << WSAGetLastError() << std::endl;
			closesocket(ClientSocket);
			WSACleanup();
			return;
		}
		//std::this_thread::sleep_for(std::chrono::milliseconds(5));
	} while (iResult > 0);
}
