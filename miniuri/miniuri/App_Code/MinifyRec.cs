using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MinifyRec
/// </summary>
public class MinifyRec
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int Visits { get; set; }

	public MinifyRec()
	{
	}

    //Query the NOSQL DB to get the URL
    public static string GetUrl(string Base36ID)
    {
        // Open database (or create if not exits)
        using (var db = new LiteDatabase(HttpContext.Current.Server.MapPath("~/") + @"Minify.db"))
        {
            var col = db.GetCollection<MinifyRec>("MinifyRec");

            // Use Linq to query documents
            //var results = col.Find(x => x.Id.StartsWith("Jo"));
            //var results = col.Find(x => x.Id.Equals(Base36.Decode(Base36ID)));            
            //var results = col.Find(Query.EQ("Id", Base36.Decode(Base36ID)));


            int Id2Find; Int32.TryParse(Base36.Decode(Base36ID).ToString(), out Id2Find);

            if (Id2Find > 0)
            {
                var results = col.FindById(Id2Find);

                if (results == null)
                {
                    return "";
                }
                else
                {
                    // Update a visit a collection
                    results.Visits = results.Visits + 1;                    
                    col.Update(results);

                    //Check URL validity end add HTTP if not specified
                    if (!results.Url.Contains("http://"))
                    {
                        if(!results.Url.Contains("https://"))
                        {
                            results.Url = "http://" + results.Url;
                        }
                    }

                   return results.Url;
                }                
            }
            else
            {
                return "";
            }
        }

        
    }

    /// <summary>
    /// Insert new URL in NOSQL DB
    /// </summary>
    /// <param name="strUrlIn"></param>
    /// <returns>Base36 string ID</returns>
    public static string putUrl(string strUrlIn)
    {
        try
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(HttpContext.Current.Server.MapPath("~/") + @"Minify.db"))
            {
                // Get Minify Rec collection
                var col = db.GetCollection<MinifyRec>("MinifyRec");

                // Create your new Minify Rec instance
                var MinifyRecIn = new MinifyRec
                {
                    Url = strUrlIn,
                    Visits = 0
                };

                // Insert new customer document (Id will be auto-incremented)
                col.Insert(MinifyRecIn);
                return Base36.Encode(MinifyRecIn.Id);
            }
        }
        catch
        {
            return "";
        }
    }
}