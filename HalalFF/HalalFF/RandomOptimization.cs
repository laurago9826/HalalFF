using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using Accord.Math.Geometry;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Visualizations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HalalFF
{
    //feladat: egy megadott színű objektumba berajzolni a lehető legnagyobb négyzetet.
    //3 dimenzió: mozgatjuk a négyzet középpontját, méretét és elforgatási szögét
    //Megoldottnak számít  feladat, ha a négyzeten belül az összes pont a bemeneti színnel azonos
    class RandomOptimization
    {
        private const int maxIterations = 5000;
        public static Random rand = new Random();
        public Bitmap GreyImage { get; set; }
        public RandomOptimization(Bitmap bmp)
        {
            this.GreyImage = new Grayscale(1,1,1).Apply(bmp);
        }

        //keresési tér 
        public Rectangle RandomOptimizationAlgorithm(int Epoint, int Esize, byte intensity)
        {
            IntPoint pPoint = FindStartingPoint(intensity);
            int pSize = rand.Next(GreyImage.Width / 8, GreyImage.Width / 4);
            MyRectangle pRect = new MyRectangle(pPoint, pSize);
            for (int i = 0; i < maxIterations; i++)
            {
                MyRectangle qRect = pRect.MoveRectangle(Epoint, Esize);
                if (Fitness(qRect, intensity) <= Fitness(pRect, intensity))
                {
                    pRect = qRect;
                }
            }
            return pRect.Rectangle;
        }
        
        private double Fitness(MyRectangle rect, byte intensity)
        {
            Crop crop = new Crop(rect.Rectangle);
            var cropped = crop.Apply(GreyImage);
            ImageStatistics stat = new ImageStatistics(crop.Apply(GreyImage));
            Histogram hist = stat.Gray;
            double sameColorRatio = (double)hist.Values[intensity] / hist.TotalCount;
            return sameColorRatio == 1 ? -rect.Size : - sameColorRatio;
        }


        public IntPoint FindStartingPoint(byte intensity)
        {
            Bitmap bmp = GreyImage;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            byte[] data_orig = new byte[Math.Abs(stride * bmData.Height)];
            Marshal.Copy(Scan0, data_orig, 0, data_orig.Length);
            int offset = stride - bmp.Width;
            IntPoint randomPoint = new IntPoint(rand.Next(bmp.Width), rand.Next(bmp.Height));
            while (data_orig[randomPoint.Y * bmp.Width + randomPoint.X + randomPoint.Y * offset] != intensity)
            {
                randomPoint = new IntPoint(rand.Next(bmp.Width), rand.Next(bmp.Height));
            }
            bmp.UnlockBits(bmData);
            return randomPoint;
        }


        class MyRectangle
        {
            public IntPoint Center { get; set; }
            public int Size { get; set; }
            public Rectangle Rectangle {get;set;}

            public MyRectangle(IntPoint center, int size)
            {
                this.Center = center;
                this.Size = size;
                CreateRectangle();
            }

            public MyRectangle MoveRectangle(int Epoint, int Esize)
            {
                double stddev = 0.5;
                NormalDistribution normal = new NormalDistribution(0, stddev);
                var randomNumber = rand.Next(3);
                var x = ((double)rand.Next((int)(stddev * 3 * 10000 * 2)) - (10000 * stddev * 3)) / 10000;
                if (randomNumber % 3 == 0)
                {
                    var numbX = (int)Math.Round(normal.DistributionFunction(x) * Epoint);
                    int shiftX = rand.Next(2) % 2 == 1 ? -numbX : numbX;
                    return new MyRectangle(new IntPoint(this.Center.X + shiftX, this.Center.Y), this.Size);
                }
                else if(randomNumber % 3 == 1)
                {
                    var numbY = (int)Math.Round(normal.DistributionFunction(x) * Epoint);
                    int shiftY = rand.Next(2) % 2 == 1 ? -numbY : numbY;
                    return new MyRectangle(new IntPoint(this.Center.X, this.Center.Y + shiftY), this.Size);
                }
                else
                {
                    var numbSize = (int)Math.Round(normal.DistributionFunction(x) * Esize);
                    int sizeDiff = rand.Next(2) % 2 == 1 ? -numbSize : numbSize;
                    return new MyRectangle(Center, this.Size + sizeDiff);
                }
               
            }

            public void CreateRectangle()
            {
                int rad = Size / 2;
                List<IntPoint> rect = new List<IntPoint>()
                {
                    new IntPoint(Center.X + rad, Center.Y + rad),
                    new IntPoint(Center.X + rad, Center.Y - rad),
                    new IntPoint(Center.X - rad, Center.Y + rad),
                    new IntPoint(Center.X - rad, Center.Y - rad)
                };
                this.Rectangle = new Rectangle(Center.X - rad, Center.Y + rad, Size, Size);
            }
        }
    }
}
