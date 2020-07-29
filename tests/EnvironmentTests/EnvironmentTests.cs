using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace EnvironmentTests
{
    [TestClass]
    public class EnvironmentTests
    {
        static TestContext Context;

        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            Context = context;
        }

        [TestMethod]
        public async Task EnvironmentTests_ResolveEndpoints()
        {
            var endpoints = await BluEnvironment.ResolveEndpoints().ToArray();
            Assert.IsTrue(endpoints.Length > 0);
        }

        [TestMethod]
        public async Task EnvironmentTests_ResolvePlayers()
        {
            var players = await BluEnvironment.ResolveEndpoints()
                  .SelectAsync(endpoint => BluPlayer.Connect(endpoint))
                  .ToArray();

            Assert.IsTrue(players.Length > 0);
        }
    }
}
