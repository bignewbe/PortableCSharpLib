using System.Linq;
using System.Reflection;

namespace PortableCSharpLib.Util
{
    public class EqualAndCopyUseReflection<T> where T : class
    {
        public bool Equals(T other)
        {
            if (other == null) return false;
            var properties = this.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var p in properties)
            {
                if (!p.CanRead) continue;

                var v1 = p.GetValue(this);
                var v2 = p.GetValue(other);
                if (v1 == null && v2 == null) continue;
                if (v1 == null || v2 == null) return false;
                if (!v1.Equals(v2)) return false;
            }
            return true;
        }

        public void Copy(T other)
        {
            if (other == null) return;
            var properties = this.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var p in properties)
            {
                if (!p.CanWrite || !p.CanRead) continue;
                var v2 = p.GetValue(other);
                p.SetValue(this, v2);
            }
        }
    }
}
