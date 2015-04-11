/**************************************************************************************************************************
 * Resource_Pane.cs
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
 * The GUI Pane used for the adding, removing, and modifying of resources in the Database.
 * This is based on the resources currently stored on the Client after a search has been performed.
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
    using Client_GUI.GUI_Files;
    using Client_GUI.GUI_Files.Base_Controller;

    /// <summary>
    /// The GUI Pane used for the adding, removing, and modifying of resources in the Database.
    /// This is based on the resources currently stored on the Client after a search has been performed.
    /// </summary>
    class Resource_Pane : Form
    {
        #region Variables
        /// <summary>
        /// The Resource_Pane event handlers
        /// </summary>
        private Resource_Pane_Controller controller;

        private TableLayoutPanel newResourceTable;
        private Button addNewResourceButton;

        private TableLayoutPanel currentResourcesTable;
        private Button modifyResourcesButton;
        private Button deleteResourcesButton;

        private Button exportButton;


        #endregion

        /// <summary>
        /// The constructor for our Resource_Pane class
        /// </summary>
        /// <param name="mainController">The main GUI controller which contains all shared data.</param>
        public Resource_Pane(Main_Window_Frame.Main_Window_Control mainController)
        {
            controller = new Resource_Pane_Controller(mainController, this);
            Initialize();
        }

        #region All Initialization Methods
        /// <summary>
        /// Used for the initial Initialization of our Resource_Pane, sets up all GUI elements.
        /// </summary>
        private void Initialize()
        {
            // Seting up the Form
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = controller.getMainController().getWindowBackgroundColor();
            CheckForIllegalCrossThreadCalls = false;

            // New Resource Stuff
            InitializeNewResourceTable(); // This method builds and adds the newResourceTable
            InitializeCurrentResourceTable(); // This method builds and adds the currentResourceTable

            exportButton = new Button();
            exportButton.Location = new System.Drawing.Point(649, 11);
            exportButton.Name = "Export Button";
            exportButton.Size = new System.Drawing.Size(123, 23);
            exportButton.Text = "Export All Resources";
            exportButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            exportButton.Click += controller.exportButtonClick;

            addNewResourceButton = new Button();
            addNewResourceButton.Location = new System.Drawing.Point(289, 151);
            addNewResourceButton.Size = new System.Drawing.Size(204, 23);
            addNewResourceButton.Text = "Add New Resource";
            addNewResourceButton.Name = "Add New Resource Button";
            addNewResourceButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            addNewResourceButton.Click += controller.addNewResource;

            modifyResourcesButton = new Button();
            modifyResourcesButton.Text = "Modify Selected Resources";
            modifyResourcesButton.Name = "Modify Resources Button";
            modifyResourcesButton.Location = new System.Drawing.Point(124, 505);
            modifyResourcesButton.Size = new System.Drawing.Size(240, 23);
            modifyResourcesButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            modifyResourcesButton.Click += controller.modifyResources;

            deleteResourcesButton = new Button();
            deleteResourcesButton.Text = "Delete Selected Resources";
            deleteResourcesButton.Name = "Delete Resources Button";
            deleteResourcesButton.Location = new System.Drawing.Point(402, 505);
            deleteResourcesButton.Size = new System.Drawing.Size(266, 23);
            deleteResourcesButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            deleteResourcesButton.Click += controller.deleteResources;

            Controls.Add(addNewResourceButton);
            Controls.Add(modifyResourcesButton);
            Controls.Add(deleteResourcesButton);
            Controls.Add(exportButton);


        } // End Initialization

        /// <summary>
        /// This method is used to build and add the newResourceTable both on program startup and any time we receive a new list of attributes.
        /// </summary>
        private void InitializeNewResourceTable()
        {
            if (newResourceTable != null)
            {
                Controls.Remove(newResourceTable); // We need to remove the old table before we build a new one
            }

            newResourceTable = new TableLayoutPanel(); // Let the garbage collector deal with destroying the old TableLayout if it exists
            newResourceTable.AutoScroll = true;
            newResourceTable.Name = "New Resource Table";
            newResourceTable.Location = new System.Drawing.Point(70, 40);
            newResourceTable.Size = new System.Drawing.Size(649, 94);
            newResourceTable.ColumnCount = 2;
            newResourceTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            newResourceTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            newResourceTable.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            newResourceTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            // Calculate the number of rows required
            int rows = (int)Math.Ceiling( (double)(controller.getMainController().getAttributes().Length / 2) );
            for (int i = 0; i < rows; ++i)
            {
                newResourceTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            }

            // Build all the Input TextBoxes for a New Resource
            // Names are equal to the attribute field
            foreach (string temp in controller.getMainController().getAttributes())
            {
                if (temp.Equals("ID"))
                {
                    continue;
                }
                Label tempLabel = new Label();
                tempLabel.Text = temp;
                tempLabel.Dock = DockStyle.Left;

                TextBox tempBox = new TextBox();
                tempBox.Name = temp;
                tempBox.Dock = DockStyle.Fill;

                Panel tempPanel = new Panel();
                tempPanel.Dock = DockStyle.Fill;
                tempPanel.Controls.Add(tempBox);
                tempPanel.Controls.Add(tempLabel);

                newResourceTable.Controls.Add(tempPanel);
            }

            Controls.Add(newResourceTable);
        } // End InitializeNewResourceTable

        /// <summary>
        /// This method is used to build and add the currentResourceTable both on program startup and any time we receive a new list of resources.
        /// </summary>
        public void InitializeCurrentResourceTable() 
        {
            if (currentResourcesTable != null)
            {
                Controls.Remove(currentResourcesTable);
            }

            // Creating the currentResourceTable
            currentResourcesTable = new TableLayoutPanel();
            currentResourcesTable.AutoScroll = true;
            currentResourcesTable.Size = new System.Drawing.Size(649, 285);
            currentResourcesTable.Location = new System.Drawing.Point(70, 195);
            currentResourcesTable.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            currentResourcesTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            // Setting up Columns
            currentResourcesTable.ColumnCount = controller.getMainController().getAttributes().Length;
            currentResourcesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            for (int i = 0; i < controller.getMainController().getAttributes().Length; ++i)
            {
                currentResourcesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            }

            // Setting up Rows
            int count = controller.getMainController().getResources().Count;
            currentResourcesTable.RowCount = controller.getMainController().getResources().Count + 2;
            
            // Building the table header row
            int k = 1;
            foreach (string att in controller.getMainController().getAttributes())
            {

                if (att.Equals("ID"))
                {
                    continue;
                }
                Label tempLabel = new Label();
                tempLabel.Text = att;
                tempLabel.Dock = DockStyle.Fill;
                tempLabel.TextAlign = ContentAlignment.MiddleCenter;
                tempLabel.Font = new Font("Arial", 10, FontStyle.Bold);
                currentResourcesTable.Controls.Add(tempLabel, k++, 0);
            }

            k = 1;
            foreach (Resource t in controller.getMainController().getResources())
            {
                CheckBox tempCheckBox = new CheckBox();
                tempCheckBox.Name = "" + t.getID();
                currentResourcesTable.Controls.Add(tempCheckBox, 0, k);

                for (int j = 1; j <= controller.getMainController().getAttributes().Length; ++j)
                {
                    string currentAttribute = controller.getMainController().getAttributes()[j - 1];

                    if (currentAttribute.Equals("ID"))
                    {
                        continue;
                    }
                    TextBox tempBox = new TextBox();
                    tempBox.Name = currentAttribute + t.getID();
                    tempBox.Text = t.findAttribute(currentAttribute);
                    tempBox.Dock = DockStyle.Fill;

                    currentResourcesTable.Controls.Add(tempBox, j - 1, k);
                }
                ++k;
            }
            //hasBeenCreated = true;
            Controls.Add(currentResourcesTable);
        } //  End InitializeCurrentResourceTable
        #endregion

        /// <summary>
        /// Used by the Main_Window_Frame to get the refreshScreen event handler method.
        /// </summary>
        /// <returns>A reference to the Event handler for this screen.</returns>
        public Resource_Pane_Controller getController()
        {
            return controller;
        }
        
        /// <summary>
        /// The event handler for the Resource_Pane
        /// </summary>
        public class Resource_Pane_Controller : Base_Controller
        {
            /// <summary>
            /// A reference to the Resource_Pane GUI page that this controller handles events for.
            /// </summary>
            Resource_Pane resourcePane;
            /// <summary>
            /// The constructor for this Controller class.
            /// </summary>
            /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
            /// <param name="resourcePane">A reference to the Resource_Pane creating an object of this class</param>
            public Resource_Pane_Controller(Main_Window_Frame.Main_Window_Control mainController, Resource_Pane resourcePane)
                : base(mainController)
            {
                this.resourcePane = resourcePane;
            } // End constructor

            /// <summary>
            /// Event handler called whenever the Resource_Pane needs to be refreshed
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void refreshScreen(object sender, EventArgs e)
            {
                resourcePane.InitializeNewResourceTable();
                resourcePane.InitializeCurrentResourceTable();
            } // End refreshScreen

            /// <summary>
            /// Event handler called when the Add New Resource button is clicked.  Tells the server to add a new resource to the database.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void addNewResource(object sender, EventArgs e)
            {
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;

                foreach (String t in getMainController().getAttributes())
                {
                    if (t.Equals("ID"))
                    {
                        continue;
                    }
                    if (!isFirst)
                    {
                        sb.Append("\t");
                    }

                    sb.Append(resourcePane.Controls.Find(t, true)[0].Text);
                    isFirst = false;
                }

                getMainController().parseAttributes(getClient().requestListOfAttributes());
                if (MessageBox.Show("Are you sure you want to add new resource?", "Confirm Add", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    getClient().addNewResource(sb.ToString());
                }
            } // End addNewResource

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void exportButtonClick(object sender, EventArgs e)
            {
                getClient().exportDatabase();
            }

            /// <summary>
            /// Event handler called when the Modify Selected Resources button is clicked.  Tells the server which resources need to be modified and how.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void modifyResources(object sender, EventArgs e)
            {
                foreach (Resource t in getMainController().getResources())
                {
                    int i = t.getID();
                    if (((CheckBox)resourcePane.currentResourcesTable.Controls.Find("" + i, true)[0]).Checked && MessageBox.Show("Are you sure you want to modify resource with ID '" + i + "'?", "Confirm modification", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(i);
                        foreach (string tString in getMainController().getAttributes())
                        {
                            if (tString.Equals("ID"))
                            {
                                continue;
                            }
                            sb.Append("\t");
                            sb.Append(((TextBox)resourcePane.currentResourcesTable.Controls.Find(tString + i, true)[0]).Text);
                        }
                        if (!getClient().modifyResource(sb.ToString()))
                        {
                            MessageBox.Show("Error updating resource.  Maybe the resource has been deleted.", "Error");
                        }
                    } // End If Checked
                } // End resource loop
            } // End Modified Event Handler

            /// <summary>
            /// Event handler called when the Delete Selected Resources button is clicked.  Tells the server which resources need to be deleted.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void deleteResources(object sender, EventArgs e)
            {
                List<Resource> toRemove = new List<Resource>();
                foreach (Resource t in getMainController().getResources())
                {
                    int i = t.getID();
                    if (((CheckBox)resourcePane.currentResourcesTable.Controls.Find("" + i, true)[0]).Checked && MessageBox.Show("Are you sure you want to delete resource with ID '" + i + "'?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (getClient().removeResource("" + i))
                        {
                            toRemove.Add(t);
                        }
                        else
                        {
                            MessageBox.Show("Error deleting resource.  Maybe the resource has already been deleted.", "Error");
                        }
                    } // End If Checked
                } // End resource loop

                foreach (Resource t in toRemove)
                {
                    getMainController().getResources().Remove(t); // Remove all resources we successfully deleted from our current list of resources
                }

                refreshScreen(sender, e); // Cause the Resource_Pane to refresh
            } // End Delete Event Handler
        } // End Resource_Pane_Controller class
    } // End Resource_Pane class
} // End Namespace
