using System;
using System.Collections.Generic;

namespace MoqFixture
{
    public class DefaultMocks
    {
        private static readonly List<Action<IMoqFixture>> SetupFunctions = new List<Action<IMoqFixture>>();

        public static void AddSetup(Action<IMoqFixture> setupFunction)
        {
            SetupFunctions.Add(setupFunction);
        }

        internal static void SetupFixture(IMoqFixture fixture)
        {
            foreach (var setupFunction in SetupFunctions)
            {
                setupFunction(fixture);
            }
        }
    }
}
