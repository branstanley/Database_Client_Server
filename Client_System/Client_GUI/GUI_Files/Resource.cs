/**************************************************************************************************************************
 * Resource.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 26th, 2015
 * Date last modified: April 6th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * A class that represents all resources recieved from the server, and is used for display purposes.
 ***************************************************************************************************************************/

namespace Client_GUI.GUI_Files
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is used to create objects for the resources that are recieved from the database.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The ID of a resource.  This is used by the server to uniquely identify a resource
        /// </summary>
        private int resourceID;
        /// <summary>
        /// A list of all the attributes and their associated values.
        /// </summary>
        private List<KeyValuePair<string, string>> resources;

        /// <summary>
        /// The constructor for a resource
        /// </summary>
        /// <param name="attributes">The list of all attributes in our system</param>
        /// <param name="toParse">An unparsed string containing all the fields of this resource.</param>
        public Resource(string[] attributes, string toParse)
        {
            resources = new List<KeyValuePair<string, string>>();

            string[] fields = toParse.Split('\t');
            resourceID = int.Parse(fields[0]);
            
            for (int i = 1; i < attributes.Length; ++i)
            {
                resources.Add(new KeyValuePair<string, string>(attributes[i], fields[i]));
            }
        }

        /// <summary>
        /// Used to get a resource's ID
        /// </summary>
        /// <returns>The ID of the resource</returns>
        public int getID()
        {
            return resourceID;
        }

        /// <summary>
        /// Gets the value of a specific attribute
        /// </summary>
        /// <param name="attribute">The value of that attribute, or else null (name malformed or wrong)</param>
        /// <returns></returns>
        public string findAttribute(string attribute)
        {
            if (attribute.Equals("ID"))
            {
                return "" + getID();
            }
            foreach (KeyValuePair<string, string> t in resources)
            {
                if(attribute.Equals(t.Key))
                {
                    return t.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Modifies the value of a specific attribute
        /// </summary>
        /// <param name="attribute">Attribute field to moddify</param>
        /// <param name="value">The value we're modifying to</param>
        public void modifyResource(string attribute, string value)
        {
            for (int i = 0; i < resources.Count; ++i)
            {
                if(attribute.Equals(resources[i].Key))
                {
                    resources[i] = new KeyValuePair<string, string>(resources[i].Key, value);
                }
            }
        }

        /// <summary>
        /// A \t seperated string of the attribute values
        /// </summary>
        /// <returns>A formated string for socket transfer</returns>
        public string Serialize()
        {
            return resourceID + "\t" + values();
        }

        /// <summary>
        /// Used to get all the values in the Resource without the ID number
        /// </summary>
        /// <returns>A \t seperated string of attribute values with the ID number</returns>
        public string values()
        {
            StringBuilder sb = new StringBuilder();
            bool first = false;

            foreach (KeyValuePair<string, string> t in resources)
            {
                if (first)
                    sb.Append("\t");
                sb.Append(t.Value);
                first = true;
            }
            return sb.ToString();
        }
    }
}
