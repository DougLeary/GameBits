using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameBits
{
    /// <summary>
    /// Static class for debugging and activity logging
    /// </summary>
    public static class Logger
    {
        static List<String> Log = new List<String>();
        static int LogSize = 0;

        /// <summary>
        /// Add an entry to the log
        /// </summary>
        /// <param name="Msg"></param>
        public static void Write(String Msg)
        {
            if (Logger.LogSize > 5000)
            {
                Log.Add("[continued...]");
                Logger.Save();
                Log.Add("[...continued]");
            }

            Log.Add(Msg);
            Logger.LogSize += Msg.Length;
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        public static void Clear()
        {
            Log.Clear();
            Logger.LogSize = 0;
        }

        /// <summary>
        /// Flush the entire log to the Application Event Log
        /// </summary>
        public static void Save(EventLogEntryType EventType)
        {
            if (Logger.LogSize == 0) return;

            string SourceName = "GameBits";
            string LogType = "Application";

            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, LogType);
            }

            EventInstance ev = new EventInstance(0, 0, EventType);
            EventLog.WriteEvent(SourceName, ev, Log.ToArray());
            Logger.Clear();
        }

        /// <summary>
        /// Flush the entire log to the Event Log
        /// </summary>
        public static void Save()
        {
            Logger.Save(EventLogEntryType.Information);
        }
    }
}
