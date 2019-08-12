using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    class Progress
    {
        public static void progIndication()
        {
           // пока не реализовано в архиваторе
           int bWidth = Console.BufferWidth;
           Console.WriteLine("Ширина консольного буфера {0}", bWidth);
           Console.WriteLine("Вывод при count больше BufferWidth");
           Console.CursorVisible = false;
           int count = 7856;
           int pos;
           int beforePos = -1;
           for (int i = 0; i < count; i++)
           {
               pos=i*bWidth/count;
               if (pos > beforePos)
               {
                   Console.SetCursorPosition(pos, 2);
                   Console.WriteLine("*");
                   beforePos = pos;
                   Thread.Sleep(100);
               }               
           }
           //Console.WriteLine();
           Console.WriteLine("Вывод при count меньше BufferWidth");
           count = 63;
           int afterPos;
           pos = 0;
           for (int i = 0; i <= count; i++)
           {
               afterPos = i * bWidth / count;
               while (pos < afterPos)
               {
                   Console.SetCursorPosition(pos, 5);
                   Console.WriteLine("*");
                   pos++; Thread.Sleep(100);
               }
           }

           
        }
    }
}
