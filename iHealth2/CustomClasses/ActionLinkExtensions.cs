using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Security.Cryptography;


namespace SecuredUrl.Links
{
    //This class is an extension for the Html Helper
    public static class ActionLinkExtensions
    {
        //This password is included in the hash. If needed it can be changed.
        public static string tokenPassword = "@#123%#";

        //This is the extended method of Html.ActionLink.This class has the extended methods for the 10 overloads of this method
        //All the overloaded method applys the same logic excepts the parameters passed in
        public static MvcHtmlString ActionLink(this HtmlHelper helper,string LinkText, string actionName,bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary();
            if (generateToken == true)
            {
                //Call the generateUrlToken method which create the hash
                string token = TokenUtility.generateUrlToken("", actionName, rvd, tokenPassword);
                //The hash is added to the route value dictionary
                rvd.Add("urltoken", token);
            }
            //the link is formed by using the GenerateLink method.
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, null,rvd,null);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName,object routeValues, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary(routeValues);
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken("", actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
           string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, null, rvd, null);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }
        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName,RouteValueDictionary routeValues, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = routeValues;
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken("", actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, null, rvd, null);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }
        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName, string controllerName, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary();
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken(controllerName, actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, controllerName, rvd, null);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }
        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName, object routeValues, object htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary(routeValues);
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken("", actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
            IDictionary<string, object> attrib;
            attrib = getDictionaryFromObject(htmlAttributes);
            //attrib.Add("attributes", htmlAttributes);
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, null, rvd, attrib);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName,RouteValueDictionary routeValues,  IDictionary<string, object> htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken("", actionName, routeValues, tokenPassword);
                routeValues.Add("urltoken", token);
            }
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, null, routeValues, htmlAttributes);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }


        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary(routeValues);
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken(controllerName, actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
            IDictionary<string, object> attrib = new Dictionary<string, object>();
            attrib=getDictionaryFromObject(htmlAttributes); ;
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, controllerName, rvd, attrib);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }
        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName,string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken(controllerName, actionName, routeValues, tokenPassword);
                routeValues.Add("urltoken", token);
            }
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, controllerName, routeValues, htmlAttributes);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName, string controllerName, string protocol,string hostName,string fragment,object routeValues, object htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            RouteValueDictionary rvd = new RouteValueDictionary(routeValues);
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken(controllerName, actionName, rvd, tokenPassword);
                rvd.Add("urltoken", token);
            }
            IDictionary<string, object> attrib = new Dictionary<string, object>();
            attrib = getDictionaryFromObject(htmlAttributes);
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, controllerName,protocol,hostName,fragment, rvd, attrib);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }
        public static MvcHtmlString ActionLink(this HtmlHelper helper, string LinkText, string actionName, string controllerName,string protocol,string hostName,string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool generateToken)
        {

            RequestContext rc = helper.ViewContext.RequestContext;
            RouteCollection rtCollection = helper.RouteCollection;
            if (generateToken == true)
            {
                string token = TokenUtility.generateUrlToken(controllerName, actionName, routeValues, tokenPassword);
                routeValues.Add("urltoken", token);
            }
            string link = HtmlHelper.GenerateLink(rc, rtCollection, LinkText, null, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes);
            MvcHtmlString mString = MvcHtmlString.Create(link);
            return mString;
        }

        private static IDictionary<string, Object> getDictionaryFromObject(object sourceObject)
        {
            IDictionary<string, Object> propDictionary = new Dictionary<string, Object>();
            Type objType = sourceObject.GetType();
            foreach(PropertyInfo prop in objType.GetProperties()){
                propDictionary.Add(prop.Name, prop.GetValue(sourceObject,null));
            }

            return propDictionary;
        }

 

    }

   //This is a static helper class which creates the URL Hash
    public static class TokenUtility
    {
        public static string generateUrlToken(string controllerName, string actionName, RouteValueDictionary argumentParams, string password)
        {
            //The URL hash is dynamic by assign a dynamic key in each session. So
            //eventhough your URL is stolen, it will not work in other session
            if (HttpContext.Current.Session["url_dynamickey"] == null)
            {
                HttpContext.Current.Session["url_dynamickey"] = RandomString();
            }
            string token = "";
            //The salt include the dynamic session key and valid for an hour.
            string salt = HttpContext.Current.Session["url_dynamickey"].ToString() + DateTime.Now.ToShortDateString() + " " + DateTime.Now.Hour; ;
            //generating the partial url
            string stringToToken = controllerName + "/" + actionName + "/";
            foreach (KeyValuePair<string, object> item in argumentParams)
            {
                if (item.Key != "controller" && item.Key != "action" && item.Key != "urltoken")
                {
                    stringToToken += "/" + item.Value;
                }
            }
            //Converting the salt in to a byte array
            byte[] saltValueBytes = System.Text.Encoding.ASCII.GetBytes(salt);
            //Encrypt the salt bytes with the password
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, saltValueBytes);
            //get the key bytes from the above process
            byte[] secretKey = key.GetBytes(16);
            //generate the hash
            HMACSHA1 tokenHash = new HMACSHA1(secretKey);
            tokenHash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(stringToToken));
            //convert the hash to a base64string
            token = Convert.ToBase64String(tokenHash.Hash).Replace("/","_");
            return token;
        }
        //This validates the token
        public static bool validateToken(string token,string controllerName, string actionName, RouteValueDictionary argumentParams, string password)
        {
            //compute the token for the currrent parameter
            string computedToken = generateUrlToken(controllerName, actionName, argumentParams, password);
            //compare with the submitted token
            if (computedToken != token)
            {
                computedToken = generateUrlToken("", actionName, argumentParams, password);
            }
            else { return true; }

            if (computedToken != token)
            {
                return false;
            }
            else { return true; }
        }
        //It validates the token, where all the parameters passed as a RouteValueDictionary
        public static bool validateToken(RouteValueDictionary requestUrlParts, string password)
        {
            //get the parameters
            string controllerName;
            try
            {
                controllerName = Convert.ToString(requestUrlParts["controller"]);
            }
            catch (Exception)
            {
                controllerName = "";
            }
            string actionName = Convert.ToString(requestUrlParts["action"]);
            string token = Convert.ToString(requestUrlParts["urltoken"]);
            //Compute a new hash
            string computedToken = generateUrlToken(controllerName, actionName, requestUrlParts, password);
            //compare with the submited hash
            if (computedToken != token)
            {
                computedToken = generateUrlToken("", actionName, requestUrlParts, password);
            }
            else { return true; }

            if (computedToken != token)
            {
                return false;
            }
            else { return true; }
        }
        //This method create the random dynamic key for the session
        private static string RandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 8; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

    }

    //This is a attribute class which actually calls the validation and to be used with the controller
    public  class IsValidURLRequestAttribute : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
           // basic athorization check 
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext != null)
            {
                //Http referrer check and do the redirection if error occurs
                //It uses a controller named ErrorViewController and action named DisplayHttpReferrerError
                //These controller and action need to be present in your project in the project name space
                if (filterContext.HttpContext.Request.UrlReferrer == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                         new System.Web.Routing.RouteValueDictionary
                        {
                                { "langCode", filterContext.RouteData.Values[ "langCode" ] },
                                { "controller", "ErrorView" },
                                { "action", "DisplayHttpReferrerError" },
                                { "ReturnUrl", filterContext.HttpContext.Request.RawUrl },
                        });
                }

                    
            }

            /*Add code here to check the domain name the request come from*/

            // The call for validation of URL hash and do the redirection if error occurs
            //It uses a controller named ErrorViewController and action named DisplayURLError
            //These controller and action need to be present in your project in the project name space
            if (TokenUtility.validateToken(filterContext.RequestContext.RouteData.Values, ActionLinkExtensions.tokenPassword) == false)
            {
                filterContext.Result = new RedirectToRouteResult(
                     new System.Web.Routing.RouteValueDictionary
                        {
                                { "langCode", filterContext.RouteData.Values[ "langCode" ] },
                                { "controller", "ErrorView" },
                                { "action", "DisplayURLError" },
                                { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                        });

            }


        }

    }

}