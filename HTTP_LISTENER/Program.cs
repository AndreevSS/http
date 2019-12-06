using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Threading;

//using static System.Convert;

namespace HTTP_LISTENER
{
    public class CustomerObject
    {
   
        public string CustomerID;
        public string CompanyName;
        public string ContactName;
        public string ContactTitle;
        public string Address;
        public string City;
        public string PostalCode;  
        public string Country; 
        public string Phone;      
        public string Fax;

    }

 
    public static class Program
    {
        // This is the class that will be deserialized.

        public class ListenerObject
        {
            public static void CreateListener(int port)
            {

                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }


                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:" + port + "/connection/");
                listener.Start();
                Console.WriteLine(port + ": Ожидание подключений...");

                //      HttpListenerRequest request = context.Request;
                //      HttpListenerResponse response = context.Response;

                while (true)
                {
                    // метод GetContext блокирует текущий поток, ожидая получение запроса 
                    HttpListenerContext context = listener.GetContext();
                    ConnectionInfo(context.Request);

                    switch (Path(context.Request.RawUrl))
                    {
                        default: StringFromXML(context, "CustomerID"); break;
                        case "/connection/CompanyName/": StringFromXML(context, "CompanyName"); break;
                        case "/connection/CustomerName/": StringFromXML(context, "CustomerName"); break;
                    }

                    Thread.Sleep(0);

                }

            }

        }



            public static void CreateListener(int port)
        {

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }


            HttpListener listener = new HttpListener();
            try
            {
                listener.Prefixes.Add("http://localhost:" + port + "/");
                listener.Start();
                Console.WriteLine("http://localhost:" + port + ": Ожидание подключений...");
                Thread.Sleep(0);



            //      HttpListenerRequest request = context.Request;
            //      HttpListenerResponse response = context.Response;

            while (true)
            {
                // метод GetContext блокирует текущий поток, ожидая получение запроса 
                HttpListenerContext context = listener.GetContext();
                ConnectionInfo(context.Request);

                switch (Path(context.Request.RawUrl))
                {
                    default: StringFromXML(context, "CustomerID"); break;
                    case "/connection/CompanyName/": StringFromXML(context, "CompanyName"); break;
                    case "/connection/CustomerName/": StringFromXML(context, "CustomerName"); break;
                }

                Thread.Sleep(0);

            }

            }
            catch (HttpListenerException e)
            {
                Console.WriteLine(port + "/ Подключение не создано");
                Console.WriteLine(e.Message);
            }

            finally
            {
          //      listener.Stop();
            }

            //  Console.WriteLine("ThreadProc: {0}", i);
            // Yield the rest of the time slice.

            // останавливаем прослушивание подключений
    /*        listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
            Console.Read();*/
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
            int i = 0;
            foreach (String s in myCol.AllKeys)
            {
                if (s == "xml")
                {

                    Console.WriteLine("XML POS FOUND: {0}", i);
                    return i;
                }
                i++;
                //   Console.WriteLine();
            }
            return -1;
        }

        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static String OpenFile(string file)
        {
            try { using (StreamReader sr = new StreamReader(file))
                {
                    String fileString = sr.ReadToEnd();
                    return fileString;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static void  ConnectionInfo(HttpListenerRequest request)
        {
            Console.WriteLine("URL: {0}", request.Url.OriginalString);
            Console.WriteLine("Raw URL: {0}", request.RawUrl);
            Console.WriteLine("Path: {0}", Path(request.RawUrl));
            Console.WriteLine("method: {0}", request.HttpMethod);
            Console.WriteLine("{0} request was caught: {1}",
            request.HttpMethod, request.Url);
            Console.WriteLine("Query: {0}", request.QueryString);
            PrintKeysAndValues(request.QueryString);
        }

        public static string Path (string url)
        {
            if (url.IndexOf('?') > 0)
            url = url.Substring(0, url.IndexOf('?'));
            return url;
        }

        static void StringFromXML(HttpListenerContext context, String ObjectFieldType)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

           int XMLpos = FindXML(request.QueryString);
            String ObjectField;
            String ResponseString = "XML NOT FOUND";

            if (XMLpos >= 0)
            {
                request.QueryString.Get(XMLpos);
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = "Customer";
                xRoot.IsNullable = true;
                CustomerObject myObject = new CustomerObject();
                XmlSerializer mySerializer = new
                XmlSerializer(typeof(CustomerObject), xRoot);
                Stream mystream = ToStream(request.QueryString.Get(XMLpos));
                myObject = (CustomerObject)mySerializer.Deserialize(mystream);

                switch (ObjectFieldType)
                {
                    default: ObjectField = myObject.CustomerID; break;
                    case "CompanyName": ObjectField = myObject.CompanyName; break;
                    case "CustomerName": ObjectField = myObject.ContactName; break;

                }

                ResponseString = ObjectFieldType + " = " + ObjectField;
            }
 
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ResponseString);

        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        }


 


        static void Main(string[] args)
        {



      //      Thread t = new Thread(new ThreadStart(ThreadProc));

            ArrayList ThreadList = new ArrayList();

      ///      Thread Thread1 = new Thread(ListenerObject.CreateListener));

           /* List<string>[] list;
            List<Thread> threads = new List<Thread>();*/


            //    list = dbConnect.Select();

            int size =  30;

            for (int i = 0; i < size; i++)
            {
                int port = 8000 + i;
                Thread th = new Thread(() => {
                    CreateListener(port);
                    //calling callback function
                });
                th.Name = "Thread_" + i;
                th.Start();
                ThreadList.Add(th);
            }

          /*  for (int i = 8000; i < 8000+size; i++)
            {
                ListenerObject ListenerObject = new ListenerObject();
                ThreadList.Add(new Thread(ListenerObject.CreateListener));

            }*/
        }
    }
}