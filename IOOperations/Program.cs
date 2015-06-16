using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace IOOperations
{
    class Program
    {
        /********************Building a directory tree*************************************/
        private static void ListDirectories(DirectoryInfo directoryInfo, string searchPattern, int maxLevel, int currentLevel)
        {
            if (currentLevel >= maxLevel)
            {
                return;
            }

            string indent = new string('-', currentLevel);

            try
            {
                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories(searchPattern); //get dirs with specified search pattern
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    Console.WriteLine(indent + subDirectory.Name);
                    ListDirectories(subDirectory, searchPattern, maxLevel, currentLevel + 1);
                }
            }
            catch(UnauthorizedAccessException)
            {
                //You don't have access to this folder
                Console.WriteLine(indent + "Can't access: " + directoryInfo.Name);
                return;

            }
            catch(DirectoryNotFoundException)
            {
                //The folder was removed while iterating - so now its not there
                Console.WriteLine(indent + "Can't find: " + directoryInfo.Name);
                return;
            }
        }

        private static string ReadAllText()
        {
            string path = @"c:\Temp\readalltest.txt";
            if (File.Exists(path))
            {
                Console.WriteLine("exists");
                return File.ReadAllText(path);
            }
            return string.Empty;
        }

        private static string ReadAllTextTry()
        {
            string path = @"c:\Temp\doesntExist.txt";
            try 
            {
                return File.ReadAllText(path);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Specified directory doesn't exist");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Specified file doesn't exist");
            }
            return string.Empty;
        }

        //Writing asynchronously to a file
        public async Task CreateAndWriteAsynToFIle() //returns Task 'cos no value needs to be returned
        {
            using (FileStream stream = new FileStream("testAsync.dat", FileMode.Create,FileAccess.Write, FileShare.None, 4096, true))
            {
                byte[] data = new byte[100000];
                new Random().NextBytes(data);

                await stream.WriteAsync(data, 0, data.Length);
            }
        }

        //Executing an asynchronous HTTP request
        public async Task ReadAsyncHttpRequest()
        {
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("http://www.microsoft.com");
        }


        //Executing multiple awaits
        public async Task ExecuteMultipleRequests()
        {
            HttpClient client = new HttpClient();
            string microsoft = await client.GetStringAsync("http://www.microsoft.com");
            string msdn = await client.GetStringAsync("http://msdn.microsoft.com");
            string blogs = await client.GetStringAsync("http://blogs.msdn.com/");
        }

        //Executing multiple requests in parallel
        public async Task ExecuteMultipleRequestsInParallel()
        {
            HttpClient client = new HttpClient();
            Task microsoft = client.GetStringAsync("http://www.microsoft.com");
            Task msdn = client.GetStringAsync("http://msdn.microsoft.com");
            Task blogs = client.GetStringAsync("http://blogs.msdn.com/");

            await Task.WhenAll(microsoft, msdn, blogs);
        }
        static void Main(string[] args)
        {
            //Listing drive information
            DriveInfo[] drivesInfo = DriveInfo.GetDrives(); //get drive names of all drives on a computer

/*            foreach (DriveInfo driveInfo in drivesInfo)
            {
                Console.WriteLine("Drive {0}",driveInfo.Name);
                Console.WriteLine("  File type: {0}",driveInfo.DriveType);

                if(driveInfo.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", driveInfo.VolumeLabel);
                    Console.WriteLine("  File system: {0}",driveInfo.DriveFormat);
                    Console.WriteLine(
                        "  Available space to current user:{0,15} bytes",
                        driveInfo.AvailableFreeSpace);
                    Console.WriteLine(
                        "  Total available space:  {0,15} bytes",
                        driveInfo.TotalFreeSpace);
                    Console.WriteLine(
                        "  Total size of drive:     {0,15} bytes",
                        driveInfo.TotalSize);
                }
            }
*/

            //Create a new directory
            var directoryC = Directory.CreateDirectory(@"C:\Temp\ProgrammingInCSharp\Directory");
            var directoryInfoC = new DirectoryInfo(@"C:\Temp\ProgrammingInCSharp\DirectoryInfo");
            directoryInfoC.Create();

            //Deleting an existing directory
            if(Directory.Exists(@"C:\Temp\ProgrammingInCSharp\Directory"))
            {
                Directory.Delete(@"C:\Temp\ProgrammingInCSharp\Directory");
            }

            var directoryInfo = new DirectoryInfo(@"C:\Temp\ProgrammingInCSharp\DirectoryInfo");
            if(directoryInfo.Exists)
            {
                directoryInfo.Delete(); 
            }

            //Setting access control for a directory
            DirectoryInfo directoryInfo1 = new DirectoryInfo("TestDirectory"); //should create this in the bin directory
            directoryInfo1.Create();//create the directory
            DirectorySecurity directorySecurity = directoryInfo1.GetAccessControl(); //get object that encapsulates the access control list
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule("everyone",
                                        FileSystemRights.ReadAndExecute,
                                        AccessControlType.Allow));
            directoryInfo1.SetAccessControl(directorySecurity);

            //Building a directory tree
            //List the subdirectories for Program Files containing the character 'a' with a maximum depth of 5
            //DirectoryInfo di1 = new DirectoryInfo(@"C:\Program Files");
            //ListDirectories(di1, "*a*", 5, 0);

            //Moving a directory
            //Directory.Move(@"C:\Temp\ProgrammingInCSharp", @"C:\Temp\PIC6");

            //DirectoryInfo di2 = new DirectoryInfo(@"C:\Temp\PIC");
            //di2.MoveTo(@"C:\Temp\PIC3");

            //Listing all the files in a directory
 /*           foreach (string file in Directory.GetFiles(@"C:\Windows"))
            {
                Console.WriteLine(file);
            }

            DirectoryInfo di3 = new DirectoryInfo(@"C:\Temp");
            foreach(FileInfo fileInfo in di3.GetFiles())
            {
                Console.WriteLine(fileInfo.FullName);
            }
*/
            //Deleteing a file
/*            string path = @"C:\Temp\test.txt";
            if(File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine("file.delete");
            }
            path = @"C:\Temp\test1.txt";
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
                Console.WriteLine("fileinfo.delete");
            }
*/
            //Moving a file
            string path1 = @"C:\Temp\test.txt";
            string destpath1 = @"C:\Temp\destTest.txt";

              File.CreateText(path1).Close();
            //File.Move(path1, destpath1);

            //FileInfo fileinfo = new FileInfo(path1);
            //fileinfo.MoveTo(destpath1);
            
            //Using Path Combine
/*            string folder = @"C:\temp";
            string filename = "test.dat";

            string fullPath = Path.Combine(folder, filename);
            Console.WriteLine(fullPath);
*/
            //Using other Path methods to parse a path
/*            string path = @"C:\Temp\subdir\file.txt";

            Console.WriteLine(Path.GetDirectoryName(path)); // c:\temp\subdire
            Console.WriteLine(Path.GetExtension(path)); // .txt
            Console.WriteLine(Path.GetFileName(path));//file.txt
            Console.WriteLine(Path.GetPathRoot(path)); // C:\
*/
            //Create and use a filestream
/*            string path = @"c:\Temp\test.dat"; //path to the file of interest
            using (FileStream filestream = File.Create(path))
            {
                string myValue = "MyValue";
                byte[] data = Encoding.UTF8.GetBytes(myValue); //encodes into UTF8 format
                filestream.Write(data, 0, data.Length);
            }
*/
            //Using File.CreateText with a StreamWriter
/*            string pathS = @"C:\Temp\TestS.dat";
            using(StreamWriter streamwriter = File.CreateText(pathS))
            {
                string myValueS = "This is my CreateText value";
                streamwriter.Write(myValueS);
            }
*/
            //Opening a FileStream and decode the bytes to a string
            string path = @"C:\Temp\test.dat";
/*            using (FileStream filestream = File.OpenRead(path))
            {
                byte[] data = new byte[filestream.Length];//array of bytes that the lenght of the filestream
                for(int index = 0; index < filestream.Length; index++)
                {
                    data[index] = (byte)filestream.ReadByte();
                }
                Console.WriteLine(Encoding.UTF8.GetString(data)); //Composition Boook.
            }
 */
            //Opening a TextFile and reading the content
/*            using (StreamReader streamreader = File.OpenText(path))
            {
                Console.WriteLine(streamreader.ReadLine());
            }

 */
            //Compressing data with a GZipStream
            string folder = @"C:\Temp";
            string uncompressedFilePath = Path.Combine(folder, "uncompressed.dat");
            string compressedFilePath = Path.Combine(folder, "compressed.gz");
            byte[] dataToCompress = Enumerable.Repeat((byte)'a', 1024 * 1024).ToArray();//generate an array of 'a's. put into datatocompress object
            using (FileStream uncompressedFileStream = File.Create(uncompressedFilePath))
            {
                uncompressedFileStream.Write(dataToCompress, 0, dataToCompress.Length);
            }

            using (FileStream compressedFileStream = File.Create(compressedFilePath))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress)) //constructor takes stream, compression mode indicator
                {
                    compressionStream.Write(dataToCompress, 0, dataToCompress.Length);
                }
            }
            FileInfo unCompressedFile = new FileInfo(uncompressedFilePath);
            FileInfo compressedFile = new FileInfo(compressedFilePath);
            Console.WriteLine(unCompressedFile.Length);
            Console.WriteLine(compressedFile.Length);

            //Using a BufferedStream
            string pathB = @"c:\temp\bufferedstream.txt";
            using (FileStream fsB = File.Create(pathB))
            {
                using (BufferedStream bs = new BufferedStream(fsB))
                {
                    using (StreamWriter sw = new StreamWriter(bs))
                    {
                        
                        sw.WriteLine("A line of text");
                    }
                }
            }

            //Depending on FIle.Exists when reading file content
            //string ans = ReadAllText();
            //Console.WriteLine("{0}", ans);
            
            //Using exception handling when opening a file
            string ans = ReadAllTextTry();
            Console.WriteLine(ans);
            
            //Executing a web request
            WebRequest request = WebRequest.Create("http://www.microsoft.com");
            WebResponse response = request.GetResponse();

            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            string responseText = responseStream.ReadToEnd();
            Console.WriteLine(responseText);
            response.Close();

            //Async call

            Program p = new Program();
            Task t = p.CreateAndWriteAsynToFIle();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Execute while task runs");
            }

            Console.ReadLine();
        }
    }
}
