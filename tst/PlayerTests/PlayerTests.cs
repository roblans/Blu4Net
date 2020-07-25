using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayerTests
{
    [TestClass]
    public class PlayerTests
    {
        static BluPlayer Player;

        [ClassInitialize()]
        public static async Task Initialize(TestContext testContext) 
        {
            Player = await BluEnvironment.FindPlayers().FirstAsync();
        }
    }
}
