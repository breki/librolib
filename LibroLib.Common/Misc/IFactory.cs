using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LibroLib.GoodPolicies;

namespace LibroLib.Misc
{
    [ContractClass(typeof(IFactoryContract)), ThreadSafe]
    public interface IFactory
    {
        void Destroy([NotNull] object component);
    }

    [ContractClassFor(typeof(IFactory))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IFactoryContract : IFactory
    {
        void IFactory.Destroy(object component)
        {
        }
    }
}