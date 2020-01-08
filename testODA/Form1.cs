using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using AHK.DataAccess.OracleDataAccess;

namespace testODA
{
    public partial class Form1 : Form
    {
        DataAccessLayer mydal, mydal2;

        DataTable mydt = new DataTable();
        public Form1()
        {
            InitializeComponent();
            mydal = new DataAccessLayer("win2003dbbiller", 1521, "billerdev", "usrbiling", "usrbiling", true);
            mydal2 = new DataAccessLayer("win2003postgre", 1521, "dbgeneric", "generic", "generic", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string errormsg;
            if (mydal.TestOpenConnection(out errormsg))
            {
                MessageBox.Show("connection opened ok");
            }
            else { MessageBox.Show("connection opened error"); }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //long result;


            try
            {
                OracleParameter myparam = new OracleParameter();
                myparam.Value = "Windshield";
                myparam.OracleDbType = OracleDbType.Varchar2;

                object result;

                result = mydal.GetSingleValue("select count(*) from products where name=:1", CommandType.Text, myparam);
                MessageBox.Show(result.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OracleParameter myparam = new OracleParameter();
                myparam.ParameterName = "param1";
                myparam.Value = "COMPLETE";
                myparam.OracleDbType = OracleDbType.Varchar2;

                OracleParameter myparam2 = new OracleParameter();
                myparam2.ParameterName = "param2";
                myparam2.Value = "01067";
                myparam2.OracleDbType = OracleDbType.Varchar2;

                OracleParameter[] myparams = new OracleParameter[2];
                myparams[0] = myparam;
                myparams[1] = myparam2;

                //dataGridView1.DataSource = mydal.ExecuteCommandReader("select * from rg_plpb_wp where status is null", CommandType.Text);
                //dataGridView1.DataSource = mydal.ExecuteCommandReader("select * from rg_plpb_wp where status=:1 and substr(npwp,1,5)=:3", CommandType.Text, myparam, myparam2);
                //dataGridView1.DataSource = mydal.ExecuteCommandReader("select * from rg_plpb_wp where status=:1 and substr(npwp,1,5)=:3", CommandType.Text, myparams);
                //dataGridView1.DataSource = mydal.ExecuteCommandReader("select * from rg_plpb_wp where status=:1 and substr(npwp,1,5)=:3", CommandType.Text, "COMPLETE", "01067");
                //dataGridView1.DataSource = mydal.ExecuteCommandReader("select * from rg_plpb_wp where status=:1 and id_wp=:2 and substr(npwp,1,5)=:3", CommandType.Text, "COMPLETE", 14429, 01062);

                mydt = mydal2.GetDatatable("select * from products", CommandType.Text);


                dataGridView1.DataSource = mydt;
                //dataGridView1.Columns[0].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {


                string[] myid = new string[] { "a1", "a2", "a3", "a4", "a5" };
                string[] myname = new string[] { "abdul1", "abdul2", "abdul3", "abdul4", "abdul5" };
                int[] myprice = new int[] { 10, 20, 30, 40, 50 };
                string[] myremarks = new string[] { "wow1", "wow2", "wow3", "wow4", "wow5" };

                OracleParameter pid = new OracleParameter();
                OracleParameter pname = new OracleParameter();
                OracleParameter pprice = new OracleParameter();
                OracleParameter premarks = new OracleParameter();

                pid.ParameterName = "pid";
                pname.ParameterName = "pname";
                pprice.ParameterName = "pprice";
                premarks.ParameterName = "premarks";

                pid.OracleDbType = OracleDbType.Varchar2;
                pname.OracleDbType = OracleDbType.Varchar2;
                pprice.OracleDbType = OracleDbType.Int32;
                premarks.OracleDbType = OracleDbType.Varchar2;

                pid.Value = myid;
                pname.Value = myname;
                pprice.Value = myprice;
                premarks.Value = myremarks;

                //mydal.ExecuteCommandNonQuery("insert into products values(:0,:1,:2,:3)", CommandType.Text, 5, pid, pname, pprice, premarks);
                OracleParameter nname = new OracleParameter();
                OracleParameter nprice = new OracleParameter();

                nname.ParameterName = "nname";
                nprice.ParameterName = "nprice";

                nname.Direction = ParameterDirection.Input;
                nprice.Direction = ParameterDirection.Input;

                nname.OracleDbType = OracleDbType.Varchar2;
                nprice.OracleDbType = OracleDbType.Int32;

                nname.Value = "abdul1";
                nprice.Value = 19999;

                //MessageBox.Show(  mydal.ExecuteCommandNonQuery("PROC_UPDATEPRODUCT", CommandType.StoredProcedure, nprice,nname ).ToString());
                OracleParameter pcount = new OracleParameter();
                pcount.OracleDbType = OracleDbType.Int32;
                //pcount.Direction = ParameterDirection.Output;
                pcount.Direction = ParameterDirection.ReturnValue;
                OracleParameter[] pcol = new OracleParameter[] { pcount };


                //mydal.ExecuteCommandNonQuery("func_RetrieveCount", CommandType.StoredProcedure, false, ref pcol);
                //MessageBox.Show(pcol[0].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                textBox1.Text = mydt.Rows[e.RowIndex][0].ToString();
                textBox2.Text = mydt.Rows[e.RowIndex][1].ToString();
                numericUpDown1.Value = Convert.ToDecimal(mydt.Rows[e.RowIndex][2].ToString());
                //textBox3.Text = mydt.Rows[e.RowIndex][2].ToString();
                textBox4.Text = mydt.Rows[e.RowIndex][3].ToString();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {


        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                mydal.TransactionBegin();

                mydal.DoInsertDeleteUpdate("insert into invoice values('a1',sysdate,'invoice one')", CommandType.Text, true);

                mydal.DoInsertDeleteUpdate("insert into INVOICEDETAILSs values('a1','purchase 1',100,999)", CommandType.Text, true);

                mydal.TransactionCommit();
                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {

                mydal.TransactionRollback();
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OracleParameter p0 = new OracleParameter();
            p0.ParameterName = "p0";
            p0.OracleDbType = OracleDbType.RefCursor;
            p0.Direction = ParameterDirection.ReturnValue;


            OracleParameter p1 = new OracleParameter();
            p1.ParameterName = "p1";
            p1.OracleDbType = OracleDbType.Int32;
            p1.Direction = ParameterDirection.Input;
            p1.Value = 40;

            OracleParameter p2 = new OracleParameter();
            p2.ParameterName = "p2";
            p2.OracleDbType = OracleDbType.RefCursor;
            p2.Direction = ParameterDirection.Output;

            OracleParameter[] oracol = new OracleParameter[] { p0, p1, p2 };


            DataTable mydt = new DataTable();
            DataSet myds = new DataSet();
            myds = mydal.GetDataset("func_getproductswithdetail", CommandType.StoredProcedure, oracol);
            dataGridView1.DataSource = myds.Tables[0];
            dataGridView2.DataSource = myds.Tables[1];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OracleParameter p0 = new OracleParameter();
            p0.ParameterName = "p0";
            p0.OracleDbType = OracleDbType.Varchar2;
            p0.Direction = ParameterDirection.Input;
            p0.Value = "Windshield";

            OracleParameter p1 = new OracleParameter();
            p1.ParameterName = "p1";
            p1.OracleDbType = OracleDbType.Int32;
            p1.Direction = ParameterDirection.Output;


            OracleParameter[] orap = new OracleParameter[] { p0, p1 };

            // mydal.ExecuteCommandNonQuery("proc_getproductsout", CommandType.StoredProcedure, false, p0, p1);
            //MessageBox.Show(mydal.ExecuteCommandNonQuery("proc_getproductsout", CommandType.StoredProcedure, false, p0, p1).ToString ());
            mydal.DoInsertDeleteUpdate("proc_getproductsout", CommandType.StoredProcedure, false, orap);
            MessageBox.Show(orap[1].Value.ToString());

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            DBParameter p0 = new DBParameter("p0", AHKDbType.Varchar2, ParameterDirection.Input, 40);
            DataTable mydt = new DataTable();

            mydt = mydal.GetDatatable("", CommandType.StoredProcedure, p0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable thedt = mydal.GetDatatable("select * from billing_master_payment", CommandType.Text);

                mydal2.DoBulkCopy("billing_master_payment", thedt);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }


    }
}
