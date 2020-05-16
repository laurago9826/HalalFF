using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Statistics.Visualizations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalalFF
{
    class HillClimbing
    {
        private const int maxIterations = 50;
        private Random rand = new Random();
        private BottomHat BottomHat = new BottomHat();
        private TopHat TopHat = new TopHat();
        public Bitmap GrayImage { get; set; }

        public HillClimbing(Bitmap img)
        {
            GrayImage = new Grayscale(1, 1, 1).Apply(img);
        }

        public Bitmap DoThreshold(int threshold)
        {
            return new Threshold(threshold).Apply(GrayImage);
        }

        public int HillClimbingStochastic(int Ethresh)
        {
            int p = rand.Next(50, 150);
            for (int i = 0; i < maxIterations; i++)
            {
                int q = p + (rand.Next(2) % 2 == 1 ? -Ethresh : Ethresh);
                if (Fitness(q) < Fitness(p))
                {
                    p = q;
                }
            }
            return p;
        }

        public double Fitness(int thresh)
        {
            ImageStatistics origstat = new ImageStatistics(GrayImage);
            Histogram orighist = origstat.Gray;
            Bitmap thresholded = new Threshold(thresh).Apply(GrayImage);
            ImageStatistics stat1 = new ImageStatistics(thresholded);
            Histogram hist1 = stat1.Gray;
            Bitmap bottomHat = BottomHat.Apply(thresholded);
            ImageStatistics stat2 = new ImageStatistics(bottomHat);
            Histogram hist2 = stat2.Gray;
            Bitmap topHat = TopHat.Apply(thresholded);
            ImageStatistics stat3 = new ImageStatistics(topHat);
            Histogram hist3 = stat3.Gray;
            double whiteToAllPercent = 1 - (Math.Abs((double)hist1.Values[255] - hist1.TotalCount/ 2)) / (hist1.TotalCount/2);
            double bottomHatWhiteToAllWhite = hist1.Values[255] == 0 ? 0 : 1 - (double)hist2.Values[255] / hist1.Values[255];
            double topHatWhiteToAllWhite = hist1.Values[255] == 0 ? 0 : 1 - (double)hist3.Values[255] / hist1.Values[255];
            double neighbourValues = 1 - (double)(orighist.Values[thresh - 1] + orighist.Values[thresh] + orighist.Values[thresh + 1]) / hist1.TotalCount * 10;
            double fitness = whiteToAllPercent / 4 + bottomHatWhiteToAllWhite / 4 + topHatWhiteToAllWhite / 4 + neighbourValues / 4;
            return -fitness;
        }
    }
}
