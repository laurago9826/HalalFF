using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalalFF
{
    //feladat: legenerálunk 100 pontot egy 100x100-as környezeten belül (100x100-as kép), a feladat megkeresni azt az R sugarúk középpontját, 
    //ami a legtöbb pontot tartalmazza
    class SimulatedAnnealing
    {
        private const int n = 100;
        private const int pointsNum = 20;
        private const int maxIterations = 1000;
        private const int T0 = 1000;
        private const double alfa = 2;
        public const double kB = 2e-3;
        public static Random rand = new Random();
        public List<IntPoint> Points { get; private set; }

        public double Radius { get; private set; }

        public SimulatedAnnealing(double radius)
        {
            Radius = radius;
            Points = new List<IntPoint>();
        }
        public void CreatePoints()
        {
            for (int i = 0; i < pointsNum; i++)
            {
                Points.Add(new IntPoint(rand.Next(n), rand.Next(n)));
            }
        }

        public Bitmap DrawPoints(Color color)
        {
            Bitmap bmp = new Bitmap(n, n, PixelFormat.Format24bppRgb);
            PointsMarker pm = new PointsMarker(Points, color);
            return pm.Apply(bmp);
        }

        public IntPoint SimulatedAnnealingAlgorithm(int epsylon)
        {
            //S - keresési tér(100x100-as kép akármelyik pontja)
            IntPoint p = new IntPoint(rand.Next(n), rand.Next(n));
            int fp = Fitness(p);
            IntPoint popt = p;
            int fpopt = fp; 
            for (int t = 0; t < maxIterations; t++)
            {
                IntPoint q = newQ(p, epsylon);
                int fq = Fitness(q);
                int eDiff = fq - fp; 
                if (eDiff < 0)
                {
                    p = q;
                    fp = fq;
                    if (fp < fpopt)
                    {
                        popt = p;
                        fpopt = fp;
                    }
                }
                else
                {
                    double T = Temperature(t);
                    double P = Math.Pow(Math.E, -(double)eDiff / (kB * T));
                    if ((double)rand.Next(1001) / 1000 < P)
                    {
                        p = q;
                        fp = fq;
                    }
                }
                t++;
            }
            return popt;
        }


        public IntPoint newQ(IntPoint p, int epsylon)
        {
            int newX = Math.Min(Math.Max(0, p.X + rand.Next((epsylon + 1) * 2) - epsylon), n);
            int newY = Math.Min(Math.Max(0, p.Y + rand.Next((epsylon + 1) * 2) - epsylon), n);
            return new IntPoint(newX, newY);
        }

        //összes pont - hány pont található a körön belül
        public int Fitness(IntPoint actual)
        {
            return Points.Count() - Points.Where(x => x.DistanceTo(actual) <= Radius).Count();
        }

        public double Temperature(int t)
        {

            return (double)T0 * Math.Pow((1 - (double)t / maxIterations), alfa);
        }


        public void DrawCircle(Bitmap bmp, IntPoint center, Pen pen)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawEllipse(pen, center.X - (int)Radius, center.Y- (int)Radius, (int)Radius * 2, (int)Radius * 2);
        }
    }
}
