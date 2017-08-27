using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Used for generating the type difference between objects.
    /// </summary>
    public class TypeNameDifference
    {
        /// <summary>
        /// Gets the unique type name between expected and actual.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value causing the failure</param>
        /// <param name="expectedType">Output of the unique type name for expected</param>
        /// <param name="actualType">Output of the unique type name for actual</param>
        public void ResolveTypeNameDifference(object expected, object actual, out string expectedType, out string actualType)
        {
            ResolveSingularTypeNameDifference(expected, actual, out expectedType, out actualType);
        }

        private static void ResolveSingularTypeNameDifference(object expected, object actual, out string expectedType, out string actualType)
        {
            string[] expectedOriginalType = expected.GetType().ToString().Split('.');
            string[] actualOriginalType = actual.GetType().ToString().Split('.');
            int actualStart = 0, expectStart = 0;
            for (int expectLen = expectedOriginalType.Length - 1, actualLen = actualOriginalType.Length - 1;
                expectLen >= 0 && actualLen >= 0;
                expectLen--, actualLen--)
            {
                if (expectedOriginalType[expectLen] != actualOriginalType[actualLen])
                {
                    actualStart = actualLen;
                    expectStart = expectLen;
                    break;
                }
            }
            expectedType = String.Join(".", expectedOriginalType, expectStart, expectedOriginalType.Length - expectStart);
            actualType = String.Join(".", actualOriginalType, actualStart, actualOriginalType.Length - actualStart);
        }
    }
}
