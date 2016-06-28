using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS.KBS.CMSV3.INTERFACE.FUNCTION;

namespace Test
{
    class Program
    {
        private static Function CMSV3Function = new Function();

        static void Main(string[] args)
        {
            CMSV3Function.SelectSalesSql();
        }
    }
}
