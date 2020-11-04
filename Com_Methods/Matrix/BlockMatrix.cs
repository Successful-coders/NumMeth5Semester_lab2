using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    class BlockMatrix : IMatrix
    {
        public int M { set; get; }
        public int N { set; get; }
        public int Size_Block { set; get; }
        public Matrix[][] Block { set; get; }


        public BlockMatrix() { }
        public BlockMatrix(string PATH, int SIZE_BLOCK)
        {
            //PATH + "Sizebin" - путь файла для чтения
            //FileMode - для открытия
            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "Size.bin", FileMode.Open)))
            {
                M = Reader.ReadInt32();
                N = M;
            }

            if (M % SIZE_BLOCK != 0)
                throw new Exception("Block_Matrix: cant be separated on blocks");

            M /= SIZE_BLOCK;
            N /= SIZE_BLOCK;
            Size_Block = SIZE_BLOCK;
            Block = new Matrix[M][];

            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "Matrix.bin", FileMode.Open)))
            {
                var LU_Decomposition = new LU_Decomposition();

                for (int i = 0; i < N; i++)
                {
                    Block[i] = new Matrix[N];

                    //определение блоков в блочной матрице
                    for (int j = 0; j < M; j++)
                        Block[i][j] = new Matrix(Size_Block, Size_Block);

                    for (int ii = 0; ii < Size_Block; ii++)
                    {
                        for (int j = 0; j < M; j++)
                            for (int k = 0; k < Size_Block; k++)
                                Block[i][j].Elem[ii][k] = Reader.ReadDouble();
                    }

                    LU_Decomposition.CreateLU(Block[i][i]);

                    //копируем элементы из LU в блочную матрицу
                    for (int Row = 0; Row < Size_Block; Row++)
                        for (int Column = 0; Column < Size_Block; Column++)
                            Block[i][i].Elem[Row][Column] = LU_Decomposition.LU.Elem[Row][Column];
                }
            }
        }
    }
}
