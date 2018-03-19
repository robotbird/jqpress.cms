using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Jqpress.Framework.Configuration;
//using System.Data.SQLite;
using Mono.Data.Sqlite;

namespace Jqpress.Framework.DbProvider
{
    public class DapperHelper
    {
        private static string _mdbpath = System.Web.HttpContext.Current.Server.MapPath(ConfigHelper.SitePath + ConfigHelper.DbConnection);//for windows
       // private static string _mdbpath = ConfigHelper.SitePath + ConfigHelper.DbConnection;//for linux
        public static string ConnectionString = "Data Source=" + _mdbpath;//for sqlite & windows
        //public static string ConnectionString = "URI=file:" + _mdbpath + ",version=3";//for sqlite & linux

        //连接数据库字符串。
        private readonly string sqlconnection = "";
        //获取access的连接数据库对象。SqlConnection

        //public OleDbConnection OpenConnection()
        //{
        //     OleDbConnection conn = new OleDbConnection(ConnectionString);
        //     conn.Open();
        //     return conn;
        //}

        //public SQLiteConnection OpenConnection()// for sqlite & windows
        //{
        //    SQLiteConnection conn = new SQLiteConnection(ConnectionString);
        //    conn.Open();
        //    return conn;
        //}

        public SqliteConnection OpenConnection()// for sqlite & liunx
        {
            SqliteConnection conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }


        public SqlConnection OpenConnectionSql()
        {
            SqlConnection connection = new SqlConnection(sqlconnection);
            connection.Open();
            return connection;
        }

        ///// <summary>
        ///// 获取分页Sql
        ///// </summary>
        ///// <param name="tableName"></param>
        ///// <param name="colName"></param>
        ///// <param name="colList"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="orderBy"></param>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //public string GetPageSql(string tableName, string colName, string colList, int pageSize, int pageIndex, int orderBy, string condition)
        //{
        //    string temp = string.Empty;
        //    string sql = string.Empty;
        //    if (string.IsNullOrEmpty(condition))
        //    {
        //        condition = " 1=1 ";
        //    }

        //    //降序
        //    if (orderBy == 1)
        //    {
        //        temp = "select top {0} {1} from {2} where {5} and {3} <(select min(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} desc) t) order by {3} desc";
        //        sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
        //    }
        //    //降序
        //    if (orderBy == 0)
        //    {
        //        temp = "select top {0} {1} from {2} where {5} and {3} >(select max(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} asc) t) order by {3} asc";
        //        sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
        //    }
        //    //第一页
        //    if (pageIndex == 1)
        //    {
        //        temp = "select top {0} {1} from {2} where {3} order by {4} {5}";
        //        sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc");
        //    }

        //    return sql;
        //}

        /// <summary>
        /// 获取分页Sql for sqlite
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string GetPageSql(string tableName, string colName, string colList, int pageSize, int pageIndex, int orderBy, string condition)
        {
            string temp = string.Empty;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                condition = " 1=1 ";
            }


            temp = "select {1} from {2} where {3} order by {4} {5} limit {0} OFFSET {6}";
            sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc", pageSize * (pageIndex - 1));

            //第一页
            if (pageIndex == 1)
            {
                temp = "select {1} from {2} where {3} order by {4} {5} limit {0}";
                sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc");
            }

            return sql;
        }

    }
}
