using System;
using System.ComponentModel;

namespace Demo.Project.DemoDataExplorer.Extensions
{
    public static class SynchronizeInvokeExtensions
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            try
            {
                if (@this.InvokeRequired)
                    @this.Invoke(action, new object[] {@this});
                else
                    action(@this);
            }
            catch (Exception)
            {
                // nothing;
            }
        }
    }
}