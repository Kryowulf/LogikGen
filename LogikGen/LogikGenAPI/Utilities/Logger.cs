using System.Collections.Generic;

namespace LogikGenAPI.Utilities
{
    public class Logger
    {
        private List<string> _log = new List<string>();
        private string _currentTag = null;

        public virtual void LogInfo(object data, bool includeTag = true)
        {
            if (includeTag)
                _log.Add($"[{_currentTag}] {data}");
            else
                _log.Add($"{data}");
        }

        public virtual void SetTag(string tag)
        {
            _currentTag = tag;
        }

        public virtual List<string> GetCurrentLog()
        {
            return new List<string>(_log);
        }
    }
}
