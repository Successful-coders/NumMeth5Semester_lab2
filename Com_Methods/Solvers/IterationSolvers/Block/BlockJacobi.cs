using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    class BlockJacobi : IIterationSolver
    {
        public BlockJacobi(int MAX_ITER, double EPS)
        {
            IterrationIndex = 0;
            Eps = EPS;
            MaxIterationCount = MAX_ITER;
        }


        public BlockVector Solve(BlockMatrix matrixA, BlockVector vectorF)
        {
            double Norma_Xnew_Xold;

            Vector V_Help, V_SUM = new Vector(vectorF.Size_Block);

            Matrix M_Help;

            //для работы с диагональным блоком
            var LU_Solver = new LU_Decomposition();

            var RES = new BlockVector();
            RES.Size_Block = vectorF.Size_Block;
            RES.Block = new Vector[vectorF.N];
            RES.N = vectorF.N;

            for (int i = 0; i < RES.N; i++)
            {
                //выделяем место под элемент
                RES.Block[i] = new Vector(RES.Size_Block);

                //начальное приближение x(0)
                for (int k = 0; k < RES.Size_Block; k++)
                    RES.Block[i].Elem[k] = 0.0;
            }

            //итерация
            do
            {
                Norma_Xnew_Xold = 0;
                for (int i = 0; i < RES.N; i++)
                {
                    //инициализация суммы 
                    //инициализация F
                    for (int k = 0; k < RES.Size_Block; k++)
                        V_SUM.Elem[k] = vectorF.Block[i].Elem[k];

                    //произведение блоков на старые x

                    for (int j = 0; j < matrixA.N; j++)
                    {
                        //записывается значение ссылки, которая указывает на ij элемент
                        M_Help = matrixA.Block[i][j];
                        V_Help = RES.Block[j];

                        for (int Row = 0; Row < M_Help.M; Row++)
                            for (int Column = 0; Column < M_Help.N; Column++)
                                V_SUM.Elem[Row] -= M_Help.Elem[Row][Column] * V_Help.Elem[Column];
                    }

                    LU_Solver.LU = matrixA.Block[i][i];
                    //скобка, умноженная на обратный диагональный блок
                    V_SUM = LU_Solver.Start_Solver(LU_Solver.LU, V_SUM);

                    //формируем результат для i-ой компоненты
                    for (int k = 0; k < RES.Size_Block; k++)
                    {
                        double X_NEW = V_SUM.Elem[k];

                        //вычисляем норму
                        Norma_Xnew_Xold += Math.Pow((X_NEW - RES.Block[i].Elem[k]), 2);

                        RES.Block[i].Elem[k] = X_NEW;
                    }
                }

                Norma_Xnew_Xold = Math.Sqrt(Norma_Xnew_Xold);
                IterrationIndex++;

                //с каждой итерацией норма будет уменьшаться
                Console.WriteLine("Iter{0,-10} {1}", IterrationIndex, Norma_Xnew_Xold);
            }
            while (Norma_Xnew_Xold > Eps && IterrationIndex < MaxIterationCount);


            return RES;
        }


        public int MaxIterationCount { get; set; }
        public double Eps { set; get; }
        public int IterrationIndex { get; set; }
    }
}
