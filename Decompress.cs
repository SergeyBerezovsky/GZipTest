using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    class Decompress
    {
        static FileStream sourceStream;
        static int buffSize;
        int index;

        byte[] decArray;
        byte[] readByteArray;
        static Mutex mutexObj = new Mutex();

        public Decompress(int index)
        {
            this.index = index;
        }

        public static void Init(string sourceFile,int _buffSize)
        {
            buffSize = _buffSize;             
            sourceStream =  new FileStream(sourceFile, FileMode.Open);          
        }

        public void decompress()
        {
            mutexObj.WaitOne();
            readByteArray = new byte[index];           
            sourceStream.Read(readByteArray, 0, index);
            mutexObj.ReleaseMutex();

            byte[] decArraygz = new byte[buffSize];

            using (var inStream = new MemoryStream(readByteArray))
            {
                using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress, true))
                {
                    bigStream.Read(decArraygz, 0, buffSize);
                }
                decArray = decArraygz;
            }
        }

        public byte[] getDecompressByte()
        {
            return decArray;
        }

        public static void setOstBuff(int ostBuff)
        {
            buffSize = ostBuff;
        }
    }
}
