using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 科傻文件模拟生成
{
     struct StartingData
    {
        public double azimuthError;
        public double a;
        public double b;
        public string endPointName;
        public string startingPointName;
        public double azimuth;
        public double startingPoint;

    };
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
