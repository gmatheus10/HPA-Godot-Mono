using System.IO.IsolatedStorage;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSerial
{
 public void BinarySerialize(object data, string filePath)
 {
  FileStream fileStream;
  BinaryFormatter binaryFormatter = new BinaryFormatter();
  if (File.Exists(filePath))
  {
   fileStream = File.OpenRead(filePath);
  }
  else
  {
   fileStream = File.Create(filePath);
  }

  binaryFormatter.Serialize(fileStream, data);
  fileStream.Close();
 }

 public object BinaryDeserialize(string filePath)
 {
  object obj = null;
  FileStream fileStream;
  BinaryFormatter binaryFormatter = new BinaryFormatter();
  if (File.Exists(filePath))
  {
   fileStream = File.OpenRead(filePath);
   obj = binaryFormatter.Deserialize(fileStream);
   fileStream.Close();
  }
  return obj;
 }
}