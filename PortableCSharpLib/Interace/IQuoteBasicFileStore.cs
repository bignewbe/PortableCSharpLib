using PortableCSharpLib;
using PortableCSharpLib.TechnicalAnalysis;
using System.Threading.Tasks;

namespace PortableCSharpLib.Interface
{
    public interface IQuoteBasicFileStore
    {
        //string DataFolder { get; }
        //string Exchange { get; }
        //Task<bool> Update(IQuoteBasicDownloader download, string symbol, int interval, int numBarsToRemoveGap=-1, int timeout = 50000);

        QuoteBasicBase Load(string symbol, int interval, long? startTime, int maxCount=500);
        bool Save(IQuoteBasicBase quote, int numBarsToRemoveGap = -1);
        event EventHandlers.QuoteSavedEventHandler OnQuoteSaved;
    }
}
