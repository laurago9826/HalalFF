using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalalFF
{
    class Program
    {
        static void Main(string[] args)
        {
            //-----------------------SIMULATED ANNEALING-----------------------------
            //SimulatedAnnealing sa = new SimulatedAnnealing(20);
            //sa.CreatePoints();
            //Bitmap drawnPoints = sa.DrawPoints(Color.Red);
            //IntPoint result = sa.SimulatedAnnealingAlgorithm(15);
            //sa.DrawCircle(drawnPoints, result, Pens.Cyan);


            //drawnPoints.Save("image.jpg", ImageFormat.Jpeg);
            //ProcessStartInfo startinfo = new ProcessStartInfo("image.jpg")
            //{
            //    Verb = "edit"
            //};
            //Process.Start(startinfo);




            //------------------RANDOM OPTIMIZATION------------------
            Bitmap input = new Bitmap("randomOptimization.png");
            RandomOptimization ro = new RandomOptimization(input);
            var rect = ro.RandomOptimizationAlgorithm(6, 4, 253);
            Bitmap result = ColorPolygon(rect, input);


            result.Save("image2.jpg", ImageFormat.Jpeg);
            ProcessStartInfo startinfo = new ProcessStartInfo("image2.jpg")
            {
                Verb = "edit"
            };
            Process.Start(startinfo);




            //------------------HILL CLIMBING------------------
            //Bitmap input = new Bitmap("hillClimbing.jpg");
            //HillClimbing hc = new HillClimbing(input);
            //int resultThresh = ro.HillClimbingStochastic(3);
            //Bitmap result = ro.DoThreshold(resultThresh);

            //result.Save("image3.jpg", ImageFormat.Jpeg);
            //ProcessStartInfo startinfo = new ProcessStartInfo("image3.jpg")
            //{
            //    Verb = "edit"
            //};
            //Process.Start(startinfo);

        }

        public static Bitmap ColorPolygon(Rectangle rect, Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            Drawing.Rectangle(data, rect, Color.Red);
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
