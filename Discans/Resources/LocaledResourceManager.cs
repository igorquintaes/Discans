using System.Resources;
using System.Threading;

namespace Discans.Resources
{
    public class LocaledResourceManager<T> : ResourceManager
    {
        public LocaledResourceManager() 
            : base(typeof(T).FullName,
                   typeof(T).Assembly)
        { }

        public override string GetString(string name) => 
            base.GetString(name, Thread.CurrentThread.CurrentCulture);
    }
}
