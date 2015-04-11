/**************************************************************************************************************************
 * MainDriver.cs
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
 * The main program driver.
 ***************************************************************************************************************************/

namespace Client_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    
    /// <summary>
    /// The main program driver.
    /// </summary>
    public static class MainDriver
    {
        [STAThread]
        static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_Window_Frame());
        }
    }
}
