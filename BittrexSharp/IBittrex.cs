using System.Collections.Generic;
using System.Threading.Tasks;
using BittrexSharp.Domain;

namespace BittrexSharp
{
    public interface IBittrex
    {
        Task<ResponseWrapper<AcceptedOrder>> BuyLimit(string marketName, decimal quantity, decimal rate);
        Task<ResponseWrapper<object>> CancelOrder(string orderId);
        Task<ResponseWrapper<CurrencyBalance>> GetBalance(string currency);
        Task<ResponseWrapper<IEnumerable<CurrencyBalance>>> GetBalances();
        Task<ResponseWrapper<DepositAddress>> GetDepositAddress(string currency);
        Task<ResponseWrapper<IEnumerable<HistoricDeposit>>> GetDepositHistory(string currency = null);
        Task<ResponseWrapper<IEnumerable<Trade>>> GetMarketHistory(string marketName);
        Task<ResponseWrapper<IEnumerable<Market>>> GetMarkets();
        Task<ResponseWrapper<IEnumerable<MarketSummary>>> GetMarketSummaries();
        Task<ResponseWrapper<MarketSummary>> GetMarketSummary(string marketName);
        Task<ResponseWrapper<IEnumerable<OpenOrder>>> GetOpenOrders(string marketName = null);
        Task<ResponseWrapper<Order>> GetOrder(string orderId);
        Task<ResponseWrapper<OrderBook>> GetOrderBook(string marketName, string orderType, int depth);
        Task<ResponseWrapper<IEnumerable<HistoricOrder>>> GetOrderHistory(string marketName = null);
        Task<ResponseWrapper<IEnumerable<SupportedCurrency>>> GetSupportedCurrencies();
        Task<ResponseWrapper<Ticker>> GetTicker(string marketName);
        Task<ResponseWrapper<IEnumerable<HistoricWithdrawal>>> GetWithdrawalHistory(string currency = null);
        Task<ResponseWrapper<AcceptedOrder>> SellLimit(string marketName, decimal quantity, decimal rate);
        Task<ResponseWrapper<AcceptedWithdrawal>> Withdraw(string currency, decimal quantity, string address, string paymentId = null);
    }
}