using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    class LowerTriangularMatrix: Matrix
    {
        //конструктор по умолчанию =
        public LowerTriangularMatrix() { }
        //конструктор по размерам
        public LowerTriangularMatrix(int m, int n)
        {
            M = m;
            N = n;
            Elem = new double[m][];
            for (int i = 0; i < m; i++) Elem[i] = new double[n];
        }

        //прямая подстановка по строкам
        public Vector DirectRowSubstitution (Vector X)
        {
            Vector RES = new Vector(X.N);
            //скопируем по значениям вектор X в RES
            RES.Copy(X);

            //проход по строкам
            for (int i = 0; i < X.N; i++)
            {
                if (Math.Abs(Elem[i][i]) < CONST.EPS) throw new Exception("Direct Row Substitution: division by 0...");

                for (int j = 0; j < i; j++)
                    RES.Elem[i] -= Elem[i][j] * RES.Elem[j];

                RES.Elem[i] /= Elem[i][i];
            }
            return RES;
        }

        //прямая подстановка по столбцам
        public Vector DirectColumnSubstitution(Vector X)
        {
            Vector RES = new Vector(X.N);
            //скопируем вектор X в RES
            RES.Copy(X);

            //проход по столбцам
            for (int j = 0; j < X.N; j++)
            {
                if (Math.Abs(Elem[j][j]) < CONST.EPS) throw new Exception("Direct Column Substitution: division by 0...");

                RES.Elem[j] /= Elem[j][j];

                for (int i = j + 1; i < X.N; i++)
                    RES.Elem[i] -= Elem[i][j] * RES.Elem[j];
            }
            return RES;
        }
    }
}
