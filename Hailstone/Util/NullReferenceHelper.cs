using System;

namespace Hailstone.Util
{
    public static class NullReferenceHelper
    {
        public static string ToStringSafe(this object obj, string defaultString = null)
        {
            if (obj == null)
                return defaultString;
            else
                return obj.ToString();
        }

        public static T CheckNull<T>(this T test, T whenNullReference)
        {
            if (test == null)
                return whenNullReference;
            else
                return test;
        }

        public static TResult SafeExec<T, TResult>(this T obj, Func<T, TResult> runner, TResult result)
        {
            if (obj != null)
                return runner(obj);
            else
                return result;
        }

        public static TResult SafeExec<T, TResult>(this T obj, Func<T, TResult> runner, Func<TResult> result)
        {
            if (obj != null)
                return runner(obj);
            else
                return result();
        }
    }
}