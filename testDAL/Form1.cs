using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AHK.DataAccess.OracleDataAccess;
using System.Configuration;

namespace testDAL
{
    public partial class Form1 : Form
    {
        DataAccessLayer mydal;
        public Form1()
        {
            InitializeComponent();
            mydal = new DataAccessLayer("win2003postgre", 1521, "dbgeneric", "generic", "generic", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DBParameter p0 = new DBParameter("p0", AHKDbType.Varchar2, ParameterDirection.Input, "abdul1");
                DBParameter p1 = new DBParameter("p1", AHKDbType.Int32, ParameterDirection.Output, null);
                DataTable mydt = new DataTable();
                // mydt = mydal.GetDatatable("proc_getproductsout", CommandType.StoredProcedure, p0);

                mydal.DoInsertDeleteUpdate("proc_getproductsout", CommandType.StoredProcedure, false, p0, p1);
                MessageBox.Show(p1.Value.ToString());
         
                int[] myint = new int[] { 1, 3, 5, 7, 9 };
                DBParameter p2 = new DBParameter("p2", AHKDbType.Int32, ParameterDirection.Input, myint);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, Int32> wow = new Dictionary<string, int>();
          
        }

     

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
