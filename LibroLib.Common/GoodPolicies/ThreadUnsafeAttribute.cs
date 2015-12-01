using System;

namespace LibroLib.GoodPolicies
{
    [AttributeUsage (AttributeTargets.Interface | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ThreadUnsafeAttribute : Attribute
    {
    }
}