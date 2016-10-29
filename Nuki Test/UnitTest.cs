using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Text;
using System.Linq;

namespace Nuki_Test
{
    [TestClass]
    public class UnitTest1
    {

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }

        public static byte[] StringToByteArray2(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        [TestMethod]
        public void TestMethod1()
        {
            string strCLPrivate = "8CAA54672307BFFDF5EA183FC607158D2011D008ECA6A1088614FF0853A5AA07";
            string strCLPublic  = "F88127CCF48023B5CBE9101D24BAA8A368DA94E8C2E3CDE2DED29CE96AB50C15";
            string strSLPublic  = "2FE57DA347CD62431528DAAC5FBB290730FFF684AFC4CFC2ED90995F58CB3B74";

            var byCLPrivate = StringToByteArray(strCLPrivate);

            byte[] byDH1 = Sodium.ScalarMult.Mult(StringToByteArray(strCLPrivate), StringToByteArray(strSLPublic));
            string dh1 = ByteArrayToString(byDH1);
            
            Assert.AreEqual(dh1, "0DE40B998E0E330376F2D2FC4892A6931E25055FD09F054F99E93FECD9BA611E");

            var _0 = new byte[16];
            var sigma = System.Text.Encoding.UTF8.GetBytes("expand 32-byte k");
            var kdf1 = Sodium.KDF.HSalsa20(_0, byDH1, sigma);
            var strUTF8Kdf1 = ByteArrayToString(kdf1);

            Assert.AreEqual(strUTF8Kdf1, "217FCB0F18CAF284E9BDEA0B94B83B8D10867ED706BFDEDBD2381F4CB3B8F730");

            string strChallenge = "6CD4163D159050C798553EAA57E278A579AFFCBC56F09FC57FE879E51C42DF17";
            string strR = strCLPublic + strSLPublic + strChallenge;

            byte[] byA = Sodium.SecretKeyAuth.SignHmacSha256( StringToByteArray(strR), kdf1);
            string strA = ByteArrayToString(byA);

            Assert.AreEqual(strA, "B09A0D3979A029E5FD027B519EAA200BC14AD3E163D3BE4563843E021073BCB1");



        }
    }
}
