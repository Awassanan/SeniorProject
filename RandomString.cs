namespace SeniorProject
{
    class RandomString
    {
        private static Random random = new Random();

        private static string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        // private static string chars = "^(?=.*[a-z])(?=."
        //             + "*[A-Z])(?=.*\\d)"
        //             + "(?=.*[-+_!@#$%^&*., ?]).+$";

        public static string CreateRandomString(int length)
        {
            return (new String(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()).ToString());
        }
    }
}