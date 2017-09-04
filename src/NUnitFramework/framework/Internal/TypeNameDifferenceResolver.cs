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
    public class TypeNameDifferenceResolver
    {
        private TypeNameDifferenceUtils _helper;

        /// <summary>
        /// Instantiate a <see cref="TypeNameDifferenceResolver"/>.
        /// </summary>
        public TypeNameDifferenceResolver()
        {
            _helper = new TypeNameDifferenceUtils();
        }
        
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

            ResolveTypeNameDifference(expectedFullType, actualFullType, out expectedType, out actualType);
        }

        /// <summary>
        /// Gets the unique type name between expected and actual.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="expectedShortened"></param>
        /// <param name="actualShortened"></param>
        public void ResolveTypeNameDifference(string expected, string actual, out string expectedShortened, out string actualShortened)
        {
            if (_helper.IsObjectTypeGeneric(expected) && _helper.IsObjectTypeGeneric(actual))
            {
                string shortenedTopLevelGenericExpected, shortenedTopLevelGenericActual;
                GetShortenedTopLevelGenericTypes(expected, actual, out shortenedTopLevelGenericExpected, out shortenedTopLevelGenericActual);

                List<string> shortenedParamsExpected, shortenedParamsActual;
                GetShortenedGenericParams(expected, actual, out shortenedParamsExpected, out shortenedParamsActual);

                expectedShortened = _helper.ReconstructShortenedGenericTypeName(shortenedTopLevelGenericExpected, shortenedParamsExpected);
                actualShortened = _helper.ReconstructShortenedGenericTypeName(shortenedTopLevelGenericActual, shortenedParamsActual);
            }
            else if (_helper.IsObjectTypeGeneric(expected) || _helper.IsObjectTypeGeneric(actual))
            {
                if (_helper.IsObjectTypeGeneric(expected))
                {
                    expectedShortened = _helper.FullyShortenTypeName(expected);
                    actualShortened = _helper.FullyShortenTypeName(actual);
                }
                else
                {
                    expectedShortened = _helper.FullyShortenTypeName(expected);
                    actualShortened = _helper.FullyShortenTypeName(actual);
                }
            }
            else
            {
                _helper.ShortenTypeNames(expected, actual, out expectedShortened, out actualShortened);
            }
        }

        private void GetShortenedGenericParams(string expectedFullType, string actualFullType, out List<string> shortenedParamsExpected, out List<string> shortenedParamsActual)
        {
            List<string> templateParamsExpected = _helper.GetTopLevelFullyQualifiedGenericParameters(expectedFullType);
            List<string> templateParamsActual = _helper.GetTopLevelFullyQualifiedGenericParameters(actualFullType);

            shortenedParamsExpected = new List<string>();
            shortenedParamsActual = new List<string>();
            while ((templateParamsExpected.Count > 0) && (templateParamsActual.Count > 0))
            {
                string shortenedExpected, shortenedActual;
                ResolveTypeNameDifference(templateParamsExpected[0], templateParamsActual[0], out shortenedExpected, out shortenedActual);
                //ShortenTypeNames

                shortenedParamsExpected.Add(shortenedExpected);
                shortenedParamsActual.Add(shortenedActual);

                templateParamsExpected.RemoveAt(0);
                templateParamsActual.RemoveAt(0);
            }

            foreach (string genericParamRemaining in templateParamsExpected)
            {
                shortenedParamsExpected.Add(_helper.FullyShortenTypeName(genericParamRemaining));
            }

            foreach (string genericParamRemaining in templateParamsActual)
            {
                shortenedParamsActual.Add(_helper.FullyShortenTypeName(genericParamRemaining));
            }


        }

        private void GetShortenedTopLevelGenericTypes(string expectedFullType, string actualFullType, out string shortenedTopLevelGenericExpected, out string shortenedTopLevelGenericActual)
        {
            string toplevelGenericExpected = _helper.GetTopLevelGenericType(expectedFullType);
            string toplevelGenericActual = _helper.GetTopLevelGenericType(actualFullType);
            _helper.ShortenTypeNames(
                toplevelGenericExpected,
                toplevelGenericActual,
                out shortenedTopLevelGenericExpected,
                out shortenedTopLevelGenericActual);
        }

        

        
    }

    /// <summary>
    /// Class to provide generic utilities to assist <see cref="TypeNameDifferenceResolver"/>.
    /// </summary>
    public class TypeNameDifferenceUtils
    {
        /// <summary>
        /// Shorten a fully qualified generic type name to only the names of the 
        /// </summary>
        /// <param name="FullyQualifiedGenericType">The fully qualified name of a generic type.</param>
        public string FullyShortenTypeName(string FullyQualifiedGenericType)
        {
            if (IsObjectTypeGeneric(FullyQualifiedGenericType))
            {
                string genericType = GetOnlyTypeName(GetTopLevelGenericType(FullyQualifiedGenericType));

                List<string> genericParams = GetTopLevelFullyQualifiedGenericParameters(FullyQualifiedGenericType);
                List<string> shortenedGenericParams = new List<string>();
                genericParams.ForEach(x => shortenedGenericParams.Add(FullyShortenTypeName(x)));

                return ReconstructShortenedGenericTypeName(genericType, shortenedGenericParams);
            }
            else
            {
                return GetOnlyTypeName(FullyQualifiedGenericType);
            }
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
        /// <param name="ObjectTypeName">The fully qualified type name to check if is generic.</param>
        public bool IsObjectTypeGeneric(string ObjectTypeName)
        {
            return ObjectTypeName.Contains("`");
        }

        /// <summary>
        /// Get the fully qualified name of the generic parameters of a given object.
        /// </summary>
        /// <param name="FullyQualifiedObjectType">The fully qualified name of the generic type.</param>
        public List<string> GetTopLevelFullyQualifiedGenericParameters(string FullyQualifiedObjectType)
        {

            var regexTopLevel = new Regex(@"\[(.+)\]\z");
            var genericTopLevel = regexTopLevel.Match(FullyQualifiedObjectType).Groups[1].Value;

            //var matches = new Regex(@"([^,\[\]]+)(,|\Z)|([^,\[\]]+\[.+\])(,|\Z)").Matches(genericTopLevel);
            //List<string> rejoinedNestedGenerics = new List<string>();
            //foreach (Match match in matches)
            //{
            //    rejoinedNestedGenerics.Add(match.Value.TrimEnd(','));
            //}

            List<string> split = new List<string>(genericTopLevel.Split(','));
            List<string> rejoinedNestedGenerics = new List<string>();
            //Rejoin nested generic types.
            List<string> nestedGenerics = new List<string>();


            foreach (string token in split)
            {
                if (token.Contains("[") || token.Contains("]"))
                {
                    nestedGenerics.Add(token);

                    if (TokensContainFullyEncapsulatedGeneric(nestedGenerics))
                    {
                        rejoinedNestedGenerics.Add(string.Join(",", nestedGenerics.ToArray()));
                        nestedGenerics.Clear();
                    }
                }
                else
                {
                    rejoinedNestedGenerics.Add(token);
                }
            }

            if (nestedGenerics.Count > 0)
            {
                if (TokensContainFullyEncapsulatedGeneric(nestedGenerics))
                {
                    rejoinedNestedGenerics.Add(string.Join(",", nestedGenerics.ToArray()));
                    nestedGenerics.Clear();
                }
                else
                {
                    throw new InvalidProgramException("This should never occur, also write a better msg here.");
                }
            }

            return rejoinedNestedGenerics;
        }

        /// <summary>
        /// Determine whether or not the expression contains the full definition of its generic parameters.
        /// </summary>
        public bool TokensContainFullyEncapsulatedGeneric(List<string> tokens)
        {
            string joined = string.Join("", tokens.ToArray());

            int leftBrackets = new Regex(@"\[").Matches(joined).Count;
            int rightBrackets = new Regex(@"\]").Matches(joined).Count;

            return leftBrackets == rightBrackets;
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
    }
}
