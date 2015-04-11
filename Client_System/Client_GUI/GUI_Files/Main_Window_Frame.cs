/**************************************************************************************************************************
 * Main_Window_Frame.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 23rd, 2015
 * Date last modified: April 5th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * This is the main window that displays everything related to the GUI
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client_GUI.GUI_Files;
    using CppWrapper;

    /// <summary>
    /// This is the main window that displays everything related to the GUI
    /// </summary>
    public class Main_Window_Frame : Form
    {
        /// <summary>
        /// The Main_Window_Controller information handler
        /// </summary>
        Main_Window_Control controller;

        #region log in window variables
        /// <summary>
        /// A reference to the Login_Pane
        /// </summary>
        private Form loginPane;

        /// <summary>
        /// This panel is used to hold the login window form.
        /// </summary>
        private Panel loginContainer;


        /// <summary>
        /// Used to determine if the connection needs to be terminated or not during window close.
        /// </summary>
        private bool isLoggedIn = false;
        #endregion

        #region Logged in User Window variables
        /// <summary>
        /// This is the various window panes for the GUI after the user has logged in.
        /// </summary>
        private Form[] panes;
        /// <summary>
        /// The main Panel containing all the logged in User/Admin GUI Content.
        /// </summary>
        private TabControl main;
        /// <summary>
        /// The tabs for each of the logged in User/Admin GUI content.
        /// </summary>
        private TabPage [] tabs;
        /// <summary>
        /// A boolean expression tracking if the logged in user is an admin or not.
        /// </summary>
        private bool isAdmin;
        #endregion

        /// <summary>
        /// The constructor for the Main_Window_Frame
        /// </summary>
        public Main_Window_Frame()
        {
            isAdmin = false;
            controller = new Main_Window_Control(this);
            InitializeWindowFrame();
            InitializeLoginWindow();
        } // End Main_Window_Frame Constructor

        #region Initialization Of Components
        /// <summary>
        /// This method is used to initialize the main window frame for the client system's GUI.
        /// Note:
        /// This does not build any of the GUI pages, mearly sets up the frame.
        /// </summary>
        private void InitializeWindowFrame()
        {
            SuspendLayout();

            FormClosing += closing;

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            Name = "Main_Window_Frame";
            Text = "SAIL: Searching Artificial Intelligence Literature";
            ShowIcon = false;
            
            ClientSize = new System.Drawing.Size(784, 562);
            MaximumSize = new System.Drawing.Size(800, 600);
            MinimumSize = new System.Drawing.Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ResumeLayout(false);

        } // End InitializeWindowFrame

        /// <summary>
        /// Creates and Displays the Login_Pane GUI page.  Used on start for a user to log in.
        /// </summary>
        private void InitializeLoginWindow()
        {
            loginContainer = new Panel();
            loginContainer.Location = new System.Drawing.Point(0, 0);
            loginContainer.Name = "Login Container";
            loginContainer.Size = new System.Drawing.Size(784, 562);
            loginContainer.Dock = DockStyle.Fill;

            Controls.Add(loginContainer);
            loginPane = new Login_Pane(controller);
            loginContainer.Controls.Add(loginPane);

            loginContainer.Show();
            loginPane.Show();

        } // End InitializeLoginWindow

        /// <summary>
        /// Used to created all the User/Admin GUI content after they have logged in.  Creates all the tabs and pages that is used within the program.
        /// Content created depends on if the user is a regular user or an Admin.
        /// </summary>
        /// <param name="isAdmin">A boolean value that determines if the GUI generated is for a regular user or an Admin.</param>
        private void InitializeUserWindows(bool isAdmin)
        {
            isLoggedIn = true;
            Controls.Remove(loginContainer);
            this.isAdmin = isAdmin;
            // Set up the Tab Content Controller
            main = new TabControl();
            main.Dock = DockStyle.Fill;
            int numberOfPanesToAdd = 0;

            // Determine how many panes will be adding based on the logged in role
            if (isAdmin)
            {
                panes = new Form[5];
                tabs = new TabPage[5];
                numberOfPanesToAdd = 5;
            }
            else
            {
                panes = new Form[2];
                tabs = new TabPage[2];
                numberOfPanesToAdd = 2;
            }

            // Add the reference for panes to tabs, add the tabs to the TabController
            for (int i = 0; i < numberOfPanesToAdd; ++i)
            {
                tabs[i] = new TabPage();
            }

            // Common panes between both user and admin
            panes[0] = new Search_Pane(controller);
            tabs[0].Text = "Search";
            tabs[0].Enter += ((Search_Pane)panes[0]).getController().refreshScreen;

            panes[1] = new Tutorial_Pane(isAdmin);
            tabs[1].Text = "Tutorial";

            // add admin stuff now
            if (isAdmin)
            {
                panes[2] = new User_Pane(controller);
                tabs[2].Text = "Users";
                tabs[2].Enter += ((User_Pane)panes[2]).getController().populateUserList;

                panes[3] = new Resource_Pane(controller);
                tabs[3].Text = "Resources";
                tabs[3].Enter += ((Resource_Pane)panes[3]).getController().refreshScreen;

                panes[4] = new Attribute_Pane(controller);
                tabs[4].Text = "Attributes";
                tabs[4].Enter += ((Attribute_Pane)panes[4]).getController().refreshScreen;

            }


            Controls.Add(main);
            for (int i = 0; i < panes.Length; ++i)
            {
                tabs[i].Controls.Add(panes[i]);
                main.Controls.Add(tabs[i]);
                panes[i].Show();
            }
            main.Show();
            Show();
        } // End InitializeUserWindows
        #endregion

        /// <summary>
        /// Event handler called when the window is closed.
        /// </summary>
        /// <param name="sender">The object causing the event to fire.  Not used.</param>
        /// <param name="e">The arguements associated with the event.  Not used.</param>
        private void closing(object sender, FormClosingEventArgs e)
        {
            if (isLoggedIn)
            {
                controller.getClient().CloseConnection();
            }
        }

        /// <summary>
        /// Main Control class for the GUI.
        /// Contains common data used by all GUI Pages, as well as the Client connection to the server.
        /// </summary>
        public class Main_Window_Control
        {
            /// <summary>
            /// The client connection
            /// </summary>
            ClientWrapper client;

            /// <summary>
            /// A reference to the Main_Window_Frame
            /// </summary>
            Main_Window_Frame mwf;

            /// <summary>
            /// The current list of attributes
            /// </summary>
            string[] attributes;
            /// <summary>
            /// The current list of resources
            /// </summary>
            List<Resource> resources;

            /// <summary>
            /// The constructor for the Main_Window_Controller.
            /// </summary>
            /// <param name="mwf"></param>
            public Main_Window_Control(Main_Window_Frame mwf)
            {
                this.mwf = mwf;
                client = new ClientWrapper();
                attributes = null;
                resources = new List<Resource>(); // There are no resources until a search has been performed
            } // End Main_Window_Control

            /// <summary>
            /// Gives access to the client connection for the various operations that need to be performed.
            /// </summary>
            /// <returns>A reference to the client connection</returns>
            public ClientWrapper getClient()
            {
                return client;
            } // End getClient

            /// <summary>
            /// Used to parse a list of attributes recieved from the server.
            /// </summary>
            /// <param name="toParse">An unparsed string of attributes that are \t seperated</param>
            public void parseAttributes(string toParse)
            {
                attributes = toParse.Split('\t');
            } // End parseAttributes

            /// <summary>
            /// Used to get the current list of attributes
            /// </summary>
            /// <returns>An array of attributes</returns>
            public string[] getAttributes()
            {
                return attributes;
            } // End getAttributes

            /// <summary>
            /// Parse a string containing all the resources from a query into individual resources
            /// </summary>
            /// <param name="toParse">A string containing a the resources that need to be parsed, each resource is ~ seperated</param>
            public void parseResources(string toParse)
            {
                string[] resourceString;
                resources = new List<Resource>();

                if (toParse.Equals("0"))
                {
                    return;
                }

                try
                {
                    resourceString = toParse.Split('\n');
                    foreach (string t in resourceString)
                    {
                        resources.Add(new Resource(attributes, t));
                    }
                }
                catch (Exception e)
                {
                    if (toParse != null && toParse.Length > 0)
                        resources.Add(new Resource(attributes, toParse));
                }
                ((Search_Pane)mwf.panes[0]).UpdateResults();

                // Update the resources page
                if (mwf.isAdmin)
                {
                    ((Resource_Pane)mwf.panes[3]).InitializeCurrentResourceTable();
                }
            } // End parseResources

            /// <summary>
            /// Used to get the current List of resources
            /// </summary>
            /// <returns>A list of resources</returns>
            public List<Resource> getResources()
            {
                return resources;
            } // End getResources

            /// <summary>
            /// Starts the set up upon a successful log in
            /// </summary>
            /// <param name="isAdmin">A boolean stating if the user is an admin or not</param>
            public void performLogin(bool isAdmin)
            {
                parseAttributes(client.requestListOfAttributes());
                mwf.InitializeUserWindows(isAdmin);
            } // End performLogin

            /// <summary>
            /// Used to get the background color for all ScrollPanel
            /// </summary>
            /// <returns>The color white</returns>
            public System.Drawing.Color getScrollwindowBackgroundColor()
            {
                return System.Drawing.Color.White;
            } // End getScrollwindowBackgroundColor

            /// <summary>
            /// Used to get the background color for all Panes
            /// </summary>
            /// <returns>A light blue color</returns>
            public System.Drawing.Color getWindowBackgroundColor()
            {
                return System.Drawing.ColorTranslator.FromHtml("#EAF8FD");
            } // End getWindowBackgroundColor

            /// <summary>
            ///  Used to get the background color for all buttons
            /// </summary>
            /// <returns>A light gray color</returns>
            public System.Drawing.Color getButtonBackgroundColor()
            {
                return System.Drawing.ColorTranslator.FromHtml("#F0F0F0");
            } // End getButtonBackgroundColor


        } // End Main_Window_Control inner class
    } // End Main_Window_Frame class
}
