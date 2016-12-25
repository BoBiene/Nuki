﻿using MetroLog;
using Nuki.Communication.Commands;
using Nuki.Communication.Commands.Request;
using Nuki.Communication.Commands.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    internal abstract class BluetoothGattCharacteristicConnection
    {
        protected ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<BluetoothGattCharacteristicConnection>();
        private object syncroot = new object();
        private GattCharacteristic m_GattCharacteristic = null;
        private TaskCompletionSource<RecieveBaseCommand> m_responseWaitHandle = null;
        private RecieveBaseCommand m_cmdInProgress = null;
        public BluetoothConnection Connection { get; private set; }
        public BluetoothGattCharacteristicConnection(BluetoothConnection connection)
        {
            Connection = connection;
        }
        public bool IsValid => m_GattCharacteristic != null;

        internal void SetConnection(GattCharacteristic characteristic)
        {
            Log.Debug("SetConnection");
            if (m_GattCharacteristic != null)
                m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
            m_GattCharacteristic = characteristic;
            m_GattCharacteristic.ValueChanged += M_GattCharacteristic_ValueChanged;
        }

        ~BluetoothGattCharacteristicConnection()
        {
            m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
        }

        protected abstract bool TryGetRecieveBuffer(IBuffer value,out DataReader reader);

        private void M_GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var recieveBuffer = args.CharacteristicValue;
            Log.Debug("M_GattCharacteristic_ValueChanged");
            lock (syncroot)
            {
                DataReader reader = null;
                if (TryGetRecieveBuffer(recieveBuffer, out reader))
                {
                    if (m_cmdInProgress == null)
                        m_cmdInProgress = ResponseCommandParser.Parse(reader);

                    if (m_cmdInProgress != null)
                    {
                        m_cmdInProgress.ProcessRecievedData(reader);
                        if (m_cmdInProgress.Complete)
                        {
                            var cmd = m_cmdInProgress;
                            m_cmdInProgress = null;
                            Log.Info($"Recieved Command {cmd}...");
                            if (m_responseWaitHandle?.TrySetResult(cmd) != true)
                            {
                               Log.Warn($"Recieved Command {cmd} is not handlet...");
                            }
                            else
                            {
                               Log.Debug("m_responseWaitHandle set");
                            }
                        }
                        else
                        {
                            Log.Debug($"Command {m_cmdInProgress.CommandType} not complete (Recieved {m_cmdInProgress.BytesRecieved} from {m_cmdInProgress.BytesTotal})...");
                        }
                    }
                    else
                    {
                        Log.Warn("Recieved unknown command");
                    }
                }
                else
                {

                }
            }
        }

        public Task<bool> Send(SendBaseCommand cmd)
        {
            return Send(cmd, 2000);
        }

        public virtual void Reset()
        {
            m_cmdInProgress = null;
        }


        public async Task<bool> Send(SendBaseCommand cmd, int nTimeout)
        {
            bool blnRet = false;
            m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            Log.Info($"Send Command {cmd}...");
            var writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            if (await Send(cmd, writer))
            {
                var result = await m_GattCharacteristic.WriteValueAsync(writer.DetachBuffer());
                blnRet = result == GattCommunicationStatus.Success;
            }
            else { }
            return blnRet;
        }

        protected abstract Task<bool> Send(SendBaseCommand cmd, DataWriter writer);
        

        public async Task<RecieveBaseCommand> Recieve(int nTimeout)
        {
            RecieveBaseCommand retCommand = null;
            Log.Debug("Await response...");
            Task completedTask = await Task.WhenAny(m_responseWaitHandle.Task, Task.Delay(nTimeout));

            if (completedTask == m_responseWaitHandle.Task)
            {
                Log.Debug("Recieved command...");
                retCommand = m_responseWaitHandle.Task.Result;
                if (retCommand is RecieveChallengeCommand)
                    this.Connection.SmartLockNonce = ((RecieveChallengeCommand)retCommand).Nonce;
            }
            else
            {
                Log.Warn("Recieve timed out...");
                //Timeout
            }

            return retCommand;
        }
    }
}