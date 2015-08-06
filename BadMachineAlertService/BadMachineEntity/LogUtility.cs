// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------
namespace BadMachineEntity
{
    using System;
    using System.Diagnostics;

    public static class LogUtility
    {
        public static void WriteLog(LogLevel level, string message, Exception exception, string logFileLocation)
        {
            var traceSource = new TraceSource("BadMachineTraceSource");

            var tr = new TextWriterTraceListener(logFileLocation);
            var eventTrace = new EventLogTraceListener("BadMachineAnalyzer");

            traceSource.Switch = new SourceSwitch("BadMachineAnlalyzerSwitch", "BadMachineAnlalyzerSwitch")
            {
                Level = SourceLevels.Warning
            };

            tr.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Timestamp | TraceOptions.Callstack;

            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(tr);

            Trace.AutoFlush = true;

            switch (level)
            {
                case LogLevel.Error:
                    traceSource.TraceData(TraceEventType.Error, 0, message);
                    break;
                case LogLevel.Warning:
                    traceSource.TraceEvent(TraceEventType.Warning, 0, message);
                    break;
                case LogLevel.Information:
                    traceSource.TraceEvent(TraceEventType.Information, 0, message);
                    break;
            }
        }
    }

    public enum LogLevel
    {
        Error,
        Warning,
        Information
    }
}