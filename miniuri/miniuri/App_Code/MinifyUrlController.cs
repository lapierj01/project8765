using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


public class MinifyUrlController : ApiController
{

    [HttpGet]
    [Route("{id}")]
    public IHttpActionResult GoToMinUrl(string id)
    {
        //Foward to url
        string URL = MinifyRec.GetUrl(id);

        if (!String.IsNullOrEmpty(URL))
        {
            //302
            return Redirect(URL);
        }
        else
        {
            //404
            return NotFound();
        }            
    }

    [HttpGet]
    [Route("")]
    public IHttpActionResult GoToDefault()
    {       
        return Redirect(Request.RequestUri.AbsoluteUri + "default.aspx");
    }

    [HttpPost]
    [Route("")]
    public IHttpActionResult GetMiniUrl([FromBody] JObject url)
    {
        JObject j = new JObject();
        
        if (url["url"] == null)      
            return BadRequest();

        string urlrecived = url["url"].ToString();

        if (urlrecived.Length == 0)
        {
            j["url"] = "";
        }
        else
        {
            if (!urlrecived.Contains("http://") || urlrecived.Contains("https://"))
            {
                url["url"] = "http://" + url["url"];
            }

            string strUrl = MinifyRec.putUrl(url["url"].ToString());
            j["url"] = strUrl;
        }

        //200
        return Ok(j);
    }
}

