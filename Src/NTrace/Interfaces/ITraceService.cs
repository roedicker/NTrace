using System;
using System.Runtime.CompilerServices;

namespace NTrace
{
    public interface ITraceService
    {
        /// <summary>
        /// Trace options for this trace service
        /// </summary>
        TraceOptions Options
        {
            get;
        }

        /// <summary>
        /// Writes an error message
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Error(string message);

        /// <summary>
        /// Writes an error named data object
        /// </summary>
        /// <param name="name">Name of the data object to be written</param>
        /// <param name="data">Data to be written</param>
        void Error(string name, object data);

        /// <summary>
        /// Writes an exception error message
        /// </summary>
        /// <param name="exception">Exception to be written</param>
        void Error(Exception exception);

        /// <summary>
        /// Writes a warning message
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Warn(string message);

        /// <summary>
        /// Writes a warning named data object
        /// </summary>
        /// <param name="name">Name of the data object to be written</param>
        /// <param name="data">Data to be written</param>
        void Warn(string name, object data);

        /// <summary>
        /// Writes an informational message of a specific category
        /// </summary>
        /// <param name="message">Message to be written</param>
        /// <param name="categories">Optional. Trace category of this message. Default value is <see cref="TraceCategories.Debug"/>.</param>
        void Info(string message, TraceCategories categories = TraceCategories.Debug);

        /// <summary>
        /// Writes an informational named data object of a specific category
        /// </summary>
        /// <param name="name">Name of the data object to be written</param>
        /// <param name="data">Data to be written</param>
        /// <param name="categories">Optional. Trace category of this message. Default value is <see cref="TraceCategories.Debug"/>.</param>
        void Info(string name, object data, TraceCategories categories = TraceCategories.Debug);

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Application"/>
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Application(string message);

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Method"/> that indicates a beginning method
        /// </summary>
        /// <param name="memberName">Optional. Runtime information of the caller method</param>
        void BeginMethod([CallerMemberName] string memberName = "");

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Method"/> that indicates an ending method
        /// </summary>
        /// <param name="memberName">Optional. Runtime information of the caller method</param>
        void EndMethod([CallerMemberName] string memberName = "");

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Connection"/>
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Connection(string message);

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Query"/>
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Query(string message);

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Data"/>
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Data(string message);

        /// <summary>
        /// Writes a named data object of categroy <see cref="TraceCategories.Data"/>
        /// </summary>
        /// <param name="name">Name of the data object to be written</param>
        /// <param name="data"></param>
        void Data(string name, object data);

        /// <summary>
        /// Write an informational message of categroy <see cref="TraceCategories.Debug"/>
        /// </summary>
        /// <param name="message">Message to be written</param>
        void Debug(string message);

        /// <summary>
        /// Signals end of writing trace messages to all registered tracers
        /// </summary>
        void EndWrite();
    }
}
