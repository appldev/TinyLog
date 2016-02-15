using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.MSMQ
{
    internal static class Helpers
    {
        public  static void PostMessage(LogEntry logEntry, MessageQueue queue)
        {
            using (MessageQueueTransaction trans = new MessageQueueTransaction())
            {
                trans.Begin();
                queue.Send(logEntry, trans);
                trans.Commit();
            }
        }


    }
}
