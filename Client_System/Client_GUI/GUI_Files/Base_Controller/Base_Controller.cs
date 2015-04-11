/**************************************************************************************************************************
 * Base_Controller.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 26th, 2015
 * Date last modified: April 6th, 2015
 * **************************************************************************************************************************
 * Approved By:
 * Date Approved:
 * **************************************************************************************************************************
 * Approved By:
 * Date Approved:
 * **************************************************************************************************************************
 * Description:
 * The base class for all of the GUI Pane event handler classes.
 * Defines methods that all of the controllers will need in operation.
 ***************************************************************************************************************************/

namespace Client_GUI.GUI_Files.Base_Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class Base_Controller
    {
        /// <summary>
        /// A reference to the Main_Window_Frame's controller
        /// </summary>
        Main_Window_Frame.Main_Window_Control mainController;

        /// <summary>
        /// The Base_Controller constructor
        /// </summary>
        /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
        public Base_Controller(Main_Window_Frame.Main_Window_Control mainController)
        {
            this.mainController = mainController;
        } // End Base_Controller

        /// <summary>
        /// Used to gain access to the Main_Window_Frame_Controller
        /// </summary>
        /// <returns>A refer to the Main_Window_Frame_Controller</returns>
        public Main_Window_Frame.Main_Window_Control getMainController()
        {
            return mainController;
        } // End getMainController

        /// <summary>
        /// Used to gain access to the client system to communicate to the server
        /// </summary>
        /// <returns>A reference to the client system</returns>
        public CppWrapper.ClientWrapper getClient(){
            return mainController.getClient();
        } // End getClient
    } // End Base_Controller
} // End Namespace
