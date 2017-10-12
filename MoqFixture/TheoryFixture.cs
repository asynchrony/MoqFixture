using System;
using NUnit.Framework;

namespace MoqFixture
{
    public class TheoryFixture
    {
        [Datapoint] public int TestInt0 = 0;
        [Datapoint] public int TestInt1 = 187;

        [Datapoint] public string TestString0 = "string";
        [Datapoint] public string TestString1 = "beans";

        [Datapoint] public DateTime TestDateTime0 = new DateTime(1,1,1);
        [Datapoint] public DateTime TestDateTime1 = new DateTime(1984, 1, 1);

        [Datapoint] public DateTime? TestNullableDateTime0 = new DateTime(1, 1, 1);
        [Datapoint] public DateTime? TestNullableDateTime1 = null;
    }
}
