/**************************************************************************************************************************
 * Search_Pane.cs
 * 
 * Created By: Brandon Stanley
 * Date of Creation: March 23rd, 2015
 * Date last modified: April 4th, 2015
 * **************************************************************************************************************************
 * Approved By: John Simko
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Approved By: Shahood Mirza
 * Date Approved: April 9th, 2015
 * **************************************************************************************************************************
 * Description:
 * The Admin and User GUI page used for searching, sorting, filtering, and browsing resources.
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client_GUI.GUI_Files;
    using Client_GUI.GUI_Files.Base_Controller;

    /// <summary>
    /// The Admin and User GUI page used for searching, sorting, filtering, and browsing resources.
    /// </summary>
    public class Search_Pane : Form
    {
        #region Variables
        /// <summary>
        /// The Search_Pane event handler
        /// </summary>
        Search_Pane_Controller controller;

        private Label searchLabel;
        private Label filterLabel;
        private TextBox searchBox;
        private Button searchButton;
        private ComboBox searchAttributes;
        private CheckedListBox filterKeywords;
        private TableLayoutPanel searchResults;

        #endregion

        /// <summary>
        /// The constructor for our Search_Pane class
        /// </summary>
        /// <param name="mainController">The main GUI controller which contains all shared data.</param>
        public Search_Pane(Main_Window_Frame.Main_Window_Control mainController)
        {
            controller = new Search_Pane_Controller(mainController, this);
            Initialize();
        } // End Search_Pane Constructor

        /// <summary>
        /// Used for the initial Initialization of our Search_Pane, sets up all GUI elements.
        /// </summary>
        private void Initialize()
        {
            // Set up the search pane
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = controller.getMainController().getWindowBackgroundColor();
            CheckForIllegalCrossThreadCalls = false;

            // Set up the search related parts
            searchLabel = new Label();
            searchLabel.Text = "Search";
            searchLabel.Name = "Search Label";
            searchLabel.Location = new System.Drawing.Point(32, 34);

            searchBox = new TextBox();
            searchBox.Name = "Search Box";
            searchBox.Location = new System.Drawing.Point(35, 57);
            searchBox.Size = new System.Drawing.Size(353, 20);

            UpdateDropdown();

            searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Name = "Search Button";
            searchButton.Location = new System.Drawing.Point(521, 54);
            searchButton.Size = new System.Drawing.Size(75, 23);
            searchButton.BackColor = controller.getMainController().getButtonBackgroundColor();
            searchButton.Click += controller.searchButtonClicked;

            // Filter related parts
            filterLabel = new Label();
            filterLabel.Name = "Filter Label";
            filterLabel.Text = "Filter";
            filterLabel.Location = new System.Drawing.Point(651, 34);

            UpdateFilters();

            // Search Results related stuff
            
            UpdateResults();

            // Add everything
            Controls.Add(searchLabel);
            Controls.Add(searchBox);
            Controls.Add(searchButton);
            Controls.Add(filterLabel);
            Controls.Add(filterKeywords);
            Controls.Add(searchResults);
        } // End Initialize

        /// <summary>
        /// Updates the drowdown of attributes to search by.  Excludes the ID attributes.
        /// </summary>
        public void UpdateDropdown()
        {
            if (searchAttributes != null)
            {
                Controls.Remove(searchAttributes);
            }
            searchAttributes = new ComboBox();
            searchAttributes.Name = "Search Attributes";
            searchAttributes.FormattingEnabled = true;
            foreach (string t in controller.getMainController().getAttributes())
            {
                if (t.Equals("ID"))
                {
                    continue;
                }
                searchAttributes.Items.Add(t);
            }
            searchAttributes.SelectedIndex = 0;
            searchAttributes.Location = new System.Drawing.Point(394, 56);
            searchAttributes.Size = new System.Drawing.Size(121, 21);
            Controls.Add(searchAttributes);
        } // End UpdateDropdown

        /// <summary>
        /// Used to update all the keyword filters
        /// </summary>
        public void UpdateFilters()
        {
            if (filterKeywords != null)
            {
                Controls.Remove(filterKeywords);
            }

            filterKeywords = new CheckedListBox(); // Note: This is empty at this point, since a user has to do a search before the keywords can be set.
            filterKeywords.Location = new System.Drawing.Point(630, 57);
            filterKeywords.Size = new System.Drawing.Size(120, 469);
            filterKeywords.Name = "Filter Keywords";

            if (controller.getMainController().getResources().Count > 0)
            {
                List<string> tempList = new List<string>();
                foreach (Resource tempResource in controller.getMainController().getResources())
                {
                    if (tempResource.findAttribute("Keywords").Length == 0)
                    {
                        continue;
                    }
                    foreach (string tempString in tempResource.findAttribute("Keywords").Split(','))
                    {
                        if (!tempList.Contains(tempString))
                        {
                            tempList.Add(tempString);
                        }
                    } // End Keyword parsing loop
                }// End resource loop
                filterKeywords.Items.AddRange(tempList.ToArray());
            }// End Count > 0 if statement

            filterKeywords.MouseUp+= controller.filterChecked;

            Controls.Add(filterKeywords);
        } // End UpdateFilters

        /// <summary>
        /// Updates the list of keywords to filter by when there has been some already checked.  Calls the UpdateFilters() method.
        /// </summary>
        /// <param name="toRecheck">A list of already checked off filters that need to be rechecked after the creation of the filter box</param>
        public void UpdateFilters(List<string> toRecheck)
        {
            UpdateFilters();
            foreach (string t in toRecheck)
            {
                filterKeywords.SetItemChecked(filterKeywords.FindString(t), true);
            }
        } // End UpdateFilters

        /// <summary>
        /// Methed used for displaying all resources from a search/sort/filter result.
        /// </summary>
        public void UpdateResults()
        {
            if (searchResults != null)
            {
                Controls.Remove(searchResults);
            }

            searchResults = new TableLayoutPanel();
            searchResults.AutoScroll = true;
            searchResults.Location = new System.Drawing.Point(35, 102); ;
            searchResults.Size = new System.Drawing.Size(561, 421);
            searchResults.Name = "Search Results";
            searchResults.BackColor = controller.getMainController().getScrollwindowBackgroundColor();
            searchResults.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            searchResults.ColumnCount = controller.getMainController().getAttributes().Length;

            searchResults.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F)); //ID Column
            for (int i = 1; i < controller.getMainController().getAttributes().Length; ++i)
            {
                searchResults.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            }

            searchResults.RowCount = controller.getMainController().getResources().Count + 2;

            // Build the table header row
            int k = 0;
            foreach (string att in controller.getMainController().getAttributes())
            {
                Label tempLabel = new Label();
                tempLabel.Text = att;
                tempLabel.Dock = DockStyle.Fill;
                tempLabel.TextAlign = ContentAlignment.MiddleCenter;
                tempLabel.Font = new Font("Arial", 10, FontStyle.Bold);
                tempLabel.Click += controller.sortByAttribute;
                searchResults.Controls.Add(tempLabel, k++, 0);
            }


            int THREADCOUNTER = 4;
            int finishCount = 0;



            k = 1;
            foreach (Resource t in controller.getMainController().getResources())
            {
                for (int j = 0; j < controller.getMainController().getAttributes().Length; ++j)
                {
                    string currentAttribute = controller.getMainController().getAttributes()[j];

                    Label tempLebel = new Label();
                    tempLebel.Name = currentAttribute;
                    tempLebel.Text = t.findAttribute(currentAttribute);
                    tempLebel.Dock = DockStyle.Fill;
                    tempLebel.TextAlign = ContentAlignment.MiddleLeft;
                    tempLebel.Click += controller.seeMoreDetails;

                    searchResults.Controls.Add(tempLebel, j, k);
                }
                ++k;
                finishCount = THREADCOUNTER;
            }

            Controls.Add(searchResults);
        } // End UpdateResults

        /// <summary>
        /// Used by the Main_Window_Frame to get the refreshScreen event handler method.
        /// </summary>
        /// <returns>A reference to the Event handler for this screen.</returns>
        public Search_Pane_Controller getController()
        {
            return controller;
        } // End getController

        /// <summary>
        /// The event handler for the Search_Pane
        /// </summary>
        public class Search_Pane_Controller : Base_Controller
        {
            /// <summary>
            /// A reference to the Search_Pane GUI page that this controller handles events for.
            /// </summary>
            Search_Pane searchPane;

            string lastSorted = "";
            bool isDesc = true;
            int filterCount = 0;
            List<string> currentFilter = new List<string>();

            /// <summary>
            /// The constructor for this Controller class.
            /// </summary>
            /// <param name="mainController">A reference to the Main_Window_Frame's controller</param>
            /// <param name="searchPane">A reference to the Search_Pane creating an object of this class</param>
            public Search_Pane_Controller(Main_Window_Frame.Main_Window_Control mainController, Search_Pane searchPane)
                : base(mainController)
            {
                this.searchPane = searchPane;
            } // End Search_Pane_Controller Constructor

            /// <summary>
            /// Event handler called when the Search button is clicked.  Tells the server to search by a specific attribute and search query, and return the list of resources.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void searchButtonClicked(object sender, EventArgs e)
            {
                filterCount = 0;
                string searchAttribute = (string)searchPane.searchAttributes.SelectedItem;
                string searchWords = searchPane.searchBox.Text;

                getMainController().parseAttributes(getClient().requestListOfAttributes());
                getMainController().parseResources(getClient().search(searchAttribute, searchWords));

                searchPane.UpdateFilters();

            } // End searchButtonClicked

            /// <summary>
            /// Event handler called when a Keyword to filter by is checked off.  Tells the return a list of resources filtered by that keyword.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void filterChecked(object sender, EventArgs e)
            {
                List<string> currentChecked = new List<string>();

                if (searchPane.filterKeywords.CheckedItems.Count == filterCount)
                    // No new check resource have been added or removed
                    return;


                // Build list of currently checked filters
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;
                foreach (object t in searchPane.filterKeywords.CheckedItems)
                {
                    if (!isFirst)
                        sb.Append("\t");
                    sb.Append((string)t);
                    currentChecked.Add((string)t);
                    isFirst = false;
                }
                

                if (searchPane.filterKeywords.CheckedItems.Count > filterCount)
                {
                    // We're adding
                    string temp = "";
                    foreach (string t in currentChecked.ToArray())
                    {
                        if (!currentFilter.Contains(t))
                        {
                            temp = t;
                        }
                    }

                    getMainController().parseAttributes(getClient().requestListOfAttributes());
                    getMainController().parseResources(getMainController().getClient().filterByKeyword(temp, true));

                    filterCount++;
                }
                else
                {
                    if (sb.Length == 0)
                    {
                        sb.Append("0"); // No items to send, we removed them all
                    }
                    // We're removing
                    getMainController().parseAttributes(getClient().requestListOfAttributes());
                    getMainController().parseResources(getMainController().getClient().filterByKeyword(sb.ToString(), false));

                    filterCount--;
                }

                currentFilter = currentChecked;
                searchPane.UpdateResults();
                searchPane.UpdateFilters(currentFilter);
            } // End filterChecked

            /// <summary>
            /// Event handler called when a attribute label is clicked.  Tells the server to return a sorted list based on the attribute clicked.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void sortByAttribute(object sender, EventArgs e)
            {
                string current = ((Label)sender).Text;
                if (!current.Equals(lastSorted))
                {
                    lastSorted = current;
                    isDesc = true; //Default to saying it's descending because the first sort click makes it ascending
                }

                isDesc = !isDesc;

                getMainController().parseAttributes(getClient().requestListOfAttributes());
                getMainController().parseResources(getClient().sortByAttribute(current, isDesc));

                searchPane.UpdateResults();
                refreshScreen(sender, e);
            } // End sortByAttribute

            /// <summary>
            /// Event handler that fires when the screen needs to be updated.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.  Not used.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void refreshScreen(object sender, EventArgs e)
            {
                searchPane.UpdateDropdown();
            } // End refreshScreen

            /// <summary>
            /// Generates a pop up window that contains more information about a clicked on cell.  Especially useful for attributes with values so large they don't fit in the cell they reside in.
            /// </summary>
            /// <param name="sender">The object causing the event to fire.</param>
            /// <param name="e">The arguements associated with the event.  Not used.</param>
            public void seeMoreDetails(object sender, EventArgs e)
            {
                Label temp = (Label)sender;
                MessageBox.Show(temp.Text, temp.Name, MessageBoxButtons.OK);
                // MessageBox.Show("Are you sure you want to add attribute '" + attributeToAdd + "' with default value '" + defaultToAdd + "'?", "Confirm Add", MessageBoxButtons.YesNo) != DialogResult.Yes
            }
        }
    }
}
