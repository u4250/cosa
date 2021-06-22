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
            foreach (List<double> L in dltXY)
            {
                atan.Add(Math.Atan(L[0] / L[1]));
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
                if ( dltXY[0][j] > 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI - atan[j]); continue; }
                if ( dltXY[0][j] < 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI + atan[j]); continue; }
                if ( dltXY[0][j] < 0 &&  dltXY[1][j] > 0) { azimuthRAD.Add(Math.PI * 2 - atan[j]); continue; }
                if ( dltXY[0][j] < 0 &&  dltXY[1][j] > 0) { azimuthRAD.Add(Math.PI * 2 - atan[j]); continue; }
                if ( dltXY[0][j] == 0 &&  dltXY[1][j] > 0) { azimuthRAD.Add(0.0); continue; }
                if ( dltXY[0][j] == 0 &&  dltXY[1][j] < 0) { azimuthRAD.Add(Math.PI); continue; }
                if ( dltXY[0][j] < 0 &&  dltXY[1][j] == 0) { azimuthRAD.Add(Math.PI * 3 / 2); continue; }
                if ( dltXY[0][j] > 0 &&  dltXY[1][j] == 0) { azimuthRAD.Add(Math.PI * 2); continue; }


            }

            return azimuthRAD;
        }
    }
}
