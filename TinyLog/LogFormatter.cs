using System;

namespace TinyLog
{
    /// <summary>
    /// The base class all log entry formatters must inherit from
    /// </summary>
    public abstract class LogFormatter
    {
        public LogFormatter(Type typeFilter, bool filterOnChildTypes)
        {
            _TypeFilter = typeFilter;
            _FilterOnChildTypes = filterOnChildTypes;
        }
        private Type _TypeFilter;
        /// <summary>
        /// Returns the Type of custom data that the formatter will handle
        /// </summary>
        public Type TypeFilter
        {
            get
            {
                return _TypeFilter;
            }
        }

        /// <summary>
        /// Returns true if the formatter will also format the custom data's child types.
        /// Example: If the Filter is System.Exception and FilterOnChildTypes is true, the formatter will format all types inheritted from System.Exception
        /// </summary>
        public bool FilterOnChildTypes
        {
            get
            {
                return _FilterOnChildTypes;
            }
        }



        private bool _FilterOnChildTypes = false;

        /// <summary>
        /// Determines if this formatter is valid for a specific custom data object
        /// </summary>
        /// <param name="customData">The object to evaluate</param>
        /// <returns>true if this filter should be used to format the custom data info</returns>
        public virtual bool IsValidFormatterFor(object customData)
        {

            Type t = customData.GetType();
            while (t != null)
            {
                if (_TypeFilter.IsInterface)
                {
                    if (t.GetInterface(_TypeFilter.Name, true) != null)
                    {
                        return true;
                    }
                    else
                    {
                        t = t.BaseType;
                    }
                }
                else if (t == _TypeFilter)
                {
                    return true;
                }
                else
                {
                    t = t.BaseType;
                }
            }
            return false;
        }

        /// <summary>
        /// Formats a log entry with the custom data
        /// </summary>
        /// <param name="logEntry">The LogEntry to format</param>
        /// <param name="customData">The custom data object to use</param>
        public void Format(LogEntry logEntry, object customData)
        {
            // TODO: Determine if IsValidFormatterFor() should be called again by the formatter itself
            //if (customData.GetType() != TypeFilter)
            //{
            //    throw new ArgumentException(string.Format("Custom data must be a type of '{0}'", TypeFilter.Name), "customData");
            //}
            FormatLogEntry(logEntry, customData);
        }

        /// <summary>
        /// Performs the actual formatting of a log entry
        /// </summary>
        /// <param name="logEntry">The LogEntry to format</param>
        /// <param name="customData">The custom data object to use</param>
        protected abstract void FormatLogEntry(LogEntry logEntry, object customData);



    }





}
