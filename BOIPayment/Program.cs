﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIPayment
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new PaymentProcessor();
            processor.Test_Report();
            Console.ReadLine();
        }
    }
}
