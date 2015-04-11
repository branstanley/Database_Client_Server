/**************************************************************************************************************************
* Client_System.cpp
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
* Client system for communicating between the GUI and the Server
* Esablishes a connection with the server and accepts user input through the GUI for using the database
***************************************************************************************************************************/

#pragma once

#include "Client_System.h"
#include <sstream>

/// Default constructor for the client system class
/// Takes no arguments on creation of an object
Client_System::Client_System()
{
}

/// Destructor method for the client system
/// Closes any open socket connection when called
Client_System::~Client_System()
{
	close();
}

/// Function for when the user attempts to login to the system
/// Username/password are parsed and sent to the server along with the login function code
/// Accepts the username, password, ip address and port number from the GUI
/// Returns a integer value depending on user access (ie. 1 for user, 3 for admin)
int Client_System::login(string userName, string password, string ip, string port)
{
	//connection takes char** as ip, need to convert from string
	ip1 = ip.c_str();
	ip3 = &ip1;

	portnumber = port.c_str();

    sendstr = userName + "\t" + password;
	open();
    sendData("\xE"); //function code
	sendData(sendstr.c_str());

	//close connection if user is not authorized for access
    int result = atoi(getData());
    if (result == 0){
        close();
    }

	return result;

}

/// Function for searching the database
/// The search criteria is parsed and sent to the server along with the search function code
/// Accepts the name of the column to search and the string to search for from the GUI
/// Returns a string of results found from the server
string Client_System::search(string columnName, string searchString)
{
	string sendstr1 = columnName + "\t" + searchString;
	sendData("\x3"); //function code
	sendData(sendstr1.c_str());

	return getData();
}

/// Function for filtering returned results
/// Filtering is done by selected keyword and different function codes based on adding or removing filter
/// Accepts the keyword to filter by and whether the filter is checked or unchecked
/// Returns a string of filtered results
string Client_System::filterByKeyword(string keyword, bool adding)
{
    if (adding)
	    sendData("\x2"); //add filter
    else
        sendData("\x5"); //remove filter

    sendData(keyword.c_str());

	return getData();
}

/// Function for sorting returned results
/// Sorting is done by selecting the column (attribute) to sort by in the GUI
/// Accepts the name of the column to search and either ascending or descending order
/// Returns a string of sorted results
string Client_System::sortByAttribute(string attributeName, bool descending)
{
	sendstr = attributeName + "\t" + (descending ? "true" : "false");

	sendData("\x4"); //function code
	sendData(sendstr.c_str());

	return getData();
}

