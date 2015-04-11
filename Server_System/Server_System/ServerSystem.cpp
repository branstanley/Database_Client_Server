/*************************************************************************************************************************//*
* ServerSystem.cpp
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
* Description: ServerSystem class is the core control class for our Database Server. It has the majority of the
* Server's methods and spawns threads for each client socket connection. This is the implementation file for ServerSystem
*//*************************************************************************************************************************/


#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <vector>
#include <process.h>
#include <Windows.h>
#include <thread>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <string>

#include "ServerSystem.h"
#include "ServerSideConnectionToClient.h"
#include "Login.h"
#include "Resource.h"

/* This is needed to tell the MS VC compiler to link the Winsock library */
#pragma comment(lib, "Ws2_32.lib")

int ServerSystem::MAXXCONN = 0;
int ServerSystem::DEFAULT_BUFLEN = 512;
std::string ServerSystem::DEFAULT_PORT = "65222";
char ServerSystem::LINE_DELIM = '\n';
char ServerSystem::COL_DELIM = ',';

/// handles the inital loading of logins and resources.
/// once loaded, call acceptConnection which listens for
/// client connection requests
void ServerSystem::run(){

	std::vector<std::string> entries;
	std::ifstream resourceFile("./ServerSettings.txt");
	std::string row;
	if (resourceFile.is_open()){
		int i = 0;
		while (getline(resourceFile, row, '\n')){
			entries.push_back(row);
			if (entries[i][0] == '#') entries.pop_back();
			else i++;
		}
	}
	resourceFile.close();

	MAXXCONN = std::stoi(entries[2]);
	DEFAULT_PORT = entries[3];
	DEFAULT_BUFLEN = std::stoi(entries[4]);
	if (entries[5] != "") LINE_DELIM = entries[5][0];
	else LINE_DELIM = '\n';
	COL_DELIM = entries[6][0];

	loadLogins(entries[1]);
	std::cout << "logins loaded." << std::endl;

	loadResources(entries[0]);
	std::cout << "Resources loaded." << std::endl;

	acceptConnection();

}

/// is called if an admin makes changes to data then disconnects.
/// exports all logins and resources to file.
void ServerSystem::export(){
	//export resources
	std::string export = "";
	for (size_t i = 0; i < Resources.size(); i++){
		if (i > 0) export.append("\n");
		export.append(Resources[i].serializeValues(getAttributeList(), COL_DELIM));
	}
	//open resources file and write export
	std::ofstream resfile("outRes.txt");
	if (resfile.is_open()){
		resfile << export;
	}
	std::cout << "Done exporting resources." << std::endl;
	//export logins
	//open logins file and write:

	std::ofstream logFile("outLog.txt");
	if (logFile.is_open()){
		logFile << getListOfLogins();
	}
	std::cout << "Done exporting logins." << std::endl;
}

/// non-class method used when spawning new thread.
/// calls constructor for ClientSideConnectionToServer, 
/// then calls it's run method.
void newThread(SOCKET clientConn, ServerSystem* serverPtr){
	ServerSideConnectionToClient newClient(
		clientConn, serverPtr, ServerSystem::DEFAULT_BUFLEN, ServerSystem::DEFAULT_PORT, ServerSystem::LINE_DELIM, ServerSystem::COL_DELIM);
	newClient.run();
}

/// lengthy method that sets up all socket connection settings, then 
/// listens for client connection requests, and passes each connection
/// to its own thread, up to a limit of MAXXCONN (set in ServerSettings.txt)
void ServerSystem::acceptConnection(){

	int iResult;
	struct addrinfo *result = NULL, *addrPtr = NULL, hints;
	WSADATA wsaData;

	//	char *filename, *contents;
	//	struct sockaddr_in test;

#pragma region Socket setup
	/**** Initialize Winsock: needed for Windows */
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		std::cout << "WSAStartup failed: " << iResult << std::endl;
		return;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	// Resolve the local address and port to be used by the server
	iResult = getaddrinfo(NULL, DEFAULT_PORT.c_str(), &hints, &result);
	if (iResult != 0) {
		std::cout << "get addr info failed: " << iResult << std::endl;
		WSACleanup();
		return;
	}

	listenSocket = INVALID_SOCKET;

	listenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);

	if (listenSocket == INVALID_SOCKET) {
		std::cout << "Error at socket(): " << WSAGetLastError() << std::endl;
		freeaddrinfo(result);
		WSACleanup();
		return;
	}

	// Setup the TCP listening socket
	iResult = bind(listenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) {
		printf("bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(listenSocket);
		WSACleanup();
		return;
	}
	freeaddrinfo(result);

	if (listen(listenSocket, MAXXCONN) == SOCKET_ERROR) {
		printf("Listen failed with error: %ld\n", WSAGetLastError());
		closesocket(listenSocket);
		WSACleanup();
		return;
	}

#pragma endregion

	std::cout << "Socket built" << std::endl;
	SOCKET ClientSocket = INVALID_SOCKET;
	currThreads = 0;
	std::vector<std::thread> serverThreads;
	do{
		std::cout << "waiting to accept..." << std::endl;
		ClientSocket = accept(listenSocket, NULL, NULL);

		if (ClientSocket == INVALID_SOCKET){
			std::cout << "Accept Failed: " << WSAGetLastError() << std::endl;
		}
		else{
			serverThreads.push_back(std::thread(newThread, ClientSocket, this));
		}
	} while (true);

	closesocket(listenSocket);
	WSACleanup();

}

