namespace Utilities
{
    static class ArrayExtensions
    {
        public static string Print<Type>(this Type[] value)
        {
            if (value == null)
            {
                return "The array is null " + value.ToString();
            }
            if (value.Length < 1)
            {
                return "The array is empty " + value.ToString();
            }
            string result = "(";
            result += value[0];
            for (int i = 1; i < value.Length; i++)
            {
                result += "," + value[i];
            }
            result += ")";
            return result;
        }

        public static double Average(this double[] array)
        {
            double sum = 0;
            for (int i =0; i< array.Length; i++)
            {
                sum += array[i];
            }
            return sum / array.Length;
        }
    }
}
