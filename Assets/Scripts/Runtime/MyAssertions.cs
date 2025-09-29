using UnityEngine;

namespace Runtime
{
    public static class MyAssertions
    {
        public static void EnsureIsNotNull(Component component)
        {
            UnityEngine.Assertions.Assert.IsNotNull(
                component, $"{nameof(component)} is missing");
        }

        public static void EnsureIsTrue(bool condition)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                condition, $"{condition.ToString()} is not valid");
        }
    }
}