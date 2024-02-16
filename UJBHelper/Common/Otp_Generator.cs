using System;
using System.Linq;

namespace UJBHelper.Common
{
    public static class Otp_Generator
    {
        public static string Generate_Otp()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Get_Google_Api_Key()
        {
            return "AIzaSyC6VxPVTfH1rZuNkTmG9e7PVmqx5njMUF0";
        }

        private static Random rng = new Random(Environment.TickCount);

        public static string GetNumber(object objlength)
        {
            var number = "";
            for (int index = 0; index < 20; index++)
            {
                int length = Convert.ToInt32(objlength);
                 number = rng.NextDouble().ToString("0.000000000000").Substring(2, length);               
            }
            return number;

        }

        //static Random random = new Random();

        //// Note, max is exclusive here!
        //public static List<int> GenerateRandom(int count, int min, int max)
        //{

        //    if (max <= min || count < 0 ||
        //            (count > max - min && max - min > 0))
        //    {

        //        throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
        //                " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
        //    }

        //    // generate count random values.
        //    HashSet<int> candidates = new HashSet<int>();

        //    // start count values before max, and end at max
        //    for (int top = max - count; top < max; top++)
        //    {               
        //        if (!candidates.Add(random.Next(min, top + 1)))
        //        {                   
        //            candidates.Add(top);
        //        }
        //    }

        //    // load them in to a list, to sort
        //    List<int> result = candidates.ToList();

        //    // shuffle the results because HashSet has messed
        //    // with the order, and the algorithm does not produce
        //    // random-ordered results (e.g. max-1 will never be the first value)
        //    for (int i = result.Count - 1; i > 0; i--)
        //    {
        //        int k = random.Next(i + 1);
        //        int tmp = result[k];
        //        result[k] = result[i];
        //        result[i] = tmp;
        //    }
        //    return result;
        //}
    }
}
