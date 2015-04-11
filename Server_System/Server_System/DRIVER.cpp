/**************************************************************************************************************************
* DRIVER.cpp
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
* Description: This is a very simple driver for the Server to run. It only contains a main method which instantiates
* the ServerSystem class and calls it's run method.
***************************************************************************************************************************/

#include<string>

#include"ServerSystem.h"

void main(){

	ServerSystem MainServer;

	MainServer.run();

}