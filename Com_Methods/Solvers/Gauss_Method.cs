using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    class Gauss_Method
    {
        //поиск ведущего элемента
        public static int Find_Name_Element(Matrix A, int j)
        {
            //рассматривается  j-ый столбец
            int max = j;

            for (int i = j + 1; i < A.M; i++)
                if (Math.Abs(A.Elem[max][j]) < Math.Abs(A.Elem[i][j]))
                    max = i;

            if (Math.Abs(A.Elem[max][j]) < CONST.EPS)
                throw new Exception("Gauss Method: degenerate matrix");

            return max;
        }

        //прямой ход с правой частью
        public void Direct_Way(Matrix A, Vector F)
        {
            double help;

            for (int i = 0; i < A.M - 1; i++)
            {
                //поиск ведущего элемента
                int I = Find_Name_Element(A, i);

                //если элемент не диагональный, то элементы меняются местами
                if (I != i)
                {
                    var Help = A.Elem[I];
                    A.Elem[I] = A.Elem[i];
                    A.Elem[i] = Help;

                    help = F.Elem[i];
                    F.Elem[i] = F.Elem[I];
                    F.Elem[I] = help;
                }

                //преобразование к верхнему треугольному виду матрицы A
                for (int j = i + 1; j < A.N; j++)
                {
                    help = A.Elem[j][i] / A.Elem[i][i];
                    A.Elem[j][i] = 0;

                    for (int k = i + 1; k < A.N; k++)
                        A.Elem[j][k] -= help * A.Elem[i][k];

                    F.Elem[j] -= help * F.Elem[i];
                }

            }
        }

        //прямой ход без правой части (для LU-разложения)
        public static void Direct_Way(Matrix A)
        {
            for (int i = 0; i < A.M - 1; i++)
            {
                //поиск ведущего элемента
                int I = Find_Name_Element(A, i);

                //если элемент не диагональный, то элементы меняются местами
                if (I != i)
                {
                    var Help = A.Elem[I];
                    A.Elem[I] = A.Elem[i];
                    A.Elem[i] = Help;
                }

                //изменяются поддиагольные элементы
                for (int j = i + 1; j < A.N; j++)
                {
                    var help = A.Elem[j][i] / A.Elem[i][i];
                    A.Elem[j][i] = 0;

                    for (int k = i + 1; k < A.N; k++)
                        A.Elem[j][k] -= help * A.Elem[i][k];
                }

            }
        }

        //решение СЛАУ
        public Vector Start_Solver(Matrix A, Vector F)
        {
            // вектор-решение
            var res = new Vector(F.N);
            // приведение к верхнему треугольному виду матрицы A, изменяя вместе с этим вектор F
            Direct_Way(A, F);

            // так как в Matrix нет метода для решения, то скопируем в другой класс
            var U = new UpperTriangularMatrix(A.M, A.N);
            for (int i = 0; i < A.M; i++)
                for (int j = i; j < A.N; j++)
                    U.Elem[i][j] = A.Elem[i][j];

            // решение СЛАУ обратным ходом по столбцам
            res = U.BackColumnSubstitution(F);
            return res;

        }
    }
}
