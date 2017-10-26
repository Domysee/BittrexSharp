using BittrexSharp.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BittrexSharp
{
    public class Bittrex
    {
        public const string Version = "v1.1";
        public const string BaseUrl = "https://bittrex.com/api/" + Version + "/";
        public const string SignHeaderName = "apisign";

        private readonly Encoding encoding = Encoding.UTF8;

        private HttpClient httpClient;
        private string apiKey;
        private string apiSecret;
        private byte[] apiSecretBytes;

        public Bittrex()
        {
            this.apiKey = null;
            this.apiSecret = null;
            this.apiSecretBytes = null;
            this.httpClient = new HttpClient();
        }

        public Bittrex(string apiKey, string apiSecret)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.apiSecretBytes = encoding.GetBytes(apiSecret);
            this.httpClient = new HttpClient();
        }

        #region Helper
        private string byteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        private string convertParameterListToString(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0) return "";
            return parameters.Select(param => WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value)).Aggregate((l, r) => l + "&" + r);
        }

        private (string uri, string hash) createRequestAuthentication(string uri) => createRequestAuthentication(uri, new Dictionary<string, string>());
        private (string uri, string hash) createRequestAuthentication(string uri, IDictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>(parameters);

            var nonce = DateTime.Now.Ticks;
            parameters.Add("apikey", apiKey);
            parameters.Add("nonce", nonce.ToString());

            var parameterString = convertParameterListToString(parameters);
            var completeUri = uri + "?" + parameterString;

            var uriBytes = encoding.GetBytes(completeUri);
            using (var hmac = new HMACSHA512(apiSecretBytes))
            {
                var hash = hmac.ComputeHash(uriBytes);
                var hashText = byteToString(hash);
                return (completeUri, hashText);
            }
        }

        protected HttpRequestMessage createRequest(HttpMethod httpMethod, string uri, bool includeAuthentication = true) => createRequest(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        protected HttpRequestMessage createRequest(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication)
        {
            if (includeAuthentication)
            {
                (var completeUri, var hash) = createRequestAuthentication(uri, parameters);
                var request = new HttpRequestMessage(httpMethod, completeUri);
                request.Headers.Add(SignHeaderName, hash);
                return request;
            }
            else
            {
                var parameterString = convertParameterListToString(parameters);
                var completeUri = uri + "?" + parameterString;
                var request = new HttpRequestMessage(httpMethod, completeUri);
                return request;
            }
        }

        protected async Task<ResponseWrapper<TResult>> request<TResult>(HttpMethod httpMethod, string uri, bool includeAuthentication = true)
            => await request<TResult>(httpMethod, uri, new Dictionary<string, string>(), includeAuthentication);
        protected async Task<ResponseWrapper<TResult>> request<TResult>(HttpMethod httpMethod, string uri, IDictionary<string, string> parameters, bool includeAuthentication = true)
        {
            var request = createRequest(HttpMethod.Get, uri, parameters, includeAuthentication);
            HttpResponseMessage response = null;
            while (response == null)
            {
                try
                {
                    response = await httpClient.SendAsync(request);
                }
                catch (Exception)
                {
                    response = null;
                }
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var bittrexResponse = JsonConvert.DeserializeObject<BittrexResponse>(content);
            var result = new ResponseWrapper<TResult>
            {
                Success = bittrexResponse.Success,
                Message = bittrexResponse.Message
            };
            if (bittrexResponse.Success)
            {
                try
                {
                    result.Result = bittrexResponse.Result.ToObject<TResult>();
                }
                catch (Exception e)
                {
                    throw new Exception("Error converting json to .Net types", e);
                }
            }
            return result;
        }
        #endregion

        #region Public Api
        /// <summary>
        /// Get a list of all markets and associated metadata
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<Market>>> GetMarkets()
        {
            var uri = BaseUrl + "public/getmarkets";
            var marketsResponse = await request<IEnumerable<Market>>(HttpMethod.Get, uri, false);
            return marketsResponse;
        }

        /// <summary>
        /// Get a list of all supported currencies and associated metadata
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<SupportedCurrency>>> GetSupportedCurrencies()
        {
            var uri = BaseUrl + "public/getcurrencies";
            var supportedCurrenciesResponse = await request<IEnumerable<SupportedCurrency>>(HttpMethod.Get, uri, false);
            return supportedCurrenciesResponse;
        }

        /// <summary>
        /// Get the current bid, ask and last prices for the given market
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<Ticker>> GetTicker(string marketName)
        {
            var uri = BaseUrl + "public/getticker";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName }
            };
            var tickerResponse = await request<Ticker>(HttpMethod.Get, uri, parameters, false);
            if (tickerResponse.Result != null)
                tickerResponse.Result.MarketName = marketName;
            return tickerResponse;
        }

        /// <summary>
        /// Get summaries of the last 24 hours of all markets
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<MarketSummary>>> GetMarketSummaries()
        {
            var uri = BaseUrl + "public/getmarketsummaries";
            var marketSummariesResponse = await request<IEnumerable<MarketSummary>>(HttpMethod.Get, uri, false);
            return marketSummariesResponse;
        }

        /// <summary>
        /// Get the summary of the last 24 hours of the given market
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<MarketSummary>> GetMarketSummary(string marketName)
        {
            var uri = BaseUrl + "public/getmarketsummary";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName }
            };
            var marketSummaryResponse = await request<MarketSummary>(HttpMethod.Get, uri, parameters, false);
            return marketSummaryResponse;
        }

        /// <summary>
        /// Get the order book for the given market
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <param name="orderType">The types of orders you want to get, use the static properties of OrderType.</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<OrderBook>> GetOrderBook(string marketName, string orderType, int depth)
        {
            var uri = BaseUrl + "public/getorderbook";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName },
                { "type", orderType },
                { "depth", depth.ToString(CultureInfo.InvariantCulture) }
            };

            var orderBookResponse = new ResponseWrapper<OrderBook> { Result = new OrderBook() };

            if (orderType == OrderType.Both)
                orderBookResponse = await request<OrderBook>(HttpMethod.Get, uri, parameters, false);
            else if (orderType == OrderType.Buy)
            {
                var buyOrderBookEntriesResponse = await request<IEnumerable<OrderBookEntry>>(HttpMethod.Get, uri, parameters, false);
                orderBookResponse.Success = buyOrderBookEntriesResponse.Success;
                orderBookResponse.Message = buyOrderBookEntriesResponse.Message;
                orderBookResponse.Result.Buy = buyOrderBookEntriesResponse.Result;
            }
            else if (orderType == OrderType.Sell)
            {
                var buyOrderBookEntriesResponse = await request<IEnumerable<OrderBookEntry>>(HttpMethod.Get, uri, parameters, false);
                orderBookResponse.Success = buyOrderBookEntriesResponse.Success;
                orderBookResponse.Message = buyOrderBookEntriesResponse.Message;
                orderBookResponse.Result.Sell = buyOrderBookEntriesResponse.Result;
            }

            orderBookResponse.Result.MarketName = marketName;
            return orderBookResponse;
        }

        /// <summary>
        /// Get a list of recent orders for the given market
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<Trade>>> GetMarketHistory(string marketName)
        {
            var uri = BaseUrl + "public/getmarkethistory";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName }
            };
            var ordersResponse = await request<IEnumerable<Trade>>(HttpMethod.Get, uri, parameters, false);
            return ordersResponse;
        }
        #endregion

        #region Market Api
        /// <summary>
        /// Place a buy order
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <param name="quantity">How much of the currency you want to buy</param>
        /// <param name="rate">The price at which you want to buy</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<AcceptedOrder>> BuyLimit(string marketName, decimal quantity, decimal rate)
        {
            var uri = BaseUrl + "market/buylimit";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) }
            };
            var acceptedOrderResponse = await request<AcceptedOrder>(HttpMethod.Get, uri, parameters);
            return acceptedOrderResponse;
        }

        /// <summary>
        /// Place a sell order
        /// </summary>
        /// <param name="marketName">The name of the market, e.g. BTC-LTC</param>
        /// <param name="quantity">How much of the currency you want to sell</param>
        /// <param name="rate">The price at which you want to sell</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<AcceptedOrder>> SellLimit(string marketName, decimal quantity, decimal rate)
        {
            var uri = BaseUrl + "market/selllimit";
            var parameters = new Dictionary<string, string>
            {
                { "market", marketName },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) }
            };
            var acceptedOrderResponse = await request<AcceptedOrder>(HttpMethod.Get, uri, parameters);
            return acceptedOrderResponse;
        }

        /// <summary>
        /// Cancel the order with the given id
        /// </summary>
        /// <param name="orderId">The uuid of the order to cancel</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<object>> CancelOrder(string orderId)
        {
            var uri = BaseUrl + "market/cancel";
            var parameters = new Dictionary<string, string>
            {
                { "uuid", orderId }
            };
            return await request<object>(HttpMethod.Get, uri, parameters);
        }

        /// <summary>
        /// Get open orders
        /// </summary>
        /// <param name="marketName">If given, only get the open orders of the given market</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<OpenOrder>>> GetOpenOrders(string marketName = null)
        {
            var uri = BaseUrl + "market/getopenorders";
            var parameters = new Dictionary<string, string>();
            if (marketName != null) parameters.Add("market", marketName);

            var openOrdersResponse = await request<IEnumerable<OpenOrder>>(HttpMethod.Get, uri, parameters);
            return openOrdersResponse;
        }
        #endregion

        #region Account Api
        /// <summary>
        /// Get the balance of all cryptocurrencies
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<CurrencyBalance>>> GetBalances()
        {
            var uri = BaseUrl + "account/getbalances";
            var balancesResponse = await request<IEnumerable<CurrencyBalance>>(HttpMethod.Get, uri);
            return balancesResponse;
        }

        /// <summary>
        /// Get the balance of the given currency
        /// </summary>
        /// <param name="currency">Currency symbol, e.g. BTC</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<CurrencyBalance>> GetBalance(string currency)
        {
            var uri = BaseUrl + "account/getbalance";
            var parameters = new Dictionary<string, string>
            {
                { "currency", currency }
            };
            var balanceResponse = await request<CurrencyBalance>(HttpMethod.Get, uri, parameters);
            return balanceResponse;
        }

        /// <summary>
        /// Get the deposit address for the given currency
        /// </summary>
        /// <param name="currency">Currency symbol, e.g. BTC</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<DepositAddress>> GetDepositAddress(string currency)
        {
            var uri = BaseUrl + "account/getdepositaddress";
            var parameters = new Dictionary<string, string>
            {
                { "currency", currency }
            };
            var depositAddressResponse = await request<DepositAddress>(HttpMethod.Get, uri, parameters);
            return depositAddressResponse;
        }

        /// <summary>
        /// Send funds to another address
        /// </summary>
        /// <param name="currency">Currency symbol, e.g. BTC</param>
        /// <param name="quantity">How much of the currency should be withdrawn</param>
        /// <param name="address">The address to which the funds should be sent</param>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<AcceptedWithdrawal>> Withdraw(string currency, decimal quantity, string address, string paymentId = null)
        {
            var uri = BaseUrl + "account/withdraw";
            var parameters = new Dictionary<string, string>
            {
                { "currency", currency },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture) },
                { "address", address },
                { "paymentid", paymentId }
            };
            var acceptedWithdrawalResponse = await request<AcceptedWithdrawal>(HttpMethod.Get, uri, parameters);
            return acceptedWithdrawalResponse;
        }

        /// <summary>
        /// Get a specific order
        /// </summary>
        /// <param name="orderId">The uuid of the order</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<Order>> GetOrder(string orderId)
        {
            var uri = BaseUrl + "account/getorder";
            var parameters = new Dictionary<string, string>
            {
                { "uuid", orderId }
            };
            var orderResponse = await request<Order>(HttpMethod.Get, uri, parameters);
            return orderResponse;
        }

        /// <summary>
        /// Get the order history of the account
        /// </summary>
        /// <param name="marketName">If given, restricts the history to the given market</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<HistoricOrder>>> GetOrderHistory(string marketName = null)
        {
            var uri = BaseUrl + "account/getorderhistory";
            var parameters = new Dictionary<string, string>();
            if (marketName != null) parameters.Add("market", marketName);

            var orderHistoryResponse = await request<IEnumerable<HistoricOrder>>(HttpMethod.Get, uri, parameters);
            return orderHistoryResponse;
        }

        /// <summary>
        /// Get the withdrawal history
        /// </summary>
        /// <param name="currency">If given, restricts the history to the given currency</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<HistoricWithdrawal>>> GetWithdrawalHistory(string currency = null)
        {
            var uri = BaseUrl + "account/getwithdrawalhistory";
            var parameters = new Dictionary<string, string>();
            if (currency != null) parameters.Add("currency", currency);

            var withdrawalHistoryResponse = await request<IEnumerable<HistoricWithdrawal>>(HttpMethod.Get, uri, parameters);
            return withdrawalHistoryResponse;
        }

        /// <summary>
        /// Get the deposit history
        /// </summary>
        /// <param name="currency">If given, restricts the history to the given currency</param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<IEnumerable<HistoricDeposit>>> GetDepositHistory(string currency = null)
        {
            var uri = BaseUrl + "account/getdeposithistory";
            var parameters = new Dictionary<string, string>();
            if (currency != null) parameters.Add("currency", currency);

            var depositHistoryResponse = await request<IEnumerable<HistoricDeposit>>(HttpMethod.Get, uri, parameters);
            return depositHistoryResponse;
        }
        #endregion
    }
}
