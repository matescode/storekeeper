using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonBase.Utils
{
    public static class ArgumentValidator
    {
        #region null

        public static void IsNotNull(string argumentName, object argument, string message = null)
        {
            if (argument == null)
            {
                ThrowArgumentNullException(argumentName, message);
            }
        }

        public static void IsNotNull(string argumentName, object argument, Func<bool> nextCondition, string message = null)
        {
            if (argument == null || !nextCondition())
            {
                ThrowArgumentNullException(argumentName, message);
            }
        }

        public static void IsNotNullOrEmpty(string argumentName, string argument, string message = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                message = message ?? "Value of the argument is null or empty string.";
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsNotNullOrEmpty<T>(string argumentName, IEnumerable<T> argument, string message = null)
        {
            if (argument == null || !argument.Any())
            {
                message = message ?? "Collection is null or contains no elements.";
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsNotNullOrEmpty(string argumentName, ObjectId argument, string message = null)
        {
            if (ObjectId.IsNullOrEmpty(argument))
            {
                message = message ?? "Value of the argument is null or empty ObjectId.";
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsNotNullOrWhiteSpace(string argumentName, string argument, string message = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                message = message ?? "Specified argument is null or white-space only.";
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void ElementsNotNull(string argumentName, IEnumerable<object> argument, string message = null)
        {
            if (argument == null)
            {
                ThrowArgumentNullException(argumentName, message);
            }
            else if (argument.Any(arg => arg == null))
            {
                message = message ?? "Array contains 'null' element(s).";
                throw new ArgumentException(message, argumentName);
            }
        }

        #endregion

        #region greater

        public static void IsGreaterThan<T>(string argumentName, T argument, T lowerBound, string message = null)
            where T : IComparable<T>
        {
            IsGreaterThan(argumentName, argument, lowerBound, Comparer<T>.Default, message);
        }

        public static void IsGreaterThan<T>(string argumentName, T argument, T lowerBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, lowerBound) <= 0)
            {
                message = message ?? string.Format("Argument value has to be greater than {0}.", lowerBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        public static void IsGreaterThanOrEqual<T>(string argumentName, T argument, T lowerBound, string message = null)
            where T : IComparable<T>
        {
            IsGreaterThanOrEqual(argumentName, argument, lowerBound, Comparer<T>.Default, message);
        }

        public static void IsGreaterThanOrEqual<T>(string argumentName, T argument, T lowerBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, lowerBound) < 0)
            {
                message = message ?? string.Format("Argument value has to be greater than or equal to {0}.", lowerBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        #endregion

        #region less

        public static void IsLessThan<T>(string argumentName, T argument, T upperBound, string message = null)
            where T : IComparable<T>
        {
            IsLessThan(argumentName, argument, upperBound, Comparer<T>.Default, message);
        }

        public static void IsLessThan<T>(string argumentName, T argument, T upperBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, upperBound) >= 0)
            {
                message = message ?? string.Format("Argument value has to be less than {0}.", upperBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        public static void IsLessThanOrEqual<T>(string argumentName, T argument, T upperBound, string message = null)
            where T : IComparable<T>
        {
            IsLessThanOrEqual(argumentName, argument, upperBound, Comparer<T>.Default, message);
        }

        public static void IsLessThanOrEqual<T>(string argumentName, T argument, T upperBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, upperBound) > 0)
            {
                message = message ?? string.Format("Argument value has to be less than or equal to {0}.", upperBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        #endregion

        #region equal & same

        public static void IsEqual<T>(string argumentName, T argument, T otherValue, string message = null)
        {
            EqualImpl(true, argumentName, argument, otherValue, null, message ?? string.Format("Argument value is not equal to '{0}'.", otherValue));
        }

        public static void IsEqual<T>(string argumentName, T argument, T otherValue, IEqualityComparer<T> comparer, string message = null)
        {
            EqualImpl(true, argumentName, argument, otherValue, comparer, message ?? string.Format("Argument value is not equal to '{0}'.", otherValue));
        }

        public static void IsNotEqual<T>(string argumentName, T argument, T otherValue, string message = null)
        {
            EqualImpl(false, argumentName, argument, otherValue, null, message ?? string.Format("Argument value is equal to '{0}'.", otherValue));
        }

        public static void IsNotEqual<T>(string argumentName, T argument, T otherValue, IEqualityComparer<T> comparer, string message = null)
        {
            EqualImpl(false, argumentName, argument, otherValue, comparer, message ?? string.Format("Argument value is equal to '{0}'.", otherValue));
        }

        private static void EqualImpl<T>(bool expectedResult, string argumentName, T argument, T otherValue, IEqualityComparer<T> comparer, string message = null)
        {
            if (comparer == null)
            {
                if (Equals(argument, otherValue) != expectedResult)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }
            else
            {
                if (comparer.Equals(argument, otherValue) != expectedResult)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }
        }

        /// <summary>Uses Object.ReferenceEquals().</summary>
        /// <typeparam name="T">Type of argument.</typeparam>
        /// <param name="argumentName">Name of argument.</param>
        /// <param name="argument">Value of argument.</param>
        /// <param name="otherValue">Value to test with.</param>
        /// <param name="message">Message for ArgumentException that is throw in case condition is not met.</param>
        public static void IsSame<T>(string argumentName, T argument, T otherValue, string message = null)
        {
            if (!ReferenceEquals(argument, otherValue))
            {
                message = message ?? string.Format("Argument value is not same as {0}", otherValue);
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>Uses Object.ReferenceEquals().</summary>
        /// <typeparam name="T">Type of argument.</typeparam>
        /// <param name="argumentName">Name of argument.</param>
        /// <param name="argument">Value of argument.</param>
        /// <param name="otherValue">Value to test with.</param>
        /// <param name="message">Message for ArgumentException that is throw in case condition is not met.</param>
        public static void IsNotSame<T>(string argumentName, T argument, T otherValue, string message = null)
        {
            if (ReferenceEquals(argument, otherValue))
            {
                message = message ?? string.Format("Argument value is same as {0}", otherValue);
                throw new ArgumentException(message, argumentName);
            }
        }

        #endregion

        #region in range

        public static void IsInRange<T>(string argumentName, T argument, T lowerBound, T upperBound, bool inclusive = true, string message = null)
            where T : IComparable<T>
        {
            if (inclusive)
            {
                IsInRangeInclusive(argumentName, argument, lowerBound, upperBound, Comparer<T>.Default, message);
            }
            else
            {
                IsInRangeExclusive(argumentName, argument, lowerBound, upperBound, Comparer<T>.Default, message);
            }
        }

        public static void IsInRange<T>(string argumentName, T argument, T lowerBound, T upperBound, IComparer<T> comparer, bool inclusive = true, string message = null)
            where T : IComparable<T>
        {
            if (inclusive)
            {
                IsInRangeInclusive(argumentName, argument, lowerBound, upperBound, comparer, message);
            }
            else
            {
                IsInRangeExclusive(argumentName, argument, lowerBound, upperBound, comparer, message);
            }
        }

        public static void IsInRangeInclusive<T>(string argumentName, T argument, T lowerBound, T upperBound, string message = null)
            where T : IComparable<T>
        {
            IsInRangeInclusive(argumentName, argument, lowerBound, upperBound, Comparer<T>.Default, message);
        }

        public static void IsInRangeInclusive<T>(string argumentName, T argument, T lowerBound, T upperBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, lowerBound) < 0 || comparer.Compare(argument, upperBound) > 0)
            {
                message = message ?? string.Format("Argument value has to be greater than or equal to {0} and less than or equal to {1}. [{0},{1}]", lowerBound, upperBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        public static void IsInRangeExclusive<T>(string argumentName, T argument, T lowerBound, T upperBound, string message = null)
            where T : IComparable<T>
        {
            IsInRangeExclusive(argumentName, argument, lowerBound, upperBound, Comparer<T>.Default, message);
        }

        public static void IsInRangeExclusive<T>(string argumentName, T argument, T lowerBound, T upperBound, IComparer<T> comparer, string message = null)
            where T : IComparable<T>
        {
            if (comparer.Compare(argument, lowerBound) <= 0 || comparer.Compare(argument, upperBound) >= 0)
            {
                message = message ?? string.Format("Argument value has to be greater than {0} and less than {1}. ({0},{1})", lowerBound, upperBound);
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        #endregion

        public static void IsTrue(string argumentName, bool condition, string message = null)
        {
            if (!condition)
            {
                message = message ?? string.Empty;
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsFalse(string argumentName, bool condition, string message = null)
        {
            if (condition)
            {
                message = message ?? string.Empty;
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsTypeOf(string argumentName, object argument, Type expectedType, string message = null)
        {
            if (argument == null)
            {
                throw new ArgumentException("Cannot validate argument (cannot be null).", "argument");
            }
            if (expectedType == null)
            {
                throw new ArgumentException("Cannot validate argument (cannot be null).", "expectedType");
            }

            if (argument.GetType() != expectedType)
            {
                message = message ?? string.Format("Argument is not of expected type: {0}", expectedType.Name);
                throw new ArgumentException(message, argumentName);
            }
        }

        [Obsolete("Use ArgumentValidator.IsTrue")]
        public static void Check(string argumentName, bool condition, string message = null)
        {
            if (!condition)
            {
                message = message ?? string.Empty;
                throw new ArgumentException(message, argumentName);
            }
        }

        #region I/O

        /// <summary>Checks whether specified value can be used as a file name. Does NOT check whether file exists.</summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argument">Value of the argument.</param>
        /// <param name="message">Optional message.</param>
        public static void IsValidFileName(string argumentName, string argument, string message = null)
        {
            try
            {
                new FileInfo(argument);
            }
            catch (Exception e)
            {
                message = message ?? string.Format("Specified file name is not valid: '{0}'", argument);
                throw new ArgumentException(message, argumentName, e);
            }
        }

        /// <summary>Checks whether specified value can be used as a directory name. Does NOT check whether directory exists.</summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argument">Value of the argument.</param>
        /// <param name="message">Optional message.</param>
        public static void IsValidDirectoryName(string argumentName, string argument, string message = null)
        {
            try
            {
                new DirectoryInfo(argument);
            }
            catch (Exception e)
            {
                message = message ?? string.Format("Specified directory name is not valid: '{0}'", argument);
                throw new ArgumentException(message, argumentName, e);
            }
        }

        #endregion

        public static void IsWellFormedUri(string argumentName, string uri, UriKind kind = UriKind.RelativeOrAbsolute, string message = null)
        {
            if (uri == null || !Uri.IsWellFormedUriString(uri, kind))
            {
                message = message ?? string.Format("Argument is not well-formed-uri (UriKind:{0})", kind);
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>Checks whether specified value is in range [0;65.535] (inclusive).</summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="port">Value of the argument.</param>
        /// <param name="message">Optional message.</param>
        public static void IsValidPort(string argumentName, int port, string message = null)
        {
            IsInRangeInclusive(argumentName, port, 0, 65535, message);
        }

        private static void ThrowArgumentNullException(string argumentName, string message)
        {
            if (message != null)
            {
                throw new ArgumentNullException(argumentName, message);
            }

            throw new ArgumentNullException(argumentName);
        }
    }
}