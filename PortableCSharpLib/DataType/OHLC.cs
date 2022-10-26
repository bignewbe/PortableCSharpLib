using Newtonsoft.Json;
using PortableCSharpLib.Interface;
using System;

namespace PortableCSharpLib.DataType
{
    public class OHLC : IIdEqualCopy<OHLC>
    {
        [JsonIgnore]
        public string Id { get { return this.Symbol; } }
        [JsonProperty(PropertyName = "S")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "I")]
        public int Interval { get; set; }
        [JsonProperty(PropertyName = "T")]
        public long Time { get; set; }
        [JsonProperty(PropertyName = "O")]
        public double Open { get; set; }
        [JsonProperty(PropertyName = "H")]
        public double High { get; set; }
        [JsonProperty(PropertyName = "P")]
        public double Low { get; set; }
        [JsonProperty(PropertyName = "C")]
        public double Close { get; set; }
        [JsonProperty(PropertyName = "V")]
        public double Volume { get; set; }

        public OHLC() { }
        public OHLC(OHLC other) => this.Copy(other);
        public void Copy(OHLC other)
        {
            this.Symbol = other.Symbol;
            this.Interval = other.Interval;
            this.Time = other.Time;
            this.Open = other.Open;
            this.High = other.High;
            this.Low = other.Low;
            this.Close = other.Close;
            this.Volume = other.Volume;
        }

        public bool Equals(OHLC other)
        {
            return
                  this.Symbol == other.Symbol &
                  this.Interval == other.Interval &
                  this.Time == other.Time &
                  this.Open == other.Open &
                  this.High == other.High &
                  this.Low == other.Low &
                  this.Close == other.Close &
                  this.Volume == other.Volume;
        }
    }
}
