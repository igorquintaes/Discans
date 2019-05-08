using System.Reflection;
using System.Resources;
using System.Threading;

namespace Discans.Resources
{
    public class LocaledResourceManager : ResourceManager
    {
        public LocaledResourceManager(string baseName, Assembly assembly) 
            : base(baseName, assembly)
        { }

        public override string GetString(string name) => 
            base.GetString(name, Thread.CurrentThread.CurrentCulture);
    }
}
