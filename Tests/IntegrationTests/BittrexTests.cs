using BittrexSharp;
using BittrexSharp.Domain;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class BittrexTests
    {
        private const string DefaultMarketName = "BTC-ETH";
        private const string DefaultApiKey = "";
        private const string DefaultApiSecret = "";

        #region Public Api
        [TestMethod]
        public void GetMarkets_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetMarkets(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetSupportedCurrencies_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetSupportedCurrencies(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetTicker_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetTicker(DefaultMarketName); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetMarketSummaries_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetMarketSummaries(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetMarketSummary_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetMarketSummary(DefaultMarketName); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetOrderBook_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetOrderBook(DefaultMarketName, OrderType.Both, 1); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetMarketHistory_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetMarketHistory(DefaultMarketName); };
            action.ShouldNotThrow();
        }
        #endregion

        #region Market Api
        [TestMethod]
        public void BuyLimit_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.BuyLimit(DefaultMarketName, 1, 1); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void SellLimit_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.SellLimit(DefaultMarketName, 1, 1); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void CancelOrder_ShouldNotThrowException()
        {
            var orderId = Guid.NewGuid().ToString();
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.CancelOrder(orderId); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetOpenOrders_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetOpenOrders(); };
            action.ShouldNotThrow();
        }
        #endregion
    }
}
