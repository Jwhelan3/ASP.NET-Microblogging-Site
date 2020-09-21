using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;

namespace CO3708.Controllers
{
    public class UserController : Controller
    {
        //Check whether the email address is valid using regular expression
        protected bool ValidEmail(string email)
        {
            string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);

            if (match.Success)
                return true;
            else
                return false;
        }
        //
        // GET: /User/
        //Homepage - display the user list 
        public ActionResult Index()
        {
            //Fetch all of the user data and store it in a list
            var model = new List<CO3708.Models.User>();
            String sql = "SELECT * FROM [dbo].[Users]";
            using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var user = new CO3708.Models.User();
                    user.Id = (int)r["Id"];
                    user.email = r["email"].ToString();
                    model.Add(user);
                }

            }

            //Send the list to the view
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.CheckLogon(user.email, user.password))
                {
                    FormsAuthentication.SetAuthCookie(user.email, true);
                    //Get the ID of the user
                    int userId = user.GetUserId(user.email);
                    int admin = user.GetUserRole(user.email);
                    FormsAuthentication.SetAuthCookie(userId.ToString(), true);
                    Session["userID"] = userId;
                    Session["email"] = user.email;
                    Session["admin"] = admin;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login details were incorrect");
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["userID"] = null;
            Session["email"] = null;
            Session["admin"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register(Models.User user)
        {
            if (ModelState.IsValid)
            {
                //Is the email address valid?
                if (ValidEmail(user.email))
                {
                    //If so, does the email exist already?
                    if (user.EmailExists(user.email))
                    {
                        ModelState.AddModelError("Email", "Email address already in use");
                    }
                }

                else
                {
                    ModelState.AddModelError("Email", "Email address is invalid");
                }

                if (user.password.Length < 5 || user.password.Length > 30)
                {
                    ModelState.AddModelError("Password", "Password must be between 5 and 30 characters");
                }
            }

            if (ModelState.IsValid)
            {
                if(user.Register(user.email, user.password))
                {
                    //Get the ID of the user
                    int userId = user.GetUserId(user.email);
                    int admin = user.GetUserRole(user.email);
                    FormsAuthentication.SetAuthCookie(userId.ToString(), true);
                    Session["userID"] = userId;
                    Session["email"] = user.email;
                    Session["admin"] = admin;
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", "Registration failed");
                }
            }
            return View(user);
        }

        //Delete the user based on the URI, after veryfiying the user is authorised to do so
        public ActionResult DeleteUser()
        {
            int uId = Request.QueryString["id"].AsInt(0);

            //Fetch the user model
            Models.User u = new Models.User();

            //Is the user authenticated?
            if (Session["userID"] != null)
            {
                //Is the user an administrator?
                if ((int)Session["admin"] == 1)
                {
                    //Delete the user
                    u.DeleteUser(uId);
                }
            }

            return RedirectToAction("Index", "User");
        }
    }
}