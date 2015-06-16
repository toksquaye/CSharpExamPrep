using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security;
using System.IO;
using System.Runtime.InteropServices;

namespace Encryption
{
    class Program
    {
        //Use a symmetric encryption algorithm
        public static void EncryptionSomeText()
        {
            string original = "My secret data";
            using (SymmetricAlgorithm symmetricAlgorithm = new AesManaged ())
            {
                byte[] encrypted = Encrypt(symmetricAlgorithm, original);
                string roundtrip = Decrypt(symmetricAlgorithm, encrypted);

                Console.WriteLine("Original:    {0}", original);
                Console.WriteLine("Round Trip: {0}",    roundtrip);
            }
        }

        static byte[] Encrypt(SymmetricAlgorithm aesAlg, string plainText)
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV); //symmetric encryptor object
            using (MemoryStream msEncrypt = new MemoryStream()) //stream object that writes to memory
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))//link datastream to cryptographic transformation
                {
                    using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))//streamwriter object
                    {
                        swEncrypt.Write(plainText); //write plaintext into crypstream object. that inturns encrypts it and passes into memmory stream object
                    }
                }
                
                return msEncrypt.ToArray();//return encrypted object in memory
            }
        }

        static string Decrypt(SymmetricAlgorithm aesAlg, byte[] cipherText)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV); //symmetric decryptor object
            using (MemoryStream msDecrypt = new MemoryStream(cipherText)) //stream object loads to memory data that needs to be decrypted
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))//link datastream to cryptographic transformation
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))//streamreader object
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
                
                
        }


        /*****************************A set implementation that uses hashing********************************/
        class Set<T>
        {
            private List<T>[] buckets = new List<T>[100];

            public bool Contains(T item)
            {
                return Contains(item, GetBucket(item.GetHashCode()));
            }

            public void Insert(T item)
            {
                int bucket = GetBucket(item.GetHashCode());
                if (Contains(item, bucket)) return;
                if (buckets[bucket] == null)
                    buckets[bucket] = new List<T>();
                buckets[bucket].Add(item);
            }

            private int GetBucket(int hashcode) //find the bucket that a value hashes to
            {
                unchecked
                {
                    return (int)((uint)hashcode % (uint)buckets.Length);
                }
            }
            private bool Contains(T item, int bucket)
            {
                if (buckets[bucket] != null)
                {
                    foreach (T member in buckets[bucket])
                        if (member.Equals(item))
                            return true;
                }
                return false;
            }
        }
        
        /************************************Signing and Verifying data with a certificate*************************************/
        public static void SignAndVerify()
        {
            string textToSign = "Test Paragraph";
            byte[] signature = Sign(textToSign, "cn=TQuaye");
            //uncomment this line to make the verification step fail
            //signature[0] = 0;
            Console.WriteLine(Verify(textToSign, signature));
        }

        static bool Verify(string text, byte[] signature)
        {
            X509Certificate2 cert = GetCertificate();
            var csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            byte[] hash = HashData(text);
            return csp.VerifyHash(hash,
                CryptoConfig.MapNameToOID("SHA1"),
                signature);
        }

        static byte[] Sign(string text, string certSubject)
        {
            X509Certificate2 cert = GetCertificate();
            var csp = (RSACryptoServiceProvider)cert.PrivateKey;
            byte[] hash = HashData(text);
            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }
        private static byte[] HashData(string text)
        {
            HashAlgorithm hashAlgorithm = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(text);
            byte[] hash = hashAlgorithm.ComputeHash(data);
            return hash;

        }
        private static X509Certificate2 GetCertificate()
        {
            X509Store my = new X509Store("testCertStore", StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);
            var certificate = my.Certificates[0];
            return certificate;
        }

        /***************************************Declarative Code Access Security******************************/
        [FileIOPermission(SecurityAction.Demand,
            AllLocalFiles = FileIOPermissionAccess.Read)]
        public void DeclarativeCAS()
        {

        }

        /******************************Getting the value of a Secure String**************************************/
        public static void ConvertToUnsecureString(SecureString securePassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;

            try 
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                Console.WriteLine(Marshal.PtrToStringUni(unmanagedString));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        /****************************************************MAIN***********************************************/

        static void Main(string[] args)
        {
            EncryptionSomeText();

            //Exporting a public key
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKeyXML = rsa.ToXmlString(false); // only public key
            string privateKeyXML = rsa.ToXmlString(true); //pubic & private key

            Console.WriteLine(publicKeyXML);
            Console.WriteLine(privateKeyXML);

            //Using a public and private key to encrypt and decrypt data
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes("My Secret Data");
            byte[] encryptedData;
            string containerName = "SecretContainer"; //container name
            CspParameters csp = new CspParameters() { KeyContainerName = containerName }; //container
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))//put data in container
            {
                RSA.FromXmlString(publicKeyXML);
                encryptedData = RSA.Encrypt(dataToEncrypt, false);
            }

            byte[] decryptedData;
            using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(csp))//get data out of container
            {
                RSA.FromXmlString(privateKeyXML);
                decryptedData = RSA.Decrypt(encryptedData, false);
            }

            string decryptedString = ByteConverter.GetString(decryptedData);
            Console.WriteLine(decryptedString);

            //use Set<T>
            Set<int> setInt = new Set<int>();
            setInt.Insert(45);
            setInt.Insert(54);
            Console.WriteLine(setInt.Contains(5));

            //Using SHA256Managed to calculate a hash code
           /* UnicodeEncoding byteConverter = new UnicodeEncoding();
            SHA256 sha256 = SHA256.Create();

            string data = "A paragraph of text";
            byte[] hashA = sha256.ComputeHash(byteConverter.GetBytes(data));//compute hashvalue of data

            data = "A paragrah of changed text";
            byte[] hashB = sha256.ComputeHash(byteConverter.GetBytes(data));

            data = "A paragraph of text";
            byte[] hashC = sha256.ComputeHash(byteConverter.GetBytes(data));
            Console.WriteLine(hashA.SequenceEqual(hashB)); //false
            Console.WriteLine(hashA.SequenceEqual(hashC));//true*/

            // SignAndVerify();

            //Imperative CAS

            FileIOPermission f = new FileIOPermission(PermissionState.None);
            f.AllLocalFiles = FileIOPermissionAccess.Read;
            try { f.Demand(); }
            catch (SecurityException s )
            {
                Console.WriteLine(s.Message);
            }

            //Initializing a secure string
            using (SecureString ss = new SecureString())
            {
                Console.Write("Please enter a password: ");
                while (true)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.Enter) break;

                    ss.AppendChar(cki.KeyChar);
                    Console.Write("*");
                }
                ss.MakeReadOnly();

                ConvertToUnsecureString(ss);
            }

            Console.ReadLine();
        }
    }
}
