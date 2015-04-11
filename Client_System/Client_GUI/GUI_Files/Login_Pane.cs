/**************************************************************************************************************************
 * Login_Pane.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 23rd, 2015
 * Date last modified: April 6th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * The GUI Page used for loggin into the Server and gaining access to the database.
 * Determines if the user is authorized to use this software, and what level of privilages they have.
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client_GUI.GUI_Files.Base_Controller;

    /// <summary>
    /// The GUI Page used for loggin into the Server and gaining access to the database.
    /// Determines if the user is authorized to use this software, and what level of privilages they have.
    /// </summary>
    public class Login_Pane : Form
    {
        #region variable declarations
        /// <summary>
        /// The Login_Pane event handler
        /// </summary>
        private Login_Pane_Controller controller;

        private Label usernameLabel;
        private Label passwordLabel;
        private Label IPAddressLabel;
        private Label portLabel;

        private TextBox usernameBox;
        private TextBox passwordBox;
        private TextBox IPAddressBox;
        private TextBox portBox;

        private Button loginButton;

        private PictureBox logo;
        #endregion

        /// <summary>
        /// The constructor for our Login_Pane class
        /// </summary>
        /// <param name="mainController">The main GUI controller which contains all shared data.</param>
        public Login_Pane(Main_Window_Frame.Main_Window_Control mainController)
        {
            controller = new Login_Pane_Controller(mainController, this);
            Initialize();
        } // End Login_Pane

        /// <summary>
        /// Used for the initial Initialization of our Login_Pane, sets up all GUI elements.
        /// </summary>
        private void Initialize()
        {
            // Seting up the Form
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = System.Drawing.Color.White;

            // Label definitions
            usernameLabel = new Label();
            usernameLabel.Text = "User Name";
            usernameLabel.Name = "User Name Label";
            usernameLabel.AutoSize = true;
            usernameLabel.Location = new System.Drawing.Point(220, 256);

            passwordLabel = new Label();
            passwordLabel.Text = "Password";
            passwordLabel.Name = "Password Label";
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(220, 292);

            IPAddressLabel = new Label();
            IPAddressLabel.Text = "Server's IP Address";
            IPAddressLabel.Name = "IP Address Label";
            IPAddressLabel.AutoSize = true;
            IPAddressLabel.Location = new System.Drawing.Point(303, 344);

            portLabel = new Label();
            portLabel.Text = "Port #";
            portLabel.Name = "Port Label";
            portLabel.AutoSize = true;
            portLabel.Location = new System.Drawing.Point(490, 344);

            // TextBox definitions
            usernameBox = new TextBox();
            usernameBox.Location = new System.Drawing.Point(287, 249);
            usernameBox.Name = "Username TextBox";
            usernameBox.Size = new System.Drawing.Size(274, 20);

            passwordBox = new TextBox();
            passwordBox.Location = new System.Drawing.Point(287, 292);
            passwordBox.Name = "Password TextBox";
            passwordBox.Size = new System.Drawing.Size(274, 20);
            passwordBox.PasswordChar = '*';

            IPAddressBox = new TextBox();
            IPAddressBox.Location = new System.Drawing.Point(223, 371);
            IPAddressBox.Name = "IP Address TextBox";
            IPAddressBox.Size = new System.Drawing.Size(216, 20);

            portBox = new TextBox();
            portBox.Location = new System.Drawing.Point(461, 371);
            portBox.Name = "Port TextBox";
            portBox.Size = new System.Drawing.Size(100, 20);

            // Button definition
            loginButton = new Button();
            loginButton.Text = "Login";
            loginButton.Name = "Login Button";
            loginButton.Location = new System.Drawing.Point(306, 468);
            loginButton.Size = new System.Drawing.Size(186, 23);
            loginButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            loginButton.Click += controller.loginButtonClicked;

            // Picture box definition
            logo = new PictureBox();
            logo.Location = new System.Drawing.Point(223, 69);
            logo.Size = new System.Drawing.Size(338, 134);
            logo.Name = "Logo";
            logo.ImageLocation = @".\GUI_Files\Images\saillogo.png";
            logo.SizeMode = PictureBoxSizeMode.CenterImage;

            // Adding everything to the Form
            Controls.Add(usernameLabel);
            Controls.Add(passwordLabel);
            Controls.Add(IPAddressLabel);
            Controls.Add(portLabel);
            Controls.Add(usernameBox);
            Controls.Add(passwordBox);
            Controls.Add(IPAddressBox);
            Controls.Add(portBox);
            Controls.Add(loginButton);
            Controls.Add(logo);


        } // End Initialize

        private class Login_Pane_Controller : Base_Controller
        {
            /// <summary>
            /// A reference to the Login_Pane GUI page that this controller handles events for.
            /// </summary>
            private Login_Pane loginPane;

            /// <summary>
            /// The constructor for this Controller class.
            /// </summary>
            /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
            /// <param name="loginPane">A reference to the Login_Pane creating an object of this class</param>
            public Login_Pane_Controller(Main_Window_Frame.Main_Window_Control mainController, Login_Pane loginPane)
                : base(mainController)
            {
                this.loginPane = loginPane;
            } // End Login_Pane_Controller constructor

            /// <summary>
            /// Event handler that fires when the user clicks the Login Button.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void loginButtonClicked(object sender, EventArgs e)
            {
                int result = getClient().login(loginPane.usernameBox.Text, loginPane.passwordBox.Text, loginPane.IPAddressBox.Text, loginPane.portBox.Text);

                if (result == 1)
                {
                    getMainController().performLogin(false);

                }
                else if (result == 3)
                {
                    getMainController().performLogin(true);
                }
                else if (result == 0)
                {
                    MessageBox.Show("Invalid Username and/or password.", "Error");
                }
                else
                {
                    MessageBox.Show("Could not connect to the server, please check the server IP and port number.  If this problem persists, contact the server administrator.", "Error");
                }
            } // End loginButtonClicked
        } // End Login_Pane_Controller
    } // End Login_Pane
} // End Namespace
