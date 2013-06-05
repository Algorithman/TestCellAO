using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Dao
{
    using System.Data;

    using AO.Database.Entities;

    using Dapper;

    public static class LoginDataDao
    {
        public static IEnumerable<DBLoginData> GetAll()
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBLoginData>(
                        "SELECT * FROM login");
            }
        }

        public static DBLoginData GetByUsername(string username)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                try
                {
                    return
                        conn.Query<DBLoginData>(
                            "SELECT * FROM login where Username=@user", new { user = username }).First();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static void WriteLoginData(DBLoginData login)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute(
                    "INSERT INTO login (CreationDate, Email, FirstName, LastName, Username, Password, Allowed_Characters, Flags, AccountFlags, Expansions, GM) VALUES (@creationdate, @email, @firstname, @lastname,@username, @password, @allowed_characters, @flags, @accountflags, @expansions, @gm)",
                    new
                        {
                            creationdate = DateTime.Now,
                            email = login.Email,
                            firstname = login.FirstName,
                            lastname = login.LastName,
                            username = login.Username,
                            password = login.Password,
                            allowed_characters = login.Allowed_Characters,
                            flags = login.Flags,
                            accountflags = login.AccountFlags,
                            expansions = login.Expansions,
                            gm = login.GM
                        });
            }
        }

        public static void WriteNewPassword(DBLoginData login)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute(
                    "UPDATE login SET password=@pwd WHERE Username=@user",
                    new
                    {
                        pwd=login.Password,
                        user=login.Username
                    });
            }
        }
    }

}
