#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Using...

//SQL Using Area...
//end SQL Using Area...
#endregion

namespace AO.Core
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;

    using AO.Core.Config;

    using MySql.Data.MySqlClient;

    using Npgsql;

    #endregion

    /// <summary>
    /// MySQL wrapper for CellAO database
    /// </summary>
    public class SqlWrapper : IDisposable
    {
        /// <summary>
        /// Sqltye is set in the config.xml and can be 'MySql', 'MsSql' or 'PostgreSQL' as for now those are the 3 database types supported by cellao at this time.
        /// </summary>
        public string Sqltype = ConfigReadWrite.Instance.CurrentConfig.SQLType;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public bool ismysql;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public bool ismssql;

        /// <summary>
        /// shortcuts for checking sql type
        /// </summary>
        public bool isnpgsql;

        // Text file for Sql Errors
        #region Sql Setup...

        #region Mysql Area

        /// <summary>
        /// Opens a Public MySql Datareader named 'myreader' to be used in any class that has AO.Core in the using
        /// </summary>
        public MySqlDataReader myreader;

        /// <summary>
        /// Opens a Public Mysql Connection named 'mcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public MySqlConnection mcc;

        /// <summary>
        /// Opens a Public Mysql Command named 'mcom' to be used in any class that has AO.Core in using.
        /// </summary>
        public MySqlCommand mcom;

        #endregion

        #region MsSql Area

        /// <summary>
        /// Opens a Public MsSql Datareader named 'sqlreader' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlDataReader sqlreader;

        /// <summary>
        /// Opens a Public MsSql Connection named 'sqlcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlConnection sqlcc;

        /// <summary>
        /// Opens a Public MsSql Command named 'sqlcom' to be used in any class that has AO.Core in the using.
        /// </summary>
        public SqlCommand sqlcom;

        #endregion

        #region PostgreSQL Area

        /// <summary>
        /// Opens a public PostgreSQL Datareader named 'npgreader' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlDataReader npgreader;

        /// <summary>
        /// Opens a public PostgreSQL Connection named 'npgcc' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlConnection npgcc;

        /// <summary>
        /// Opens a Public PostgreSQL Command named ' npgcom' to be used in any class that has AO.Core in the using.
        /// </summary>
        public NpgsqlCommand npgcom;

        #endregion

        #endregion

        /// <summary>
        /// setting ismysql, ismssql and isnpgsql shortcut variables, so no more typos should occur in strings to check
        /// </summary>
        public SqlWrapper()
        {
            // determine which SQL engine we use
            this.ismysql = this.Sqltype == "MySql";
            this.ismssql = this.Sqltype == "MsSql";
            this.isnpgsql = this.Sqltype == "PostgreSQL";
        }

        #region Connection String Setup

        // set Connect String..

        /// <summary>
        /// only needed once to read this
        /// </summary>
        private readonly string ConnectionString_MySQL = ConfigReadWrite.Instance.CurrentConfig.MysqlConnection;

        /// <summary>
        /// </summary>
        private readonly string ConnectionString_MSSQL = ConfigReadWrite.Instance.CurrentConfig.MsSqlConnection;

        /// <summary>
        /// </summary>
        private readonly string ConnectionString_PostGreSQL = ConfigReadWrite.Instance.CurrentConfig.PostgreConnection;

        #endregion

        #region Sql Count System

        /// <summary>
        /// Read Data into a DataTable object
        /// </summary>
        /// <param name="SqlQuery">
        /// Insert SQL Query here
        /// </param>
        /// <returns>
        /// </returns>
        public int SqlCount(string SqlQuery)
        {
            DataTable dt = this.ReadDatatable(SqlQuery);
            try
            {
                return (Int32)(Int64)dt.Rows[0][0];
            }
            catch
            {
                try
                {
                    return (Int32)dt.Rows[0][0];
                }
                catch
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        #endregion

        #region Sql Read System...

        /// <summary>
        /// Reads SQL Table. 
        /// Be sure to call sqlclose() after done reading!
        /// </summary>
        /// <param name="SqlQuery">
        /// Reads data from SQL DB, SqlQuery =  string SqlQuery = "SELECT * FROM `table` WHERE ID = "+ "'"+charID+"'";
        /// </param>
        public void SqlRead(string SqlQuery)
        {
            // MysqlRead: Create a SqlQuery to send to this wrapper for Reading the DB
            if (this.ismysql)
            {
                try
                {
                    this.mcom = new MySqlCommand(SqlQuery);
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    this.mcom.Connection = this.mcc;
                    this.mcom.CommandTimeout = 10000;
                    this.mcc.Open();
                    this.myreader = this.mcom.ExecuteReader();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                
                #region MSSQL
            else if (this.ismssql)
            {
                try
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    this.sqlcom = new SqlCommand(SqlQuery);
                    this.sqlcom.Connection = this.sqlcc;
                    this.sqlcom.CommandTimeout = 10000;
                    if (this.sqlcc.State == 0)
                    {
                        this.sqlcc.Open();
                    }

                    this.sqlreader = this.sqlcom.ExecuteReader();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }

                #endregion
                #region Postgresql
            else if (this.isnpgsql)
            {
                try
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);
                    this.npgcom = new NpgsqlCommand(SqlQuery);
                    this.npgcom.Connection = this.npgcc;
                    this.npgcom.CommandTimeout = 10000;
                    if (this.npgcc.State == 0)
                    {
                        this.npgcc.Open();
                    }

                    this.npgreader = this.npgcom.ExecuteReader();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Sql Update System...

        /// <summary>
        /// Updates Data in SQL Table
        /// </summary>
        /// <param name="SqlQuery">
        /// Updates data in SQL DB, SqlQuery = string SqlQuery = "UPDATE `table` SET `collum` = "+data+" WHERE ID = "'"+charID+"'";
        /// </param>
        /// <returns>
        /// </returns>
        public int SqlUpdate(string SqlQuery)
        {
            if (this.ismysql)
            {
                try
                {
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    this.mcom = new MySqlCommand(SqlQuery);
                    this.mcom.Connection = this.mcc;
                    this.mcom.CommandTimeout = 10000;
                    if (this.mcc.State == 0)
                    {
                        this.mcc.Open();
                    }

                    return this.mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                
                #region MSSQL
            else if (this.ismssql)
            {
                try
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    this.sqlcom = new SqlCommand(SqlQuery);
                    this.sqlcom.Connection = this.sqlcc;
                    this.sqlcom.CommandTimeout = 10000;
                    if (this.sqlcc.State == 0)
                    {
                        this.sqlcc.Open();
                    }

                    return this.sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }

                #endregion
                #region Postgresql
            else if (this.isnpgsql)
            {
                try
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);
                    this.npgcom = new NpgsqlCommand(SqlQuery);
                    this.npgcom.Connection = this.npgcc;
                    this.npgcom.CommandTimeout = 10000;
                    if (this.npgcc.State == 0)
                    {
                        this.npgcc.Open();
                    }

                    return this.npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }

            #endregion

            this.sqlclose();
            return 0;
        }

        #endregion

        #region Sql Insert System...

        /// <summary>
        /// Inserts data to SQL DB
        /// </summary>
        /// <param name="SqlQuery">
        /// Insert data into the SQL db, SqlQuery = INSERT INTO `dbname`  VALUES (`item1_value`, `item2_value`, `etc`)  
        /// </param>
        public void SqlInsert(string SqlQuery)
        {
            if (this.ismysql)
            {
                try
                {
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    this.mcom = new MySqlCommand(SqlQuery);
                    this.mcom.Connection = this.mcc;
                    this.mcom.CommandTimeout = 10000;
                    if (this.mcc.State == 0)
                    {
                        this.mcc.Open();
                    }

                    this.mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                
                #region MSSQL
            else if (this.ismssql)
            {
                try
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    this.sqlcom = new SqlCommand(SqlQuery);
                    this.sqlcom.Connection = this.sqlcc;
                    this.sqlcom.CommandTimeout = 10000;
                    if (this.sqlcc.State == 0)
                    {
                        this.sqlcc.Open();
                    }

                    this.sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }

                #endregion
                #region Postgresql
            else if (this.isnpgsql)
            {
                try
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);
                    this.npgcom = new NpgsqlCommand(SqlQuery);
                    this.npgcom.Connection = this.npgcc;
                    this.npgcom.CommandTimeout = 10000;
                    if (this.npgcc.State == 0)
                    {
                        this.npgcc.Open();
                    }

                    this.npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }

            #endregion

            this.sqlclose();
        }

        #endregion

        #region Sql Delete System...

        /// <summary>
        /// Deletes data from the SQL Table
        /// </summary>
        /// <param name="SqlQuery">
        /// SQL Query to execute DELETE from SQL DB, SqlQuery = DELETE FROM `database` WHERE (`Field` = {value})
        /// </param>
        public void SqlDelete(string SqlQuery)
        {
            if (this.ismysql)
            {
                try
                {
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    this.mcom = new MySqlCommand(SqlQuery);
                    this.mcom.Connection = this.mcc;
                    this.mcom.CommandTimeout = 10000;
                    if (this.mcc.State == 0)
                    {
                        this.mcc.Open();
                    }

                    this.mcom.ExecuteNonQuery();
                }
                catch (MySqlException me)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MySqlLogger(me, SqlQuery);
                    }
                    else
                    {
                        throw me;
                    }
                }
            }
                
                #region MSSQL
            else if (this.ismssql)
            {
                try
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    this.sqlcom = new SqlCommand(SqlQuery);
                    this.sqlcom.Connection = this.sqlcc;
                    this.sqlcom.CommandTimeout = 10000;
                    if (this.sqlcc.State == 0)
                    {
                        this.sqlcc.Open();
                    }

                    this.sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.MsSqlLogger(se, SqlQuery);
                    }
                    else
                    {
                        throw se;
                    }
                }
            }

                #endregion
                #region Postgresql
            else if (this.isnpgsql)
            {
                try
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);
                    this.npgcom = new NpgsqlCommand(SqlQuery);
                    this.npgcom.Connection = this.npgcc;
                    this.npgcom.CommandTimeout = 10000;
                    if (this.npgcc.State == 0)
                    {
                        this.npgcc.Open();
                    }

                    this.npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                    {
                        this.PostgressqlLogger(ne, SqlQuery);
                    }
                    else
                    {
                        throw ne;
                    }
                }
            }

            #endregion

            this.sqlclose();
        }

        #endregion

        #region SQL read into DataTable

        /// <summary>
        /// Read Data into a DataTable object
        /// </summary>
        /// <param name="sqlQuery">
        /// Insert SQL Query here
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable ReadDatatable(string sqlQuery)
        {
            DataSet ds = new DataSet();
            try
            {
                if (this.ismysql)
                {
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    MySqlDataAdapter mda = new MySqlDataAdapter(sqlQuery, this.mcc);
                    mda.Fill(ds);
                }

                if (this.ismssql)
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    SqlDataAdapter mda = new SqlDataAdapter(sqlQuery, this.sqlcc);
                    mda.Fill(ds);
                }

                if (this.isnpgsql)
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);
                    NpgsqlDataAdapter mda = new NpgsqlDataAdapter(sqlQuery, this.npgcc);
                    mda.Fill(ds);
                }
            }
            catch (MySqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    this.MySqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (SqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    this.MsSqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }
            catch (NpgsqlException me)
            {
                if (ConfigReadWrite.Instance.CurrentConfig.SqlLog)
                {
                    this.PostgressqlLogger(me, sqlQuery);
                }
                else
                {
                    throw me;
                }
            }

            this.sqlclose();
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        #endregion

        // Loggers
        #region MysqlLogger

        /// <summary>
        /// </summary>
        /// <param name="me">
        /// </param>
        /// <param name="SqlQuery">
        /// </param>
        private void MySqlLogger(MySqlException me, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = t.AppendText();
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(me);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(me);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }

        #endregion

        #region MSSQLLogger

        /// <summary>
        /// </summary>
        /// <param name="se">
        /// </param>
        /// <param name="SqlQuery">
        /// </param>
        private void MsSqlLogger(SqlException se, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = new StreamWriter(t.OpenWrite());
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(se);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(se);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }

        #endregion

        #region PostgresqlLogger

        /// <summary>
        /// </summary>
        /// <param name="ne">
        /// </param>
        /// <param name="SqlQuery">
        /// </param>
        private void PostgressqlLogger(NpgsqlException ne, string SqlQuery)
        {
            FileInfo t = new FileInfo("SqlError.log");
            if (t.Exists)
            {
                TextWriter tex = new StreamWriter(t.OpenWrite());
                tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                tex.WriteLine(" ");
                tex.WriteLine("Sql String: " + SqlQuery);
                tex.WriteLine(" ");
                tex.WriteLine("Sql Error: ");
                tex.WriteLine(ne);
                tex.Write(tex.NewLine);
                tex.Flush();
                tex.Close();
                tex = null;
                t = null;
            }
            else
            {
                StreamWriter sw = t.CreateText();
                sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Sql String: " + SqlQuery);
                sw.WriteLine(" ");
                sw.WriteLine("Sql Error: ");
                sw.WriteLine(ne);
                sw.Write(sw.NewLine);
                sw.Flush();
                sw.Close();
                sw = null;
                t = null;
            }
        }

        #endregion

        #region SQL closer

        /// <summary>
        /// sqlclose
        /// </summary>
        public void sqlclose()
        {
            if (this.ismysql)
            {
                if (this.mcc != null)
                {
                    this.mcc.Close();
                }
            }

            if (this.ismssql)
            {
                try
                {
                    this.sqlcc.Close();
                }
                catch
                {
                }

                try
                {
                    this.sqlreader.Close();
                }
                catch
                {
                }
            }

            if (this.isnpgsql)
            {
                try
                {
                    this.npgcc.Close();
                }
                catch
                {
                }

                try
                {
                    this.npgreader.Close();
                }
                catch
                {
                }
            }

            this.Dispose();
        }

        #endregion

        #region SQL connection/reader/command disposer

        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            if (this.mcom != null)
            {
                this.mcom.Dispose();
            }

            if (this.myreader != null)
            {
                this.myreader.Dispose();
            }

            if (this.ismysql)
            {
                try
                {
                    this.mcc.Close();
                    this.mcc.Dispose();
                }
                catch
                {
                }
            }

            if (this.ismssql)
            {
                try
                {
                    this.sqlcc.Close();
                    this.sqlcc.Dispose();
                }
                catch
                {
                }
            }

            if (this.isnpgsql)
            {
                try
                {
                    this.npgcc.Close();
                    this.npgcc.Dispose();
                }
                catch
                {
                }
            }
        }

        #endregion

        #region SQL Checks

        /// <summary>
        /// Tests SQL Connection/Database setup/Rights to create tables
        /// </summary>
        /// <returns>DBCheckCodes</returns>
        public DBCheckCodes SQLCheck()
        {
            string dbname = this.GetDBName();
            if (this.ismysql)
            {
                try
                {
                    this.mcc = new MySqlConnection(this.ConnectionString_MySQL);
                    this.mcom = new MySqlCommand("Use " + dbname);
                    this.mcom.Connection = this.mcc;
                    this.mcom.CommandTimeout = 10000;
                    if (this.mcc.State == 0)
                    {
                        this.mcc.Open();
                    }

                    // Test if database can be used
                    this.mcom.ExecuteNonQuery();

                    // Test if table can be created
                    this.mcom.CommandText = "CREATE TABLE " + dbname + ".TEMPDBTEMP (t int);";
                    this.mcom.ExecuteNonQuery();

                    // Drop the table again
                    this.mcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    this.mcom.ExecuteNonQuery();
                }
                catch (MySqlException myex)
                {
                    this.lasterrorcode = myex.Number;
                    this.lasterrormessage = myex.Message;
                    switch (myex.Number)
                    {
                        case 1044:
                            return DBCheckCodes.DBC_NoRightsToAccessDatabase;
                        case 1049:
                            return DBCheckCodes.DBC_DatabaseDoesNotExist;
                        case 1142:
                            return DBCheckCodes.DBC_NotEnoughRightsForTableAction;
                        default:
                            return DBCheckCodes.DBC_Somethingwentwrong;
                    }
                }
            }

            if (this.ismssql)
            {
                try
                {
                    this.sqlcc = new SqlConnection(this.ConnectionString_MSSQL);
                    this.sqlcom = new SqlCommand("use " + dbname);
                    this.sqlcom.Connection = this.sqlcc;
                    this.sqlcom.CommandTimeout = 10000;
                    if (this.sqlcc.State == 0)
                    {
                        this.sqlcc.Open();
                    }

                    this.sqlcom.ExecuteNonQuery();

                    this.sqlcom.CommandText = "CREATE TABLE " + dbname + ".TEMPDBTEMP (`c` INTEGER )";
                    this.sqlcom.ExecuteNonQuery();

                    this.sqlcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    this.sqlcom.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    this.lasterrormessage = se.Message;
                    this.lasterrorcode = se.Number;
                    return DBCheckCodes.DBC_Somethingwentwrong;
                }
            }

            if (this.isnpgsql)
            {
                try
                {
                    this.npgcc = new NpgsqlConnection(this.ConnectionString_PostGreSQL);

                    this.npgcom = new NpgsqlCommand("CREATE TABLE " + dbname + ".TEMPDBTEMP (c integer)");
                    this.npgcom.Connection = this.npgcc;
                    this.npgcom.CommandTimeout = 10000;
                    if (this.npgcc.State == 0)
                    {
                        this.npgcc.Open();
                    }

                    this.npgcom.ExecuteNonQuery();

                    this.npgcom.CommandText = "DROP TABLE " + dbname + ".TEMPDBTEMP";
                    this.npgcom.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    this.lasterrorcode = ne.ErrorCode;
                    this.lasterrormessage = ne.Message;
                    return DBCheckCodes.DBC_Somethingwentwrong;
                }
            }

            return DBCheckCodes.DBC_ok;
        }

        /// <summary>
        /// Our Database Check Codes
        /// </summary>
        public enum DBCheckCodes
        {
            /// <summary>
            /// All fine
            /// </summary>
            DBC_ok, 

            /// <summary>
            /// Database does not exists
            /// </summary>
            DBC_DatabaseDoesNotExist, 

            /// <summary>
            /// No rights to use the database
            /// </summary>
            DBC_NoRightsToAccessDatabase, 

            /// <summary>
            /// User has no rights to create a table
            /// </summary>
            DBC_NotEnoughRightsForTableAction, 

            /// <summary>
            /// All other errors
            /// </summary>
            DBC_Somethingwentwrong
        };

        /// <summary>
        /// Last error message from failed sql query
        /// </summary>
        public string lasterrormessage = string.Empty;

        /// <summary>
        /// Last error code from failed sql query
        /// </summary>
        public int lasterrorcode;

        #endregion

        #region GetDBName (extract database name from configuration)

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string GetDBName()
        {
            string dbn = string.Empty;
            if (this.ismysql)
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(this.ConnectionString_MySQL);
                dbn = builder.Database;
            }

            if (this.ismssql)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.ConnectionString_MSSQL);
                dbn = builder.DataSource;
            }

            if (this.isnpgsql)
            {
                NpgsqlConnectionStringBuilder builder =
                    new NpgsqlConnectionStringBuilder(this.ConnectionString_PostGreSQL);
                dbn = builder.Database;
            }

            return dbn;
        }

        #endregion

        #region Check Database

        /// <summary>
        /// Check our tables and create/fill them if they don't exist
        /// </summary>
        public void CheckDBs()
        {
            SqlWrapper ms = new SqlWrapper();
            List<string> tablelist = new List<string>();
            List<string> tabletodo = new List<string>();
            bool allok = true;

            // ToDo: check if database exists and create it if not (parsing the connection string)
            if (this.ismssql)
            {
                ms.SqlRead("SELECT table_name FROM INFORMATION_SCHEMA.TABLES;");
            }
            else if (this.isnpgsql)
            {
                ms.SqlRead("SELECT table_name FROM information_schema.tables;");
            }
            else if (this.ismysql)
            {
                ms.SqlRead("show Tables");
            }

            if (ms.myreader.HasRows)
            {
                while (ms.myreader.Read())
                {
                    tablelist.Add(ms.myreader.GetString(0));
                }
            }
            else
            {
                allok = false;
            }

            ms.sqlclose();

            string[] sqlfiles = Directory.GetFiles("SQLTables");
            bool isin;
            foreach (string s in sqlfiles)
            {
                isin = false;
                foreach (string table in tablelist)
                {
                    if (s.ToLower() == Path.Combine("SQLTables", table + ".sql").ToLower())
                    {
                        isin = true;
                        break;
                    }
                }

                if (!isin)
                {
                    tabletodo.Add(s);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Table " + s + " doesn't exist.");
                    allok = false;
                }
            }

            if (!allok)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("SQL Tables are not complete. Should they be created? (Y/N) ");
                string answer = Console.ReadLine();
                string sqlquery;
                if (answer.ToLower() == "y")
                {
                    foreach (string todo in tabletodo)
                    {
                        long filesize = new FileInfo(todo).Length;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Table " + todo.PadRight(67) + "[  0%]");

                        if (filesize > 10000)
                        {
                            string[] queries = File.ReadAllLines(todo);
                            int c = 0;
                            sqlquery = string.Empty;
                            string lastpercent = "0";
                            while (c < queries.Length)
                            {
                                if (queries[c].IndexOf("INSERT INTO") == -1)
                                {
                                    sqlquery += queries[c] + "\n";
                                }
                                else
                                {
                                    c--;
                                    break;
                                }

                                c++;
                            }

                            ms.SqlInsert(sqlquery);
                            c++;
                            string buf1 = string.Empty;
                            while (c < queries.Length)
                            {
                                if (queries[c].ToLower().Substring(0, 11) == "insert into")
                                {
                                    break;
                                }

                                c++;
                            }

                            if (c < queries.Length)
                            {
                                buf1 = queries[c].Substring(0, queries[c].ToLower().IndexOf("values"));
                                buf1 = buf1 + "VALUES ";
                                StringBuilder Buffer = new StringBuilder(0, 1 * 1024 * 1024);
                                while (c < queries.Length)
                                {
                                    if (Buffer.Length == 0)
                                    {
                                        Buffer.Append(buf1);
                                    }

                                    string part = string.Empty;
                                    while (c < queries.Length)
                                    {
                                        if (queries[c].Trim() != string.Empty)
                                        {
                                            part = queries[c].Substring(queries[c].ToLower().IndexOf("values"));
                                            part = part.Substring(part.IndexOf("(")); // from '(' to end
                                            part = part.Substring(0, part.Length - 1); // Remove ';'
                                            if (Buffer.Length + 1 + part.Length > 1024 * 1000)
                                            {
                                                Buffer.Remove(Buffer.Length - 2, 2);
                                                Buffer.Append(";");
                                                ms.SqlInsert(Buffer.ToString());
                                                Buffer.Clear();
                                                Buffer.Append(buf1);
                                                string lp2 =
                                                    Convert.ToInt32(Math.Floor((double)c / queries.Length * 100))
                                                           .ToString();
                                                if (lp2 != lastpercent)
                                                {
                                                    Console.Write(
                                                        "\rTable " + todo.PadRight(67) + "[" + lp2.PadLeft(3) + "%]");
                                                    lastpercent = lp2;
                                                }
                                            }

                                            Buffer.Append(part + ", ");
                                        }

                                        c++;
                                    }

                                    Buffer.Remove(Buffer.Length - 2, 2);
                                    Buffer.Append(";");
                                    ms.SqlInsert(Buffer.ToString());
                                    Buffer.Clear();
                                    string lp = Convert.ToInt32(Math.Floor((double)c / queries.Length * 100)).ToString();
                                    if (lp != lastpercent)
                                    {
                                        Console.Write("\rTable " + todo.PadRight(67) + "[" + lp.PadLeft(3) + "%]");
                                        lastpercent = lp;
                                    }
                                }
                            }
                            else
                            {
                                Console.Write("\rTable " + todo.PadRight(67) + "[100%]");
                            }
                        }
                        else
                        {
                            sqlquery = File.ReadAllText(todo);
                            ms.SqlInsert(sqlquery);
                            Console.Write("\rTable " + todo.PadRight(67) + "[100%]");
                        }

                        Console.WriteLine();
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database is fine.");
        }

        #endregion
    }
}