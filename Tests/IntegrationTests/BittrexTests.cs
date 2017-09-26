using BittrexSharp;
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
        [TestMethod]
        public void GetMarketSummaries_ShouldNotThrowException()
        {
            var bittrex = new Bittrex();
            Func<Task> action = async () => { var _ = await bittrex.GetMarketSummaries(); };
            action.ShouldNotThrow();
        }
    }
}
