using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Text;
using System.IO;

namespace WebMVC.Common
{
    public class Helper
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static string GenerateSearchStatement(Dictionary<string, string> searchData)
        {
            StringBuilder searchStatement = new StringBuilder();
            string whereCondition = string.Empty;
            string keyVal = string.Empty;
            if (searchData != null && searchData.Count > 0)
            {
                foreach (KeyValuePair<string, string> search in searchData)
                {

                    if (search.Key.ToLower().IndexOf("date") != -1)
                    {
                        keyVal = "CAST(" + search.Key + " AS DATE)";
                    }
                    else
                    {
                        keyVal = search.Key;
                    }
                    searchStatement.Append(keyVal + " LIKE '%" + search.Value.Replace("'", "''").Trim() + "%'");
                    searchStatement.Append(" AND ");
                }
                whereCondition = searchStatement.ToString();
                whereCondition = whereCondition.Substring(0, whereCondition.Length - 5);
            }
            else
            {
                whereCondition = "1=1";
            }
            return whereCondition;
        }
      

        /// <summary>
        /// Decrypt an encrypted text
        /// </summary>
        /// <returns></returns>
        /// Date                            Author/(Reviewer)                       Description
        /// ------------------------------------------------------------------------------------          
        /// 07-June-2016                     Gamunu Amunugama        
        public static string DecryptText(string encryptedText)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(Constants.InitVector);
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(Constants.PassPhrase, null);
                byte[] keyBytes = password.GetBytes(Constants.Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        /// <summary>
        /// Encrypt a text
        /// </summary>
        /// <returns></returns>
        /// Date                            Author/(Reviewer)                       Description
        /// ------------------------------------------------------------------------------------          
        /// 07-June-2016                     Gamunu Amunugama        
        public static string EncryptText(string plainText)
        {
            try
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(Constants.InitVector);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(Constants.PassPhrase, null);
                byte[] keyBytes = password.GetBytes(Constants.Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();

                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }

    public static class QueryHelper
        {
            private static readonly MethodInfo OrderByMethod =
                typeof(Queryable).GetMethods().Single(method =>
               method.Name == "OrderBy" && method.GetParameters().Length == 2);

            private static readonly MethodInfo OrderByDescendingMethod =
                typeof(Queryable).GetMethods().Single(method =>
               method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

            public static bool PropertyExists<T>(string propertyName)
            {
                return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                    BindingFlags.Public | BindingFlags.Instance) != null;
            }

            public static IQueryable<T> OrderByProperty<T>(
               this IQueryable<T> source, string propertyName)
            {
                if (typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                    BindingFlags.Public | BindingFlags.Instance) == null)
                {
                    return null;
                }
                ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
                Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
                LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
                MethodInfo genericMethod =
                  OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                return (IQueryable<T>)ret;
            }

            public static IQueryable<T> OrderByPropertyDescending<T>(
                this IQueryable<T> source, string propertyName)
            {
                if (typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                    BindingFlags.Public | BindingFlags.Instance) == null)
                {
                    return null;
                }
                ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
                Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
                LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
                MethodInfo genericMethod =
                  OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                object ret = genericMethod.Invoke(null, new object[] { source, lambda });
                return (IQueryable<T>)ret;
            }
        }
    }
