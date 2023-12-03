using PortableCSharpLib.TechnicalAnalysis;
using System.Threading.Tasks;

namespace PortableCSharpLib.Interface
{
    public interface IQuoteBasicDownloader
    {
        string Exchange { get; }
        Task<IQuoteBasicBase> Download(string symbol, int interval, int timeout = 50000);
    }
}
