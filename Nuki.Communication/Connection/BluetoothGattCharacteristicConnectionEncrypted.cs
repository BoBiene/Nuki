using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Request;
using Windows.Storage.Streams;
using System.IO;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Connection
{
    internal class BluetoothGattCharacteristicConnectionEncrypted : BluetoothGattCharacteristicConnection
    {
        private RecieveState m_RecieveState = RecieveState.ReadHeader;
        private RecieveBuffer m_RecieveBuffer = null;
        private class RecieveBuffer
        {
            private MemoryStream m_EncryptedInputBuffer = new MemoryStream();
            public SmartLockNonce Nonce { get; private set; }
            public MessageAuthentication Authentication { get; private set; }
            public UInt16 Length { get; private set; }

            public bool Complete => Length <= m_EncryptedInputBuffer.Length;

            public RecieveBuffer(IBuffer value)
            {
                using (var reader = DataReader.FromBuffer(value))
                {
                    reader.ByteOrder = ByteOrder.LittleEndian;
                    byte[] byNonce = new byte[24];
                    reader.ReadBytes(byNonce);
                    Nonce = new SmartLockNonce(byNonce);

                    byte[] byAuth = new byte[4];
                    reader.ReadBytes(byAuth);
                    Authentication = new MessageAuthentication(byAuth);

                    Length = reader.ReadUInt16();

                    Consume(reader);
                }
            }

            public void Consume(IBuffer value)
            {
                using (var reader = DataReader.FromBuffer(value))
                {
                    Consume(reader);
                }
            }

            private void Consume(DataReader reader)
            {
                byte[] byRemain = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(byRemain);
                m_EncryptedInputBuffer.Write(byRemain, 0, byRemain.Length);

            }

            public DataReader Decrypt(IConnectionContext connection)
            {
                if (Complete)
                {
                    byte[] message = m_EncryptedInputBuffer.ToArray();
                    byte[] decryptedMessage = Sodium.SecretBox.Open(message, Nonce, connection.SharedKey);

                    using (DataWriter w = new DataWriter())
                    {
                        w.WriteBytes(decryptedMessage);
                        return DataReader.FromBuffer(w.DetachBuffer());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Recieved Buffer is not completed...");
                }
            }
        }
        private enum RecieveState
        {
            ReadHeader,
            ReadContent,
        }

        public BluetoothGattCharacteristicConnectionEncrypted(BluetoothConnection connection) 
            : base(connection)
        {

        }
        protected override Task<bool> Send(SendBaseCommand cmd, DataWriter writer)
        {
            bool blnRet = false;
            m_RecieveState = RecieveState.ReadHeader;

            byte[] byNonce = Sodium.Core.GetRandomBytes(24);
            writer.WriteBytes(byNonce);
            writer.WriteUInt32(Connection.UniqueClientID.Value);

            byte[] byDecryptedMessage = cmd.Serialize().ToArray();
            byte[] byEncryptedMessage = Sodium.SecretBox.Create(byDecryptedMessage, byNonce, Connection.SharedKey);

            writer.WriteUInt16((UInt16)byEncryptedMessage.Length);
            writer.WriteBytes(byEncryptedMessage);


            return Task.FromResult(blnRet);
        }
        private static byte[] To32ByteLength(byte[] byArry)
        {
            byte[] byRet = new byte[32];

            for (int i = 0, len = Math.Min(byArry.Length, byRet.Length); i < len; ++i)
                byRet[i] = byArry[i];

            return byRet;
        }

        protected override bool TryGetRecieveBuffer(IBuffer value, out DataReader reader)
        {
            bool blnRet = false;
            reader = null;
            if (m_RecieveState == RecieveState.ReadHeader)
            {
                m_RecieveBuffer = new RecieveBuffer(value);
            }
            else
            {
                m_RecieveBuffer?.Consume(value);
            }

            if (m_RecieveBuffer?.Complete == true)
            {
                reader = m_RecieveBuffer.Decrypt(Connection);
                blnRet = true;
            }
            else { }

            return blnRet;
        }
    }
}
