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
        public StartingData Q;
        public DataTable dt;
        public string stationName;
        public Main(DataTable dt,string stationName, StartingData Q)
        {
            this.dt = dt;
            this.stationName = stationName;
            this.Q = Q;
            
        }
        public List<List<double>> getdlt()
        {
            List<List<double>> dltAB = new List<List<double>>();
            List<double> dltY = new List<double>();
            List<double>dltX = new List<double>();
            DataRow station=dt.Rows.Find(stationName);
            foreach(DataRow row in dt.Rows)
            {
                if (stationName.Equals(row[0].ToString()))
                {
                    dltX.Add(double.NaN);
                    dltY.Add(double.NaN);
                }
                else
                {
                    dltY.Add(Convert.ToDouble(row[1]) - Convert.ToDouble(station[1]));
                    dltX.Add(Convert.ToDouble(row[2]) - Convert.ToDouble(station[2]));
                }
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
        public List<double> getAzimuthRAD(List<List<double>> dltXY, List<double> atan)
        {
            //计算弧度方位角
            //List<List<double>> dltXY = getdlt();
            //List<double> atan = getatan(dltXY);
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
            //弧度转角度方位角
            List<double> DEG=new List<double>();
            foreach(double i in rad)
            {
                DEG.Add(i * 180 / Math.PI);
            }
            return DEG;
        }
        public List<double> getFXJ(List<double> DEG)
        {
            //方向值  设置第一个为已知点
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
        static public  string TranDegreeToDMs(double d)
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
        public List<double> getFXJObs(List<double> fxj)
        {
            List<double> Obs = new List<double>();
            double ra = 0.0;
            for(int i = 0; i < fxj.Count; i++)
            {
                if(double.IsNaN(fxj[i]))
                {
                    Obs.Add(double.NaN);
                }
                else if(fxj[i] == 0.0)
                {
                    Obs.Add(fxj[i]);
                }
                else
                {
                    ra = Rand(0, Q.azimuthError);
                    Obs.Add(fxj[i] + ra);  //模拟观测值  方向角+随机值
                }
            }
            return Obs;
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
        public List<double> getDistanceObs(List<double> dis)
        {
            List<double> disObs = new List<double>();
            double ra = 0.0;
            for (int i = 0; i < dis.Count; i++)
            {
                if (double.IsNaN(dis[i]))
                {
                    disObs.Add(dis[i]);
                }
                else
                {
                    ra = Rand(0, Q.a + Q.b * dis[i] / 1000);
                    disObs.Add(dis[i] + ra);  //模拟观测值  方向角+随机值
                }
            }
            return disObs;
        }
        public  static List<double> getRand(int n)
        {
            List<double> rand = new List<double>();
            for(int i = 0; i < n; i++)
            {
                rand.Add(Rand(1, 2));
            }
            return rand;
        }

        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static double Rand(double u, double d)
        {
            double u1, u2, z, x;
            //Random ram = new Random();
            if (d <= 0)
            {
                return u;
            }
            u1 = (new Random(GetRandomSeed())).NextDouble();
            u2 = (new Random(GetRandomSeed())).NextDouble();
            z = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            x = u + d * z;
            return x;

        }
    }
}
