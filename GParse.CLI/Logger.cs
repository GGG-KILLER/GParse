using System;
using System.Collections.Generic;
using System.Threading;

namespace GParse.CLI
{
    public static class Logger
    {
        private static readonly Queue<String> LineQueue = new Queue<String> ( );

        private static readonly Thread WriteThread = new Thread ( WriteThreadHandler )
        {
            IsBackground = true,
            Name = "LoggerThread",
            Priority = ThreadPriority.Lowest
        };

        public static Boolean ShouldPrint;

        static Logger ( )
        {
            WriteThread.Start ( );
        }

        private static void WriteThreadHandler ( Object obj )
        {
            while ( true )
            {
                if ( ShouldPrint )
                    lock ( LineQueue )
                        while ( LineQueue.Count > 0 )
                            Console.WriteLine ( LineQueue.Dequeue ( ) );
                Thread.Sleep ( 100 );
            }
        }

        public static void WriteLine ( Object value )
        {
            lock ( LineQueue )
                LineQueue.Enqueue ( value.ToString ( ) );
        }

        public static void ClearQueue ( )
        {
            lock ( LineQueue )
                LineQueue.Clear ( );
        }
    }
}
