using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BittrexSharp.Domain;
using System.Linq;

namespace BittrexSharp.BittrexOrderSimulation
{
    /// <summary>
    /// Behaves exactly like Bittrex, except that buy and sell orders are not put through to Bittrex, and are simulated instead
    /// </summary>
    public class BittrexOrderSimulation : Bittrex
    {
        private List<OpenOrder> simulatedOpenOrders = new List<OpenOrder>();
        private List<Order> simulatedFinishedOrders = new List<Order>();
        private List<CurrencyBalance> simulatedBalances = new List<CurrencyBalance>();

        public BittrexOrderSimulation(string apiKey, string apiSecret) : base(apiKey, apiSecret)
        {
        }

        private void addBalance(string currency, decimal quantity)
        {
            var existingBalance = simulatedBalances.SingleOrDefault(b => b.Currency == currency);
            if (existingBalance != null)
                existingBalance.Balance += quantity;
            else
                simulatedBalances.Add(new CurrencyBalance
                {
                    Balance = quantity,
                    Currency = currency
                });
        }

        private void removeBalance(string currency, decimal quantity)
        {
            var existingBalance = simulatedBalances.Single(b => b.Currency == currency);
            existingBalance.Balance -= quantity;
        }

        public override async Task<AcceptedOrder> BuyLimit(string marketName, decimal quantity, decimal rate)
        {
            var currentRate = (await GetTicker(marketName)).Last;

            var acceptedOrderId = Guid.NewGuid().ToString();
            if (currentRate <= rate)
            {
                var order = new Order
                {
                    Closed = DateTime.Now,
                    IsOpen = false,
                    Limit = rate,
                    Opened = DateTime.Now,
                    OrderUuid = acceptedOrderId,
                    Price = quantity * rate,
                    PricePerUnit = rate,
                    Quantity = quantity
                };
                simulatedFinishedOrders.Add(order);

                var currency = Helper.GetTargetCurrencyFromMarketName(marketName);
                addBalance(currency, quantity);
            }
            else
            {
                var order = new OpenOrder
                {
                    Closed = DateTime.Now,
                    Limit = rate,
                    Opened = DateTime.Now,
                    OrderUuid = acceptedOrderId,
                    Price = quantity * rate,
                    PricePerUnit = rate,
                    Quantity = quantity
                };
                simulatedOpenOrders.Add(order);
            }

            return new AcceptedOrder
            {
                Uuid = acceptedOrderId
            };
        }

        public override async Task<AcceptedOrder> SellLimit(string marketName, decimal quantity, decimal rate)
        {
            var currentRate = (await GetTicker(marketName)).Last;

            var acceptedOrderId = Guid.NewGuid().ToString();
            if (currentRate >= rate)
            {
                var order = new Order
                {
                    Closed = DateTime.Now,
                    IsOpen = false,
                    Limit = rate,
                    Opened = DateTime.Now,
                    OrderUuid = acceptedOrderId,
                    Price = -quantity * rate,
                    PricePerUnit = rate,
                    Quantity = -quantity
                };
                simulatedFinishedOrders.Add(order);

                var currency = Helper.GetTargetCurrencyFromMarketName(marketName);
                removeBalance(currency, quantity);
            }
            else
            {
                var order = new OpenOrder
                {
                    Closed = DateTime.Now,
                    Limit = rate,
                    Opened = DateTime.Now,
                    OrderUuid = acceptedOrderId,
                    Price = -quantity * rate,
                    PricePerUnit = rate,
                    Quantity = -quantity
                };
                simulatedOpenOrders.Add(order);
            }

            return new AcceptedOrder
            {
                Uuid = acceptedOrderId
            };
        }
    }
}
