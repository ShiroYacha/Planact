using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planact.Common
{
    public static class MathExtension
    {
        public static Tuple<double,double> LeastSquareLinearRegression(double[] xdata, double[] ydata)
        {
            // build matrices
            var X = DenseMatrix.OfColumnVectors(
              new[] { DenseVector.Create(xdata.Length, 1), new DenseVector(xdata) });
            var y = new DenseVector(ydata);

            // solve using QR
            var p = X.QR().Solve(y);
            return new Tuple<double, double>(p[0],p[1]);
        }

        public static double NextGaussian(this Random rng, double mean, double std)
        {
            double u1 = rng.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
    }
}