/// calls file parsing method with filename from 
/// ServerSettings.txt for pre-existing user logins
void ServerSystem::loadLogins(std::string filename){

	std::vector<std::string> entries = parseFileToRows(filename, LINE_DELIM);

	for (size_t i = 0; i < entries.size(); i++){
		addNewLogin(entries[i]);
	}
}

/// calls file parsing method with filename from
/// ServerSettings.txt for pre-existing Resources.
void ServerSystem::loadResources(std::string filename){

	std::vector<std::string> entries = parseFileToRows(filename, LINE_DELIM);

	Attributes = parseStrToVector(entries[0], COL_DELIM);

	entries.erase(entries.begin());

	for (size_t i = 0; i < entries.size(); i++){
		addNewResource(entries[i]);
	}

}

/// Takes in a string filename to read and char for row delimiter, 
/// and returns vector of strings, each holding a row.
std::vector<std::string> ServerSystem::parseFileToRows(std::string fileName, char delim){
	std::vector<std::string> entries;
	std::ifstream resourceFile(fileName);
	std::string row;
	if (resourceFile.is_open()){
		while (getline(resourceFile, row, delim)){
			if (row.size() == 0) continue;
			entries.push_back(row);
		}
	}
	resourceFile.close();
	return entries;
}

/// Takes in a string and char delimiter, and returns a 
/// vector of strings seperated by the given delimiter.
std::vector<std::string> ServerSystem::parseStrToVector(std::string row, char delim){
	std::stringstream strm(row);
	std::vector<std::string> entry;
	std::string tempStr;
	while (std::getline(strm, tempStr, delim)){
		entry.push_back(tempStr);
	}
	return entry;
}

/// takes in string containing sent user name and password, 
/// parses and checks against all existing names and passwords.
/// returns 0 for fail, 1 for user login, 3 for admin login
int ServerSystem::login(std::string loginStr){
	std::string name = "";
	std::string pass = "";

	//parse the loginStr into name and pass
	std::vector<std::string> entry = parseStrToVector(loginStr, COL_DELIM);
	name = entry[0];
	pass = entry[1];

	//compare name to each login.username
	for (size_t i = 0; i < Logins.size(); i++){
		if (name == Logins[i].getName()){
			//if name match found, compare password
			int result = Logins[i].comparePassword(pass);
			return result;
		}
	}
	return 0;
}

/// takes in attribute to search by and string to search for.
/// checks the given attribute of each resource for the full search string
/// first, and then for each seperate word in the search string.
/// returns vector of resource pointers containing search results.
std::vector<Resource*> ServerSystem::search(std::string attribute, std::string searchString){
	std::vector<Resource*> results;
	std::string resourceValue;
	size_t pos;
	if (searchString.length() == 0){	//if blank search string, return all resources.
		for (size_t i = 0; i < Resources.size(); i++){
			results.push_back(&Resources[i]);
		}
		return results;
	}
	else{
		//make search string lowercase
		std::transform(searchString.begin(), searchString.end(), searchString.begin(), ::tolower);

		//compare to entire string first
		//.push_back matches
		for (size_t i = 0; i < Resources.size(); i++){
			resourceValue = Resources[i].getAttributeValue(attribute);
			//make resource field lower case, then search for search string
			std::transform(resourceValue.begin(), resourceValue.end(), resourceValue.begin(), ::tolower);
			pos = resourceValue.find(searchString);
			if (pos != std::string::npos){
				results.push_back(&Resources[i]);
			}
		}

		//parse to seperate words and add matches for each word.
		std::vector<std::string> subSearch = parseStrToVector(searchString, ' ');
		if (subSearch.size() <= 1) return results;

		//search for each string
		//.push_back matches (unless match already present)
		for (size_t i = 0; i < subSearch.size(); i++){
			for (size_t j = 0; j < Resources.size(); j++){
				resourceValue = Resources[j].getAttributeValue(attribute);
				//make resource field lower case, then search for search string
				std::transform(resourceValue.begin(), resourceValue.end(), resourceValue.begin(), ::tolower);
				pos = resourceValue.find(subSearch[i]);
				if (pos != std::string::npos){
					int check = 0;
					for (size_t k = 0; k < results.size(); k++){
						if (results[k]->getResourceId() == Resources[j].getResourceId())
							check = 1;
					}
					if (check == 0)
						results.push_back(&Resources[j]);
				}
			}
		}
	}
	return results;
}

