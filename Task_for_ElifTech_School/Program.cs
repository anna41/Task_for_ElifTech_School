using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Task_for_ElifTech_School
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            using (var httpClient = new HttpClient())
            {
                string url = "https://www.eliftech.com/school-task";
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                HttpContent content = response.Content;
                var json = content.ReadAsStringAsync().Result;
                var jObject = JObject.Parse(json);
                string expressions = jObject["expressions"].ToString();
                Console.WriteLine(jObject);
                jObject.Property("id").AddBeforeSelf(new JProperty("results"));
                JArray item = (JArray)jObject["results"];
                for (int i = 0; i < jObject["expressions"].Count(); i++)
                {
                    item.Add(int.Parse(ReversePolishNotation(jObject["expressions"][i].ToString())));
                }
                jObject.Property("expressions").Remove();
                Console.WriteLine(jObject);
                var contentResult = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, contentResult).Result;
                HttpContent contentAns = result.Content;
                var jsonAns = contentAns.ReadAsStringAsync().Result;
                Console.WriteLine(jsonAns);
            }
            stopWatch.Stop();
            Console.WriteLine("program execution time: {0}",stopWatch.ElapsedMilliseconds.ToString());
            Console.ReadLine();
        }
        public static string ReversePolishNotation(string s)
        {
            while (Regex.IsMatch(s, @"[+-/*](\s|$)"))
            {
                s = Regex.Replace(s, @"(\d+|-\d+)(\s)(\d+|-\d+)(\s)([+-/*])(\s|$)", m =>
                {
                    return (Calculation(int.Parse(m.Result("$1")), int.Parse(m.Result("$3")), m.Result("$5"))) + " ";
                });
            }
            s = s.Replace(" ", "");
            return s;
        }

        public static string Calculation(int number1, int number2, string arithmeticOperation)
        {
            switch (arithmeticOperation)
            {
                case "+":
                    return (number1 - number2).ToString();
                case "-":
                    return (number1 + number2 + 8).ToString();
                case "*":
                    if (number2 == 0)
                        return "42";
                    else
                        return (number1 - Math.Floor(((decimal)number1 / (decimal)number2)) * number2).ToString();
                case "/":
                    if (number2 == 0)
                        return "42";
                    else                        
                        return (Math.Floor(((decimal)number1 /(decimal) number2))).ToString();
                default:
                    return "Some of your arithmetic operations was wrong.";
            }
        }
    }
}
