using System;
using System.Collections.Generic;
using System.Text;

namespace AHK.DataAccess.OracleDataAccess
{
    class DBErrorStatement
    {
        public string Build(string queryExecuted, params string[] parameterUsed)
        {
            StringBuilder mystrb = new StringBuilder(2048);
            mystrb.Append("error execute:" + queryExecuted);
            if (parameterUsed.Length > 0)
            {
                mystrb.Append(" using values:");
            }
            for (int i = 0; i < parameterUsed.Length; i++)
            {
                mystrb.Append(parameterUsed[i]);
                if (i != parameterUsed.Length - 1)
                {
                    mystrb.Append(',');
                }
            }
            return mystrb.ToString();
        }
    }
}
