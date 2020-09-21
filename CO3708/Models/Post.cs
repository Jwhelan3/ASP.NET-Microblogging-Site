namespace CO3708.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity.Spatial;
    using System.Data.SqlClient;

    public partial class Post
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Post Title")]
        public string title { get; set; }

        [Column(TypeName = "text")]
        [Required]
        [Display(Name = "Post Content")]
        public string body { get; set; }

        public int author { get; set; }

        public DateTime date { get; set; }

        //Get a post
        public Post GetPost(int _id)
        {
            Post p = new Post();
            p.Id = 0;
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string insertQuery = "SELECT * FROM [dbo].[Posts] WHERE Id = @i";
                var cmd = new SqlCommand(insertQuery, cn);
                cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.NVarChar)).Value = _id;
                cn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        p.Id = (int)r["Id"];
                        p.title = r["title"].ToString();
                        p.body = r["body"].ToString();
                        p.author = (int)r["author"];
                    }

                    cn.Close();
                }
            }

            return p;
        }

        //Add a new post
        public bool Create(String _title, String _body, int _userID)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string insertQuery = "INSERT into [dbo].[Posts] ([title], [body], [date], [author]) VALUES (@t, @b, @d, @u)";
                var cmd = new SqlCommand(insertQuery, cn);
                cmd.Parameters.Add(new SqlParameter("@t", SqlDbType.NVarChar)).Value = _title;
                cmd.Parameters.Add(new SqlParameter("@b", SqlDbType.NVarChar)).Value = _body;
                cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.DateTime)).Value = DateTime.Now;
                cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = _userID;
                cn.Open();
                var success = cmd.ExecuteNonQuery();
                if (success > 0)
                {
                    //Registration was successful
                    return true;
                }
                else
                {
                    //Registration failed
                    return false;
                }
            }
        }

        public bool Edit(int _pId, String _title, String _body)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string insertQuery = "UPDATE [dbo].[Posts] SET [title] = @t, [body] = @b WHERE [Id] = @p";
                var cmd = new SqlCommand(insertQuery, cn);
                cmd.Parameters.Add(new SqlParameter("@t", SqlDbType.NVarChar)).Value = _title;
                cmd.Parameters.Add(new SqlParameter("@b", SqlDbType.NVarChar)).Value = _body;
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = _pId;
                cn.Open();
                var success = cmd.ExecuteNonQuery();
                if (success > 0)
                {
                    //Registration was successful
                    return true;
                }
                else
                {
                    //Registration failed
                    return false;
                }
            }
        }

        //Delete the post with the given id
        public void Delete(int _pId)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string q = "DELETE FROM [dbo].[Posts] WHERE [Id] = @i";
                var cmd = new SqlCommand(q, cn);
                cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.NVarChar)).Value = _pId;
                cn.Open();
                var success = cmd.ExecuteNonQuery();
            }
        }
    }
}
