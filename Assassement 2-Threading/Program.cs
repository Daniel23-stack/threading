
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Assassement_2_Threading;

public class Program
{
    //declaring variables
    private static List<int> globalList = new List<int>();
    private static int itemCount = 0;
    private static readonly object lockObject = new object();
    private static AutoResetEvent thirdThreadReady = new AutoResetEvent(false);
    
    
    
    static void Main()
    {
        Thread oddThread = new Thread(GenerateOddNumbers);
        Thread primeThread = new Thread(GeneratePrimeNumbers);
        
        //starting the threads
        oddThread.Start();
        primeThread.Start();
        
        
        while (itemCount < 250000) ;

        thirdThreadReady.Set();

        Thread evenThread = new Thread(GenerateEvenNumbers);
        evenThread.Start();

        while (itemCount < 1000000) ;
        
        // Sorting the global list to be able to read the numbers
        lock (lockObject)
        {
            globalList.Sort();
        }
        
        // Counting and displaying the number of odd and even numbers
        int oddCount = globalList.Count(n => n % 2 != 0);
        int evenCount = globalList.Count(n => n % 2 == 0);
        Console.WriteLine($"Number of odd numbers: {oddCount}");
        Console.WriteLine($"Number of even numbers: {evenCount}");
        
        // Serialize the list to binary and XML files
        SerializeToBinary(globalList, "globalListBinary.dat");
        SerializeToXml(globalList, "globalListXml.xml");

        Console.WriteLine("Total items in the global list: " + itemCount);
    }

    //function to serialize to xml
    private static void SerializeToXml(List<int> list, string filePath)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<int>));
            serializer.Serialize(stream, list);
        }
    }

    
    //function to serialize to binary
    private static void SerializeToBinary(List<int> list,  string filePath)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, list);
        }
    }

    
    //function to generate Even numbers
    private static void GenerateEvenNumbers()
    {
        Random rand = new Random();

        while (itemCount < 1000000)
        {
            int num = rand.Next(2, int.MaxValue);
            if (num % 2 == 0)
            {
                lock (lockObject)
                {
                    globalList.Add(num);
                    itemCount++;
                }
            }
        }
    }

    
    //function to generate prime numbers
    private static void GeneratePrimeNumbers()
    {
        int primeCandidate = 2;

        while (itemCount < 250000)
        {
            if (IsPrime(primeCandidate))
            {
                lock (lockObject)
                {
                    globalList.Add(-primeCandidate);
                    itemCount++;
                }
            }
            primeCandidate++;
        }

        thirdThreadReady.WaitOne();
    }

    //function to generate oodNumbers
    private static void GenerateOddNumbers()
    {
        Random rand = new Random();

        while (itemCount < 250000)
        {
            int num = rand.Next(1, int.MaxValue);
            if (num % 2 != 0)
            {
                lock (lockObject)
                {
                    globalList.Add(num);
                    itemCount++;
                }
            }
        }
    }
    
    //function to check if its prime
    static bool IsPrime(int num)
    {
        if (num <= 1) return false;
        if (num <= 3) return true;
        if (num % 2 == 0 || num % 3 == 0) return false;

        int i = 5;
        while (i * i <= num)
        {
            if (num % i == 0 || num % (i + 2) == 0) return false;
            i += 6;
        }

        return true;
    }
}