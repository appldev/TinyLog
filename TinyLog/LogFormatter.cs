using System;
using System.Collections.Generic;

namespace TinyLog
{
    /// <summary>
    /// The base class all log entry formatters must inherit from
    /// </summary>
    public abstract class LogFormatter
    {
        public LogFormatter(IEnumerable<Type> typeFilters, bool filterOnChildTypes)
        {
            _TypeFilters = new List<Type>(typeFilters);
            _FilterOnChildTypes = filterOnChildTypes;
        }
        [Obsolete("This constructor will be removed. Use the other constructor for future compatibility")]
        public LogFormatter(Type typeFilter, bool filterOnChildTypes)
        {
            _TypeFilters = new List<Type>() { typeFilter };
            _FilterOnChildTypes = filterOnChildTypes;
        }

        private List<Type> _TypeFilters;
        /// <summary>
        /// Returns the Types of custom data that the formatter will handle
        /// </summary>
        public List<Type> TypeFilters
        {
            get
            {
                return _TypeFilters;
            }
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

        protected bool MatchType(Type type, Type typeToMatch)
        {
            if (typeToMatch.IsInterface && type.GetInterface(typeToMatch.Name, true) != null)
            {
                return true;
            }
            else if (type == typeToMatch)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if this formatter is valid for a specific custom data object
        /// </summary>
        /// <param name="customData">The object to evaluate</param>
        /// <returns>true if this filter should be used to format the custom data info</returns>
        public virtual bool IsValidFormatterFor(object customData)
        {
            Type t = customData.GetType();
            if (!FilterOnChildTypes)
            {
                foreach (Type type in TypeFilters)
                {
                    if (MatchType(t, type))
                    {
                        return true;
                    }
                }
            }
            else
            {
                while (t != null)
                {
                    foreach (Type type in TypeFilters)
                    {
                        if (MatchType(t, type))
                        {
                            return true;
                        }
                    }
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
            FormatLogEntry(logEntry, customData);
            logEntry.CustomDataFormatter = GetType().FullName;
            logEntry.CustomDataType = customData.GetType().FullName;
        }

        /// <summary>
        /// Performs the actual formatting of a log entry
        /// </summary>
        /// <param name="logEntry">The LogEntry to format</param>
        /// <param name="customData">The custom data object to use</param>
        protected abstract void FormatLogEntry(LogEntry logEntry, object customData);



    }





}
