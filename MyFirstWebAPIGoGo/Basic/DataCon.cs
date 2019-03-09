using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Drawing;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace MyFirstWebAPIGoGo
{
    class DataCon
    {
        public string sHostIP { get; private set; }
        public string sDBName { get; private set; }
        public string sHostPort { get; private set; }
        public string sHostAcct { get; private set; }
        public string sHostPass { get; private set; }
        public string sSqlConnect { get; private set; }
        public SqlConnection SQLCon { get; private set; }

        public DataCon()
        {
            SQLCon = new SqlConnection(GetConnectionString());
        }
        //建立連線
        public SqlConnection GetConnection()
        {
            this.SQLCon = new SqlConnection(GetConnectionString());
            return this.SQLCon;
        }
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            //return "Data Source=" + sHostIP + ";Database=" + sDBName + ";User id=" + sHostAcct + ";PWD=" + sHostPass + ";Connection Timeout=2;";
        }

        //讀取資料表
        public DataTable GetDataTable(string sSql)
        {
            DataTable dtTemp = new DataTable();
            SqlDataAdapter sqlDataAdp = new SqlDataAdapter(sSql, GetConnection());
            try
            {
                sqlDataAdp.Fill(dtTemp);
                sqlDataAdp.Dispose();
                return dtTemp;
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetDataSet(string sSql)
        {
            DataSet dtTemp = new DataSet();
            SqlDataAdapter sqlDataAdp = new SqlDataAdapter(sSql, GetConnection());
            try
            {
                sqlDataAdp.Fill(dtTemp);
                sqlDataAdp.Dispose();
                return dtTemp;
            }
            catch
            {
                throw;
            }
        }
        public SqlDataAdapter GetDataAdapter(string sSql)
        {
            try
            {
                SqlDataAdapter sqlDataAdp = new SqlDataAdapter(sSql, GetConnection());

                SqlCommandBuilder sqlCombl = new SqlCommandBuilder(sqlDataAdp);
                sqlDataAdp.UpdateCommand = sqlCombl.GetUpdateCommand();
                sqlDataAdp.DeleteCommand = sqlCombl.GetDeleteCommand();
                sqlDataAdp.InsertCommand = sqlCombl.GetInsertCommand();

                if (sqlDataAdp != null)
                    return sqlDataAdp;
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }

        public SqlCommandBuilder GetCommandBuilder(SqlDataAdapter SqlAdp)
        {
            SqlCommandBuilder cmbBuilder = new SqlCommandBuilder(SqlAdp);
            return cmbBuilder;
        }

        public void IsConnected()
        {
            SqlConnection sqlConn = GetConnection();
            sqlConn.Open();
        }

        public int ExecSQLCmd(string sql)
        {
            SqlConnection sqlcon = GetConnection();
            SqlCommand sCmd = new SqlCommand(sql, sqlcon);
            sCmd.CommandTimeout = 300;

            int icnt = 0;
            sqlcon.Open();
            try
            {
                icnt = sCmd.ExecuteNonQuery();
            }
            catch (SqlException err)
            {
                throw;
            }

            sCmd.Dispose();
            sqlcon.Close();
            sqlcon.Dispose();
            return icnt;
        }

        public SqlDataReader GetDataReader(string M_str_sqlstr)
        {
            SqlConnection sqlcon = GetConnection();
            SqlCommand sqlcom = new SqlCommand(M_str_sqlstr, sqlcon);
            

            sqlcon.Open();
            try
            {
                SqlDataReader sqlread = sqlcom.ExecuteReader(CommandBehavior.CloseConnection);
                sqlcom.Dispose();

                return sqlread;
            }
            catch (SqlException Err)
            {
                throw;
            }
        }

        //交易機制
        public int ExecSQLCmdTrans(string sql)
        {
            int icnt = 0;
            SqlConnection sqlcon = GetConnection();
            sqlcon.Open();

            SqlTransaction tran = sqlcon.BeginTransaction();
            SqlCommand sCmd = new SqlCommand(sql, sqlcon);
            sCmd.CommandTimeout = 300;
            sCmd.Transaction = tran;

            try
            {
                icnt = sCmd.ExecuteNonQuery();
                tran.Commit();      //執行交易
            }
            catch (Exception Err)
            {
                tran.Rollback();    //交易取消
                throw;
            }

            sqlcon.Close();
            sCmd.Dispose();
            sqlcon.Dispose();
            return icnt;
        }
        public int ExecSQLCmdTrans(string sql, SqlConnection sqlcon, SqlTransaction tran)
        {
            int icnt = 0;

            SqlCommand sCmd = sqlcon.CreateCommand();
            sCmd.CommandTimeout = 300;
            sCmd.Connection = sqlcon;
            sCmd.Transaction = tran;

            try
            {
                sCmd.CommandText = sql;
                icnt = sCmd.ExecuteNonQuery();
            }
            catch (Exception Err)
            {
                tran.Rollback();    //交易取消
                throw;
            }

            sCmd.Dispose();

            return icnt;
        }

        //從資料表中撈特定欄位資料 
        public string sGetDataFromTable(DataTable dtSource, string sCol, string sCondition)
        {
            DataRow[] rowTemp = dtSource.Select(sCondition);

            if (rowTemp.Length > 0)
            {
                return rowTemp[0][sCol].ToString().Trim();
            }
            else
            {
                return String.Empty;
            }
        }
        //從資料表中撈特定資料列 
        public DataRow[] rowGetDataFromTable(DataTable dtSource, string sCondition)
        {
            DataRow[] rowTemp = dtSource.Select(sCondition);

            if (rowTemp.Length > 0)
            {
                return rowTemp;
            }
            else
            {
                return null;
            }
        }
        //確認資料表是否存在
        public bool bCheckTableExist(string tablename)
        {
            string sTableName = "'" + tablename + "'";
            SqlConnection SQLCon = GetConnection();

            SQLCon.Open();
            SqlCommand cmd = new SqlCommand("select count(1) from sysobjects where name = " + sTableName, SQLCon);
            bool result = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            SQLCon.Close();
            SQLCon.Dispose();
            return result;
        }
        //確認資料欄位是否存在
        public bool bCheckColumnExist(string tablename, string ColumnName)
        {
            string sTableName = "'" + tablename + "'", sColumnName = "'" + ColumnName + "'";
            SqlConnection SQLCon = GetConnection();

            SQLCon.Open();
            SqlCommand cmd = new SqlCommand("select count(1) from syscolumns where id = OBJECT_ID(" + sTableName + ") and name = " + sColumnName, SQLCon);
            bool result = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            SQLCon.Close();
            SQLCon.Dispose();
            return result;
        }
        //取得資料庫某個欄位值
        public string GetFieldData(int placeId, string sSQL) 
        {
            SqlConnection sqlcon = GetConnection();
            sqlcon.Open();
            SqlCommand command = new SqlCommand(sSQL, sqlcon);
            SqlDataReader reader = command.ExecuteReader();

            string placeIp = "";

            if (reader.Read()) {
                var rData = reader.GetSqlValue(0);
                placeIp = rData.ToString();
            } else {
                //do nothing
            }

            sqlcon.Close();
            sqlcon.Dispose();
            return placeIp;
        }

        //新增取得select出來的資料列數量
        public int RowNums(string sql)
        {
            SqlConnection sqlcon = GetConnection();
            SqlCommand sCmd = new SqlCommand(sql, sqlcon);
            sCmd.CommandTimeout = 300;

            int icnt = 0;
            sqlcon.Open();
            try
            {
                icnt = (int)sCmd.ExecuteScalar();
            }
            catch
            {
                icnt = 0;
            }

            sCmd.Dispose();
            sqlcon.Close();
            sqlcon.Dispose();

            return icnt;
        }
        //SqlDataAdapter轉換方法
        public SqlDataAdapter SetInsertAdapter(SqlDataAdapter adapter, SqlCommandBuilder cmdBuilder)
        {
            adapter.InsertCommand = cmdBuilder.GetInsertCommand();

            return adapter;
        }
        public SqlDataAdapter SetInsertAdapter(SqlDataAdapter adapter, SqlCommandBuilder cmdBuilder, SqlTransaction trans)
        {
            adapter.InsertCommand = cmdBuilder.GetInsertCommand();
            adapter.InsertCommand.Transaction = trans;
            adapter.InsertCommand.Connection = trans.Connection;

            return adapter;
        }

        public SqlDataAdapter SetUpdateAdapter(SqlDataAdapter adapter, SqlCommandBuilder cmdBuilder)
        {
            adapter.UpdateCommand = cmdBuilder.GetUpdateCommand();

            return adapter;
        }
        public SqlDataAdapter SetUpdateAdapter(SqlDataAdapter adapter, SqlCommandBuilder cmdBuilder, SqlTransaction trans)
        {
            adapter.UpdateCommand = cmdBuilder.GetUpdateCommand();
            adapter.UpdateCommand.Transaction = trans;
            adapter.UpdateCommand.Connection = trans.Connection;

            return adapter;
        }
    }
}
