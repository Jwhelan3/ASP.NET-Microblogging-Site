namespace CO3708.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity.Spatial;
    using System.Data.SqlClient;
    using System.Web.Helpers;

    public partial class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Email address")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string password { get; set; }

        public DateTime date_created { get; set; }

        public int admin { get; set; }

        //Log a user in
        public bool CheckLogon(String _email, String _password)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))

            {
                string _sql = @"SELECT [email], [password] FROM [dbo].[Users] WHERE [email] = @e";
                var cmd = new SqlCommand(_sql, cn);
                string hash = "";
                cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = _email;
                cn.Open();
                /*
                 *             var hashedPassword = GetPasswordFromDatabase(username);
            var doesPasswordMatch = Crypto.VerifyHashedPassword(hashedPassword, password);
            return doesPasswordMatch;
                 */

                //Get the stored hash
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        hash = r["password"].ToString();
                    }

                    cn.Close();
                }

                try
                {
                    var doesPasswordMatch = Crypto.VerifyHashedPassword(hash, password);
                    return doesPasswordMatch;
                }

                catch
                {
                    return false;
                }
            }
        }

        public bool EmailExists(String _email)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))

            {
                string _sql = @"SELECT [email] FROM [dbo].[Users] WHERE [email] = @e";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = _email;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    //Email found
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    //Email not found
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }

        //Get the user's ID based on the email address
        public int GetUserId(String _email)
        {
            int result = 0;
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string q = "SELECT id FROM [dbo].[Users] WHERE [email] = @e";
                var cmd = new SqlCommand(q, cn);
                cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = _email;
                cn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        result = (int)r["Id"];
                    }

                    cn.Close();
                }
            }

            return result;
        }

        //Get the users role based on their email address
        public int GetUserRole(String _email)
        {
            int result = 0;
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string q = "SELECT * FROM [dbo].[Users] WHERE [email] = @e";
                var cmd = new SqlCommand(q, cn);
                cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = _email;
                cn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        result = (int)r["admin"];
                    }

                    cn.Close();
                }
            }

            return result;
        }

        //Register a new user
        public bool Register(String _email, String _password) 
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string insertQuery = "INSERT into [dbo].[Users] ([email], [password], [date_created]) VALUES (@e, @p, @d)";
                var cmd = new SqlCommand(insertQuery, cn);
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = Crypto.HashPassword(_password); ;
                cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar)).Value = _email;
                cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.DateTime)).Value = DateTime.Now;
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

        //Delete a user with the provided user ID
        public void DeleteUser(int _uId)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshu\source\repos\CO3708\CO3708\App_Data\Database.mdf;Integrated Security=True"))
            {
                string q = "DELETE FROM [dbo].[Users] WHERE [Id] = @i";
                var cmd = new SqlCommand(q, cn);
                cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.NVarChar)).Value = _uId;
                cn.Open();
                var success = cmd.ExecuteNonQuery();
            }
        }

    }
}
