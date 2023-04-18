using System.Security.Cryptography.X509Certificates;

List<byte[]> listOfArrays = new List<byte[]>();
byte[] byteArrayToCheck = new byte[] { 0x01, 0x02, 0x03 };

// Add some byte arrays to the list
listOfArrays.Add(new byte[] { 0x01, 0x02, 0x03 });
listOfArrays.Add(new byte[] { 0x04, 0x05, 0x06 });

// Check whether byteArrayToCheck is in the list
if (listOfArrays.Any(x=>x.SequenceEqual(byteArrayToCheck)))
{
    Console.WriteLine("The byte array is in the list.");
}
else
{
    Console.WriteLine("The byte array is not in the list.");
}
