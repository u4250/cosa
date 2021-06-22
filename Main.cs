using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace 科傻文件模拟生成
{
    class Main
    {
        public DataTable dt;
        public string stationName;
        public Main(DataTable dt,string stationName)
        {
            this.dt = dt;
            this.stationName = stationName;
        }
        public List<List<double>> getdlt()
        {
            List<List<double>> dltAB = new List<List<double>>();
            List<double> dltY = new List<double>();
            List<double>dltX = new List<double>();
            DataRow station=dt.Rows.Find(stationName);
            foreach(DataRow row in dt.Rows)
            {
                //if (row[0].ToString() != stationName)
                //{

                //}
                dltY.Add(Convert.ToDouble(row[1]) - Convert.ToDouble(station[1]));
                dltX.Add(Convert.ToDouble(row[2]) - Convert.ToDouble(station[2]));
            }
           
            dltAB.Add(dltY);
            dltAB.Add(dltX);
            return dltAB;
        }

        public List<double> getatan(List<List<double>> dltXY)
        {
  
            List<double> atan=new List<double>();
            for (int j = 0; j < dltXY[0].Count; j++)
            {
                atan.Add(Math.Atan(dltXY[0][j] / dltXY[1][j]));
            }
            return atan;
        }
        public List<double> getAzimuthRAD()
        {
            List<List<double>> dltXY = getdlt();
            List<double> atan = getatan(dltXY);
            List<double> azimuthRAD = new List<double>();
            for(int j=0;j<atan.Count;j++)
            {
                if ( dltXY[0][j] > 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI + atan[j]); continue; }
                if ( dltXY[0][j] < 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI + atan[j]); continue; }
                if (dltXY[0][j] < 0 && dltXY[1][j] > 0) { azimuthRAD.Add(Math.PI * 2 + atan[j]); continue; }
                //if ( dltXY[0][j] < 0 &&  dltXY[1][j] > 0) { azimuthRAD.Add(Math.PI * 2 + atan[j]); continue; }
                //if ( dltXY[0][j] == 0 &&  dltXY[1][j] > 0) { azimuthRAD.Add(0.0); continue; }
                //if ( dltXY[0][j] == 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI); continue; }
                //if ( dltXY[0][j] < 0 &&  dltXY[1][j] == 0) { azimuthRAD.Add(Math.PI * 3 / 2); continue; }
                //if (dltXY[0][j] > 0 && dltXY[1][j] == 0) { azimuthRAD.Add(Math.PI * 2); continue; }
                else azimuthRAD.Add(atan[j]);


            }

            return azimuthRAD;
        }

        public List<double> radToDEG(List<double> rad)
        {
            List<double> DEG=new List<double>();
            foreach(double i in rad)
            {
                DEG.Add(i * 180 / Math.PI);
            }
            return DEG;
        }
        public List<double> getFXJ(List<double> DEG)//方向角  设置第一个为已知点
        {
            double initAngle;
            List<double> FXJ = new List<double>();
            if (!double.IsNaN(DEG[0]))
            {
                initAngle = DEG[0];
            }
            else
            {
                initAngle = DEG[1];
            }
            for (int i = 0; i < DEG.Count; i++)
            {
                if (!double.IsNaN(DEG[i]))
                {
                   double c= DEG[i]- initAngle;
                    if (c < 0)
                    {
                        c += 360;
                    }
                    FXJ.Add(c);
                }
                else
                {
                    FXJ.Add(double.NaN);
                }
            }
            return FXJ;

        }
        public  string TranDegreeToDMs(double d)
        {
            int Degree = Convert.ToInt16(Math.Truncate(d));//度
            d = d - Degree;
            int M = Convert.ToInt16(Math.Truncate((d) * 60));//分
            int S = Convert.ToInt16(Math.Round((d * 60 - M) * 60));
            if (S == 60)
            {
                M = M + 1;
                S = 0;
            }
            if (M == 60)
            {
                M = 0;
                Degree = Degree + 1;
            }
            string rstr = Degree.ToString() + ".";
            if (M < 10)
            {
                rstr = rstr + "0" + M.ToString();
            }
            else
            {
                rstr = rstr + M.ToString();
            }
            if (S < 10)
            {
                rstr = rstr + "0" + S.ToString();
            }
            else
            {
                rstr = rstr + S.ToString();
            }
            return rstr;
        }
        public List<double> getDistance(List<List<double>> dltXY)
        {
            List<double> distance = new List<double>();
            for (int j = 0; j < dltXY[0].Count; j++)
            {
                distance.Add(Math.Pow((Math.Pow(dltXY[0][j],2) + Math.Pow(dltXY[1][j],2)),0.5));
            }
            return distance;
        }
        }
}
