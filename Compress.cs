using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    class Compress
    {
        static FileStream sourceStream;
        static int buffSize;
        byte[] cmprsByteArray;
        byte[] readByteArray;
        static Mutex mutexObj = new Mutex();

        public static void Init(string sourceFile, int _buffSize)
        {
            buffSize = _buffSize;
            sourceStream = new FileStream(sourceFile,FileMode.Open);                             
        }

        public void compress()
        {
            readByteArray = new byte[buffSize];

            mutexObj.WaitOne();
            int readByteLen = sourceStream.Read(readByteArray, 0, buffSize);
            mutexObj.ReleaseMutex();

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gz.Write(readByteArray, 0, readByteLen);
                }              
                cmprsByteArray = ms.ToArray();
            }                                               
        }

        public byte[] getCompressByteArray()
        {
            return cmprsByteArray;
        }

        public static void setOstBuff(long ostBuff)
        {
            buffSize = (int) ostBuff;
        }

    }
}
