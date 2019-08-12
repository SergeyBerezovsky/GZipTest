using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    class Operator
    {
        FileStream destStr;
        
        string sourceFileName;
        string destFileName;
        string command;
        int buffSize;
        int error;
        delegate byte[] compressArray();
        delegate byte[] decompressArray();
        
        public Operator(string _command, string _sourceFileName, string _destFileName)
        {
             command = _command;
             destFileName = _destFileName;
             destStr = new FileStream(destFileName, FileMode.Create);            
             sourceFileName = _sourceFileName;
             buffSize = 1 << 20;
             error = 0;
        }

        public  void Start()
        {
            if (command == "compress") compress();
            if (command == "decompress") decompress();
        }

        void compress()
        {            
            FileStream sourceStr = new FileStream(sourceFileName, FileMode.Open);
            StreamWriter indexFile = File.CreateText(destFileName + ".ind"); ;
            long count = sourceStr.Length / (buffSize);
            long ostByte = sourceStr.Length - count * (buffSize);
            int procCount = Environment.ProcessorCount;
            long threadPool = count / procCount;
            long ostBuff = count - threadPool * procCount;
            sourceStr.Close();

            // Готовим класс Compress для работы
            Compress.Init(sourceFileName, buffSize);

            // Создаем окружение для работы с потоками
            Thread[] thrd = new Thread[procCount];
            Compress[] compr = new Compress[procCount];
            compressArray[] cmprArray = new compressArray[procCount];

            for (int i = 0; i < threadPool; i++)
            {
                // Собираем пул потоков ставим задачи и запускаем на выполнение
                for (int thrdNum = 0; thrdNum < procCount; thrdNum++)
                {
                    compr[thrdNum] = new Compress();
                    cmprArray[thrdNum] = compr[thrdNum].getCompressByteArray;
                    thrd[thrdNum] = new Thread(new ThreadStart(compr[thrdNum].compress));
                    thrd[thrdNum].Start();
                    Thread.Sleep(1);
                }

                // Ждем завершения работы потоков, получаем результат.
                // Пишем массивы сжатых данных в архивный файл
                // Пишем размеры сжатых массивов в индексный файл 
                for (int thrdNum = 0; thrdNum < procCount; thrdNum++)
                {
                    thrd[thrdNum].Join();
                    destStr.Write(cmprArray[thrdNum](), 0, cmprArray[thrdNum]().Length);
                    indexFile.WriteLine(cmprArray[thrdNum]().Length);
                }

                // Индикация выполнения
                int progress = i*Console.BufferWidth/(int)threadPool;
                Console.SetCursorPosition(progress, 2);
                Console.Write("*");               
            }

            if (ostBuff != 0)
            {
                // Собираем пул из остатков потоков ставим задачи и запускаем на выполнение
                for (int thrdNum = 0; thrdNum < ostBuff; thrdNum++)
                {
                    compr[thrdNum] = new Compress();
                    cmprArray[thrdNum] = compr[thrdNum].getCompressByteArray;
                    thrd[thrdNum] = new Thread(compr[thrdNum].compress);
                    thrd[thrdNum].Start();
                    Thread.Sleep(1);
                }

                // Ждем завершения работы потоков, получаем результат.
                // Пишем массивы сжатых данных в архивный файл
                // Пишем размеры сжатых массивов в индексный файл 
                for (int thrdNum = 0; thrdNum < ostBuff; thrdNum++)
                {
                    thrd[thrdNum].Join();
                    destStr.Write(cmprArray[thrdNum](), 0, cmprArray[thrdNum]().Length);
                    indexFile.WriteLine(cmprArray[thrdNum]().Length);
                }
            }

            // Создаем заключительную часть архива
            // Отправляем в работу остаток файла длиной меньше размера буфера
            // Записываем в индексный файл размер сжатых данных и размер несжатых
            // Отделяем их от остальных индексов
            if (ostByte != 0)
            {
                Compress.setOstBuff(ostByte);
                Compress cmpr = new Compress();
                cmpr.compress();
                destStr.Write(cmpr.getCompressByteArray(), 0, cmpr.getCompressByteArray().Length);
                indexFile.WriteLine("ENDPART");
                indexFile.WriteLine(cmpr.getCompressByteArray().Length);
                indexFile.WriteLine(ostByte);
            }
            destStr.Close();
            indexFile.Close();
            error = 0;
        }
        
        void decompress()
        {
            StreamReader indexFile = new StreamReader(sourceFileName+".ind");

            Decompress.Init(sourceFileName, buffSize);

            string line;
            int prcCount = Environment.ProcessorCount;
            int[] index = new int[prcCount];
            int count = 0;

            while ((line = indexFile.ReadLine()) != "ENDPART")
            {
                index[count] = int.Parse(line);
                count++;

                if (count == prcCount - 1)
                {
                    goToThreads(index, count);
                    count = 0;
                }                               
            }
            // Проверяем наличие несобранного пула после завершения чтения
            if (count > 0)
            {
                goToThreads(index, count);
            }

            // Распаковываем последний том архива
            int endPartIndex = int.Parse(indexFile.ReadLine());
            int endBuff = int.Parse(indexFile.ReadLine());
            Decompress.setOstBuff(endBuff);
            Decompress dec = new Decompress(endPartIndex);
            dec.decompress();

            destStr.Write(dec.getDecompressByte(),0,dec.getDecompressByte().Length);


            destStr.Close();
            indexFile.Close();
            error = 0; 
        }

        private void goToThreads(int[] index, int count)
        {
            Thread[] thrd = new Thread[count];
            Decompress[] decompr = new Decompress[count];
            decompressArray[] decomprArray = new decompressArray[count];

            // Собираем пул потоков ставим задачи и запускаем на выполнение
            for (int thrdNum = 0; thrdNum < count; thrdNum++)
            {
                decompr[thrdNum] = new Decompress(index[thrdNum]);
                decomprArray[thrdNum] = decompr[thrdNum].getDecompressByte;
                thrd[thrdNum] = new Thread(new ThreadStart(decompr[thrdNum].decompress));
                thrd[thrdNum].Start();
                Thread.Sleep(1);
            }

            // Ждем завершения работы потоков и получаем результат
            for (int thrdNum = 0; thrdNum < count; thrdNum++)
            {
                thrd[thrdNum].Join();
                destStr.Write(decomprArray[thrdNum](), 0, decomprArray[thrdNum]().Length);
            }
        }

        public int errorMsg()
        {
            return error;
        }
    }
}
