﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Context;
using Spring.Context.Support;

namespace LittleOrange.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IApplicationContext ctx = ContextRegistry.GetContext();

                Console.Out.WriteLine("Server listening...");
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
