

using System;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects
{
    public class Interceptable : MarshalByRefObject
    {
        public virtual int DoSomething()
        {
            return 10;
        }
    }
}
