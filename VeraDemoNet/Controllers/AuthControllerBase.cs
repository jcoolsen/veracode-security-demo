using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using VeraDemoNet.DataAccess;

namespace VeraDemoNet.Controllers
{
    public abstract class AuthControllerBase : Controller
    {
        protected BasicUser LoginUser(string userName, string passWord)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            using (var dbContext = new BlabberDB())
            {
                var found = dbContext.Database.SqlQuery<BasicUser>(@"
                    SELECT username, real_name as realname, blab_name as blabname, is_admin as isadmin, password as hash
                    FROM users
                    WHERE username = @username",
                    new SqlParameter("username", userName)
                ).ToList();


                if (found.Count != 0)
                {
                    PasswordHash ph;
                    try {
                        ph = new PasswordHash(Convert.FromBase64String(found[0].Hash));
                    }
                    catch (FormatException) {
                        return null; 
                    }
                    if (ph.Verify(passWord)) {
                        //Session.Abandon();
                        //Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

                        Session["username"] = userName;
                        return found[0];
                    }
                }
            }

            return null;
        }

        protected string GetLoggedInUsername()
        {
            return Session["username"].ToString();
        }

        protected void LogoutUser()
        {
            Session["username"] = null;
        }

        protected bool IsUserLoggedIn()
        {
            return string.IsNullOrEmpty(Session["username"] as string) == false;

        }

        protected RedirectToRouteResult RedirectToLogin(string targetUrl)
        {
            return new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                (new
                {
                    controller = "Account",
                    action = "Login",
                    ReturnUrl = HttpContext.Request.RawUrl
                }));
        }

    }
}