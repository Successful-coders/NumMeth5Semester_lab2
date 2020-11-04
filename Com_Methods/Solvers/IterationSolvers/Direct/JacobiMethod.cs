using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    class JacobiMethod : IIterationSolver
    {
        public JacobiMethod(int maxIterationCount, double eps)
        {
            MaxIterationCount = maxIterationCount;
            Eps = eps;
            IterrationIndex = 0;
        }


        public Vector Solve(Matrix matrixA, Vector vectorF)
        {
            double normX;
            
            var result = new Vector(vectorF.N);
            var resultNext = new Vector(vectorF.N);

            for (int i = 0; i < result.N; i++)
            {
                result.Elem[i] = 0.0d;
            }

            do
            {
                normX = 0;

                for (int i = 0; i < result.N; i++)
                {
                    double F_Ax = vectorF.Elem[i];

                    for (int j = 0; j < i; j++)
                    {
                        F_Ax -= matrixA.Elem[i][j] * result.Elem[j];
                    }

                    for (int j = i + 1; j < matrixA.N; j++)
                    {
                        F_Ax -= matrixA.Elem[i][j] * result.Elem[j];
                    }

                    resultNext.Elem[i] = F_Ax / matrixA.Elem[i][i];

                    normX += Math.Pow(result.Elem[i] - resultNext.Elem[i], 2);
                }

                result.Copy(resultNext);

                normX = Math.Sqrt(normX);
                IterrationIndex++;

                Console.WriteLine("Iter{0,-10} {1}", IterrationIndex, normX);
            }
            while (normX > Eps && IterrationIndex < MaxIterationCount);


            return result;
        }


        public int MaxIterationCount { get; set; }
        public double Eps { set; get; }
        public int IterrationIndex { get; set; }
    }
}
