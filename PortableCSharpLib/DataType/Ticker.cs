using Newtonsoft.Json;
using PortableCSharpLib.Interface;

namespace PortableCSharpLib.DataType
{
    public class Ticker : IIdEqualCopy<Ticker>
    {
        [JsonIgnore]
        public string Id { get { return this.Symbol; } }

        //public string TimeStr { get { return Seconds.GetUTCFromUnixTime().ToLocalTime().ToString("yyyyMMdd hh:mm:ss"); } }
        [JsonProperty(PropertyName = "a")]
        public string Symbol { get; set; }
        //public int Miliseconds { get { return (int)(Timestamp % 1000); } }
        //public long Seconds { get { return Timestamp / 1000; } }
        //public int OpenBuyOrders { get; set; }
        //public int OpenSellOrders { get; set; }
        [JsonProperty(PropertyName = "b")]
        public long Timestamp { get; set; }
        [JsonProperty(PropertyName = "c")]
        public double Bid { get; set; }
        [JsonProperty(PropertyName = "d")]
        public double Ask { get; set; }
        [JsonProperty(PropertyName = "e")]
        public double Last { get; set; }
        [JsonProperty(PropertyName = "f")]
        public double High { get; set; }
        [JsonProperty(PropertyName = "g")]
        public double Low { get; set; }
        [JsonProperty(PropertyName = "h")]
        public double Volume { get; set; }
        [JsonProperty(PropertyName = "i")]
        public double BaseVolume { get; set; }
        [JsonProperty(PropertyName = "j")]
        public double BidQty { get; set; }
        [JsonProperty(PropertyName = "k")]
        public double AskQty { get; set; }

        public Ticker()
        {
        }

        public Ticker(Ticker other)
        {
            this.Copy(other);
        }

        public void Copy(Ticker other)
        {
            if (other != null)
            {
                this.Symbol = other.Symbol;
                this.Bid = other.Bid;
                this.Ask = other.Ask;
                this.BidQty = other.BidQty;
                this.AskQty = other.AskQty;
                this.Last = other.Last;
                this.High = other.High;
                this.Low = other.Low;
                this.BaseVolume = other.BaseVolume;
                this.Volume = other.Volume;
                this.Timestamp = other.Timestamp;
                //this.OpenBuyOrders = other.OpenBuyOrders;
                //this.OpenSellOrders = other.OpenSellOrders;
            }
        }

        public bool Equals(Ticker other)
        {
            if (other == null) return false;

            return (this.Symbol == other.Symbol &&
                    this.Bid == other.Bid &&
                    this.Ask == other.Ask &&
                    this.BidQty == other.BidQty &&
                    this.AskQty == other.AskQty &&
                    this.Last == other.Last &&
                    this.High == other.High &&
                    this.Low == other.Low &&
                    this.BaseVolume == other.BaseVolume &&
                    this.Volume == other.Volume &&
                    this.Timestamp == other.Timestamp);
                    //this.OpenBuyOrders == other.OpenBuyOrders &&
                    //this.OpenSellOrders == other.OpenSellOrders);
        }
    }
}
