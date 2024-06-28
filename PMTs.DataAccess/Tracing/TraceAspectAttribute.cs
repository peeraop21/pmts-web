using AspectInjector.Broker;
using System;
using System.Diagnostics;
//using DataAccess.Utils;
//using System.Diagnostics;

namespace PMTs.DataAccess.Tracing
{
    [Aspect(Scope.Global)]
    [Injection(typeof(TraceAspectAttribute))]
    public sealed class TraceAspectAttribute : Attribute
    {
        Activity activity;
        [Advice(Kind.Before, Targets = Target.Method)]
        public void TraceStart([Argument(Source.Type)] Type type, [Argument(Source.Name)] string name)
        {
            activity = ActivitySourceProvider.Source!.StartActivity($"{type.Name} :: {name}");
        }

        [Advice(Kind.After, Targets = Target.Method)]
        public void TraceFinish([Argument(Source.Type)] Type type, [Argument(Source.Name)] string name)
        {
            activity.Stop();
        }
    }
}
