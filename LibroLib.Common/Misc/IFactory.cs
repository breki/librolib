using System.Diagnostics.Contracts;
using LibroLib.GoodPolicies;

namespace LibroLib.Misc
{
    [ContractClass(typeof(IFactoryContract)), ThreadSafe]
    public interface IFactory
    {
        void Destroy(object component);
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