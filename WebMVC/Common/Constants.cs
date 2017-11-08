using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVC.Common
{
    public class Constants
    {
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        public const string InitVector = "tu89geji340t89u2";
        // This constant is used to determine the keysize of the encryption algorithm.
        public const int Keysize = 256;
        public const string PassPhrase = "tu89geji340t89u2ACB76ffhlm111";
    }

   
}