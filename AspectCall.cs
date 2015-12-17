using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Aspecting
{
    public class AspectCall
    {
        public object[] Arguments { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public object Result { get; set; }
        public bool Canceled { get; set; }
        public Exception Exception { get; set; }
    }
}
