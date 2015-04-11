/**************************************************************************************************************************
* Login.h
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
* Description: Login class has all methods and variables needed to manage logins for our database.
*
***************************************************************************************************************************/


#ifndef LOGIN_H
#define LOGIN_H

#include<string>

class Login{
private:
	std::string userName;
	std::string password;
	bool isAdmin;

public:
	Login(std::string name, std::string pass, std::string admin);
	int comparePassword(std::string passToCompare);
	std::string getName();
	std::string getAdmin();
	//possibly add/implement a hashing method and/or salting for password security
	//std::string getPass();
	
};

#endif