using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TaskManager
{
    public static class Tracer
    {
        public static TraceSource TaskManagerTrace = new TraceSource("TaskManagerTrace");
    }
}
