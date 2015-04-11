/**************************************************************************************************************************
 * Attribute_Pane.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 24th, 2015
 * Date last modified: March 30th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * The GUI Pane used for the adding and removing of all attributes within the system.
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
    /// The GUI Pane used for the adding and removing of all attributes within the system.
    /// </summary>
    class Attribute_Pane : Form
    {
        #region Variables
        /// <summary>
        /// The Attribute_Pane event handler
        /// </summary>
        private Attribute_Pane_Controller controller;

        private List<string> writeProtected;

        private Label newAttributeNameLabel;
        private Label defaultValueLabel;

        private TextBox newAttributeNameBox;
        private TextBox defaultValueBox;

        private Button addNewAttributeButton;

        private TableLayoutPanel attributesTablePanel;

        private Button deleteAttributesButton;

        #endregion

        /// <summary>
        /// The constructor for our Resource_Pane class
        /// </summary>
        /// <param name="mainController">The main GUI controller which contains all shared data.</param>
        public Attribute_Pane(Main_Window_Frame.Main_Window_Control mainController)
        {
            controller = new Attribute_Pane_Controller(mainController, this);
            string[] toProtect = { "ID", "Document Title", "Authors", "Publication Date", "Keywords" };
            writeProtected = new List<string>(toProtect);
            Initialize();
        } // End Attribute_Pane Constructor

        /// <summary>
        /// Used for the initial Initialization of our Attribute_Pane, sets up all GUI elements.
        /// </summary>
        private void Initialize()
        {
            // Seting up the Form
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = controller.getMainController().getWindowBackgroundColor();

            // New attributes stuff
			newAttributeNameLabel = new Label();
			newAttributeNameLabel.Text = "New Attribute Name";
			newAttributeNameLabel.Name = "Attribute Name Label";
			newAttributeNameLabel.Location = new System.Drawing.Point(58, 35);
			newAttributeNameLabel.Size = new System.Drawing.Size(102, 13);

			defaultValueLabel = new Label();
			defaultValueLabel.Text = "Default Value";
			defaultValueLabel.Name = "Default Value Label";
			defaultValueLabel.Location = new System.Drawing.Point(58, 71);
			defaultValueLabel.Size = new System.Drawing.Size(102, 13);

			newAttributeNameBox = new TextBox();
			newAttributeNameBox.Name = "New Attibute Name Box";
			newAttributeNameBox.Location = new System.Drawing.Point(190, 32);
			newAttributeNameBox.Size = new System.Drawing.Size(396, 20);

			defaultValueBox = new TextBox();
			defaultValueBox.Name = "Default Value Box";
			defaultValueBox.Location = new System.Drawing.Point(190, 68);
			defaultValueBox.Size = new System.Drawing.Size(396, 20);

			addNewAttributeButton = new Button();
			addNewAttributeButton.Text = "Add New Attribute";
			addNewAttributeButton.Name = "Add New Attibute Button";
			addNewAttributeButton.Location = new System.Drawing.Point(610, 50);
			addNewAttributeButton.Size = new System.Drawing.Size(120, 23);
            addNewAttributeButton.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            addNewAttributeButton.Click += controller.addAttributeClick;

			// Remove attributes stuff
            InitializeAttributeTable();

			deleteAttributesButton = new Button();
            deleteAttributesButton.Text = "Delete Selected Attributes";
			deleteAttributesButton.Name = "Delete Selected Attributes Button";
            deleteAttributesButton.Location = new System.Drawing.Point(287, 505);
            deleteAttributesButton.Size = new System.Drawing.Size(182, 23);
            deleteAttributesButton.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            deleteAttributesButton.Click += controller.deleteAttributeClick;

			// Add everything to the form
			Controls.Add(newAttributeNameLabel);
			Controls.Add(defaultValueLabel);
			Controls.Add(newAttributeNameBox);
			Controls.Add(defaultValueBox);
			Controls.Add(addNewAttributeButton);
			Controls.Add(deleteAttributesButton);

        } //End Initialize

        /// <summary>
        /// This method is used to build and add the attributesTablePanel both on program startup and any time we receive a new list of attributes.
        /// </summary>
        public void InitializeAttributeTable()
        {
            if (attributesTablePanel != null)
            {
                Controls.Remove(attributesTablePanel);
            }

            attributesTablePanel = new TableLayoutPanel();
            attributesTablePanel.Name = "Attibutes Table Panel";
            attributesTablePanel.Size = new System.Drawing.Size(631, 369);
            attributesTablePanel.Location = new System.Drawing.Point(61, 126);
            attributesTablePanel.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            attributesTablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            attributesTablePanel.ColumnCount = 4;
            for (int i = 0; i < 4; ++i) // Define each of the columns to be 20% of the width
                attributesTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            attributesTablePanel.RowCount = (int)Math.Ceiling((double)(controller.getMainController().getAttributes().Length - 5) / 4);

            foreach (string t in controller.getMainController().getAttributes())
            {
                if (!writeProtected.Contains(t))
                {
                    CheckBox tempAttribute = new CheckBox();
                    tempAttribute.Name = t;
                    tempAttribute.Text = t;

                    attributesTablePanel.Controls.Add(tempAttribute);
                }
            }

            Controls.Add(attributesTablePanel);
        } // End InitializeAttributeTable

        /// <summary>
        /// Used by the Main_Window_Frame to get the refreshScreen event handler method.
        /// </summary>
        /// <returns>A reference to the Event handler for this screen.</returns>
        public Attribute_Pane_Controller getController()
        {
            return controller;
        } //End getController

        /// <summary>
        /// The event handler for the Attribute_Pane
        /// </summary>
        public class Attribute_Pane_Controller : Base_Controller
        {
            /// <summary>
            /// A reference to the Attribute_Pane GUI page that this controller handles events for.
            /// </summary>
            Attribute_Pane attributePane;

            /// <summary>
            /// The constructor for this Controller class.
            /// </summary>
            /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
            /// <param name="attributePane">A reference to the Attribute_Pane creating an object of this class</param>
            public Attribute_Pane_Controller(Main_Window_Frame.Main_Window_Control mainController, Attribute_Pane attributePane)
                : base(mainController)
            {
                this.attributePane = attributePane;
            } // End Attribute_Pane_Controller Constructor

            /// <summary>
            /// Event handler that fires when the Add Attribute Button is clicked.  Tells the server to add a new attribute to all current and future resources.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void addAttributeClick(object sender, EventArgs e)
            {
                string attributeToAdd = attributePane.newAttributeNameBox.Text, defaultToAdd = attributePane.defaultValueBox.Text; // On the off chance these text fields change
                // Make sure an attribute name was actually added
                if (string.IsNullOrEmpty(attributeToAdd))
                {
                    MessageBox.Show("A new attribute requires a name.", "Error");
                    return;
                }

                // Confirm the admin actually wants to Add the attribute
                if (MessageBox.Show("Are you sure you want to add attribute '" + attributeToAdd + "' with default value '" + defaultToAdd + "'?", "Confirm Add", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                if (!getClient().addAttribute(attributeToAdd, defaultToAdd))
                {
                    MessageBox.Show("Attribute names have to be unique.  This attribute name already exists, please choose a new attribute name.", "Error");
                }

                refreshScreen(sender, e);
            } // End addAttributeClick

            /// <summary>
            /// Event handler that fires when the Delete Selected Attributes button is clicked.  Tells the server to delete the selected attribute(s) from all current and future resources.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void deleteAttributeClick(object sender, EventArgs e)
            {
                List<KeyValuePair<string, CheckBox>> toDelete = new List<KeyValuePair<string, CheckBox>>();

                foreach (string t in getMainController().getAttributes())
                {
                    if (!attributePane.writeProtected.Contains(t))
                    {
                        CheckBox tempCheckBox = (CheckBox)attributePane.attributesTablePanel.Controls.Find(t, true)[0];
                        if (tempCheckBox.Checked && MessageBox.Show("Are you sure you want to delete attribute '" + t + "'?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            toDelete.Add(new KeyValuePair<string,CheckBox>(t, tempCheckBox));
                        }
                    }
                }



                foreach (KeyValuePair<string, CheckBox> t in toDelete)
                {
                    if (!getClient().removeAttribute(t.Key))
                    {
                        MessageBox.Show("Error deleting attribute: " + t + ".  Maybe the attribute has already been deleted.", "Error");
                    }
                    else
                    {
                        attributePane.Controls.Remove(t.Value);
                    }
                }

                refreshScreen(sender, e);
            } // End deleteAttributeClick

            /// <summary>
            /// Event handler that fires when the screen needs to be updated.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void refreshScreen(object sender, EventArgs e)
            {

                getMainController().parseAttributes(getClient().requestListOfAttributes());
                attributePane.InitializeAttributeTable();
            } // End refreshScreen

        } // End Attribute_Pane_Controller inner class
    } // End Attribute_Pane class
} // End Namespace
