using System;
using System.Linq;

namespace TinyLog
{
    /// <summary>
    /// A generic filter used to evaluate a LogEntry object. Among others, the filter is used by the LogSubscriber class to
    /// evaluate if a subscriber should receive a LogEntry Created event
    /// </summary>
    public class LogEntryFilter
    {
        public LogEntryFilter()
        {

        }

        /// <summary>
        /// Create a LogEntryFilter from a predicate. When set the predicate takes precedence over the filter properties
        /// </summary>
        /// <param name="predicate">the match predicate</param>
        /// <returns>a new LogEntryFilter</returns>
        public static LogEntryFilter Create(Func<LogEntry, bool> predicate)
        {
            return new LogEntryFilter()
            {
                Predicate = predicate
            };
        }

        public static LogEntryFilter Create()
        {
            return new LogEntryFilter();
        }

        /// <summary>
        /// Creates a LogEntryFilter
        /// </summary>
        /// <param name="sources">the list of sources that will match the filter. When null, all sources will match</param>
        /// <param name="areas">The list of areas that will match the filter. When null, all areas will match</param>
        /// <param name="severities">The list of severities that will match the filter. When null, all severities will match</param>
        /// <param name="EqualityComparer">A stringComparer to use when matching sources and areas. When null the OrdinalIgnoreCase comparer will be used</param>
        /// <returns>a new LogEntryFilter</returns>
        public static LogEntryFilter Create(string[] sources = null, string[] areas = null, LogEntrySeverity[] severities = null, StringComparer EqualityComparer = null)
        {
            return new LogEntryFilter()
            {
                SourcesFilter = sources,
                AreasFilter = areas,
                SeveritiesFilter = severities,
                EqualityComparer = EqualityComparer ?? StringComparer.OrdinalIgnoreCase
            };
        }


        private string[] _SourcesFilter = null;
        private string[] _AreasFilter = null;
        LogEntrySeverity[] _SeveritiesFilter = null;
        private StringComparer _EqualityComparer = StringComparer.OrdinalIgnoreCase;
        private Func<LogEntry, bool> _predicate = null;

        private DateTimeOffset? _fromDate = null;
        private DateTimeOffset? _toDate = null;



        /// <summary>
        /// Defines the list of sources for the filter
        /// </summary>
        public string[] SourcesFilter
        {
            get
            {
                return _SourcesFilter;
            }

            set
            {
                _SourcesFilter = value;
                _predicate = null;
            }
        }

        /// <summary>
        /// Defines the list of areas for the filter
        /// </summary>
        public string[] AreasFilter
        {
            get
            {
                return _AreasFilter;
            }

            set
            {
                _AreasFilter = value;
                _predicate = null;
            }
        }

        /// <summary>
        /// Defines the Log Severity levels for the filter
        /// </summary>
        public LogEntrySeverity[] SeveritiesFilter
        {
            get
            {
                return _SeveritiesFilter;
            }

            set
            {
                _SeveritiesFilter = value;
                _predicate = null;
            }
        }

        /// <summary>
        /// If specified, the predicate will be used to evaluate this filter. The match predicate takes precedence over other filters
        /// </summary>
        public Func<LogEntry, bool> Predicate
        {
            get
            {
                if (_predicate != null)
                {
                    return _predicate;
                }
                else
                {
                    return x => (SourcesFilter == null || SourcesFilter.Contains(x.Source, EqualityComparer)) &&
                            (AreasFilter == null || AreasFilter.Contains(x.Area, EqualityComparer)) &&
                            (SeveritiesFilter == null || SeveritiesFilter.Contains(x.Severity)) &&
                            (FromDate == null || x.CreatedOn >= FromDate.Value) &&
                            (ToDate == null || x.CreatedOn <= ToDate.Value);
                }
            }
            set
            {
                _predicate = value;
                _AreasFilter = null;
                _SourcesFilter = null;
                _SeveritiesFilter = null;
            }
        }

        /// <summary>
        /// The StringComparer to use when matching sources and areas
        /// </summary>
        public StringComparer EqualityComparer
        {
            get
            {
                return _EqualityComparer;
            }

            set
            {
                _EqualityComparer = value;
            }
        }

        /// <summary>
        /// The from date
        /// </summary>
        public DateTimeOffset? FromDate
        {
            get
            {
                return _fromDate;
            }

            set
            {
                _fromDate = value;
            }
        }

        public string FromDateString
        {
            get
            {
                if (FromDate.HasValue)
                {
                    return FromDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
                }
                else
                {
                    return null;
                }
            }
        }

        public string ToDateString
        {
            get
            {
                if (ToDate.HasValue)
                {
                    return ToDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The to date
        /// </summary>
        public DateTimeOffset? ToDate
        {
            get
            {
                return _toDate;
            }

            set
            {
                _toDate = value;
            }
        }



        /// <summary>
        /// Matches a filter against a LogEntry object
        /// </summary>
        /// <param name="logEntry">The LogEntry object to match</param>
        /// <returns>return true if the LogEntry matches the filter</returns>
        public bool IsMatch(LogEntry logEntry)
        {
            return Predicate(logEntry);
        }
    }
}
