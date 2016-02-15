using System.Messaging;

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
