using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace MoqFixture
{
    public class DefaultMocks
    {
        internal static readonly Dictionary<string, Mock> Mocks = new Dictionary<string, Mock>();
        public static Mock<TMock> Mock<TMock>() where TMock : class
        {
            var typeName = typeof(TMock).FullName;
            if (Mocks.TryGetValue(typeName, out var mock))
            {
                return (Mock<TMock>)mock;
            }

            mock = new Mock<TMock>();
            Mocks.Add(typeName, mock);
            return (Mock<TMock>)mock;
        }

    }
}
