/**************************************************************************************************************************
* Resource.cpp
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
* Description: Resource class has all methods needed to manage individual Resources for our Database.
* This is the implementation file for all Resource class methods
***************************************************************************************************************************/


#include<string>
#include<vector>
#include<sstream>

#include "Resource.h"

/// This is a static integer used to keep track of the current 
/// highest resource ID, to prevent duplicate IDs
int Resource::idCount = 0;

/// One of two Resource constructors.
/// This constructor is called when receiving new resource during runtime, 
/// because it does not take ID as an argument.
Resource::Resource(std::vector<std::string> attributes, std::vector<std::string> values){
	resourceID = ++idCount;
	std::stringstream ss;
	ss << resourceID;
	AttrValPair[attributes[0]] = ss.str();
	for (size_t i = 1; i < attributes.size(); i++){
		AttrValPair[attributes[i]] = values[i-1];
	}
}

/// One of two Resource constructors.
/// This constructor is called when loading resources from file, 
/// because it takes the resource ID stored on file as an argument.
Resource::Resource(std::vector<std::string> attributes, std::vector<std::string> values, int theID){
	if (theID > idCount){
		resourceID = theID;
		idCount = theID;
	}
	else{
		resourceID = ++idCount;
	}

	for (size_t i = 0; i < attributes.size(); i++){
		if (values[i] == " ")
			AttrValPair[attributes[i]] = "";
		else AttrValPair[attributes[i]] = values[i];
	}
}

/// This method takes no arguments
/// and returns an integer resource ID.
int Resource::getResourceId(){
	return resourceID;
}

/// takes string denoting attribute value wanted.
/// returns resource's value mapped to that attribute.
std::string Resource::getAttributeValue(std::string attribute){
	return AttrValPair[attribute];
}

/// takes in the server's list vector of attributes and the desired delimiter.
/// serializes all values in resource, with given delimiter between each. returns string.
std::string Resource::serializeValues(std::vector<std::string> attributes, char delim){
	std::string serialized;
	std::stringstream ss;
	ss << delim;
	std::string delimiter = ss.str();
	for (size_t i = 0; i < attributes.size(); i++){
		serialized.append(AttrValPair[attributes[i]]);
		if(i < attributes.size()-1) serialized.append(delimiter);
	}
	return serialized;
}

/// used for modifying existing resources attributes.
/// takes in vector of strings holding new values, and list of current attributes.
/// returns int 1 for success.
int Resource::setNewValues(std::vector<std::string> newValues, std::vector<std::string> attributes){

	for (size_t i = 1; i < attributes.size(); i++){
		AttrValPair[attributes[i]] = newValues[i];
	}
	return 1;
}

/// used to add a new attribute to the resource map of values.
/// takes in a string for new attribute name, and string for 
/// default value. no return.
void Resource::addAttribute(std::string attrName, std::string defValue){
	AttrValPair[attrName] = defValue;
}

/// takes in string with name of attribute to remove.
/// removes attribute from resource map. no return.
void Resource::removeAttribute(std::string attr){
	AttrValPair.erase(attr);
}