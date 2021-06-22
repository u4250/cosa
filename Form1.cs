using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
namespace 科傻文件模拟生成
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ToolTip tooltip1 = new ToolTip();
            tooltip1.IsBalloon = true;
            tooltip1.SetToolTip(textBox1, "保存设定");
            tooltip1.SetToolTip(textBox6, "顾客将会看到你的公告，请不要乱写！");

        }
        private struct StartingData
        {
            public double azimuthError;
            public double a;
            public double b;
            public string endPointName;
            public string startingPointName;
            public double azimuth;
            public double startingPoint;

        };
        public DataTable dt=new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            //设置打开对话框的标题
            ofd.Title = "请选择要打开的文件";             
            //设置打开对话框可以多选
            ofd.Multiselect =false;
            //设置对话框打开的文件类型
            ofd.Filter = "文本文件|*.txt|Excel文件|*.xls;*.xlsx|dat文件|*.dat";
           //设置文件对话框当前选定的筛选器的索引
            ofd.FilterIndex = 3;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                string strCon = string.Empty;
                string path = ofd.FileName;
                string extension = Path.GetExtension(path);//扩展名 
                switch (extension)
                {
                    case ".dat":
                    case ".txt":
                        StreamReader sr = new StreamReader(path, Encoding.Default);
                        string line = sr.ReadLine();
                        if (line == null)
                        {
                            MessageBox.Show("文件为空！");
                            return;
                        }
                        try
                        {
                            string[] cols = line.ToString().Split(',');
                            foreach (var col in cols)
                            {
                                dt.Columns.Add(col);
                            }
                            while ((line = sr.ReadLine()) != null)
                            {
                                cols = line.ToString().Split(',');
                                dt.Rows.Add(cols);
                            }
                            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
                            this.dataGridView1.DataSource = dt;
                        }
                        catch
                        {
                            MessageBox.Show("格式错误!");
                        }
                        break;
                    case ".xls":
                        strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=Excel 8.0;";
                        break;
                    case ".xlsx":
                        strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=0;'";
                        break;
                }
                if (strCon == string.Empty)
                {
                    return;
                }
                OleDbConnection con = new OleDbConnection(strCon);
                //try
                //{
                con.Open();
                //默认读取第一个有数据的工作表
                var tables = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { });
                if (tables.Rows.Count == 0)
                { throw new Exception("Excel必须包含一个表"); }
                foreach (System.Data.DataRow row in tables.Rows)
                {
                    string strSheetTableName = row["TABLE_NAME"].ToString();
                    //过滤无效SheetName   
                    if (strSheetTableName.Contains("$") && strSheetTableName.Replace("'", "").EndsWith("$"))
                    {
                        DataTable tableColumns = con.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, strSheetTableName, null });
                        //if (tableColumns.Rows.Count < 2)                     //工作表列数
                        //    continue;
                        OleDbCommand cmd = new OleDbCommand("select * from [" + strSheetTableName + "]", con);
                        OleDbDataAdapter apt = new OleDbDataAdapter(cmd);
                        apt.Fill(dt);
                        dt.TableName = strSheetTableName.Replace("$", "").Replace("'", "");
                        break;
                    }
                }
                dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
                this.dataGridView1.DataSource = dt;

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.ToString());
                //}

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Columns.Count == 0)
            {
                MessageBox.Show("请先读取文件");
                return ;

            }
            //if (groupBox1.Controls.Cast<Control>().Where(T => T is TextBox)
            //             .Any(T => (T as TextBox).Text.Trim().Length == 0))
            //{
            //    MessageBox.Show("设置起算数据!");
            //    return;
            //}
            //StartingData Q = new StartingData();
            //Q.azimuthError= Convert.ToDouble(this.textBox1.Text);
            //Q.a = Convert.ToDouble(this.textBox6.Text);
            //Q.b = Convert.ToDouble(this.textBox7.Text);
            //Q.startingPoint =Convert.ToDouble(this.textBox3.Text);
            //Q.startingPointName = this.textBox2.Text.ToString();
            //Q.endPointName = this.textBox4.Text.ToString();
            //Q.azimuth= Convert.ToDouble(this.textBox5.Text);

            Main M = new Main(this.dt,"QZ1");
            M.getAzimuthRAD();

        }
    }
}
