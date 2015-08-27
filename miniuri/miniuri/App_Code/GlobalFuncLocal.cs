using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using ThoranLib.GlobalUtilities;
using System.Xml;

/// <summary>
/// Summary description for GlobalFunc
/// </summary>
public static class GlobalFuncLocal
{
    // Variables pour le LOG
    static Object MyCriticalRessource = new Object();
    public enum ErrorLevel { WARNING=1, CRITICAL=2, MESSAGE=3 }
     



    //+-----------------------------------------------------------------------------------------------------
    //| l o g
    //+-----------------------------------------------------------------------------------------------------
    public static void log(string msg, ErrorLevel ErrorLevel)
    {
            string currentFilePath = HttpContext.Current.Server.MapPath("~") + "\\Logs\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string strText2Write = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.000    ") +
                                   ErrorLevel.ToString().PadRight(20) +
                                   msg;
            try
            {
                lock (MyCriticalRessource)
                {
                    using (StreamWriter sw = new StreamWriter(currentFilePath, true))
                    {
                        sw.WriteLine(strText2Write);
                    }
                }
            }
            catch
            {
                throw new Exception(" Error on Log File");
            }
          

    }


    //+-----------------------------------------------------------------------------------------------------
    //| G e t P o s t B a c k C o n t r o l
    //+-----------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Will return the post back control if found. Empty control if not
    /// </summary>
    public static Control GetPostBackControl(Control BaseControl)
    {
        Control ctlReturn = new Control(); //Found control to return
        Control ctlDefaultReturn = new Control(); //Default not found control ID=""
        ctlDefaultReturn.ID = "";

        if (HttpContext.Current.Request.Form["__EVENTTARGET"] != null)
        {            
        //Split the EVENTTARGET to remove de $ signs. ID is the last portion of the composed name
        string[] arrCtlSplit = (HttpContext.Current.Request.Form["__EVENTTARGET"].Replace("ctl100$", "").Replace("$ctl00", "")).Split('$');
        string ctlID = arrCtlSplit[arrCtlSplit.Length - 1];

        //Find the control with this ID
        ctlReturn = FindControlRecursive(BaseControl, ctlID) ?? ctlDefaultReturn;
        
        return ctlReturn;
        }
        else 
        {
            return ctlDefaultReturn;
        }
    }

    /// <summary>
    /// Source: http://www.codinghorror.com/blog/2005/06/recursive-pagefindcontrol.html
    /// Used by the method: GetPostBackControl()
    /// </summary>
    public static  Control FindControlRecursive(Control root, string id)
    {
        if (root.ID == id)
        {
            return root;
        }

        foreach (Control c in root.Controls)
        {
            Control t = FindControlRecursive(c, id);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }


    //+-----------------------------------------------------------------------------------------------------
    //| F o r m a t D a t e
    //+-----------------------------------------------------------------------------------------------------
    /// <summary>
    /// This method will format the date depending on the current culture
    /// </summary>
    public static string FormatDate(DateTime dateToFormat, string culture, string dateFormat)
    {
        try
        {
            string strDate = "";

            //if french then day month, year
            if (culture.ToLower().Contains("fr"))
            {
                strDate = dateToFormat.ToString(dateFormat, CultureInfo.CreateSpecificCulture("fr-CA")).Replace(".", "").ToUpper();

            }
            else //else month day, year
            {
                strDate = dateToFormat.ToString(dateFormat, CultureInfo.CreateSpecificCulture("en-CA")).Replace(".", "").ToUpper();
            }

            return strDate;
        }
        catch (Exception err)
        {
            log(err.Message + " | " + err.StackTrace, GlobalFuncLocal.ErrorLevel.WARNING);
            return "";
        }
    }



        
}


