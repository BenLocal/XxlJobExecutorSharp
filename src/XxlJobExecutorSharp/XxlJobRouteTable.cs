using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Action;

namespace XxlJobExecutorSharp
{
    public class XxlJobRouteTable : IEnumerable<RouteParameter>
    {
        IEnumerable<RouteParameter> _routes;

        public XxlJobRouteTable()
        {
            AddRoute<RunController>("run", "/run");
            AddRoute<BeatController>("beat", "/beat");
            AddRoute<IdleBeatController>("idleBeat", "/idleBeat");
            AddRoute<KillController>("kill", "/kill");
            AddRoute<LogController>("run", "/log");
            AddRoute<OutCompleteController>("OutComplete", "/out/complete");
        }

        public IEnumerator<RouteParameter> GetEnumerator()
        {
            return _routes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _routes.GetEnumerator();
        }

        private void AddRoute<T>(string name, string pattern) where T : IJobController
        {
            var routeParameter = new RouteParameter(name, pattern, typeof(T));
            if (_routes == null)
            {
                _routes = new RouteParameter[] { routeParameter };
                return;
            }

            _routes = _routes.Concat(new RouteParameter[] { routeParameter });
        }

        public RouteParameter FindByPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return null;
            }

            return _routes.First(x => x.Pattern.ToLower() == pattern.ToLower());
        }
    }

    public class RouteParameter
    {
        public string Name { get; }

        public string Pattern { get; }

        public Type TypeController { get; }

        public RouteParameter(string name, string pattern, Type type)
        {
            Name = name;
            Pattern = pattern;
            TypeController = type;
        }
    }
}
