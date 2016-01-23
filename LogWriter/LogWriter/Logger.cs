using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace LogWriter
{
    public enum LogType
    {
        DebugLog = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Custom = 4
    }

    public class Logger
    {
        public Brush CustomBrush;
        public string CustomLogType;
        public Brush DebugBrush;
        public string DebugLogType;
        public Dispatcher Dispatcher;
        public Brush ErrorBrush;
        public string ErrorLogType;
        public Queue<Exception> Exceptions = new Queue<Exception>();
        public string Format;
        public Brush InfoBrush;
        public string InfoLogType;
        public RichTextBox LogBox;
        public string LogFileName;
        public int MaxLogLength;
        public bool PrependLogType;
        public bool ShowTime;
        public bool ShowTimeInDebug;
        public Brush WarningBrush;
        public string WarningLogType;
        public bool WriteDebugToFile;
        public bool WriteDebugToLog;
        //TODO: Maybe add some kind of log for when the log method has been called even though there is no logbox?       
        public bool WriteToFile;
        public static Logger CurrentLogger;

        public Logger() : this(null, null)
        {
            CurrentLogger = this;
        }

        /// <summary>
        ///     Creates a new instance of the LogWriter class with the following options
        /// </summary>
        /// <param name="logBox">The RichTextBox to write to</param>
        /// <param name="dispatcher">Dispatcher used for calling the RichTextBox</param>
        /// <param name="writeToFile">[Optional] Should LogWriter write all logs to file? (Defaults to true)</param>
        /// <param name="logFileName">[Optional] Name of the file to write to (Defaults to "log.txt")</param>
        /// <param name="showTimeInDebug">[Optional] Should LogWriter write the time for Debug Logs? (Defaults to true)</param>
        /// <param name="showTime">
        ///     [Optional] Should LogWriter write the time for all logs? (Will still write the debug logs if
        ///     false, Defaults to true)
        /// </param>
        /// <param name="showDate">[Optional] Should LogWriter write the date for all logs? (Defaults to true)</param>
        /// <param name="customDateTimeFormat">[Optional] Use a custom DateTime Format? (Defaults to false)</param>
        /// <param name="customFormat">[Optional] Custom DateTime format for if customDateTimeFormat == true</param>
        /// <param name="debugBrush">[Optional] Custom color for Debug Messages (Defaults to Brushes.MediumBlue)</param>
        /// <param name="infoBrush">[Optional] Custom color for Info Messages (Defaults to Brushes.Blue)</param>
        /// <param name="warningBrush">[Optional] Custom color for Warning Messages (Defaults to Brushes.Goldenrod)</param>
        /// <param name="errorBrush">[Optional] Custom color for Error Messages (Defaults to Brushes.DarkRed)</param>
        /// <param name="customBrush">[Optional] Custom color for Custom Messages (Defaults to Brushes.LimeGreen)</param>
        /// <param name="maxLogLength">[Optional] Maximum amount of lines for the Log (Defaults to 1000)</param>
        /// <param name="prependLogType">[Optional] Should LogWriter prepend the logType for each log entry?</param>
        /// <param name="debugLogType">
        ///     [Optional] String to prepend for DebugLogType if prependLogType == true (Defaults to
        ///     "[Debug]")
        /// </param>
        /// <param name="infoLogType">[Optional] String to prepend for InfoLogType if prependLogType == true (Defaults to "[Info]")</param>
        /// <param name="warningLogType">
        ///     [Optional] String to prepend for WarningLogType if prependLogType == true (Defaults to
        ///     "[Warning]")
        /// </param>
        /// <param name="errorLogType">
        ///     [Optional] String to prepend for ErrorLogType if prependLogType == true (Defaults to
        ///     "[Error]")
        /// </param>
        /// <param name="customLogType">[Optional] String to prepend for CustomLogType if prependLogType == true (Defaults to "")</param>
        /// <param name="writeDebugToLog">[Optional] Should LogWriter write debug logs to logBox? (Defaults to false)</param>
        /// <param name="writeDebugToFile">[Optional] Shouwl LogWriter write debug logs to file? (Defaults to true)</param>
        public Logger(RichTextBox logBox, Dispatcher dispatcher, bool writeToFile = true,
            string logFileName = "log.txt", bool showTimeInDebug = true, bool showTime = true, bool showDate = true,
            bool customDateTimeFormat = false, string customFormat = "G", Brush debugBrush = null,
            Brush infoBrush = null, Brush warningBrush = null, Brush errorBrush = null, Brush customBrush = null,
            int maxLogLength = 1000, bool prependLogType = false, string debugLogType = "[Debug]",
            string infoLogType = "[Info]", string warningLogType = "[Warning]", string errorLogType = "[Error]",
            string customLogType = "", bool writeDebugToLog = false, bool writeDebugToFile = true)
        {
            LogBox = logBox;
            Dispatcher = dispatcher;
            WriteToFile = writeToFile;
            LogFileName = logFileName;
            DebugBrush = debugBrush ?? Brushes.MediumBlue;
            InfoBrush = infoBrush ?? Brushes.Blue;
            WarningBrush = warningBrush ?? Brushes.Goldenrod;
            ErrorBrush = errorBrush ?? Brushes.DarkRed;
            CustomBrush = customBrush ?? Brushes.LimeGreen;
            ShowTimeInDebug = showTimeInDebug;
            ShowTime = showTime;
            if (customDateTimeFormat)
                Format = customFormat;
            else if (showDate)
                Format = "G";
            else
                Format = "T";
            DebugLogType = debugLogType;
            InfoLogType = infoLogType;
            WarningLogType = warningLogType;
            ErrorLogType = errorLogType;
            CustomLogType = customLogType;
            PrependLogType = prependLogType;
            WriteDebugToFile = writeDebugToFile;
            WriteDebugToLog = writeDebugToLog;
            MaxLogLength = maxLogLength;
            CurrentLogger = this;
        }

        /// <summary>
        ///     Logs a message to the Logbox that was setup earlier.
        /// </summary>
        /// <param name="message">Message to log in a special format</param>
        /// <param name="arg0">The object to format in the log message</param>
        /// <param name="logType">LogType of the message to log, logs in different colors depending on the LogType</param>
        /// <param name="prependLogType">Override the default setting for prepending logtype</param>
        /// <param name="logTypeString">
        ///     Override the log type to prepend (defaults to whatever is the default for the LogType of
        ///     the message)
        /// </param>
        /// <param name="showTime">
        ///     Override showing time (false will completely hide the time, true will show time in "G" format
        ///     unless a timeformat has been previously specified in the constructor)
        /// </param>
        /// <param name="showDate">Override showing date (false will show time in "T" Format)</param>
        /// <param name="timeFormat">Override the previously specified time format</param>
        /// <param name="writeToFile">Override writing to file (false will cancel writing to file even if it's enabled in settings</param>
        /// <param name="filename">Override the file to write to</param>
        /// <param name="writeToLog">Override writing to log (false will only write to file if it's enabled</param>
        /// <param name="brushColor">Override the Color of the message(whatever the type of the message)</param>
        /// <returns>
        ///     True if writing the log succeeded (even if nothing was written), False if failed (exceptions are available in
        ///     the Exceptions property of the class
        /// </returns>
        public bool Log(string message, object arg0, LogType logType = LogType.Info, bool? prependLogType = null,
            string logTypeString = null, bool? showTime = null, bool? showDate = null, string timeFormat = null,
            bool? writeToFile = null, string filename = null, bool? writeToLog = null, Brush brushColor = null)
        {
            return Log(string.Format(message, arg0), logType, prependLogType, logTypeString, showTime, showDate,
                timeFormat, writeToFile, filename, writeToLog, brushColor);
        }

        /// <summary>
        ///     Logs a message to the Logbox that was setup earlier.
        /// </summary>
        /// <param name="message">Message to log in a special format</param>
        /// <param name="arg0">First object to format in the log message</param>
        /// <param name="arg1">Second object to format in the log message</param>
        /// <param name="logType">LogType of the message to log, logs in different colors depending on the LogType</param>
        /// <param name="prependLogType">Override the default setting for prepending logtype</param>
        /// <param name="logTypeString">
        ///     Override the log type to prepend (defaults to whatever is the default for the LogType of
        ///     the message)
        /// </param>
        /// <param name="showTime">
        ///     Override showing time (false will completely hide the time, true will show time in "G" format
        ///     unless a timeformat has been previously specified in the constructor)
        /// </param>
        /// <param name="showDate">Override showing date (false will show time in "T" Format)</param>
        /// <param name="timeFormat">Override the previously specified time format</param>
        /// <param name="writeToFile">Override writing to file (false will cancel writing to file even if it's enabled in settings</param>
        /// <param name="filename">Override the file to write to</param>
        /// <param name="writeToLog">Override writing to log (false will only write to file if it's enabled</param>
        /// <param name="brushColor">Override the Color of the message(whatever the type of the message)</param>
        /// <returns>
        ///     True if writing the log succeeded (even if nothing was written), False if failed (exceptions are available in
        ///     the Exceptions property of the class
        /// </returns>
        public bool Log(string message, object arg0, object arg1, LogType logType = LogType.Info,
            bool? prependLogType = null, string logTypeString = null, bool? showTime = null, bool? showDate = null,
            string timeFormat = null, bool? writeToFile = null, string filename = null, bool? writeToLog = null,
            Brush brushColor = null)
        {
            return Log(string.Format(message, arg0, arg1), logType, prependLogType, logTypeString, showTime, showDate,
                timeFormat, writeToFile, filename, writeToLog, brushColor);
        }

        /// <summary>
        ///     Logs a message to the Logbox that was setup earlier.
        /// </summary>
        /// <param name="message">Message to log in a special format</param>
        /// <param name="arg0">First object to format in the log message</param>
        /// <param name="arg1">Second object to format in the log message</param>
        /// <param name="arg2">Third object to format in the log message</param>
        /// <param name="logType">LogType of the message to log, logs in different colors depending on the LogType</param>
        /// <param name="prependLogType">Override the default setting for prepending logtype</param>
        /// <param name="logTypeString">
        ///     Override the log type to prepend (defaults to whatever is the default for the LogType of
        ///     the message)
        /// </param>
        /// <param name="showTime">
        ///     Override showing time (false will completely hide the time, true will show time in "G" format
        ///     unless a timeformat has been previously specified in the constructor)
        /// </param>
        /// <param name="showDate">Override showing date (false will show time in "T" Format)</param>
        /// <param name="timeFormat">Override the previously specified time format</param>
        /// <param name="writeToFile">Override writing to file (false will cancel writing to file even if it's enabled in settings</param>
        /// <param name="filename">Override the file to write to</param>
        /// <param name="writeToLog">Override writing to log (false will only write to file if it's enabled</param>
        /// <param name="brushColor">Override the Color of the message(whatever the type of the message)</param>
        /// <returns>
        ///     True if writing the log succeeded (even if nothing was written), False if failed (exceptions are available in
        ///     the Exceptions property of the class
        /// </returns>
        public bool Log(string message, object arg0, object arg1, object arg2, LogType logType = LogType.Info,
            bool? prependLogType = null, string logTypeString = null, bool? showTime = null, bool? showDate = null,
            string timeFormat = null, bool? writeToFile = null, string filename = null, bool? writeToLog = null,
            Brush brushColor = null)
        {
            return Log(string.Format(message, arg0, arg1, arg2), logType, prependLogType, logTypeString, showTime,
                showDate, timeFormat, writeToFile, filename, writeToLog, brushColor);
        }

        /// <summary>
        ///     Logs a message to the Logbox that was setup earlier.
        /// </summary>
        /// <param name="message">Message to log in a special format</param>
        /// <param name="logType">LogType of the message to log, logs in different colors depending on the LogType</param>
        /// <param name="prependLogType">Override the default setting for prepending logtype</param>
        /// <param name="logTypeString">
        ///     Override the log type to prepend (defaults to whatever is the default for the LogType of
        ///     the message)
        /// </param>
        /// <param name="showTime">
        ///     Override showing time (false will completely hide the time, true will show time in "G" format
        ///     unless a timeformat has been previously specified in the constructor)
        /// </param>
        /// <param name="showDate">Override showing date (false will show time in "T" Format)</param>
        /// <param name="timeFormat">Override the previously specified time format</param>
        /// <param name="writeToFile">Override writing to file (false will cancel writing to file even if it's enabled in settings</param>
        /// <param name="filename">Override the file to write to</param>
        /// <param name="writeToLog">Override writing to log (false will only write to file if it's enabled</param>
        /// <param name="brushColor">Override the Color of the message(whatever the type of the message)</param>
        /// <param name="args">An array of objects to format in the log message</param>
        /// <returns>
        ///     True if writing the log succeeded (even if nothing was written), False if failed (exceptions are available in
        ///     the Exceptions property of the class
        /// </returns>
        public bool Log(string message, LogType logType = LogType.Info, bool? prependLogType = null,
            string logTypeString = null, bool? showTime = null, bool? showDate = null, string timeFormat = null,
            bool? writeToFile = null, string filename = null, bool? writeToLog = null, Brush brushColor = null,
            params object[] args)
        {
            return Log(string.Format(message, args), logType, prependLogType, logTypeString, showTime, showDate,
                timeFormat, writeToFile, filename, writeToLog, brushColor);
        }

        //MAIN LOGGER
        /// <summary>
        ///     Logs a message to the Logbox that was setup earlier.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logType">LogType of the message to log, logs in different colors depending on the LogType</param>
        /// <param name="prependLogType">Override the default setting for prepending logtype</param>
        /// <param name="logTypeString">
        ///     Override the log type to prepend (defaults to whatever is the default for the LogType of
        ///     the message)
        /// </param>
        /// <param name="showTime">
        ///     Override showing time (false will completely hide the time, true will show time in "G" format
        ///     unless a timeformat has been previously specified in the constructor)
        /// </param>
        /// <param name="showDate">Override showing date (false will show time in "T" Format)</param>
        /// <param name="timeFormat">Override the previously specified time format</param>
        /// <param name="writeToFile">Override writing to file (false will cancel writing to file even if it's enabled in settings</param>
        /// <param name="filename">Override the file to write to</param>
        /// <param name="writeToLog">Override writing to log (false will only write to file if it's enabled</param>
        /// <param name="brushColor">Override the Color of the message(whatever the type of the message)</param>
        /// <returns>
        ///     True if writing the log succeeded (even if nothing was written), False if failed (exceptions are available in
        ///     the Exceptions property of the class
        /// </returns>
        public bool Log(string message, LogType logType = LogType.Info, bool? prependLogType = null,
            string logTypeString = null, bool? showTime = null, bool? showDate = null, string timeFormat = null,
            bool? writeToFile = null, string filename = null, bool? writeToLog = null, Brush brushColor = null)
        {
            if (Dispatcher == null || LogBox == null)
                return false;

            var logPrependType = prependLogType ?? PrependLogType;
            var logShowTime = showTime ?? (ShowTime || (ShowTimeInDebug && logType == LogType.DebugLog));
            var logTimeFormat = Format;
            if (showDate != null)
                logTimeFormat = showDate == true ? "G" : "T";
            logTimeFormat = timeFormat ?? logTimeFormat;

            var logWriteToFile = writeToFile ?? (WriteToFile || (WriteDebugToFile && logType == LogType.DebugLog));
            var logWriteToLog = writeToLog ?? !(logType == LogType.DebugLog && !WriteDebugToLog);
            var logFileName = filename ?? LogFileName;
            if (!(logWriteToFile && logWriteToLog)) return true;
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    var par = new Paragraph { LineHeight = 0.5 };
                    var msg = message;
                    switch (logType)
                    {
                        case LogType.DebugLog:
                            par.Foreground = brushColor ?? DebugBrush;
                            if (logPrependType)
                                msg = (logTypeString ?? DebugLogType) + " " + msg;
                            break;
                        case LogType.Info:
                            par.Foreground = brushColor ?? InfoBrush;
                            if (logPrependType)
                                msg = (logTypeString ?? InfoLogType) + " " + msg;
                            break;
                        case LogType.Warning:
                            par.Foreground = brushColor ?? WarningBrush;
                            if (logPrependType)
                                msg = (logTypeString ?? WarningLogType) + " " + msg;
                            break;
                        case LogType.Error:
                            par.Foreground = brushColor ?? ErrorBrush;
                            if (logPrependType)
                                msg = (logTypeString ?? ErrorLogType) + " " + msg;
                            break;
                        case LogType.Custom:
                            par.Foreground = brushColor ?? CustomBrush;
                            if (logPrependType)
                                msg = (logTypeString ?? CustomLogType) + " " + msg;
                            break;
                    }
                    if (logShowTime)
                        msg = $"{DateTime.Now.ToString(logTimeFormat)}: {msg}";

                    par.Inlines.Add(msg);
                    if (logWriteToLog)
                        LogBox.Document.Blocks.Add(par);

                    if (MaxLogLength != int.MaxValue && LogBox.Lines() > MaxLogLength)
                        while (LogBox.Lines() > MaxLogLength)
                            LogBox.Document.Blocks.Remove(LogBox.Document.Blocks.FirstBlock);

                    if (!logWriteToFile) return;
                    if (!File.Exists(logFileName))
                        File.Create(logFileName).Close();
                    File.AppendAllText(logFileName, "\r\n" + msg);
                }));
            }
            catch (Exception ex)
            {
                Exceptions.Enqueue(ex);
                return false;
            }

            return true;
        }
    }

    public static class Extensions
    {
        public static long Lines(this RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text.Split('\n').Length;
        }
    }
}