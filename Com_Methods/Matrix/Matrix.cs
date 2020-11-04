using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace Com_Methods
{
    public interface IMatrix
    {
        //размер матрицы
        int M { set; get; } //строки
        int N { set; get; } //столбцы
    }

    //класс матрицы
    //атрибут Serializable разрешает сериализацию экзепляров данного класса
    //сериализация будет нужна при тестировании ваших программ
    [Serializable]
    class Matrix : IMatrix
    {
        //размер матрицы
        public int M { set; get; }
        public int N { set; get; }
        //элементы матрицы
        public double[][] Elem { set; get; }

        //конструктор по умолчанию
        public Matrix(){}

        //конструктор нуль-матрицы m X n
        public Matrix(int m, int n)
        {
            N = n; M = m;
            Elem = new double[m][];
            for (int i = 0; i < m; i++) Elem[i] = new double[n];
        }

        //умножение на скаляр с выделением памяти под новую матрицу
        public static Matrix operator *(Matrix T, double Scal)
        {
            Matrix RES = new Matrix(T.M, T.N);

            for (int i = 0; i < T.M; i++)
            {
                for (int j = 0; j < T.N; j++)
                {
                    RES.Elem[i][j] = T.Elem[i][j] * Scal;
                }
            }
            return RES;
        }

        //умножение на скаляр, результат запишется в исходную матрицу
        public void Dot_Scal (double Scal)
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Elem[i][j] *= Scal;
                }
            }
        }

        //умножение матрицы на вектор
        public static Vector operator *(Matrix T, Vector V)
        {
            if (T.N != V.N) throw new Exception("M * V: dim(Matrix) != dim(Vector)...");

            Vector RES = new Vector(T.M);

            for (int i = 0; i < T.M; i++)
            {
                for (int j = 0; j < T.N; j++)
                {
                    RES.Elem[i] += T.Elem[i][j] * V.Elem[j];
                }
            }
            return RES;
        }

        //умножение транспонированной матрицы на вектор
        public Vector Multiplication_Trans_Matrix_Vector (Vector V)
        {
            if (N != V.N) throw new Exception("Mt * V: dim(Matrix) != dim(Vector)...");

            Vector RES = new Vector(M);

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    RES.Elem[i] += Elem[j][i] * V.Elem[j];
                }
            }
            return RES;
        }

        //умножение матрицы на матрицу
        public static Matrix operator *(Matrix T1, Matrix T2)
        {
            if (T1.N != T2.M) throw new Exception("M * M: dim(Matrix1) != dim(Matrix2)...");

            Matrix RES = new Matrix(T1.M, T2.N);

            for (int i = 0; i < T1.M; i++)
            {
                for (int j = 0; j < T2.N; j++)
                {
                    for (int k = 0; k < T1.N; k++)
                    {
                        RES.Elem[i][j] += T1.Elem[i][k] * T2.Elem[k][j];
                    }
                }
            }
            return RES;
        }

        //сложение матриц с выделением памяти под новую матрицу
        public static Matrix operator +(Matrix T1, Matrix T2)
        {
            if (T1.M != T2.M || T1.N != T2.N) throw new Exception("dim(Matrix1) != dim(Matrix2)...");

            Matrix RES = new Matrix(T1.M, T2.N);

            for (int i = 0; i < T1.M; i++)
            {
                for (int j = 0; j < T2.N; j++)
                {
                    RES.Elem[i][j] = T1.Elem[i][j] + T2.Elem[i][j];
                }
            }
            return RES;
        }

        //сложение матриц без выделение памяти под новую матрицу (добавление в ту же матрицу)
        public void Add(Matrix T2)
        {
            if (M != T2.M || N != T2.N) throw new Exception("dim(Matrix1) != dim(Matrix2)...");

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < T2.N; j++)
                {
                    Elem[i][j] += T2.Elem[i][j];
                }
            }
        }

        //транспонирование матрицы
        public Matrix Transpose_Matrix()
        {
            var RES = new Matrix(N, M);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    RES.Elem[i][j] = Elem[j][i];
                }
            }
            return RES;
        }
                
        //вывод матрицы на консоль
        public void Console_Write_Matrix()
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                    Console.Write(String.Format("{0, -20}", Elem[i][j]));
                Console.WriteLine();
            }
        }

        //Объект, ссылающийся на некоторую функцию
        delegate void Threads_Solving(int Number);
        
        //Вычисление числа обусловленности
        public double Cond_Square_Matrix_Parallel()
        {
            if (M != N) throw new Exception("Cond_Square_Matrix: matrix doesn't square");

            //решатель СЛАУ
            var LU_Solver = new LU_Decomposition();
            LU_Solver.CreateLU(this.Transpose_Matrix());
            
            //число допустимых виртуальных ядер
            int Number_Threads = Environment.ProcessorCount;

            //семафор для потоков(используется список)
            var Semaphors = new List<bool>();

            //норма строк (разделяется по потокам)
            var Norma_Row_A = new double[Number_Threads];
            var Norma_Row_A1 = new double[Number_Threads];
            
            //вход в параллельную секцию
            //объявление объекта типа делегат
            //для создания объекта в параметрах конструктора передаётся анонимный метод
            var Thread_Solver = new Threads_Solving(Number =>
            //лямбда-выражение
            {
                    //строк для отображения матрицы
                    //в каждом потоке хранится своя строка матрицы
                    var A1 = new Vector(M);
                    double S1, S2;

                    //индексы указывающие на первую и последнюю строку для потока
                    int Begin = N / Number_Threads * Number;
                    int End = Begin + N / Number_Threads;

                    //если деление было нецелочисленное, записываем в конец остаток
                    if (Number + 1 == Number_Threads)
                        End += N % Number_Threads;

                    //решение СЛАУ для End-Begin строк
                    for (int i = Begin; i < End; i++)
                    {
                        A1.Elem[i] = 1.0;
                        A1 = LU_Solver.Start_Solver(LU_Solver.LU,A1);

                        //нормы S1, S2
                        S1 = 0;
                        S2 = 0;

                        for (int j = 0; j < M; j++)
                        {
                            S1 += Math.Abs(Elem[i][j]);
                            S2 += Math.Abs(A1.Elem[j]);
                            A1.Elem[j] = 0.0;
                        }

                        //определение наибольшей из норм
                        if (Norma_Row_A[Number] < S1)
                            Norma_Row_A[Number] = S1;

                        if (Norma_Row_A1[Number] < S2)
                            Norma_Row_A1[Number] = S2;
                    }

                    //сигнал о завершении потока
                    Semaphors[Number] = true;
             });

            //отцовский поток вызывает дочерние
            for (int I = 0; I < Number_Threads; I++)
            {
                int Number = I;
                Semaphors.Add(false);

                //пул(очередь) потоков
                ThreadPool.QueueUserWorkItem(Param => Thread_Solver(Number));
            }

            //просмотр списка семафора на наличие незавершённых потоков
            while (Semaphors.Contains(false)) ;

            for (int i = 1; i < Number_Threads; i++)
            {
                if (Norma_Row_A[0] < Norma_Row_A[i])
                    Norma_Row_A[0] = Norma_Row_A[i];

                if (Norma_Row_A1[0] < Norma_Row_A1[i])
                    Norma_Row_A1[0] = Norma_Row_A1[i];
            }

            return Norma_Row_A[0] * Norma_Row_A1[0];
        }

        //конструктор с чтением из бинарного файла
        public Matrix(string PATH)
        {
            //PATH + "Size.bin" - путь файла для чтения
            //FileMode - для открытия
            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "Size.bin", FileMode.Open)))
            {
                M = Reader.ReadInt32();
                N = M;
            }

            Elem = new double[M][];

            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "Matrix.bin", FileMode.Open)))
            {
                try
                {
                    for (int i = 0; i < M; i++)
                    {
                        Elem[i] = new double[N];
                        for (int j = 0; j < N; j++)
                            Elem[i][j] = Reader.ReadDouble();
                    }
                }
                catch { throw new Exception("Matrix: data file is not correct..."); }
            }
        }
    }
}