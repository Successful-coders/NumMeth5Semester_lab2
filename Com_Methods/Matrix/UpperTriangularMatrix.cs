using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    //верхняя треугольная матрица
    class UpperTriangularMatrix: Matrix
    {
        //конструктор по умолчанию
        public UpperTriangularMatrix() { }
        //конструктор по размерам
        public UpperTriangularMatrix(int m, int n)
        {
            M = m;
            N = n;
            Elem = new double[m][];
            for (int i = 0; i < m; i++) Elem[i] = new double[n];
        }

        //прямая подстановка по строкам
        public Vector BackRowSubstitution(Vector X)
        {
            Vector RES = new Vector(X.N);
            //скопируем вектор X в RES
            RES.Copy(X);

            //начинаем с последней строки, двигаясь вверх
            for (int i = X.N - 1; i >= 0; i--)
            {
                if (Math.Abs(Elem[i][i]) < CONST.EPS) throw new Exception("Back Row Substitution: division by 0... ");

                //двигаемся по столбцам
                for (int j = i + 1; j < X.N; j++)
                    RES.Elem[i] -= Elem[i][j] * RES.Elem[j];

                RES.Elem[i] /= Elem[i][i];
            }
            return RES;
        }

        //прямая подстановка по столбцам
        public Vector BackColumnSubstitution(Vector X)
        {
            Vector RES = new Vector(X.N);
            //скопируем вектор X в RES
            RES.Copy(X);

            //начинаем с последнего столбца, сдвигаясь влево
            for (int j = X.N - 1; j >= 0; j--)
            {
                if (Math.Abs(Elem[j][j]) < CONST.EPS) throw new Exception("Back Column Substitution: division by 0...");

                RES.Elem[j] /= Elem[j][j];

                //двигаемся по строкам
                for (int i = j - 1; i >= 0; i--)
                    RES.Elem[i] -= Elem[i][j] * RES.Elem[j];
            }
            return RES;
        }
    }
}
