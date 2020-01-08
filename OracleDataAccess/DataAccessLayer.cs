using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;


namespace AHK.DataAccess.OracleDataAccess
{
    public class DataAccessLayer
    {
        private string databaseHost;
        private int databasePort;
        private string databaseServiceName;
        private string databaseTnsName;

        private string userName;
        private string passWord;
        private string clientID;

        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }

        private bool failOverMode;

        private int minPoolSize;
        private int connectionLifetimeSec;
        private int connectionTimeoutSec;
        private int incrementPoolSize;
        private int decrementPoolSize;

        private OracleConnection oraCon;
        private OracleCommand oraCom;
        private OracleTransaction oraTransac;

        private bool clearOracleParameter = true;



        /// <summary>constructor of the class, use to prepare internal property</summary>
        /// <param name="databasetnsname">tnsnames registered on the tnsname.ora file</param>
        /// <param name="username">username used to connect to the database</param>
        /// <param name="password">password used to connect to the database</param>
        /// <param name="enableFailoverMode"> if true use then use FailOver Mode available on Oracle High Availability Feature </param>
        public DataAccessLayer(string databasetnsname, string username, string password, bool enableFailoverMode)
        {
            this.databaseTnsName = databasetnsname;
            this.userName = username;
            this.passWord = password;
            this.failOverMode = enableFailoverMode;

            initConnection();
        }
        /// <summary>constructor of the class, use to prepare internal property</summary>
        /// <param name="databasehost">ip or hostname of the Oracle Database</param>
        /// <param name="databaseport">port to connect to the Oracle Database</param>
        /// <param name="databaseservicename">the service name registered on the Oracle Database</param>
        /// <param name="username">username used to connect to the database</param>
        /// <param name="password">password used to connect to the database</param>
        /// <param name="enableFailoverMode"> if true use then use FailOver Mode available on Oracle High Availability Feature </param>
        public DataAccessLayer(string databasehost, int databaseport, string databaseservicename, string username, string password, bool enableFailoverMode)
        {
            this.databaseHost = databasehost;
            this.databasePort = databaseport;
            this.databaseServiceName = databaseservicename;
            this.userName = username;
            this.passWord = password;
            this.failOverMode = enableFailoverMode;

            initConnection();
        }
        /// <summary>constructor of the class, use to prepare internal property</summary>
        /// <param name="databasetnsname">tnsnames registered on the tnsname.ora file</param>
        /// <param name="username">username used to connect to the database</param>
        /// <param name="password">password used to connect to the database</param>
        /// <param name="minpoolsize">poolsize used to optimize the connection</param>
        /// <param name="connectionlifetimesec">connection lifetime in seconds used to optimize the connection</param>
        /// <param name="connectiontimeoutsec">connection timeout in seconds used to optimize the connection</param>
        /// <param name="incrpoolsize">increase poolsize if the connection pool is full</param>
        /// <param name="decrpoolsize">decrease poolsize if the connection pool is timeout</param>
        /// <param name="enableFailoverMode"> if true use then use FailOver Mode available on Oracle High Availability Feature </param>
        public DataAccessLayer(string databasetnsname, string username, string password,
            int minpoolsize, int connectionlifetimesec, int connectiontimeoutsec, int incrpoolsize, int decrpoolsize, bool enableFailoverMode)
        {
            this.databaseTnsName = databasetnsname;
            this.userName = username;
            this.passWord = password;
            this.minPoolSize = minpoolsize;
            this.connectionLifetimeSec = connectionlifetimesec;
            this.connectionTimeoutSec = connectiontimeoutsec;
            this.incrementPoolSize = incrpoolsize;
            this.decrementPoolSize = decrpoolsize;
            this.failOverMode = enableFailoverMode;

            initConnection();
        }
        /// <summary>constructor of the class, use to prepare internal property</summary>
        /// <param name="databasehost">ip or hostname of the Oracle Database</param>
        /// <param name="databaseport">port to connect to the Oracle Database</param>
        /// <param name="databaseservicename">the service name registered on the Oracle Database</param>
        /// <param name="username">username used to connect to the database</param>
        /// <param name="password">password used to connect to the database</param>
        /// <param name="minpoolsize">poolsize used to optimize the connection</param>
        /// <param name="connectionlifetimesec">connection lifetime in seconds used to optimize the connection</param>
        /// <param name="connectiontimeoutsec">connection timeout in seconds used to optimize the connection</param>
        /// <param name="incrpoolsize">increase poolsize if the connection pool is full</param>
        /// <param name="decrpoolsize">decrease poolsize if the connection pool is timeout</param>
        /// <param name="enableFailoverMode"> if true use then use FailOver Mode available on Oracle High Availability Feature </param>
        public DataAccessLayer(string databasehost, int databaseport, string databaseservicename, string username, string password,
            int minpoolsize, int connectionlifetimesec, int connectiontimeoutsec, int incrpoolsize, int decrpoolsize, bool enableFailoverMode)
        {
            this.databaseHost = databasehost;
            this.databasePort = databaseport;
            this.databaseServiceName = databaseservicename;
            this.userName = username;
            this.passWord = password;
            this.minPoolSize = minpoolsize;
            this.connectionLifetimeSec = connectionlifetimesec;
            this.connectionTimeoutSec = connectiontimeoutsec;
            this.incrementPoolSize = incrpoolsize;
            this.decrementPoolSize = decrpoolsize;
            this.failOverMode = enableFailoverMode;

            initConnection();
        }

