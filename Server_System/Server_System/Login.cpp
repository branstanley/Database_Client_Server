/**************************************************************************************************************************
* Login.cpp
*
* Created By: John Simko
* Date of Creation: April 1st, 2015
* Date last modified: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Brandon Stanley
* Date Approved: April 9th, 2015
* **************************************************************************************************************************
* Approved By: Shahood Mirza
* Date Approved:April 9th, 2015
* **************************************************************************************************************************
* Description: Login class has all methods and variables needed to manage logins for our database.
* This is the implementation file for the login class.
***************************************************************************************************************************/


#include <string>

#include "Login.h"

/// Login constructor.
/// Takes in strings for name, pass, and admin status.
Login::Login(std::string name, std::string pass, std::string admin){
	userName = name;
	password = pass;
	if (admin == "1") isAdmin = 1;
	else isAdmin = 0;
}

/// takes in string containing password to check against.
/// returns int containing result. 0 for failed. 1 for user. 3 for admin.
int Login::comparePassword(std::string passToCompare){
	if (passToCompare == password){
		if (isAdmin) return 3;
		return 1;
	}
	return 0;
}

/// takes no arguments.
/// returns this objects username.
std::string Login::getName(){
	return userName;
}

/// takes no arguments.
/// returns this objects admin status in string form.
std::string Login::getAdmin(){
	if (isAdmin) return "true";
	return "false";
}
