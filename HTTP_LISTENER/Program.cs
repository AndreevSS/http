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
    public class MySerializableClass
    {
   
      public  string CustomerID;
   
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

                public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
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

                            request.QueryString.Get(a);


                            // Declare an object variable of the type to be deserialized.
                            XmlRootAttribute xRoot = new XmlRootAttribute();
                            xRoot.ElementName = "Customer";
                            // xRoot.Namespace = "http://www.cpandl.com";
                            xRoot.IsNullable = true;

                            MySerializableClass myObject = new MySerializableClass();
                            // Insert code to set properties and fields of the object.  
                            XmlSerializer mySerializer = new
                            XmlSerializer(typeof(MySerializableClass), xRoot);
                            // To write to a file, create a StreamWriter object.  
                            //                StreamWriter myWriter = new StreamWriter("myFileName.xml");
                            //  mySerializer.Serialize(request.QueryString.Get(a), myObject);

                            Stream mystream = ToStream(request.QueryString.Get(a));
                            myObject = (MySerializableClass)mySerializer.Deserialize(mystream);
                                 //               myWriter.Close();


                            Console.Write(
                                myObject.CustomerID + "\t");
                                                     //   }
                        /**/

                        /*MySerializableClass myObject;
                        // Construct an instance of the XmlSerializer with the type  
                        // of object that is being deserialized.  
                        XmlSerializer mySerializer =
                        new XmlSerializer(typeof(MySerializableClass));
                        // To read the file, create a FileStream.  
                        FileStream myFileStream =
                        new FileStream("myFileName.xml", FileMode.Open);
                        // Call the Deserialize method and cast to the object type.  
                        myObject = (MySerializableClass)
                        mySerializer.Deserialize(myFileStream);
                        */
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