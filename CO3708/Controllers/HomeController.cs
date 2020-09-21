using CO3708.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CO3708.Controllers
{
    public class HomeController : Controller
    {
        //Homepage - display the blog entries 
        public ActionResult Index()
        {
            //Fetch all of the post data and store it in a list
            var model = new List<CO3708.Models.Post>();
            String sql = "SELECT * FROM [dbo].[posts]";
            using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var post = new CO3708.Models.Post();
                    post.Id = (int)r["Id"];
                    post.title = r["title"].ToString();
                    post.body = r["body"].ToString();
                    post.author = (int)r["author"];
                    model.Add(post);
                }

            }

            //Send the list to the view
            return View(model);
        }

        public ActionResult NewPost(Models.Post post)
        {
            //Is the user authenticated?
            if (Session["userID"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (post.title.Length < 5 || post.title.Length > 30)
                    {
                        ModelState.AddModelError("Post Title", "Title must be between 5 and 30 characters");
                    }

                    if (post.body.Length < 5 || post.body.Length > 1000)
                    {
                        ModelState.AddModelError("Post Title", "Body must be between 5 and 30 characters");
                    }
                }
                if (ModelState.IsValid)
                {
                    //Try to create the new post
                    if (post.Create(post.title, post.body, (int)Session["userID"]))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        ModelState.AddModelError("", "Posting failed");
                    }
                }
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(post);
        }

        //Delete the post based on the URI, after veryfiying the user is authorised to do so
        public ActionResult DeletePost()
        {
            int pId = Request.QueryString["id"].AsInt(0);

            //Fetch the post details
            Models.Post p = new Models.Post();
            p = p.GetPost(pId);

            //Is the user authenticated?
            if (Session["userID"] != null)
            {
                //Is the user an administrator or the post's author?
                if ((int)Session["userID"] == p.author || (int)Session["admin"] == 1)
                {
                    //Delete the post
                    p.Delete(pId);
                }
            }    
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditPost(Models.Post post)
        {
            int pId = Request.QueryString["id"].AsInt(0);

            //Fetch the post details
            Models.Post p = new Models.Post();
            p = p.GetPost(pId);

            //Is the user authenticated?
            if (Session["userID"] != null)
            {
                //Is the user an administrator or the post's author?
                if ((int)Session["userID"] == p.author || (int)Session["admin"] == 1)
                {
                    if (ModelState.IsValid)
                    {
                        if (post.Edit(pId, post.title, post.body))
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        else
                        {
                            ModelState.AddModelError("", "Edit failed");
                        }
                    }

                    //Go to the post
                    return View(p);
                    

                }

                else return RedirectToAction("Index", "Home");
            }

            else return RedirectToAction("Index", "Home");
        }
    }
}