/// takes in string with delimiters, parses into vector and 
/// constructs new resource object with given values.
/// return true if succeeds, false if bad data given.
bool ServerSystem::addNewResource(std::string newResource){

	std::vector<std::string> entry = parseStrToVector(newResource, COL_DELIM);

	///if the number of fields given matches number of attributes, then
	///given data already has an ID (and is thus read from file). 
	///else, it has no ID, and was input during runtime.
	if (entry.size() == Attributes.size()){
		int thisID;
		std::stringstream convert(entry[0]);
		if (!(convert >> thisID)) thisID = 0;
		Resource temp(Attributes, entry, thisID);
		Resources.push_back(temp);
		return true;
	}
	else if (entry.size() == Attributes.size() - 1){
		Resource temp(Attributes, entry);
		Resources.push_back(temp);
		return true;
	}

	return false;
}

/// takes in string with delimiters containing all attribute values
/// for a pre-existing resource (some fields are modified)
/// if given resource ID is found, changes all values to match
/// given data. returns 1 if success, 0 if failed.
bool ServerSystem::modifyResource(std::string resourceData){

	//parse the data into a vector
	std::vector<std::string> newData = parseStrToVector(resourceData, COL_DELIM);
	//search for resource with matching ID
	std::vector<Resource*> originalData; 
	for (size_t i = 0; i < Resources.size(); i++){
		if (newData[0].compare(Resources[i].getAttributeValue(Attributes[0])) == 0){
			originalData.push_back(&Resources[i]);
		}
	}

	//if more than one match found, something is wrong (same ID on multiple resources is bad).
	if (originalData.size() != 1) return false;
	//modify each value to match given
	originalData[0]->setNewValues(newData, Attributes);
	return true;
}

/// takes in string containing resourceID of a resource to be removed
/// removes resource and returns boolean true if succeeded.
bool ServerSystem::removeResource(std::string toRemove){
	int removeMe = std::stoi(toRemove);
	for (size_t i = 0; i < Resources.size(); i++){
		if (Resources[i].getResourceId() == removeMe){
			Resources.erase(Resources.begin() + i);
			return true;
		}
	}
	return false;
}

/// takes in a string with either only a new attribute name, or
/// a new attribute name and a default value to fill that attribute with.
/// returns boolean true if succeeds.
bool ServerSystem::addAttribute(std::string newAttribute){
	//may have a default value
	std::string defaultVal = "";
	std::vector<std::string> values = parseStrToVector(newAttribute, COL_DELIM);
	// if more than one string in vector, then default value was given.
	if (values.size() > 1) defaultVal = values[1];
	for (size_t i = 0; i < Resources.size(); i++){
		Resources[i].addAttribute(values[0], defaultVal);
	}
	// add new attribute to server list of attributes.
	Attributes.push_back(values[0]);
	return true;
}

/// takes a string containing the attribute to be removed.
/// returns true if succeeds, false if fails.
int ServerSystem::removeAttribute(std::string toRemove){
	std::vector<std::string>::const_iterator ptr = std::find(Attributes.begin(), Attributes.end(), toRemove);
	if (ptr == Attributes.end())
		return 0;
	Attributes.erase(ptr);
	// for each resource in Resources vector, remove given attribute from map.
	for (size_t i = 0; i < Resources.size(); i++){
		Resources[i].removeAttribute(toRemove);
	}
	return 1; //1 for success.
}

/// takes in single delimited string containing username, password, 
/// and number representing admin status.
///returns boolean true if added, boolean false if failed to add.
bool ServerSystem::addNewLogin(std::string newLogin){

	std::vector<std::string> entry = parseStrToVector(newLogin, COL_DELIM);

	if (entry.size() != 3) return false;

	for (size_t i = 0; i < Logins.size(); i++){
		if (entry[0] == Logins[i].getName()) return false;
	}

	Login tempLog(entry[0], entry[1], entry[2]);
	Logins.push_back(tempLog);

	return true;
}

/// takes in string containing username to be removed.
///	searches existing users and returns true if found.
/// returns bool false if not found.
bool ServerSystem::removeLogin(std::string toRemove){
	for (size_t i = 0; i < Logins.size(); i++){
		if (Logins[i].getName() == toRemove){
			Logins.erase(Logins.begin() + i);
			return true;
		}
	}
	return false;
}

/// takes in no arguments.
/// returns concatenated string of all user names and their admin status
std::string ServerSystem::getListOfLogins(){
	std::string logins = "";
	std::stringstream ss1, ss2;
	ss1 << COL_DELIM;
	ss2 << LINE_DELIM;
	for (size_t i = 0; i < Logins.size(); i++){
		if (i > 0)logins.append(ss2.str());
		logins.append(Logins[i].getName());
		logins.append(ss1.str());
		logins.append(Logins[i].getAdmin());
	}
	return logins;
}

/// takes in no arguments.
/// returns vector of strings containing each current resource attribute.
std::vector<std::string> ServerSystem::getAttributeList(){
	return Attributes;
}