/// Function for adding a new resource (ADMIN ONLY)
/// Accepts a string containing information about the new resource
/// Returns a boolean value depending on whether the resource was added successfully
bool Client_System::addNewResource(string resource)
{
	sendstr = resource;

	sendData("\x8"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for changing an existing resource (ADMIN ONLY)
/// Accepts a string containing information about the resource to be changed
/// Returns a boolean value depending on whether the resource was modified successfully
bool Client_System::modifyResource(string resource)
{
	sendstr = resource;

	sendData("\x9"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for removing a resource (ADMIN ONLY)
/// Accepts a string containing information about the resource to be removed
/// Returns a boolean value depending on whether the resource was added successfully
bool Client_System::removeResource(string resource)
{
	sendstr = resource;

	sendData("\xA"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for adding a new attribute to the database (ADMIN ONLY)
/// Accepts the attribute name along with an optional default value
/// Returns a boolean value depending on whether the attribute was added successfully
bool Client_System::addAttribute(string attributeName, string defaultValue)
{
	//building the string depending on if a default value was provided
	if ((defaultValue.compare("")) == 0)
		sendstr = attributeName;
	else
		sendstr = attributeName + "\t" + defaultValue;

	sendData("\x6"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for adding removing an attribute from the database (ADMIN ONLY)
/// Accepts the attribute name to be removed
/// Returns a boolean value depending on whether the attribute was removed successfully
bool Client_System::removeAttribute(string attributeName)
{
	sendstr = attributeName;

	sendData("\x7"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for adding a new user to the system (ADMIN ONLY)
/// Accepts the username/password and a boolean value if they require admin access
/// Returns a boolean value depending on whether the user was added successfully
bool Client_System::addNewLogin(string userName, string password, bool isAdmin)
{
    ostringstream form;
    form << userName << "\t" << password << "\t" << (isAdmin ? 1 : 0);
    sendstr = form.str();
	sendData("\xC"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function for removing a user from the system (ADMIN ONLY)
/// Accepts the username to be removed
/// Returns a boolean value depending on whether the user was removed successfully
bool Client_System::removeLogin(string userName)
{
	sendstr = userName;

	sendData("\xD"); //function code
	sendData(sendstr.c_str());

	if (strcmp(getData(), "1") == 0)
		return true;
	else
		return false;
}

/// Function to export the entire database to a file (ADMIN ONLY)
/// Exports to a file called Database.txt in the folder where the program is run from
bool Client_System::exportDatabase()
{
	ofstream dbfile;

	//do a blank search to return all resources
	string s = search("Document Title","");
	string delimiter = "\n";

	dbfile.open("Database.txt");

	int pos = 0;
	string token;

	//seperate each returned resource
	while ((pos = s.find(delimiter)) != string::npos) {
		token = s.substr(0, pos);
		dbfile << token << endl; //write to file
		s.erase(0, pos + delimiter.length());
	}

	dbfile << s << endl;
	dbfile.close();

	return true;
}

/// Function to pull the list of current users stored in the system (ADMIN ONLY)
/// Returns a string of all usernames and their access level
string Client_System::requestListOfLogins()
{
    sendstr = "";
	sendData("\xB"); //function code

	//returned list in form username1\tisAdmin\nusername2\tisAdmin...
	return getData();
}

/// Function to pull the list of attribues used in the database (ADMIN ONLY)
/// Returns a string of all attributes
string Client_System::requestListOfAttributes()
{
	sendData("\x1"); //function code

	//returned list in form attribute1\tattribute2...
	return getData();
}

/// Function to send data to the server as a character array
/// Returns 1 if send fails, 0 if send is successful
int Client_System::sendData(const char *sendbuf)
{
	// Send an initial buffer
	iResult = send(ConnectSocket, sendbuf, (int)strlen(sendbuf), 0);
	if (iResult == SOCKET_ERROR)
	{
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
	}
	return 0;
}

/// Function to receive data from the server
/// Returns the data from the server as a character array
char* Client_System::getData()
{
	// Receive until the peer closes the connection

    ostringstream toBuild;


    u_long iMode = 0;
    ioctlsocket(ConnectSocket, FIONBIO, &iMode);

	do {
		iResult = recv(ConnectSocket, recvbuf, recvbuflen-1, 0);

        if (iMode == 0){
            iMode = 1;
            ioctlsocket(ConnectSocket, FIONBIO, &iMode);
        }

        if (iResult == -1)
        {
            Sleep(50);
            iResult = recv(ConnectSocket, recvbuf, recvbuflen - 1, 0);
            if (iResult == -1){

                string output = toBuild.str();

                iMode = 0;
                ioctlsocket(ConnectSocket, FIONBIO, &iMode);
                char *fix = (char*)malloc(sizeof(char)* output.length());
                strcpy(fix, output.c_str());
                return fix;
            }
		}

        recvbuf[iResult] = '\0';
        toBuild << recvbuf;
        Sleep(10);
	} while (iResult > 0);
}

/// Function for establishing a socket connection with the server
/// Returns 1 if connection fails, 0 if connection is successful
int Client_System::open()
{
	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0)
	{
		return 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	// Resolve the server address and port
	iResult = getaddrinfo(ip3[0], portnumber, &hints, &result);
	if (iResult != 0)
	{
		WSACleanup();
		return 1;
	}
	// Attempt to connect to an address until one succeeds
	for (ptr = result; ptr != NULL; ptr = ptr->ai_next)
	{
		// Create a SOCKET for connecting to server
		ConnectSocket = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);
		if (ConnectSocket == INVALID_SOCKET)
		{
			WSACleanup();
			return 1;
		}

		// Connect to server
		iResult = connect(ConnectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);
		if (iResult == SOCKET_ERROR)
		{
			closesocket(ConnectSocket);
			ConnectSocket = INVALID_SOCKET;
			continue;
		}

		break;
	}

	freeaddrinfo(result);

	if (ConnectSocket == INVALID_SOCKET)
	{
		WSACleanup();
		return 1;
	}
	return 0;
}

/// Function for closing the socket connection with the server
/// Returns -1 if connection fails, 0 if connection is successful
char* Client_System::close()
{
	// shutdown the connection since no more data will be sent
	iResult = shutdown(ConnectSocket, SD_SEND);
	if (iResult == SOCKET_ERROR)
	{
		closesocket(ConnectSocket);
		WSACleanup();
		return "-1";
	}

	// cleanup
	closesocket(ConnectSocket);
	WSACleanup();
	return "0";
}