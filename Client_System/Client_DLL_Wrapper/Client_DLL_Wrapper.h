/**************************************************************************************************************************
* managedDllWrapper.h
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
* Header file for the Client wrapper class
***************************************************************************************************************************/

#pragma once

#include "Client_System.h"
#include "Client_System.cpp"

using namespace System;

namespace CppWrapper {
	public ref class ClientWrapper
	{
	public:
		// constructor
		ClientWrapper();

		// wrapper methods
		int login(String^ userName, String^ password, String^ ip, String^ port);

        String^ search(String^ columnName, String^ searchString);
        String^ filterByKeyword(String^ keyword, bool adding);
        String^ sortByAttribute(String^ attributeName, bool descending);

        bool addNewResource(String^ resource);
		bool modifyResource(String^ resource);
		bool removeResource(String^ resource);

		bool addAttribute(String^ attributeName, String^ defaultValue);
		bool removeAttribute(String^ attributeName);

		bool addNewLogin(String^ userName, String^ password, bool isAdmin);
		bool removeLogin(String^ userName);

		bool exportDatabase();
		String^ requestListOfLogins();
		String^ requestListOfAttributes();

        void CloseConnection(){
            client_system->close();
        }

	private:
		Client_System *client_system; // an instance of class in C++

        static string GUIToClientString(String^ s);
        static String^ clientToGUIString(string ns);
	};

}