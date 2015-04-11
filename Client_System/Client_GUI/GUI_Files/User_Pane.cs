/**************************************************************************************************************************
 * User_Pane.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 24th, 2015
 * Date last modified: April 5th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * The Admin GUI Pane used for adding and removing Users in the Database.
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client_GUI.GUI_Files.Base_Controller;
    
    /// <summary>
    /// The Admin GUI Pane used for adding and removing Users in the Database.
    /// </summary>
    public class User_Pane : Form
    {
        #region Variables
        /// <summary>
        /// The User_Pane event handler
        /// </summary>
        User_Pane_Controller controller;

        // New user stuff
        private Label addUserLabel;
        private Label usernameLabel;
        private Label passwordLabel;

        private TextBox usernameBox;
        private TextBox passwordBox;
        private CheckBox isAdminBox;

        private Button addNewUserButton;

        // Existing user stuff
        private TableLayoutPanel currentUsersPanel;
        private Button deleteSelectedUsers;

        #endregion

        /// <summary>
        /// The constructor for our User_Pane class
        /// </summary>
        /// <param name="mainController">The main GUI controller which contains all shared data.</param>
        public User_Pane(Main_Window_Frame.Main_Window_Control mainController)
        {
            controller = new User_Pane_Controller(mainController, this);
            Initialize();
        } // End User_Pane

        #region initialization methods

        /// <summary>
        /// Used for the initial Initialization of our User_Pane, sets up all GUI elements.
        /// </summary>
        private void Initialize()
        {
            // Seting up the Form
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = controller.getMainController().getWindowBackgroundColor();

            // Add new user stuff
            addUserLabel = new Label();
            addUserLabel.Text = "Add New User";
            addUserLabel.Name = "Add User Label";
            addUserLabel.Location = new System.Drawing.Point(88, 27);
            addUserLabel.Size = new System.Drawing.Size(76, 13);

            usernameLabel = new Label();
            usernameLabel.Text = "User Name";
            usernameLabel.Name = "User Name Label";
            usernameLabel.Location = new System.Drawing.Point(178, 61);
            usernameLabel.Size = new System.Drawing.Size(60, 13);

            passwordLabel = new Label();
            passwordLabel.Text = "Password";
            passwordLabel.Name = "Password Label";
            passwordLabel.Location = new System.Drawing.Point(178, 103);
            passwordLabel.Size = new System.Drawing.Size(60, 13);

            usernameBox = new TextBox();
            usernameBox.Name = "User Name Box";
            usernameBox.Location = new System.Drawing.Point(243, 58);
            usernameBox.Size = new System.Drawing.Size(254, 20);

            passwordBox = new TextBox();
            passwordBox.Name = "Password Box";
            passwordBox.PasswordChar = '*';
            passwordBox.Location = new System.Drawing.Point(243, 103);
            passwordBox.Size = new System.Drawing.Size(254, 20);

            isAdminBox = new CheckBox();
            isAdminBox.Text = "Is an Admin";
            isAdminBox.Name = "Is Admin Box";
            isAdminBox.Location = new System.Drawing.Point(523, 61);

            addNewUserButton = new Button();
            addNewUserButton.Text = "Add New User";
            addNewUserButton.Name = "Add New User Button";
            addNewUserButton.Location = new System.Drawing.Point(523, 103);
            addNewUserButton.Size = new System.Drawing.Size(75, 23);
            addNewUserButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            addNewUserButton.Click += controller.addNewUserClicked;

            // Existing user stuff
            InitializeCurrentUserPanel();

            deleteSelectedUsers = new Button();
            deleteSelectedUsers.Text = "Delete Selected Users";
            deleteSelectedUsers.Name = "Delete UsersButton";
            deleteSelectedUsers.Location = new System.Drawing.Point(318, 506);
            deleteSelectedUsers.Size = new System.Drawing.Size(137, 23);
            deleteSelectedUsers.BackColor = controller.getMainController().getButtonBackgroundColor();
            deleteSelectedUsers.Click += controller.deleteUserClicked;


            // Add everything to the Form
            Controls.Add(addUserLabel);
            Controls.Add(usernameLabel);
            Controls.Add(passwordLabel);
            Controls.Add(usernameBox);
            Controls.Add(passwordBox);
            Controls.Add(isAdminBox);
            Controls.Add(addNewUserButton);
            Controls.Add(deleteSelectedUsers);
        } // End Initialize

        /// <summary>
        /// This method is used to build and add the currentUsersPanel both on program startup and any time we receive a new list of users.
        /// </summary>
        private void InitializeCurrentUserPanel()
        {
            // Build the display table
            if (currentUsersPanel != null)
            {
                Controls.Remove(currentUsersPanel);
            }

            currentUsersPanel = new TableLayoutPanel();
            currentUsersPanel.Name = "Current Users Panel";
            currentUsersPanel.AutoScroll = true;
            currentUsersPanel.Location = new System.Drawing.Point(72, 153);
            currentUsersPanel.Size = new System.Drawing.Size(626, 332);

            currentUsersPanel.ColumnCount = 3;
            currentUsersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            currentUsersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            currentUsersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            currentUsersPanel.RowCount = controller.getCurrentUserList().Count + 2;
            currentUsersPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            currentUsersPanel.BackColor = getController().getMainController().getScrollwindowBackgroundColor();

            // Build Table header row
            currentUsersPanel.Controls.Add(new Label());

            Label tempLabel1 = new Label();
            tempLabel1.Text = "User Name";
            tempLabel1.Dock = DockStyle.Fill;
            tempLabel1.TextAlign = ContentAlignment.MiddleCenter;
            tempLabel1.Font = new Font("Arial", 10, FontStyle.Bold);

            Label tempLabel2 = new Label();
            tempLabel2.Text = "Is Admin";
            tempLabel2.Dock = DockStyle.Fill;
            tempLabel2.TextAlign = ContentAlignment.MiddleCenter;
            tempLabel2.Font = new Font("Arial", 10, FontStyle.Bold);

            currentUsersPanel.Controls.Add(tempLabel1);
            currentUsersPanel.Controls.Add(tempLabel2);

            // Add all user rows
            int i = 0;
            foreach (KeyValuePair<string, bool> t in controller.getCurrentUserList())
            {
                CheckBox deleteCheckBox = new CheckBox();
                deleteCheckBox.Name = "" + i++;
                deleteCheckBox.TextAlign = ContentAlignment.MiddleCenter;
                deleteCheckBox.Dock = DockStyle.Fill;

                Label userBox = new Label();
                userBox.Text = t.Key;
                userBox.Dock = DockStyle.Fill;
                userBox.TextAlign = ContentAlignment.MiddleCenter;

                Label isAdmin = new Label();
                if (t.Value)
                    isAdmin.Text = "true";
                else
                    isAdmin.Text = "false";
                isAdmin.Dock = DockStyle.Fill;
                isAdmin.TextAlign = ContentAlignment.MiddleCenter;

                currentUsersPanel.Controls.Add(deleteCheckBox);
                currentUsersPanel.Controls.Add(userBox);
                currentUsersPanel.Controls.Add(isAdmin);
            }
            Controls.Add(currentUsersPanel);
        } // End InitializeCurrentUserPanel
        #endregion

        /// <summary>
        /// Used by the Main_Window_Frame to get the refreshScreen event handler method.
        /// </summary>
        /// <returns>A reference to the Event handler for this screen.</returns>
        public User_Pane_Controller getController()
        {
            return controller;
        } // End getController

        /// <summary>
        /// The event handler for the User_Pane
        /// </summary>
        public class User_Pane_Controller : Base_Controller
        {
            /// <summary>
            /// A reference to the User_Pane GUI page that this controller handles events for.
            /// </summary>
            User_Pane userPane;

            List<KeyValuePair<string, bool>> userList = new List<KeyValuePair<string, bool>>();

            /// <summary>
            /// The constructor for this Controller class.
            /// </summary>
            /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
            /// <param name="userPane">A reference to the User_Pane creating an object of this class</param>
            public User_Pane_Controller(Main_Window_Frame.Main_Window_Control mainController, User_Pane userPane)
                : base(mainController)
            {
                this.userPane = userPane;
            } // End User_Pane_Controller Constructor

            /// <summary>
            /// Method used to get the current list of users stored locally.
            /// </summary>
            /// <returns>A list of KeyValuePairs that represent a user.  The string is the User name, the Bool is if they're an admin or not</returns>
            public List<KeyValuePair<string, bool>> getCurrentUserList()
            {
                return userList;
            } // End getCurrentUserList

            /// <summary>
            /// Requests the current list of users from the server, and then updates the display of current users
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void populateUserList(object sender, EventArgs e)
            {
                userList = new List<KeyValuePair<string, bool>>();
                string[] userString = getClient().requestListOfLogins().Split('\n');

                if (userString.Length == 0)
                {
                    return;
                }

                // Parse the list of users received from the server
                foreach (string t in userString)
                {
                    bool admin;
                    string[] split = t.Split('\t');
                    if (split[1].Equals("true"))
                        admin = true;
                    else
                        admin = false;

                    userList.Add(new KeyValuePair<string, bool>(split[0], admin));
                }
                userPane.InitializeCurrentUserPanel();
            } // End populateUserList

            /// <summary>
            /// Event handler called when the Add New User button is clicked.  Tells the server to add a new user and then get an updated list of current users.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void addNewUserClicked(object sender, EventArgs e)
            {
                string user = userPane.usernameBox.Text, password = userPane.passwordBox.Text;
                bool isAdmin = userPane.isAdminBox.Checked;
                // Check if both the username and password have been filled out
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("New Users must have a User Name and a Password.", "Error");
                    return;
                }

                // Check if the user actually wants to add this user
                if (MessageBox.Show("Are you sure you want to add user '" + user + "'?", "Confirm add", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                // Add the new user, throw an error message if the user name already existed
                if (!getClient().addNewLogin(user, password, isAdmin))
                {
                    MessageBox.Show("Error adding user.  User name must be unique, please enter a new user name.", "Error");
                }

                populateUserList(sender, e);

            } // End addNewUserClicked

            /// <summary>
            /// Event handler called when the Delete Selected Users button is clicked.  Tells the server to delete users and then get an updated list of current users.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void deleteUserClicked(object sender, EventArgs e)
            {
                List<string> toDelete = new List<string>();
                //Go through the list of current users, and see which ones are checked.  Confirm the admin actually wants to delete that user.
                for (int i = 0; i < userList.Count; ++i)
                {
                    if (((CheckBox)userPane.Controls.Find("" + i, true)[0]).Checked && MessageBox.Show("Are you sure you want to delete user '"+ userList[i].Key +"'?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Build a list of users to delete, if they're directly deleted here it causes issues with looping
                        toDelete.Add(userList[i].Key);
                    }
                }

                // Perform the delete of all selected users, throw an error if they're somehow already deleted.
                foreach (string t in toDelete)
                {
                    if (!getClient().removeLogin(t))
                    {
                        MessageBox.Show("Error deleting user.  User may have already been deleted.", "Error");
                    }
                }

                populateUserList(sender, e);
            } // End deleteUserClicked
        } // End User_Pane_Controller inner class
    } // End User_pane class
} // End Namespace
