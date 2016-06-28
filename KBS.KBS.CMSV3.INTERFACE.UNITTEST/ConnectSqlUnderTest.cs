using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS.KBS.CMSV3.INTERFACE.FUNCTION;
using NUnit.Framework;

namespace KBS.KBS.CMSV3.INTERFACE.UNITTEST
{
    [TestFixture]
    class ConnectSqlUnderTest
    {
        Function function = new Function();

        [Test]
        public void Login_Return_Success()
        {
            function.ConnectSQLServer();

            //StringAssert.AreEqualIgnoringCase("admin", user.Username);


        }



    }
}
