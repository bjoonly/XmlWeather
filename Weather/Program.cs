using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Weather
{
    class Weather
    {

        public string Link { get; set; } = "http://informer.gismeteo.by/rss/codeCity.xml";
        public string FilePath { get; set; } = "CityCodes.txt";
        public void ShowWeather(int codeCity)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Link.Replace("codeCity", codeCity.ToString()));

            XmlNodeList xNodelst = xdoc.DocumentElement.SelectNodes("channel/item");

            foreach (XmlNode xNode in xNodelst)
            {
                Console.WriteLine($"{xNode.SelectSingleNode("title").InnerText}");
                Console.WriteLine($"{xNode.SelectSingleNode("description").InnerText}");
            }
        }
        public void AddNewCodeCity(int codeCity, string city)
        {
            string line = "";
            using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
            {
                line = sr.ReadToEnd();
            }

            if (line.Contains(codeCity.ToString()) == false)
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true, Encoding.Default)) 
                {                
                    sw.WriteLine(codeCity + " " + city);
                }

            }
        }

        public void SaveFile(int codeCity)
        {
            int count = 0;
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Link.Replace("codeCity", codeCity.ToString()));
            while (File.Exists($"{codeCity}_{count}.xml"))
            {
                count++;
            }

            xdoc.Save($"{codeCity}_{count}.xml");
        }
        public void ShowCityCodes()
        {
            using (StreamReader sw = File.OpenText(FilePath))
            {
                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    if (line != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
        public void WarmestWeather()
        {
            int min, max;
            string line;
            double avgRes = 0, avg;
            int codeCity;
            string res = "";
            Regex regex = new Regex(@"\d{1,}");
            Regex redexCity = new Regex(@"[a-яА-Я].+");
            XmlDocument xdoc = new XmlDocument();
            using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    codeCity = Convert.ToInt32(regex.Match(line).ToString());
                    xdoc.Load(Link.Replace("codeCity", codeCity.ToString()));
                    XmlNode xNode = xdoc.DocumentElement.SelectSingleNode("channel/item/description");
                    Match match = Regex.Match(xNode.InnerText, @"\d{1,2}");
                    min = Convert.ToInt32(match.ToString());
                    max = Convert.ToInt32(match.NextMatch().ToString());
                    avg = ((double)min + max) / 2;
                    if (avgRes < avg)
                    {
                    
                        res = redexCity.Match(line).ToString();
                        avgRes = avg;
                    }
                }
            }
            Console.WriteLine($"Warmest weather in {res}\nAverage temp: {avgRes}");
        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Weather weather = new Weather();
                weather.WarmestWeather();
                weather.AddNewCodeCity(33815, "Кишинев");
                weather.AddNewCodeCity(37549, "Тбилиси");
                Console.WriteLine("\nПогода в Кишиневе ");
                weather.ShowWeather(33815);
                Console.WriteLine("\nПогода в Тбилиси");
                weather.ShowWeather(37549);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
