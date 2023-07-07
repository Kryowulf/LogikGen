using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LogikGenAPI.Utilities
{
    public class Profiler
    {
        public class Context : IDisposable
        {
            private Stopwatch _stopwatch;

            public string Name { get; private set; }
            public long Invocations { get; private set; }
            public TimeSpan Elapsed => _stopwatch.Elapsed;
            public long ElapsedTicks => _stopwatch.ElapsedTicks;
            public bool IsRunning => _stopwatch.IsRunning;
            
            public Context(string name)
            {
                _stopwatch = new Stopwatch();
                this.Name = name;
                this.Invocations = 0;
            }

            public void Start()
            {
                _stopwatch.Start();
                this.Invocations++;
            }

            public void Stop()
            {
                _stopwatch.Stop();
            }

            public void Dispose()
            {
                this.Stop();
            }
        }

        private Profiler() { }

        private static Dictionary<string, Context> _contexts
            = new Dictionary<string, Context>();

        private static Stopwatch _masterStopwatch = new Stopwatch();

        private static Context _dummy = new Context("");

        public static Context Enter(string name)
        {
            if (!_masterStopwatch.IsRunning)
                _masterStopwatch.Start();

            if (!_contexts.ContainsKey(name))
                _contexts[name] = new Context(name);

            Context context = _contexts[name];

            if (context.IsRunning)
                return _dummy;

            context.Start();
            return context;
        }

        public static void PrintReport()
        {
            _masterStopwatch.Stop();

            List<Context> contextList = _contexts.Values.ToList();

            if (contextList.Any(c => c.IsRunning))
                throw new InvalidOperationException("Not all contexts have stopped.");

            contextList.Sort((c1, c2) => c2.Elapsed.CompareTo(c1.Elapsed));

            foreach (Context context in contextList)
            {
                double percentage = (double) context.ElapsedTicks / _masterStopwatch.ElapsedTicks;

                Console.Write(context.Name.PadRight(40));
                Console.Write($"{context.Elapsed.TotalSeconds:F2} seconds".PadRight(20));
                Console.Write($"{percentage:P}".PadRight(10));
                Console.WriteLine($"invocations: {context.Invocations}");
            }
        }
    }
}
