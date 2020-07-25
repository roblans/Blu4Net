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
            var endpoints = new List<Uri>();
            var subscription = BluEnvironment.PlayerEndpoints.Subscribe(endpoint =>
            {
                Context.WriteLine($"Endpoint: {endpoint}");
                endpoints.Add(endpoint);
            });
            
            await Task.Delay(TimeSpan.FromSeconds(5));

            Assert.IsTrue(endpoints.Count > 0);

            subscription.Dispose();
        }

        [TestMethod]
        public async Task EnvironmentTests_ResolvePlayers()
        {
            var players = new List<BluPlayer>();
            var subscription = BluEnvironment.Players.Subscribe(player =>
            {
                Context.WriteLine($"Player: {player}");
                players.Add(player);
            });

            await Task.Delay(TimeSpan.FromSeconds(5));

            Assert.IsTrue(players.Count > 0);

            subscription.Dispose();
        }
    }
}
