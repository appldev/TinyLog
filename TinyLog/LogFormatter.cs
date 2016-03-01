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
        /// Tries to parse the custom data content using this logformatter
        /// </summary>
        /// <param name="logEntry">The Log Entry</param>
        /// <param name="customData">the parsed custom data</param>
        /// <returns>true if parsing/deserialization succeeded, otherwise false</returns>
        public bool TryParseCustomData(LogEntry logEntry, out object customData)
        {
            if (string.IsNullOrEmpty(logEntry.CustomDataFormatter) || string.IsNullOrEmpty(logEntry.CustomData))
            {
                customData = null;
                return false;
            }
            try
            {
                customData = ParseCustomData(logEntry);
                if (customData == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception)
            {
                customData = null;
                return false;
            }    
        }

        /// <summary>
        ///  Tries to parse the custom data content using this formatter
        /// </summary>
        /// <typeparam name="T">The object type to return</typeparam>
        /// <param name="logEntry">The Log Entry</param>
        /// <param name="customData">the parsed custom data into the object type T</param>
        /// <returns>true if parsing/deserialization succeeded, otherwise false</returns>
        public bool TryParseCustomData<T>(LogEntry logEntry, out T customData)
        {
            object o;
            bool b = TryParseCustomData(logEntry, out o);
            if (b)
            {
                customData = (T)o;
            }
            else
            {
                customData = default(T);
            }
            return b;
        }

        /// <summary>
        /// Performs the actual formatting of a log entry
        /// </summary>
        /// <param name="logEntry">The LogEntry to format</param>
        /// <param name="customData">The custom data object to use</param>
        protected abstract void FormatLogEntry(LogEntry logEntry, object customData);

        /// <summary>
        /// Performs the actual parsing/deserialization of custom data in a log entry
        /// </summary>
        /// <param name="logEntry">The Log entry</param>
        /// <returns>the parsed/deserialized custom data. If the operation fails, the return type is null</returns>
        protected abstract object ParseCustomData(LogEntry logEntry);


    }





}
