using System;

namespace BittrexSharp.Domain
{
    public class HistoricDeposit
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int Confirmations { get; set; }
        public DateTime LastUpdated { get; set; }
        public string TxId { get; set; }
        public string CryptoAddress { get; set; }
    }
}
