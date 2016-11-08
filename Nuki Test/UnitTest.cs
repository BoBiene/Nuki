using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Text;
using System.Linq;
using Nuki.Communication.Commands;
using System.Linq;
using System.Collections.Generic;
using Nuki.Communication.Commands.Request;
using Nuki.Communication.Commands.Response;
using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;

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

        public static string ByteArrayToString(IEnumerable< byte> ba)
        {
            StringBuilder hex = new StringBuilder(ba.Count() * 2);
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
        private const string CLPrivate = "8CAA54672307BFFDF5EA183FC607158D2011D008ECA6A1088614FF0853A5AA07";
        private const string CLPublic =  "F88127CCF48023B5CBE9101D24BAA8A368DA94E8C2E3CDE2DED29CE96AB50C15";
        private const string SLPublic =  "2FE57DA347CD62431528DAAC5FBB290730FFF684AFC4CFC2ED90995F58CB3B74";
        private const string SLNonce =   "6CD4163D159050C798553EAA57E278A579AFFCBC56F09FC57FE879E51C42DF17";
        private const string SLNonce2 =  "E0742CFEA39CB46109385BF91286A3C02F40EE86B0B62FC34033094DE41E2C0D";
        private const string SharedKey = "217FCB0F18CAF284E9BDEA0B94B83B8D10867ED706BFDEDBD2381F4CB3B8F730";
        private static TestConnectionContext ConnectionContext = new TestConnectionContext
            (
            StringToByteArray(CLPublic),
            StringToByteArray(SharedKey),
            StringToByteArray(SLNonce),
            StringToByteArray(SLPublic),
            () => StringToByteArray("52AFE0A664B4E9B56DC6BD4CB718A6C9FED6BE17A7411072AA0D315378140577")
            );

        [TestMethod]
        public void TestSendRequestDataCommand()
        {
            var Command = new SendRequestDataCommand(CommandTypes.PublicKey);

            var data = Command.Serialize();
            string strData = ByteArrayToString(data);
            Assert.AreEqual("0100030027A7", strData);
        }

        [TestMethod]
        public void TestSendPublicKey()
        {
            var Command = new SendPublicKeyComand(new ClientPublicKey(StringToByteArray(CLPublic)));

            var data = Command.Serialize();
            string strData = ByteArrayToString(data);
            Assert.AreEqual($"0300{CLPublic}9241", strData);
        }

        [TestMethod]
        public void TestSendAuthorizationAuthenticatorCommand()
        {
            
            var Command = new SendAuthorizationAuthenticatorCommand(ConnectionContext);

            string strAuthValue = "B09A0D3979A029E5FD027B519EAA200BC14AD3E163D3BE4563843E021073BCB1";
            string strCmdAuthValue = ByteArrayToString(Command.Authenticator);
            Assert.AreEqual(strCmdAuthValue, strAuthValue);
            var data = Command.Serialize();
            string strData = ByteArrayToString(data);
            Assert.AreEqual($"0500{strAuthValue}C357", strData);
        }

        [TestMethod]
        public void TestSendAuthorizationDataCommand()
        {
            ConnectionContext.SmartLockNonce = new Nuki.Communication.SemanticTypes.SmartLockNonce(StringToByteArray(SLNonce2));
            var Command = new SendAuthorizationDataCommand("Marc (Test)",ConnectionContext);

            string strAuthValue = "CF1B9E7801E3196E6594E76D57908EE500AAD5C33F4B6E0BBEA0DDEF82967BFC";
            string strCmdAuthValue = ByteArrayToString(Command.Authenticator);
            Assert.AreEqual(strCmdAuthValue, strAuthValue);
            var data = Command.Serialize();
            string strData = ByteArrayToString(data);
            Assert.AreEqual(strData,"0600CF1B9E7801E3196E6594E76D57908EE500AAD5C33F4B6E0BBEA0DDEF82967BFC00000000004D6172632028546573742900000000000000000000000000000000000000000052AFE0A664B4E9B56DC6BD4CB718A6C9FED6BE17A7411072AA0D31537814057769F2");
        }


        [TestMethod]
        public void TestRecievePublicKeyCommand()
        {
            string strMessage = $"0300{SLPublic}9DB9";
            var Command = new RecievePublicKeyCommand(StringToByteArray(strMessage));


            Assert.AreEqual(SLPublic, ByteArrayToString(Command.PublicKey.Value));
        }


        [TestMethod]
        public void TestRecieveChallengeCommand()
        {
            
            string strMessage = $"0400{SLNonce}C3DF";
            var Command = new RecieveChallengeCommand(StringToByteArray(strMessage));


            Assert.AreEqual(SLNonce, ByteArrayToString(Command.Nonce.Value));
        }

        [TestMethod]
        public void TestMethod1()
        {
            

            var byCLPrivate = StringToByteArray(CLPrivate);

            byte[] byDH1 = Sodium.ScalarMult.Mult(StringToByteArray(CLPrivate), StringToByteArray(SLPublic));
            string dh1 = ByteArrayToString(byDH1);
            
            Assert.AreEqual(dh1, "0DE40B998E0E330376F2D2FC4892A6931E25055FD09F054F99E93FECD9BA611E");

            var _0 = new byte[16];
            var sigma = System.Text.Encoding.UTF8.GetBytes("expand 32-byte k");
            var kdf1 = Sodium.KDF.HSalsa20(_0, byDH1, sigma);
            var strUTF8Kdf1 = ByteArrayToString(kdf1);

            Assert.AreEqual(strUTF8Kdf1, SharedKey);

            
            string strR = CLPublic + SLPublic + SLNonce;

            byte[] byA = Sodium.SecretKeyAuth.SignHmacSha256( StringToByteArray(strR), kdf1);
            string strA = ByteArrayToString(byA);

            Assert.AreEqual(strA, "B09A0D3979A029E5FD027B519EAA200BC14AD3E163D3BE4563843E021073BCB1");



        }
    }
}
