using System;

namespace LibroLib.Misc
{
    public class FactoryLease<T> : IDisposable
        where T : class 
    {
        public FactoryLease(T obj, Action<T> releaseMethod)
        {
            this.obj = obj;
            this.releaseMethod = releaseMethod;
        }

        public FactoryLease (T obj, IFactory factory)
        {
            this.obj = obj;
            this.factory = factory;
        }

        public T Obj
        {
            get { return obj; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources
                    if (obj != null)
                    {
                        if (releaseMethod != null)
                            releaseMethod(obj);
                        else if (factory != null)
                            factory.Destroy(obj);

                        obj = null;
                    }
                }

                disposed = true;
            }
        }

        private T obj;
        private readonly IFactory factory;
        private bool disposed;
        private readonly Action<T> releaseMethod;
    }
}