        /// <summary>initialize connection settings based on the constructor
        /// </summary>
        private void initConnection()
        {


            string connectionDataSource;
            if (this.databaseTnsName == null || this.databaseTnsName == string.Empty || this.databaseTnsName == "")
            {
                connectionDataSource = string.Format(
                                       "Data Source = " +
                                       "(DESCRIPTION = " +
                                       " (ADDRESS_LIST = " +
                                       " (ADDRESS = (PROTOCOL = TCP)" +
                                       " (HOST = {0}) " +
                                       " (PORT = {1}) " +
                                       " )" +
                                       " )" +
                                       " (CONNECT_DATA = " +
                                       " (SERVICE_NAME = {2})));",
                                       databaseHost, databasePort.ToString(), databaseServiceName);
            }
            else
            {
                connectionDataSource = string.Format("Data Source={0};", databaseTnsName);
            }

            if (failOverMode == true)
            {
                string connectionFailoverMode = "(FAILOVER_MODE=" +
                                                "(TYPE=select)" +
                                                "(METHOD=basic)" +
                                                "(RETRIES=20)" +
                                                "(DELAY=5)" +
                                                ")";
                int insertPosition = connectionDataSource.IndexOf("));");
                connectionDataSource = connectionDataSource.Insert(insertPosition, connectionFailoverMode);
            }


            string connectionCredentials = string.Format("User Id={0};Password={1};",
                userName, passWord);

            if (minPoolSize == 0) { this.minPoolSize = 1; }
            if (connectionLifetimeSec == 0) { connectionLifetimeSec = 600; }
            if (connectionTimeoutSec == 0) { connectionTimeoutSec = 300; }
            if (incrementPoolSize == 0) { incrementPoolSize = 2; }
            if (decrementPoolSize == 0) { decrementPoolSize = 1; }

            string connectionPools = string.Format("Min Pool Size={0};Connection Lifetime={1};Connection Timeout={2};Incr Pool Size={3}; Decr Pool Size={4}",
                minPoolSize.ToString(), connectionLifetimeSec.ToString(), connectionTimeoutSec.ToString(), incrementPoolSize.ToString(), decrementPoolSize.ToString());
            oraCon = new Oracle.DataAccess.Client.OracleConnection(connectionDataSource + connectionCredentials + connectionPools);
            oraCom = new OracleCommand();
            oraCom.Connection = oraCon;
        }
        /// <summary>clear previously used oracleparameters if exists so the oracleparameter instance is reuseable
        /// </summary>
        private void doClearOracleParameters()
        {
            oraCom.Parameters.Clear();
            oraCom.ArrayBindCount = 0;
        }
        /// <summary>use to get an equivalent OracleDbType based on the object parameter type</summary>
        /// <param name="thevalue">the object that the method will attempt to get the equivalent OracleDbType</param>
        /// <returns>the OracleDbType that match the object parameter type</returns>
        private OracleDbType getEquivalentOracleDbTypeFromObject(object thevalue)
        {
            if (thevalue is String) return OracleDbType.Varchar2;
            if (thevalue is Char || thevalue is Char[]) return OracleDbType.Varchar2;
            if (thevalue is DateTime) return OracleDbType.Date;
            if (thevalue is TimeSpan) return OracleDbType.IntervalDS;
            if (thevalue is Int64) return OracleDbType.Int64;
            if (thevalue is Int32) return OracleDbType.Int32;
            if (thevalue is Int16) return OracleDbType.Int16;
            if (thevalue is Byte) return OracleDbType.Byte;
            if (thevalue is Decimal) return OracleDbType.Decimal;
            if (thevalue is Single || thevalue is float) return OracleDbType.Single;
            if (thevalue is Double) return OracleDbType.Double;
            if (thevalue is Byte[]) return OracleDbType.Blob;
            if (thevalue is IOracleCustomType) return OracleDbType.Object;
            else return OracleDbType.Varchar2;
        }

