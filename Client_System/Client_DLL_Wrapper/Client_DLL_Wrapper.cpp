/**************************************************************************************************************************
* managedDllWrapper.cpp
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
* Wrapper class required for communication between the C# GUI and C++ client-side functions
* This class builds a DLL for use wih the GUI portions of the project
***************************************************************************************************************************/

// This is the main DLL file.

#include "stdafx.h"
#include "Client_DLL_Wrapper.h"

#include "Client_System.h"
#include "Client_System.cpp"

/// Function for converting a C# string from the GUI
/// to a C++ string for the client-side
string CppWrapper::ClientWrapper::GUIToClientString(String^ s) {
    
    using namespace Runtime::InteropServices;
    const char* chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
    string os(chars);
    Marshal::FreeHGlobal(IntPtr((void*)chars));
    return os;
}

/// Function for converting a C++ string from the client-side
/// to a C# string for the GUI
String^ CppWrapper::ClientWrapper::clientToGUIString(string ns){
    using namespace System::Runtime::InteropServices;
    return Marshal::PtrToStringAnsi(static_cast<IntPtr>(const_cast<char *>(ns.c_str())));
}

/// Constructor implementaion
/// Initates the C++ client class's instance
CppWrapper::ClientWrapper::ClientWrapper()
{
	client_system = new Client_System();
}

/// Wrapper for the client login method
/// Allows for calling the client login method from the GUI
int CppWrapper::ClientWrapper::login(String^ userName, String^ password, String^ ip, String^ port)
{
	return client_system->login(GUIToClientString(userName), GUIToClientString(password), GUIToClientString(ip), GUIToClientString(port));
}

/// Wrapper for the client search method
/// Allows for calling the client search method from the GUI
String^ CppWrapper::ClientWrapper::search(String^ columnName, String^ searchString)
{
    return clientToGUIString(client_system->search(GUIToClientString(columnName), GUIToClientString(searchString)));
}

/// Wrapper for the client filter method
/// Allows for calling the client filter method from the GUI
String^ CppWrapper::ClientWrapper::filterByKeyword(String^ keyword, bool adding)
{
	return clientToGUIString(client_system->filterByKeyword(GUIToClientString(keyword), adding));
}

/// Wrapper for the client sort method
/// Allows for calling the client sort method from the GUI
String^ CppWrapper::ClientWrapper::sortByAttribute(String^ attributeName, bool descending)
{
	return clientToGUIString(client_system->sortByAttribute(GUIToClientString(attributeName), descending));
}

/// Wrapper for the client add resource method
/// Allows for calling the client add resource method from the GUI
bool CppWrapper::ClientWrapper::addNewResource(String^ resource)
{
	return client_system->addNewResource(GUIToClientString(resource));
}

/// Wrapper for the client modify resource method
/// Allows for calling the client modify resource  method from the GUI
bool CppWrapper::ClientWrapper::modifyResource(String^ resource)
{
	return client_system->modifyResource(GUIToClientString(resource));
}

/// Wrapper for the client remove resource  method
/// Allows for calling the client remove resource method from the GUI
bool CppWrapper::ClientWrapper::removeResource(String^ resource)
{
	return client_system->removeResource(GUIToClientString(resource));
}

/// Wrapper for the client add attribute method
/// Allows for calling the client add attribute method from the GUI
bool CppWrapper::ClientWrapper::addAttribute(String^ attributeName, String^ defaultValue)
{
	return client_system->addAttribute(GUIToClientString(attributeName), GUIToClientString(defaultValue));
}

/// Wrapper for the client remove attribute method
/// Allows for calling the client remove attribute method from the GUI
bool CppWrapper::ClientWrapper::removeAttribute(String^ attributeName)
{
	return client_system->removeAttribute(GUIToClientString(attributeName));
}

/// Wrapper for the client add login method
/// Allows for calling the client add login method from the GUI
bool CppWrapper::ClientWrapper::addNewLogin(String^ userName, String^ password, bool isAdmin)
{
	return client_system->addNewLogin(GUIToClientString(userName), GUIToClientString(password), isAdmin);
}

/// Wrapper for the client remove login method
/// Allows for calling the client remove login method from the GUI
bool CppWrapper::ClientWrapper::removeLogin(String^ userName)
{
	return client_system->removeLogin(GUIToClientString(userName));
}

/// Wrapper for the client export method
/// Allows for calling the client export method from the GUI
bool CppWrapper::ClientWrapper::exportDatabase()
{
	return client_system->exportDatabase();
}

/// Wrapper for the client user request method
/// Allows for calling the client user request method from the GUI
String^ CppWrapper::ClientWrapper::requestListOfLogins()
{
	return clientToGUIString(client_system->requestListOfLogins());
}

/// Wrapper for the client attribute request method
/// Allows for calling the client attribute request method from the GUI
String^ CppWrapper::ClientWrapper::requestListOfAttributes()
{
	return clientToGUIString(client_system->requestListOfAttributes());
}
