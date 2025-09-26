using UnityEngine;

namespace Runtime
{
    public static class MyAssertions
    {
        public static void EnsureIsNotNull(Component component)
        {
            UnityEngine.Assertions.Assert.IsNotNull(
                component, "component is missing");
        }
    }
}