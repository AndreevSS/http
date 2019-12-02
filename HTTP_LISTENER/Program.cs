using System;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using static System.Convert;

namespace HTTP_LISTENER
{
    class Program
    {
        // This is the class that will be deserialized.
       
        public class MySerializableClass
        {
            string CustomerID;
            string CompanyName;
            string ContactName;
            string ContactTitle;
            string Address;
            string City;
            string PostalCode;
            string Country;
            string Phone;
            string Fax;

        }
        public static void PrintKeysAndValues(NameValueCollection myCol)
        {
            Console.WriteLine("   KEY        VALUE");
            foreach (String s in myCol.AllKeys)
                Console.WriteLine("   {0,-10} {1}", s, myCol[s]);
            Console.WriteLine();
        }

        public static int FindXML(NameValueCollection myCol)
        {
            Console.WriteLine("   KEY        VALUE");
            int i = 0;
            foreach (String s in myCol.AllKeys)
            {
                if (s == "xml")
                {

                    Console.WriteLine("XML FOUND");
                    return i;
                }
                i++;
                //   Console.WriteLine();
            }
            return -1;
        }


        static void Main(string[] args)
        {


            if (Int32.TryParse("105", out int j))
                Console.WriteLine(j);
            else
                Console.WriteLine("String could not be parsed.");


            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("C:\\data\\test.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //             Console.WriteLine(line); line



                    HttpListener listener = new HttpListener();
                    // установка адресов прослушки
                    listener.Prefixes.Add("http://localhost:8888/connection/");
                    listener.Start();
                    Console.WriteLine("Ожидание подключений...");



                    while (true)
                    {
                        // метод GetContext блокирует текущий поток, ожидая получение запроса 
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;



                        // получаем объект ответа
                        HttpListenerResponse response = context.Response;
                        // создаем ответ в виде кода html
                        string responseStr = "<html><head><meta charset='utf8'></head><body>Привет мир!</body></html>";
                        //        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(line);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(line);

                        Console.WriteLine("URL: {0}", request.Url.OriginalString);
                        Console.WriteLine("Raw URL: {0}", request.RawUrl);

                        Console.WriteLine("method: {0}", request.HttpMethod);

                        Console.WriteLine("{0} request was caught: {1}",
                        request.HttpMethod, request.Url);


                        Console.WriteLine("Query: {0}", request.QueryString);
                        PrintKeysAndValues(request.QueryString);


                        int a = FindXML(request.QueryString);

                        if (a > 0)
                        {
                            Console.WriteLine("xml id = {0}", a);


                            MySerializableClass myObject = new MySerializableClass();
                            // Insert code to set properties and fields of the object.  
                            XmlSerializer mySerializer = new
                            XmlSerializer(typeof(MySerializableClass));
                            // To write to a file, create a StreamWriter object.  
                            StreamWriter myWriter = new StreamWriter("myFileName.xml");
                            mySerializer.Serialize(myWriter, myObject);
                            myWriter.Close();


                        }




                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                    // останавливаем прослушивание подключений
                    listener.Stop();
                    Console.WriteLine("Обработка подключений завершена");
                    Console.Read();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }


        }
    }
}