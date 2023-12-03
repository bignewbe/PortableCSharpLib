using System;

namespace PortableCSharpLib.Interface
{
    public interface IRateGate
    {
        bool WaitToProceed(int millisecondsTimeout);
        bool WaitToProceed(TimeSpan timeout);
        void WaitToProceed();
    }
}
