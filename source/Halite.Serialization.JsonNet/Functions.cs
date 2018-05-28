using System;

namespace Halite.Serialization.JsonNet
{
    internal static class Functions
    {
        public static T Identity<T>(T it) { return it; }

        public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g)
        {
            return a => g(f(a));
        }
    }
}