        private void bindInOracleParameter(params OracleParameter[] OrcParams)
        {
            foreach (OracleParameter item in OrcParams)
            {
                oraCom.Parameters.Add(item);
            }
        }
        private void bindInObjectParameter(params object[] ObjParams)
        {
            int i = 0;
            foreach (object item in ObjParams)
            {
                if (item is OracleParameter)
                {
                    oraCom.Parameters.Add((OracleParameter)item);
                }
                else if (item is DBParameter)
                {
                    oraCom.Parameters.Add((DBParameter)item);
                }
                else
                {
                    OracleParameter myparam = new OracleParameter();
                    myparam.ParameterName = "p" + i.ToString();
                    myparam.OracleDbType = getEquivalentOracleDbTypeFromObject(item);
                    myparam.Value = item;
                    myparam.Direction = ParameterDirection.Input;
                    oraCom.Parameters.Add(myparam);
                    i++;
                }
            }
        }
        private void bindInDBParameter(params DBParameter[] DBParams)
        {
            foreach (DBParameter item in DBParams)
            {
                OracleParameter p = new OracleParameter();
                p.ParameterName = item.ParameterName;
                p.OracleDbType = item.AHKDbtype;
                p.Direction = item.ParameterDirection;
                p.Value = item.Value != null ? item.Value : DBNull.Value;
                oraCom.Parameters.Add(p);
            }
        }

        private void bindOutOracleParameterValue(params OracleParameter[] OrcParams)
        {
            for (int i = 0; i < oraCom.Parameters.Count; i++)
            {
                OrcParams[i].Value = (oraCom.Parameters[i].Value != null ? oraCom.Parameters[i].Value : "");
            }
        }
        private void bindOutDBParameterValue(params DBParameter[] DBParams)
        {
            for (int i = 0; i < oraCom.Parameters.Count; i++)
            {
                DBParams[i].Value = (oraCom.Parameters[i].Value != null ? oraCom.Parameters[i].Value : "");
            }
        }
        private void bindOutObjectParameterValue(params object[] ObjParams)
        {
            for (int i = 0; i < oraCom.Parameters.Count; i++)
            {
                if (ObjParams[i] is OracleParameter)
                {
                    OracleParameter p = new OracleParameter();
                    p = (OracleParameter)ObjParams[i];
                    p.Value = (oraCom.Parameters[i].Value != null ? oraCom.Parameters[i].Value : "");
                    ObjParams[i] = p;
                }
                else if (ObjParams[i] is DBParameter)
                {
                    DBParameter p = new DBParameter();
                    p = (DBParameter)ObjParams[i];
                    p.Value = (oraCom.Parameters[i].Value != null ? oraCom.Parameters[i].Value : "");
                    ObjParams[i] = p;
                }
                else
                {
                    ObjParams[i] = (oraCom.Parameters[i].Value != null ? oraCom.Parameters[i].Value : "");
                }


            }
        }
        /// <summary>use to begin transaction, invoke this method before doing any CRUD operations on the database
        /// </summary>
        public void TransactionBegin()
        {
            if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
            oraTransac = oraCon.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        }
        /// <summary>use to commit transaction, invoke this method after doing any CRUD operations on the database
        /// </summary>
        public void TransactionCommit()
        {
            if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
            oraTransac.Commit();
            if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
        }
        /// <summary>use to rollback transaction, invoke this method if exception rises or the business logic fails while
        /// doing any CRUD operations on the database</summary>
        public void TransactionRollback()
        {
            if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
            oraTransac.Rollback();
            if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
        }
        /// <summary>use to test connection to the oracle database</summary>
        /// <param name="responsemessage">if connection success the out message is OK, otherwise it will contain the exception message</param>
        /// <returns>if connection success then true, otherwise false</returns>
        public bool TestOpenConnection(out string responsemessage)
        {
            try
            {
                oraCon.Open();
            }
            catch (Exception ex)
            {
                responsemessage = ex.Message;
                return false;
            }
            finally
            {
                if (oraCon != null && oraCon.State == System.Data.ConnectionState.Open)
                {
                    oraCon.Close();
                }
            }
            responsemessage = "OK";
            return true;
        }
        /// <summary>use to get a single value from database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <returns>the object being returned by the command</returns>
        public object GetSingleValue(string commandquery, CommandType type)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                if (clearOracleParameter == true) { doClearOracleParameters(); }

                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                clearOracleParameter = true;

