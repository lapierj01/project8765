using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

    //+-----------------------------------------------------------------------------------------------------
    //| V e r s i o n i n g                                                                    * C L A S S *
    //+-----------------------------------------------------------------------------------------------------
    // Auteur: Jonathan Lapierre
    // Compagnie: Thoran inc.
    // Date: 05/06/2014
    //
    /// <summary>
    /// This class will be used to generate a versioning object to be used in any application (now WEB only)
    /// The config file should be located at the base (root) of the web folder. Here is an example of the config
    /// file:
    /// 
    /// <?xml version="1.0" encoding="utf-8" ?>
    ///<root>
    ///  <version active="true" major="1" minor="1" build="0" releaseDate="2014-06-01">
    ///    <version modification="">
    ///      <description>
    ///        <![CDATA[
    ///        TEXT HERE
    ///        ]]>        
    ///      </description>
    ///    </version>
    ///  </version>
    ///</root>
    /// </summary>
public class Versioning
{
    /// <summary>The major number identifier of the version. MAJOR.MINOR.BUILD</summary>
    public string Major = "";
    /// <summary>The minor number identifier of the version. MAJOR.MINOR.BUILD</summary>
    public string Minor = "";
    /// <summary>The build number identifier of the version. MAJOR.MINOR.BUILD</summary>
    public string Build = "";
    /// <summary>The suffix sometime used</summary>
    public string suffix = "";
    /// <summary>The version information e.g versioning information</summary>
    public string VersionInfo = "";
    /// <summary>The concatenated version number Major.Minor.Build</summary>
    public string MajorMinorBuild = "";
    /// <summary>The Release Date of the version</summary>
    public DateTime ReleaseDate;
    /// <summary>.net Version object loaded with MajorMinorBuild</summary>
    public System.Version CurrentVersion;

    private XmlDocument xmlConfig = new XmlDocument();

    public Versioning()
    {
    }

    //+-----------------------------------------------------------------------------------------------------
    //| g e t C u r r e n t V e r s i o n
    //+-----------------------------------------------------------------------------------------------------
    // Auteur: Jonathan Lapierre
    // Compagnie: Thoran inc.
    //
    /// <summary>
    /// Will give the current version of the file (Version flagged as active)
    /// </summary>
    /// <returns>Will return an empty version object if not found</returns>
    public Versioning getCurrentVersion()
    {
        try
        {
            //Check if the XML was already loaded, if not load it
            if (this.xmlConfig.InnerXml == "")
            {
                this.LoadFromFile();
            }

            //Loop the nodes to find the active version
            foreach (XmlNode MasterNodeFetch in xmlConfig.SelectNodes("/root/version"))
            {
                if (MasterNodeFetch.Attributes["active"].Value == "true")
                {
                    Versioning retVers = new Versioning();
                    retVers.Major = MasterNodeFetch.Attributes["major"].Value;
                    retVers.Minor = MasterNodeFetch.Attributes["minor"].Value;
                    retVers.Build = MasterNodeFetch.Attributes["build"].Value;
                    retVers.VersionInfo = MasterNodeFetch.ChildNodes[0].InnerText;
                    retVers.MajorMinorBuild = retVers.Major + "." + retVers.Minor + "." + retVers.Build;
                    retVers.CurrentVersion = new System.Version(retVers.MajorMinorBuild);

                    DateTime ReleaseDate;

                    //Check if date is valid
                    if (DateTime.TryParse(MasterNodeFetch.Attributes["releaseDate"].Value, out ReleaseDate))
                        retVers.ReleaseDate = ReleaseDate;
                    else
                        retVers.ReleaseDate = DateTime.MinValue;

                    return retVers;
                }
            }

            return new Versioning();
        }
        catch (Exception err)
        {
            GlobalFuncLocal.log(err.Message + " | " + err.StackTrace, GlobalFuncLocal.ErrorLevel.CRITICAL);

            //Throw exception to let programmer or administrator know that their is a missing or invalid version.xml file
            throw new Exception("Missing or invalid version.xml file. Please check log for more information");
        }

    }

    //+-----------------------------------------------------------------------------------------------------
    //| g e t S p e c i f i c V e r s i o n
    //+-----------------------------------------------------------------------------------------------------
    // Auteur: Jonathan Lapierre
    // Compagnie: Thoran inc.
    /// <summary>
    /// Get a specific version information
    /// </summary>
    /// <param name="Major">The major number to look for</param>
    /// <param name="Minor">The build number to look for</param>
    /// <param name="Build">The build number to look for</param>
    /// <returns>Will return an empty version object if not found</returns>
    public Versioning getSpecificVersion(string Major, string Minor, string Build)
    {
        try
        {
            //Check if the XML was already loaded, if not load it
            if (this.xmlConfig.InnerXml == "")
            {
                this.LoadFromFile();
            }

            //Loop the nodes to find the specific version
            foreach (XmlNode MasterNodeFetch in xmlConfig.SelectNodes("/root/version"))
            {
                if (MasterNodeFetch.Attributes["major"].Value == Major)
                {
                    if (MasterNodeFetch.Attributes["minor"].Value == Minor)
                    {
                        if (MasterNodeFetch.Attributes["build"].Value == Build)
                        {
                            Versioning retVers = new Versioning();
                            retVers.Major = Major;
                            retVers.Minor = Minor;
                            retVers.Build = Build;
                            retVers.VersionInfo = MasterNodeFetch.ChildNodes[0].InnerText;
                            retVers.MajorMinorBuild = retVers.Major + "." + retVers.Minor + "." + retVers.Build;
                            retVers.CurrentVersion = new System.Version(retVers.MajorMinorBuild);

                            DateTime ReleaseDate;

                            //Check if date is valid
                            if (DateTime.TryParse(MasterNodeFetch.Attributes["releaseDate"].Value, out ReleaseDate))
                                retVers.ReleaseDate = ReleaseDate;
                            else
                                retVers.ReleaseDate = DateTime.MinValue;

                            return retVers;
                        }
                    }
                }
            }

            return new Versioning();
        }
        catch (Exception err)
        {
            GlobalFuncLocal.log(err.Message + " | " + err.StackTrace, GlobalFuncLocal.ErrorLevel.CRITICAL);

            //Throw exception to let programmer or administrator know that their is a missing or invalid version.xml file
            throw new Exception("Missing or invalid version.xml file. Please check log for more information");
        }

    }

    //+-----------------------------------------------------------------------------------------------------
    //| L o a d F r o m F i l e
    //+-----------------------------------------------------------------------------------------------------
    // Auteur: Jonathan Lapierre
    // Compagnie: Thoran inc.
    /// <summary>
    /// This method will load the version file from disk into the private XML object
    /// </summary>
    private void LoadFromFile()
    {
        try
        {
            //Check to find version file exist
            FileInfo xmlConfigFIle = new FileInfo(HttpContext.Current.Server.MapPath("~") + "\\version.xml");

            //If file is found, try loading the file in the XML document object.
            xmlConfig.Load(xmlConfigFIle.FullName);
        }
        catch (Exception err)
        {
            GlobalFuncLocal.log(err.Message + " | " + err.StackTrace, GlobalFuncLocal.ErrorLevel.CRITICAL);

            //Throw exception to let programmer or administrator know that their is a missing or invalid version.xml file
            throw new Exception("Missing or invalid version.xml file. Please check log for more information");
        }
    }
}