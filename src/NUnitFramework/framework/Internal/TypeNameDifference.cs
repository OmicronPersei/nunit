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
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            string expectedFullType = expected.GetType().ToString();
            string actualFullType = actual.GetType().ToString();

            if (IsObjectTypeGeneric(expected) && IsObjectTypeGeneric(actual))
            {
                string shortenedTopLevelGenericExpected, shortenedTopLevelGenericActual;
                GetShortenedTopLevelGenericTypes(expectedFullType, actualFullType, out shortenedTopLevelGenericExpected, out shortenedTopLevelGenericActual);

                List<string> shortenedParamsExpected, shortenedParamsActual;
                GetShortenedGenericParams(expectedFullType, actualFullType, out shortenedParamsExpected, out shortenedParamsActual);

                expectedType = ReconstructShortenedGenericTypeName(shortenedTopLevelGenericExpected, shortenedParamsExpected);
                actualType = ReconstructShortenedGenericTypeName(shortenedTopLevelGenericActual, shortenedParamsActual);
            }
            else if (IsObjectTypeGeneric(expected) || IsObjectTypeGeneric(actual))
            {
                if (IsObjectTypeGeneric(expected))
                {
                    expectedType = ShortenFullyQualifiedGenericType(expectedFullType);
                    actualType = GetOnlyTypeName(actualFullType);
                }
                else
                {
                    expectedType = GetOnlyTypeName(expectedFullType);
                    actualType = ShortenFullyQualifiedGenericType(actualFullType);
                }
            }
            else
            {
                ShortenTypeNames(expectedFullType, actualFullType, out expectedType, out actualType);
            }
        }

        private void GetShortenedGenericParams(string expectedFullType, string actualFullType, out List<string> shortenedParamsExpected, out List<string> shortenedParamsActual)
        {
            List<string> templateParamsExpected = GetFullyQualifiedGenericParameters(expectedFullType);
            List<string> templateParamsActual = GetFullyQualifiedGenericParameters(actualFullType);

            shortenedParamsExpected = new List<string>();
            shortenedParamsActual = new List<string>();
            while ((templateParamsExpected.Count > 0) && (templateParamsActual.Count > 0))
            {
                string shortenedExpected, shortenedActual;
                ShortenTypeNames(templateParamsExpected[0], templateParamsActual[0], out shortenedExpected, out shortenedActual);

                shortenedParamsExpected.Add(shortenedExpected);
                shortenedParamsActual.Add(shortenedActual);

                templateParamsExpected.RemoveAt(0);
                templateParamsActual.RemoveAt(0);
            }

            foreach (string genericParamRemaining in templateParamsExpected)
            {
                shortenedParamsExpected.Add(GetOnlyTypeName(genericParamRemaining));
            }

            foreach (string genericParamRemaining in templateParamsActual)
            {
                shortenedParamsActual.Add(GetOnlyTypeName(genericParamRemaining));
            }
        }

        private void GetShortenedTopLevelGenericTypes(string expectedFullType, string actualFullType, out string shortenedTopLevelGenericExpected, out string shortenedTopLevelGenericActual)
        {
            string toplevelGenericExpected = GetTopLevelGenericType(expectedFullType);
            string toplevelGenericActual = GetTopLevelGenericType(actualFullType);
            ShortenTypeNames(
                toplevelGenericExpected,
                toplevelGenericActual,
                out shortenedTopLevelGenericExpected,
                out shortenedTopLevelGenericActual);
        }

        /// <summary>
        /// Shorten the given type names by only including the relevant differing types, if they differ.
        /// </summary>
        /// <param name="expectedTypeShortened">The shortened expected type.</param>
        /// <param name="actualTypeShortened">The shortened actual type.</param>
        /// <param name="expectedOriginalType">The expected type.</param>
        /// <param name="actualOriginalType">The actual type.</param>
        public void ShortenTypeNames(string expectedOriginalType, string actualOriginalType, out string expectedTypeShortened, out string actualTypeShortened)
        {
            string[] expectedOriginalTypeSplit = expectedOriginalType.Split('.');
            string[] actualOriginalTypeSplit = actualOriginalType.Split('.');

            bool differenceDetected = false;
            int actualStart = 0, expectStart = 0;
            for (int expectLen = expectedOriginalTypeSplit.Length - 1, actualLen = actualOriginalTypeSplit.Length - 1;
                expectLen >= 0 && actualLen >= 0;
                expectLen--, actualLen--)
            {
                if (expectedOriginalTypeSplit[expectLen] != actualOriginalTypeSplit[actualLen])
                {
                    actualStart = actualLen;
                    expectStart = expectLen;
                    differenceDetected = true;
                    break;
                }
            }

            if (differenceDetected)
            {
                expectedTypeShortened = String.Join(".", expectedOriginalTypeSplit, expectStart, expectedOriginalTypeSplit.Length - expectStart);
                actualTypeShortened = String.Join(".", actualOriginalTypeSplit, actualStart, actualOriginalTypeSplit.Length - actualStart);
            }
            else
            {
                expectedTypeShortened = expectedOriginalTypeSplit[expectedOriginalTypeSplit.Length - 1];
                actualTypeShortened = actualOriginalTypeSplit[actualOriginalTypeSplit.Length - 1];
            }
            
        }

        /// <summary>
        /// Returns whether or not the object is a generic type.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to check if is generic.</param>
        /// <remarks>Used for cross-compatability between .NET and .NETCore.</remarks>
        public bool IsObjectTypeGeneric(object obj)
        {
#if NETSTANDARD1_3 || NETSTANDARD1_6
            return obj.GetType().IsConstructedGenericType;
#else
            return obj.GetType().IsGenericType;
#endif
        }

        /// <summary>
        /// Get the fully qualified name of the generic parameters of a given object.
        /// </summary>
        /// <param name="FullyQualifiedObjectType">The fully qualified name of the generic type.</param>
        public List<string> GetFullyQualifiedGenericParameters(string FullyQualifiedObjectType)
        {
            var regex = new Regex(@"\[(.+)\]");
            var match = regex.Match(FullyQualifiedObjectType);
            return new List<string>(match.Groups[1].Value.Split(','));
        }

        /// <summary>
        /// Returns the top level generic type of a given fully qualified type name.
        /// </summary>
        /// <param name="TypeFullName">The fully qualified name to resolve.</param>
        public string GetTopLevelGenericType(string TypeFullName)
        {
            if (TypeFullName.Contains("["))
            {
                return TypeFullName.Split('[')[0];
            }
            else
            {
                throw new ArgumentException("The given " + nameof(TypeFullName) + " does not represent a generic type.");
            }
        }

        /// <summary>
        /// Reconstruct a generic type name using the provided generic type name, and a
        /// list of the template parameters.
        /// </summary>
        /// <param name="GenericType">The name of the generic type.</param>
        /// <param name="TemplateParams">A list of names of the template parameters of the provided generic type.</param>
        public string ReconstructShortenedGenericTypeName(string GenericType, List<string> TemplateParams)
        {
            return GenericType + "[" + string.Join(",", TemplateParams.ToArray()) + "]";
        }

        /// <summary>
        /// Obtain the name of the type, given a fully qualified name.
        /// </summary>
        /// <param name="FullyQualifiedName">The fully qualified name to resolve the type of.</param>
        public string GetOnlyTypeName(string FullyQualifiedName)
        {
            if (FullyQualifiedName.Contains("."))
            {
                string[] split = FullyQualifiedName.Split('.');
                return split[split.Length - 1];
            }
            else
            {
                return FullyQualifiedName;
            }
        }

        /// <summary>
        /// Shorten a fully qualified generic type name to only the names of the 
        /// </summary>
        /// <param name="FullyQualifiedGenericType">The fully qualified name of a generic type.</param>
        public string ShortenFullyQualifiedGenericType(string FullyQualifiedGenericType)
        {
            string genericType = GetOnlyTypeName(GetTopLevelGenericType(FullyQualifiedGenericType));

            List<string> genericParams = GetFullyQualifiedGenericParameters(FullyQualifiedGenericType);
            List<string> shortenedGenericParams = new List<string>();
            genericParams.ForEach(x => shortenedGenericParams.Add(GetOnlyTypeName(x)));

            return ReconstructShortenedGenericTypeName(genericType, shortenedGenericParams);
        }
    }
}
