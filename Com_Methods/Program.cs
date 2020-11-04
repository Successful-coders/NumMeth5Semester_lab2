using System;


namespace Com_Methods
{
    static class CONST
    {
        public static double EPS = 1e-8;
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var A = new BlockMatrix("Data\\System1\\", 100);
                var V = new BlockVector("Data\\System1\\", 100);

                var Jacobi = new BlockJacobi(3000, 1E-12);

                var RES = Jacobi.Solve(A, V);

                for (int i = 0; i < RES.N; i++)
                    for (int k = 0; k < RES.Size_Block; k++)
                        Console.WriteLine("x[{0}] = {1, -20}", i*RES.Size_Block +k +1, RES.Block[i].Elem[k]);
            }
            catch (Exception E)
            {
                Console.WriteLine("\n*** Error! ***");
                Console.WriteLine("Method:  {0}", E.TargetSite);
                Console.WriteLine("Message: {0}\n", E.Message);
            }
        }
    }
}
