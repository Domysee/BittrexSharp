using BittrexSharp;
using BittrexSharp.Domain;
using BittrexSharp.Exceptions;
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
        private const string DefaultCurrency = "BTC";
        private const string DefaultApiKey = "";
        private const string DefaultApiSecret = "";
        private const string DefaultTargetAddress = "";

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
        public void BuyLimit_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.BuyLimit(DefaultMarketName, 1, 1); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void SellLimit_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.SellLimit(DefaultMarketName, 1, 1); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void SellLimit_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.SellLimit(DefaultMarketName, 1, 1); };
            action.ShouldThrow<UnauthorizedException>();
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
        public void CancelOrder_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var orderId = Guid.NewGuid().ToString();
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.CancelOrder(orderId); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetOpenOrders_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetOpenOrders(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetOpenOrders_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetOpenOrders(); };
            action.ShouldThrow<UnauthorizedException>();
        }
        #endregion

        #region Account Api
        [TestMethod]
        public void GetBalances_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetBalances(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetBalances_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetBalances(); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetBalance_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetBalance(DefaultCurrency); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetBalance_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetBalance(DefaultCurrency); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetDepositAddress_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetDepositAddress(DefaultCurrency); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetDepositAddress_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetDepositAddress(DefaultCurrency); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void Withdraw_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.Withdraw(DefaultCurrency, 0, DefaultTargetAddress); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void Withdraw_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.Withdraw(DefaultCurrency, 0, DefaultTargetAddress); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetOrder_ShouldNotThrowException()
        {
            var orderId = Guid.NewGuid().ToString();
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetOrder(orderId); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetOrder_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var orderId = Guid.NewGuid().ToString();
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetOrder(orderId); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetOrderHistory_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetOrderHistory(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetOrderHistory_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetOrderHistory(); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetWithdrawalHistory_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetWithdrawalHistory(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetWithdrawalHistory_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetWithdrawalHistory(); };
            action.ShouldThrow<UnauthorizedException>();
        }

        [TestMethod]
        public void GetDepositHistory_ShouldNotThrowException()
        {
            var bittrex = new Bittrex(DefaultApiKey, DefaultApiSecret);
            Func<Task> action = async () => { var _ = await bittrex.GetDepositHistory(); };
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void GetDepositHistory_ShouldUnauthorizedThrowException_IfNoApiKeyIsGiven()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetDepositHistory(); };
            action.ShouldThrow<UnauthorizedException>();
        }
        #endregion
    }
}
