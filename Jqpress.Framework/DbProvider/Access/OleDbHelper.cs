using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Jqpress.Framework.Configuration;

namespace Jqpress.Framework.DbProvider.Access
{
    /// <summary> 
    /// 数据基类
    /// 打开方式需要改进
    /// </summary>
    public class OleDbHelper
    {


        private static string _mdbpath = System.Web.HttpContext.Current.Server.MapPath(ConfigHelper.SitePath + ConfigHelper.DbConnection);
        public static string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _mdbpath;

        /// <summary>
        /// 查询次数统计
        /// </summary>
        private static int _querycount = 0;


        /// <summary>
        /// 查询次数统计
        /// </summary>
        public static int QueryCount
        {
            get { return _querycount; }
            set { _querycount = value; }
        }
        /// <summary>
        /// 查询次数统计 尝试不用httpcontext
        /// </summary>
        private static int m_querycount = 0;

#if DEBUG
        private static string m_querydetail = "";
        public static string QueryDetail
        {
            get { return m_querydetail; }
            set { m_querydetail = value; }
        }
        private static string GetQueryDetail(string commandText, DateTime dtStart, DateTime dtEnd, OleDbParameter[] cmdParams)
        {
            string tr = "<tr style=\"background: rgb(255, 255, 255) none repeat scroll 0%; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">";
            string colums = "";
            string dbtypes = "";
            string values = "";
            string paramdetails = "";
            if (cmdParams != null && cmdParams.Length > 0)
            {
                foreach (OleDbParameter param in cmdParams)
                {
                    if (param == null)
                    {
                        continue;
                    }

                    colums += "<td>" + param.ParameterName + "</td>";
                    dbtypes += "<td>" + param.DbType + "</td>";
                    values += "<td>" + param.Value + "</td>";
                }
                paramdetails = string.Format("<table width=\"100%\" cellspacing=\"1\" cellpadding=\"0\" style=\"background: rgb(255, 255, 255) none repeat scroll 0%; margin-top: 5px; font-size: 12px; display: block; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">{0}{1}</tr>{0}{2}</tr>{0}{3}</tr></table>", tr, colums, dbtypes, values);
            }
            return string.Format("<center><div style=\"border: 1px solid black; background:#FFF; margin: 2px; padding: 1em; text-align: left; width: 96%; clear: both;\"><div style=\"font-size: 12px; float: right; width: 100px; margin-bottom: 5px;\"><b>TIME:</b> {0}</div><span style=\"font-size: 12px;\">{1}{2}</span></div><br /></center>", dtEnd.Subtract(dtStart).TotalMilliseconds / 1000, commandText, paramdetails);
        }
#endif



        #region MakeCommand
        ///// <summary>
        ///// 创建Command命令
        ///// </summary>
        ///// <param name="conn">数据连接</param>
        ///// <param name="cmdType">命令类型</param>
        ///// <param name="cmdText">SQL语句</param>
        ///// <returns>OleDbCommand</returns>
        //private static OleDbCommand MakeCommand(OleDbConnection conn, CommandType cmdType, string cmdText)
        //{
        //    if (conn.State != ConnectionState.Open)
        //    {
        //        conn.Open();
        //    }
        //    OleDbCommand cmd = new OleDbCommand();
        //    cmd.Connection = conn;
        //    cmd.CommandText = cmdText;
        //    cmd.CommandType = cmdType;
        //    return cmd;
        //}

        /// <summary>
        /// 创建Command命令
        /// </summary>
        /// <param name="conn">数据连接</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数级</param>
        /// <returns></returns>
        private static OleDbCommand MakeCommand(OleDbConnection conn, CommandType cmdType, string cmdText, OleDbParameter[] prams)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (prams != null)
            {
                foreach (OleDbParameter p in prams)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }
        #endregion

        #region MakeParam
        /// <summary>
        /// 生成输入参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <param name="Value">Param value.</param>
        /// <returns>New parameter.</returns>
        public static OleDbParameter MakeInParam(string ParamName, OleDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// 生成输出参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <returns>New parameter.</returns>
        public static OleDbParameter MakeOutParam(string ParamName, OleDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// 生成返回参数,我添加
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static OleDbParameter MakeReturnParam(string ParamName, OleDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }

        /// <summary>
        /// 生成各种参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <param name="Direction">Parm direction.</param>
        /// <param name="Value">Param value.</param>
        /// <returns>New parameter.</returns>
        private static OleDbParameter MakeParam(string ParamName, OleDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            OleDbParameter param;

            if (Size > 0)
                param = new OleDbParameter(ParamName, DbType, Size);
            else
                param = new OleDbParameter(ParamName, DbType);

            param.Direction = Direction;
            if (Direction == ParameterDirection.Input && Value != null)
            {
                param.Value = Value;
            }
            return param;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText)
        {

            return ExecuteScalar(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, OleDbParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;
            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
#if DEBUG
                DateTime dt1 = DateTime.Now;
#endif
                OleDbCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
#if DEBUG
                DateTime dt2 = DateTime.Now;
                m_querydetail += GetQueryDetail(cmd.CommandText, dt1, dt2, prams);
                m_querycount++;
#endif
                return obj;
            }
        }

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 返回受影响的行数,常用于Update和Delete 语句
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 返回受影响的行数,常用于Update和Delete 语句
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回受影响的行数,常用于Insert,Update和Delete 语句
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, OleDbParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
#if DEBUG
                DateTime dt1 = DateTime.Now;
#endif
                OleDbCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
#if DEBUG
                DateTime dt2 = DateTime.Now;
                m_querydetail += GetQueryDetail(cmd.CommandText, dt1, dt2, prams);
                m_querycount++;
#endif
                return i;
            }
        }

        #endregion

        #region ExecuteReader
        /// <summary>
        /// 返回 OleDbDataReader
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>OleDbDataReader</returns>
        public static OleDbDataReader ExecuteReader(string cmdText)
        {
            return ExecuteReader(CommandType.Text, cmdText);
        }

        [Obsolete]
        /// <summary>
        /// 返回 OleDbDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>OleDbDataReader</returns>
        public static OleDbDataReader ExecuteReader(CommandType cmdType, string cmdText)
        {
            return ExecuteReader(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回 OleDbDataReader
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public static OleDbDataReader ExecuteReader(string cmdText, OleDbParameter[] prams)
        {
            return ExecuteReader(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 返回 OleDbDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns></returns>
        public static OleDbDataReader ExecuteReader(CommandType cmdType, string cmdText, OleDbParameter[] prams)
        {

            _querycount++;

            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            try 
            {
                //using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                //{
                    OleDbConnection conn = new OleDbConnection(ConnectionString);
#if DEBUG
                    DateTime dt1 = DateTime.Now;
#endif
                    OleDbCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                    OleDbDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
#if DEBUG
                    DateTime dt2 = DateTime.Now;
                    m_querydetail += GetQueryDetail(cmd.CommandText, dt1, dt2, prams);
                    m_querycount++;
#endif
                    return read;
               // }

            }catch(Exception e)
            {
                throw e;
            }

            return null;
        }

        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(string cmdText)
        {
            return ExecuteDataSet(CommandType.Text, cmdText, null);
        }
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText)
        {
            //using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            //{
            //    OleDbCommand cmd = MakeCommand(conn, cmdType, cmdText);
            //    OleDbDataAdapter apt = new OleDbDataAdapter(cmd);
            //    DataSet ds = new DataSet();
            //    apt.Fill(ds);
            //    cmd.Parameters.Clear();
            //    return ds;
            //}
            return ExecuteDataSet(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, OleDbParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            using (OleDbConnection conn = new OleDbConnection(ConnectionString))
            {
#if DEBUG
                DateTime dt1 = DateTime.Now;
#endif
                OleDbCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                OleDbDataAdapter apt = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                apt.Fill(ds);
                cmd.Parameters.Clear();
#if DEBUG
                DateTime dt2 = DateTime.Now;
                m_querydetail += GetQueryDetail(cmd.CommandText, dt1, dt2, prams);
                m_querycount++;
#endif
                return ds;
            }
        }

        #endregion

        /// <summary>
        /// 获取分页Sql
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string GetPageSql(string tableName, string colName, string colList, int pageSize, int pageIndex, int orderBy, string condition)
        {
            string temp = string.Empty;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                condition = " 1=1 ";
            }

            //降序
            if (orderBy == 1)
            {
                temp = "select top {0} {1} from {2} where {5} and {3} <(select min(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} desc) t) order by {3} desc";
                sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
            }
            //降序
            if (orderBy == 0)
            {
                temp = "select top {0} {1} from {2} where {5} and {3} >(select max(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} asc) t) order by {3} asc";
                sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
            }
            //第一页
            if (pageIndex == 1)
            {
                temp = "select top {0} {1} from {2} where {3} order by {4} {5}";
                sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc");
            }

            return sql;
        }
    }
}
