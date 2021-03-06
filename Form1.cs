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
            ReadLog("程序启动。。。");
        }
        public void ReadLog(string log)
        {
            string Time = Convert.ToString(DateTime.Now);
            textBox8.AppendText(Time + "  " + log + "\r\n");
        }
        public DataTable dt = new DataTable();
        public DataTable resDt=null;
        public string fileName;
        private void button1_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            //设置打开对话框的标题
            ofd.Title = "请选择要打开的文件";
            //设置打开对话框可以多选
            ofd.Multiselect = false;
            //设置对话框打开的文件类型
            ofd.Filter = "文本文件|*.txt|Excel文件|*.xls;*.xlsx|dat文件|*.dat";
            //设置文件对话框当前选定的筛选器的索引
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ReadLog("正在读取数据。。。");
                this.fileName = Path.GetFileNameWithoutExtension(ofd.SafeFileName);
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
                            ReadLog("文件为空。。。。");
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
                            dt.Columns.RemoveAt(0);
                            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
                            ReadLog("文件读取成功。。。");
                            this.dataGridView1.DataSource = dt;
                        }
                        catch
                        {
                            ReadLog("读取失败，格式错误！");
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
                try
                {
                    con.Open();
                    //默认读取第一个有数据的工作表
                    var tables = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { });
                    if (tables.Rows.Count == 0)
                    { throw new Exception("Excel必须包含一个表"); }
                    foreach (DataRow row in tables.Rows)
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
                            con.Close();
                            break;
                        }
                    }
                    dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
                    this.dataGridView1.DataSource = dt;
                    ReadLog("文件读取成功。。。");
                }
                catch (Exception ex)
                {
                    con.Close();
                    ReadLog(ex.ToString());
                    MessageBox.Show("文件读取失败");
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            StartingData Q = new StartingData();
            if (this.dataGridView1.Columns.Count == 0)
            {
                MessageBox.Show("请先读取文件");
                return;
            }
            if (groupBox1.Controls.Cast<Control>().Where(T => T is TextBox)
                         .Any(T => (T as TextBox).Text.Trim().Length == 0))
            {
                DialogResult dr = MessageBox.Show("不设置起算数据将采用示例默认数据进行计算", "提示", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    Q.azimuthError = 1.28;
                    Q.a = 3.67;
                    Q.b = 2;
                    Q.startingPointX = "2330";
                    Q.startingPointY = "5330";
                    Q.endPointName = "QZ2";
                    Q.azimuth = 30.30;
                    Q.startingPointName = "QZ1";
                    this.textBox1.Text = "1.28";
                    this.textBox6.Text = "3.67";
                    this.textBox7.Text = "2";
                    this.textBox3.Text = "2330";
                    this.textBox9.Text = "5330";
                    this.textBox2.Text = "QZ1";
                    this.textBox4.Text = "QZ2";
                    this.textBox5.Text = "30.303";
                    ReadLog("采用默认值开始计算！");
                }
                else
                {
                    return;
                }
                //MessageBox.Show("不设置起算数据将采用示例默认数据进行计算");

            }
            else
            {
                try
                {
                    Q.azimuthError = Convert.ToDouble(this.textBox1.Text);
                    Q.a = Convert.ToDouble(this.textBox6.Text);
                    Q.b = Convert.ToDouble(this.textBox7.Text);
                    Q.startingPointX = this.textBox3.Text;
                    Q.startingPointY = this.textBox9.Text;
                    Q.startingPointName = this.textBox2.Text.ToString();
                    Q.endPointName = this.textBox4.Text.ToString();
                    Q.azimuth = Convert.ToDouble(this.textBox5.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("输入有误，检查后重新输入！");
                    ReadLog(ex.ToString());
                }
            }
            resDt = new DataTable();
            DataColumn dc = new DataColumn();
            resDt.Columns.Add(dc);
            resDt.Columns.Add(new DataColumn());
            resDt.Columns.Add(new DataColumn());
            //写入文件通用前两行
            resDt.Rows.Add((new List<double> { Q.azimuthError, Q.a, Q.b }).ConvertAll<string>(x => x.ToString()).ToArray());
            string[] R = new string[] { Q.startingPointName, Q.startingPointX, Q.startingPointY };
            resDt.Rows.Add(R);
            bool Flag = true;
            ReadLog("正在计算*************");
            foreach (DataRow row in dt.Rows)
            {
                string stationName = row[0].ToString();
                ReadLog("正在计算******" + stationName + "站*******");
                resDt.Rows.Add(new List<object> { stationName }.ConvertAll<string>(x => x.ToString()).ToArray());
                Main M = new Main(this.dt, stationName, Q);
                List<List<double>> dlt = M.getdlt();
                List<double> atan = M.getatan(dlt);
                List<double> azimuthRAD = M.getAzimuthRAD(dlt, atan);
                List<double> azimuthDEG = M.radToDEG(azimuthRAD);
                List<double> FXZ = M.getFXJ(azimuthDEG);
                List<double> FXZObs = M.getFXJObs(FXZ);//角度模拟观测值 ***************
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].Equals(Q.endPointName) && Flag)
                    {
                        resDt.Rows.Add(new List<string> { Q.endPointName, "A", Q.azimuth.ToString() }.ToArray());
                        Flag = false;
                    }
                    if (dt.Rows[i][0].Equals(stationName))
                    {
                        continue;
                    }
                    else
                    {
                        resDt.Rows.Add(new List<object> { dt.Rows[i][0], "L", FXZObs[i] }.ConvertAll<string>(x => x.ToString()).ToArray());
                    }
                }
                List<double> dis = M.getDistance(dlt);
                List<double> disObs = M.getDistanceObs(dis);//距离模拟观测值 ***************
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].Equals(stationName))
                    {
                        continue;
                    }
                    resDt.Rows.Add(new List<object> { dt.Rows[i][0], "S", disObs[i] }.ConvertAll<string>(x => x.ToString()).ToArray());
                }
            }
            this.dataGridView2.DataSource = resDt;
            ReadLog("完成计算*************");
            //M.getFXJ(M.radToDEG(M.getAzimuthRAD()));
            //M.getFXJ(M.radToDEG(M.getAzimuthRAD()));
            //M.getDistance(M.getdlt());
            //M.getatan();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (resDt == null)
            {
                MessageBox.Show("计算后才能保存！");
                return;
            }
            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter = "平面网(*.in2)|*.in2";
            sa.FileName = this.fileName;
            sa.DefaultExt = "in2";
            sa.AddExtension = true;
            if (sa.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ReadLog("正在写入" + sa.FileName);
                    writer(sa.FileName);
                    ReadLog("保存成功！");
                }
                catch (Exception ex)
                {
                    ReadLog(ex.ToString());
                    MessageBox.Show("写入出错啦！");
                }
            }
        }
        public void writer(string path)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
            foreach (DataRow line in this.resDt.Rows)
            {
                string Temp = "";
                for (int i = 0; i < this.resDt.Columns.Count; i++)
                {
                    string cell = line[i].ToString();
                    if (!string.IsNullOrEmpty(cell))
                    {
                        Temp = Temp + cell + ',';
                    }
                }
                sw.WriteLine(Temp);
            }
            sw.Close();
            sw.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach(Control c in groupBox1.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
            }
        }
    }
}
