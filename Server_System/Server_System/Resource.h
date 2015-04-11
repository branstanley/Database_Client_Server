/**************************************************************************************************************************
* Resource.h
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
* Description: Resource class has all methods needed to manage individual Resources for our Database.
*
***************************************************************************************************************************/


#ifndef RESOURCE_H
#define RESOURCE_H

#define WIN32_LEAN_AND_MEAN

#include<vector>
#include<map>
#include<string>


class Resource{
private:
	std::map<std::string, std::string> AttrValPair;
	int resourceID;
	static int idCount;

public:
	Resource(std::vector<std::string> attributes, std::vector<std::string> values);
	Resource(std::vector<std::string> attributes, std::vector<std::string> values, int theID);
	int getResourceId();
	std::string getAttributeValue(std::string attribute);
	std::string serializeValues(std::vector<std::string> attributes, char delim);
	int setNewValues(std::vector<std::string> newValues, std::vector<std::string> attributes);
	void addAttribute(std::string attrName, std::string defValue);
	void removeAttribute(std::string attr);
};

#endif