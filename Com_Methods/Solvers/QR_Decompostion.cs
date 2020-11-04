using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    //QR-разложение
    class QR_Decompostion
    {
        public UpperTriangularMatrix R { set; get; }
        public LowerTriangularMatrix Q { set; get; }

        //классический метод QR-разложения
        public void QR_Decomposition_Classic_Gram_Schmidt_Process(Matrix A)
        {
            R = new UpperTriangularMatrix(A.N, A.N);
            Q = new LowerTriangularMatrix(A.M, A.M);
            var q_ = new Vector(A.M);

            for (int j = 0; j < A.N; j++)
            {
                //формирование верхнетреугольной матрицы R
                for (int i = 0; i < j; i++)
                    for (int k = 0; k < A.M; k++)                    
                        R.Elem[i][j] += A.Elem[k][j] * Q.Elem[k][i];

                //копирование j-ой строки матрицы A в вектор q_
                q_.Copy_Column(A, j);                                      

                for (int i = 0; i < j; i++)
                    for (int k = 0; k < q_.N; k++)
                        q_.Elem[k] -= Q.Elem[k][i] * R.Elem[i][j];

                //запись значения нормы вектора q_ в Rj,j элемент матрицы R
                R.Elem[j][j] = q_.Norma();

                if (R.Elem[j][j] < CONST.EPS)
                    return;

                //формирование унитарной матрицы Q
                for (int i = 0; i < A.M; i++)
                    Q.Elem[i][j] = q_.Elem[i] / R.Elem[j][j];
            }

        }

        //модифицированный метод QR-разложения
        public void QR_Decomposition_Modfied_Gram_Schmidt(Matrix A)
        {
            R = new UpperTriangularMatrix(A.N, A.N);
            Q = new LowerTriangularMatrix(A.M, A.M);
            var q_ = new Vector(A.M);

            for(int j = 0; j < A.M; j++)
            {
                //копирование j-ой строки матрицы A в вектор q_
                q_.Copy_Column(A,j);

                //формирование верхнетреугольной матрицы R
                for (int i = 0; i < j; i++)
                {
                    for(int k = 0; k < q_.N; k++)
                        // скалярное произведение
                        R.Elem[i][j] += q_.Elem[k] * Q.Elem[k][i];
                    

                    for(int k = 0; k < q_.N; k++)
                        q_.Elem[k] -= R.Elem[i][j] * Q.Elem[k][i];
                }

                //запись значения нормы вектора q_ в Rj,j элемент матрицы R
                R.Elem[j][j] = q_.Norma();

                if (R.Elem[j][j] < CONST.EPS)
                    return;

                //формирование унитарной матрицы Q
                for (int i = 0; i < A.M; i++)
                    Q.Elem[i][j] = q_.Elem[i] / R.Elem[j][j];
            }
        }

        //решение СЛАУ
        public Vector Start_Solver(Matrix A, Vector F)
        {
            var RES = Q.Multiplication_Trans_Matrix_Vector(F);
            RES = R.BackRowSubstitution(RES);

            return RES;
        }

        public void QR_Dec_Givens(Matrix A)
        {
            R = new UpperTriangularMatrix(A.M, A.N);
            Q = new LowerTriangularMatrix(A.M, A.N);
            double help1, help2;
            bool Flag_Diag = false;
            // заполним Q как единичную матрицу
            for (int i = 0; i < Q.M; i++)
                Q.Elem[i][i] = 1;
            // введем синус\косинус\тангенс
            double c = 1, s = 0, t = 0;
            for (int j = 0; j < A.N - 1; j++)
            {
                // если элемент нулевой
                if (Math.Abs(A.Elem[j][j]) < CONST.EPS)
                {
                    Flag_Diag = true;
                    c = 1;
                    s = 0;
                    t = 0;
                }
                // для каждой строки ниже диагонали
                for (int i = j + 1; i < A.M; i++)
                {
                    // если ненулевой элемент
                    if (Math.Abs(A.Elem[i][j]) > CONST.EPS)
                    {
                        if (!Flag_Diag)
                        {
                            t = A.Elem[i][j] / A.Elem[j][j];
                            c = 1.0 / (Math.Sqrt(1 + t * t));
                            s = t * c;
                        }
                        for (int k = j; k < A.N; k++)
                        {
                            help1 = c * A.Elem[j][k] + s * A.Elem[i][k];
                            help2 = c * A.Elem[i][k] - s * A.Elem[j][k];
                            R.Elem[j][k] = help1;
                            R.Elem[i][k] = help2;
                            // и в А
                            A.Elem[j][k] = help1;
                            A.Elem[i][k] = help2;
                        }
                        for (int k = 0; k < Q.M; k++)
                        {
                            help1 = c * Q.Elem[k][j] + s * Q.Elem[k][i];
                            help2 = c * Q.Elem[k][i] - s * Q.Elem[k][j];
                            Q.Elem[k][j] = help1;
                            Q.Elem[k][i] = help2;
                        }
                    }

                }
            }

        }
    }
}
