using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Request;
using Windows.Storage.Streams;
using System.IO;
using Nuki.Communication.SemanticTypes;
using System.Diagnostics;

namespace Nuki.Communication.Connection.Bluetooth
{
    internal class BluetoothGattCharacteristicConnectionEncrypted : BluetoothGattCharacteristicConnection
    {
        private object Syncroot = new object();
        private RecieveBuffer m_RecieveBuffer = null;
        private class RecieveBuffer
        {
            private List<byte> m_bufferPDATA = new List<byte>();
            private byte[] m_bufferADATA = new byte[30];
            private byte m_nADATABufferPointer = 0;
            public ADATANonce Nonce { get; private set; }
            public UniqueClientID UniqueClientID { get; private set; }
            public UInt16 Length { get; private set; }

            public bool Complete => Length <= m_bufferPDATA.Count && HeaderComplete;


            public int Recieved => m_bufferPDATA.Count;
            public bool HeaderComplete => m_nADATABufferPointer == m_bufferADATA.Length;


            public RecieveBuffer(IBuffer value)
            {
                Consume(value);
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
                int nRemainBytes = byRemain.Length;

                if(m_nADATABufferPointer < m_bufferADATA.Length)
                {
                    int nLenToCopy = Math.Min(byRemain.Length, m_bufferADATA.Length - m_nADATABufferPointer);
                    Array.Copy(byRemain, 0, m_bufferADATA, m_nADATABufferPointer, nLenToCopy);
                    m_nADATABufferPointer += (byte)nLenToCopy;
                    nRemainBytes -= nLenToCopy;

                    if (m_nADATABufferPointer == m_bufferADATA.Length)
                    {
                        //Header Complete -> read

                        byte[] byNonce = new byte[24];
                        Array.Copy(m_bufferADATA, 0, byNonce, 0, 24);
                        Nonce = new ADATANonce(byNonce);

                     
                        UniqueClientID = new UniqueClientID(BitConverter.ToUInt32(m_bufferADATA,24));

                        Length = BitConverter.ToUInt16(m_bufferADATA,28);
                    }
                    else { }
                }
                else { }




                if(nRemainBytes > 0)
                {
                    m_bufferPDATA.AddRange(byRemain.Skip(byRemain.Length - nRemainBytes));
                }
                else { }

                //m_EncryptedInputBuffer.Write(byRemain, 0, byRemain.Length);

            }

            public DataReader Decrypt(IConnectionContext connection)
            {
                if (Complete)
                {  
                    byte[] message = m_bufferPDATA.ToArray();
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
    

        public BluetoothGattCharacteristicConnectionEncrypted(BluetoothConnection connection) 
            : base(connection)
        {

        }
        protected override Task<bool> Send(SendBaseCommand cmd, DataWriter writer)
        {
            bool blnRet = false;
            m_RecieveBuffer = null;

            byte[] byNonce = Sodium.Core.GetRandomBytes(24);
            writer.WriteBytes(byNonce);
            writer.WriteUInt32(Connection.UniqueClientID.Value);

            byte[] byDecryptedMessage = cmd.Serialize(BitConverter.GetBytes(Connection.UniqueClientID.Value)).ToArray();
            byte[] byEncryptedMessage = Sodium.SecretBox.Create(byDecryptedMessage, byNonce, Connection.SharedKey);

            writer.WriteUInt16((UInt16)byEncryptedMessage.Length);
            writer.WriteBytes(byEncryptedMessage);
            blnRet = true;

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
            lock (Syncroot)
            {
                reader = null;
                try
                {
                    if (m_RecieveBuffer == null)
                    {
                        m_RecieveBuffer = new RecieveBuffer(value);
                    }
                    else
                    {
                        m_RecieveBuffer?.Consume(value);
                    }

                    if (m_RecieveBuffer?.Complete == true)
                    {
                        if (m_RecieveBuffer.UniqueClientID == this.Connection.UniqueClientID)
                        {
                            reader = m_RecieveBuffer.Decrypt(Connection);
                            reader.ByteOrder = ByteOrder.LittleEndian;
                            m_RecieveBuffer = null;
                            if (reader.ReadUInt32() == this.Connection.UniqueClientID.Value)
                            {
                                blnRet = true;
                            }
                            else
                            {
                                Log.Warn("Decryption of message failed (PDATA has wrong UniqueClientID)!");
                            }
                        }
                        else
                        {
                            Log.Warn("Recieved message for wrong ClientID?!");
                        }
                    }
                    else
                    {
                        Log.Debug($"Nonce: {m_RecieveBuffer.Nonce}");
                        Log.Debug($"UniqueClientID: {m_RecieveBuffer.UniqueClientID}");
                        Log.Debug($"Length: {m_RecieveBuffer.Length}");
                        Log.Debug($"HeaderComplete: {m_RecieveBuffer.HeaderComplete}");
                        Log.Debug($"Recieved: {m_RecieveBuffer.Recieved}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in TryRecieve: " + ex.ToString());
                }
            }
            return blnRet;
        }
    }
}
