using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Com_Methods
{
    //класс блочного вектора
    class BlockVector : IVector
    {
        public int N { set; get; }
        public int Size_Block { set; get; }
        public Vector[] Block { set; get; }
        public BlockVector() { }

        public BlockVector(string PATH, int SIZE_BLOCK)
        {
            //PATH + "Sizebin" - путь файла для чтения
            //FileMode - для открытия
            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "Size.bin", FileMode.Open)))
            {
                N = Reader.ReadInt32();
            }

            if (N % SIZE_BLOCK != 0)
                throw new Exception("Block_Vector: cant be separated on blocks");

            using (var Reader = new System.IO.BinaryReader(File.Open(PATH + "F.bin", FileMode.Open)))
            {
                N /= SIZE_BLOCK;
                Size_Block = SIZE_BLOCK;
                Block = new Vector[N];

                for(int j = 0; j < N; j++)
                {
                    Block[j] = new Vector(Size_Block);

                    for (int k = 0; k < Size_Block; k++)
                        Block[j].Elem[k] = Reader.ReadDouble();
                }
            }
        }
    }
}