                return oraCom.ExecuteScalar();
            }
            catch (Exception) { throw; }
            finally { if (oraCon.State == ConnectionState.Open) { oraCon.Close(); } }
        }
        /// <summary>use to get a single value from database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="oraparams">the oracleparameter used by the command</param>
        /// <returns>the object being returned by the command</returns>
        public object GetSingleValue(string commandquery, CommandType type, params OracleParameter[] oraparams)
        {
            doClearOracleParameters();
            bindInOracleParameter(oraparams);
            clearOracleParameter = false;
            return GetSingleValue(commandquery, type);
        }
        /// <summary>use to get a single value from database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="objparams">the parameter value used by the command, the rest of the details will be automatically contructed by the method</param>
        /// <returns>the object being returned by the command</returns>
        public object GetSingleValue(string commandquery, CommandType type, params object[] objparams)
        {
            doClearOracleParameters();
            bindInObjectParameter(objparams);
            clearOracleParameter = false;
            return GetSingleValue(commandquery, type);
        }
        /// <summary>use to get a single value from database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="dbparams">the AHK.DBparameter used by the command</param>
        /// <returns></returns>
        public object GetSingleValue(string commandquery, CommandType type, params DBParameter[] dbparams)
        {
            doClearOracleParameters();
            bindInDBParameter(dbparams);
            clearOracleParameter = false;
            return GetSingleValue(commandquery, type);
        }
        /// <summary>use to get a single table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <returns>the table being returned by the command</returns>
        public DataTable GetDatatable(string commandquery, CommandType type)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                if (clearOracleParameter == true) { doClearOracleParameters(); }

                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                DataTable mydt = new DataTable();
                OracleDataReader myreader;

                myreader = oraCom.ExecuteReader();
                if (myreader.HasRows) { mydt.Load(myreader); }
                clearOracleParameter = true;

                return mydt;
            }
            catch (Exception) { throw; }
            finally { if (oraCon.State == ConnectionState.Open) { oraCon.Close(); } }
        }
        /// <summary>use to get a single table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="oraparams">the oracleparameter used by the command</param>
        /// <returns>the table being returned by the command</returns>
        public DataTable GetDatatable(string commandquery, CommandType type, params OracleParameter[] oraparams)
        {
            doClearOracleParameters();
            bindInOracleParameter(oraparams);
            clearOracleParameter = false;
            return GetDatatable(commandquery, type);
        }
        /// <summary>use to get a single table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="objparams">the parameter value used by the command, the rest of the details will be automatically contructed by the method</param>
        /// <returns>the table being returned by the command</returns>
        public DataTable GetDatatable(string commandquery, CommandType type, params object[] objparams)
        {
            doClearOracleParameters();
            bindInObjectParameter(objparams);
            clearOracleParameter = false;
            return GetDatatable(commandquery, type);
        }
        /// <summary>use to get a single table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="dbparams">the AHK.DBparameter used by the command</param>
        /// <returns></returns>
        public DataTable GetDatatable(string commandquery, CommandType type, params DBParameter[] dbparams)
        {
            doClearOracleParameters();
            bindInDBParameter(dbparams);
            clearOracleParameter = false;
            return GetDatatable(commandquery, type);
        }
        /// <summary>use to get dataset that contains multiple table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <returns>the dataset being returned by the command</returns>
        public DataSet GetDataset(string commandquery, CommandType type)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                if (clearOracleParameter == true) { doClearOracleParameters(); }

                int wow = oraCom.ArrayBindCount;

                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                DataSet myds = new DataSet();
                OracleDataAdapter myadapt = new OracleDataAdapter(oraCom);
                myadapt.Fill(myds);

                clearOracleParameter = true;

                return myds;
            }
            catch (Exception) { throw; }
            finally { if (oraCon.State == ConnectionState.Open) { oraCon.Close(); } }
        }
        /// <summary>use to get dataset that contains multiple table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="oraparams">the oracleparameter used by the command</param>
        /// <returns>the dataset being returned by the command</returns>
        public DataSet GetDataset(string commandquery, CommandType type, params OracleParameter[] oraparams)
        {
            doClearOracleParameters();
            bindInOracleParameter(oraparams);
            clearOracleParameter = false;
            return GetDataset(commandquery, type);
        }
        /// <summary>use to get dataset that contains multiple table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="objparams">the parameter value used by the command, the rest of the details will be automatically contructed by the method</param>
        /// <returns>the dataset being returned by the command</returns>
        public DataSet GetDataset(string commandquery, CommandType type, params object[] objparams)
        {
            doClearOracleParameters();
            bindInObjectParameter(objparams);
            clearOracleParameter = false;
            return GetDataset(commandquery, type);
        }
        /// <summary>use to get dataset that contains multiple table result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="dbparams">the AHK.DBparameter used by the command</param>
        /// <returns></returns>
        public DataSet GetDataset(string commandquery, CommandType type, params DBParameter[] dbparams)
        {
            doClearOracleParameters();
            bindInDBParameter(dbparams);
            clearOracleParameter = false;
            return GetDataset(commandquery, type);
        }

        /// <summary>use to get list of string result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <returns></returns>
        public List<List<string>> GetListOfString(string commandquery, CommandType type)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                if (clearOracleParameter == true) { doClearOracleParameters(); }

                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                List<List<string>> theRows = new List<List<string>>();
                OracleDataReader myreader;

                myreader = oraCom.ExecuteReader();
                if (myreader.HasRows)
                {
                    while (myreader.Read())
                    {
                        List<string> theColumns = new List<string>();
                        for (int i = 0; i < myreader.FieldCount; i++)
                        {
                            theColumns.Add(Convert.ToString(myreader[i].ToString()));
                        }
                        theRows.Add(theColumns);
                    }
                    myreader.Close();
                }
                clearOracleParameter = true;
                return theRows;
            }
            catch (Exception) { throw; }
            finally { if (oraCon.State == ConnectionState.Open) { oraCon.Close(); } }
        }
        /// <summary>use to get list of string result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="oraparams">the oracleparameter used by the command</param>
        /// <returns></returns>
        public List<List<string>> GetListOfString(string commandquery, CommandType type, params OracleParameter[] oraparams)
        {
            doClearOracleParameters();
            bindInOracleParameter(oraparams);
            clearOracleParameter = false;
            return GetListOfString(commandquery, type);
        }
        /// <summary>use to get list of string result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="objparams"></param>
        /// <returns></returns>
        public List<List<string>> GetListOfString(string commandquery, CommandType type, params object[] objparams)
        {
            doClearOracleParameters();
            bindInObjectParameter(objparams);
            clearOracleParameter = false;
            return GetListOfString(commandquery, type);
        }
        /// <summary>use to get list of string result from the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="dbparams">the AHK.DBparameter used by the command</param>
        /// <returns></returns>
        public List<List<string>> GetListOfString(string commandquery, CommandType type, params DBParameter[] dbparams)
        {
            doClearOracleParameters();
            bindInDBParameter(dbparams);
            clearOracleParameter = false;
            return GetListOfString(commandquery, type);
        }

        /// <summary>use to execute Create-Update-Delete operations to the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// after that invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <returns>integer values that marks the rows affected by the command</returns>
        public int DoInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                if (clearOracleParameter == true) { doClearOracleParameters(); }

                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                clearOracleParameter = true;

                return oraCom.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!usetransaction)
                {
                    if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
                }
            }

        }
        /// <summary>use to execute Create-Update-Delete operations to the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// after that invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <param name="oraparams">the oracleparameter used by the command</param>
        /// <returns>integer values that marks the rows affected by the command</returns>
        public int DoInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction, params OracleParameter[] oraparams)
        {
            doClearOracleParameters();
            bindInOracleParameter(oraparams);
            clearOracleParameter = false;
            int i = DoInsertDeleteUpdate(commandquery, type, usetransaction);
            bindOutOracleParameterValue(oraparams);
            return i;
        }
        /// <summary>use to execute Create-Update-Delete operations to the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// after that invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <param name="dbparams">the AHK.DBparameter used by the command</param>
        /// <returns></returns>
        public int DoInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction, params DBParameter[] dbparams)
        {
            doClearOracleParameters();
            bindInDBParameter(dbparams);
            clearOracleParameter = false;
            int i = DoInsertDeleteUpdate(commandquery, type, usetransaction);
            bindOutDBParameterValue(dbparams);
            return i;
        }
        /// <summary>use to execute Create-Update-Delete operations to the database</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// thenss invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <param name="objparams">the parameter value used by the command, the rest of the details will be automatically contructed by the method</param>
        /// <returns>integer values that marks the rows affected by the command</returns>
        public int DoInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction, params object[] objparams)
        {
            doClearOracleParameters();
            bindInObjectParameter(objparams);
            clearOracleParameter = false;
            int i = DoInsertDeleteUpdate(commandquery, type, usetransaction);
            bindOutObjectParameterValue(objparams);
            return i;
        }
        /// <summary>use to execute multiple Create-Update-Delete operations to the database based on the arraybindingsize parameter</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="usetransaction"></param>
        /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// after that invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <param name="arraybindsize">the number of operations that will be executed</param>
        /// <param name="oraparamsarray">the oracleparameter used by the command</param>
        public void DoBulkInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction, int arraybindsize, params OracleParameter[] oraparamsarray)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                doClearOracleParameters();

                oraCom.ArrayBindCount = arraybindsize;
                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                bindInOracleParameter(oraparamsarray);
                oraCom.ExecuteNonQuery();

                clearOracleParameter = true;
            }
            catch (Exception) { throw; }
            finally
            {
                if (!usetransaction)
                {
                    if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
                }
            }
        }
        /// <summary>use to execute multiple Create-Update-Delete operations to the database based on the arraybindingsize parameter</summary>
        /// <param name="commandquery">the query command or procedure name</param>
        /// <param name="type">the command type</param>
        /// <param name="usetransaction"></param>
        /// <param name="usetransaction">decide whether will be using oracletransaction or not, 
        /// is using oracletransaction then .TransactionBegin() must be invoked before,
        /// after that invoke .TransactionCommit or TransactionRollback based on execution result</param>
        /// <param name="arraybindsize">the number of operations that will be executed</param>
        /// <param name="dbparamsarray">the AHK.DBparameter used by the command</param>
        public void DoBulkInsertDeleteUpdate(string commandquery, CommandType type, bool usetransaction, int arraybindsize, params DBParameter[] dbparamsarray)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                doClearOracleParameters();

                oraCom.ArrayBindCount = arraybindsize;
                oraCom.CommandText = commandquery;
                oraCom.CommandType = type;

                bindInDBParameter(dbparamsarray);
                oraCom.ExecuteNonQuery();

                clearOracleParameter = true;
            }
            catch (Exception) { throw; }
            finally
            {
                if (!usetransaction)
                {
                    if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
                }
            }
        }
        /// <summary>use to insert data using bulk method to the table</summary>
        /// <param name="destinationtablename">the table in which the data will be inserted</param>
        /// <param name="sourcedatatable"> the datatable contains the data to be inserted</param>
        /// <param name="columnmapping">column mapping used just in case the column structure between source and destination is different</param>
        public void DoBulkCopy(string destinationtablename, DataTable sourcedatatable, params OracleBulkCopyColumnMapping[] columnmapping)
        {
            try
            {
                if (oraCon.State == ConnectionState.Closed) { oraCon.Open(); }
                doClearOracleParameters();

                OracleBulkCopy mybulkcopy = new OracleBulkCopy(oraCon);
                mybulkcopy.DestinationTableName = destinationtablename;

                if (columnmapping != null && columnmapping.Length > 0)
                {
                    for (int i = 0; i < columnmapping.Length; i++)
                    {
                        mybulkcopy.ColumnMappings.Add(columnmapping[i]);
                    }
                }

                mybulkcopy.WriteToServer(sourcedatatable);

                mybulkcopy.Close();
                mybulkcopy.Dispose();
                clearOracleParameter = true;
                doClearOracleParameters();
            }
            catch (Exception) { throw; }
            finally
            {
                if (oraCon.State == ConnectionState.Open) { oraCon.Close(); }
            }
        }
    }
}

