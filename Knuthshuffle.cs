namespace SeniorProject
{
    class ShuffleString
    {
        public static string KnuthShuffle<T>(T[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }

            return result;
        }
    }
}