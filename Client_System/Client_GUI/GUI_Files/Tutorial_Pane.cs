/**************************************************************************************************************************
 * Tutorial.cs
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
 * The tutorial window.  Simples describes how to use the product based on your login credentials.
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// The tutorial window.  Simples describes how to use the product based on your login credentials.
    /// </summary>
    class Tutorial_Pane : Form
    {
        RichTextBox display;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAdmin">a boolean value to check if the logged in user is an admin or not.</param>
        public Tutorial_Pane(bool isAdmin)
        {
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            display = new RichTextBox();
            display.Dock = DockStyle.Fill;

            if(isAdmin)
                display.LoadFile("./GUI_Files/RichTextDocument/Admin_Tutorial.rtf");
            else
                display.LoadFile("./GUI_Files/RichTextDocument/User_Tutorial.rtf");

            display.Enabled = true;
            display.ReadOnly = true;
            

            Controls.Add(display);
        }

    }
}
