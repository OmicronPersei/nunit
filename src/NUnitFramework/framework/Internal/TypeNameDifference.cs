// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Used for resolving the type difference between objects.
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
            ShortenDifferingTypeNames(out expectedType, out actualType, expected.GetType().ToString(), actual.GetType().ToString());
        }

        private static void ShortenDifferingTypeNames(out string expectedTypeShortened, out string actualTypeShortened, string expectedOriginalType, string actualOriginalType)
        {
            string[] expectedOriginalTypeSplit = expectedOriginalType.Split('.');
            string[] actualOriginalTypeSplit = actualOriginalType.Split('.');

            int actualStart = 0, expectStart = 0;
            for (int expectLen = expectedOriginalTypeSplit.Length - 1, actualLen = actualOriginalTypeSplit.Length - 1;
                expectLen >= 0 && actualLen >= 0;
                expectLen--, actualLen--)
            {
                if (expectedOriginalTypeSplit[expectLen] != actualOriginalTypeSplit[actualLen])
                {
                    actualStart = actualLen;
                    expectStart = expectLen;
                    break;
                }
            }
            expectedTypeShortened = String.Join(".", expectedOriginalTypeSplit, expectStart, expectedOriginalTypeSplit.Length - expectStart);
            actualTypeShortened = String.Join(".", actualOriginalTypeSplit, actualStart, actualOriginalTypeSplit.Length - actualStart);
        }
    }
}
