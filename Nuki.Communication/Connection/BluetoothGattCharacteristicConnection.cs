﻿using Nuki.Communication.Commands;
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
    internal class BluetoothGattCharacteristicConnection
    {
        private GattCharacteristic m_GattCharacteristic = null;
        private TaskCompletionSource<RecieveBaseCommand> m_responseWaitHandle = null;
        private ConcurrentQueue<RecieveBaseCommand> m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();
        private RecieveBaseCommand m_cmdInProgress = null;
        public BluetoothGattCharacteristicConnection()
        {
        }
        public bool IsValid => m_GattCharacteristic != null;
       
        internal void SetConnection(GattCharacteristic characteristic)
        {
            if (m_GattCharacteristic != null)
                m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
            m_GattCharacteristic = characteristic;
            m_GattCharacteristic.ValueChanged += M_GattCharacteristic_ValueChanged;
        }

        ~BluetoothGattCharacteristicConnection()
        {
            m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
        }

        private void M_GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var recieveBuffer = args.CharacteristicValue;
            
            lock (sender)
            {
                var reader = DataReader.FromBuffer(recieveBuffer);
                reader.ByteOrder = ByteOrder.LittleEndian;
                if (m_cmdInProgress == null)
                    m_cmdInProgress = ResponseCommandParser.Parse(reader);

                if (m_cmdInProgress != null)
                {
                    m_cmdInProgress.ProcessRecievedData(reader);
                    if (m_cmdInProgress.Complete)
                    {
                        var cmd = m_cmdInProgress;
                        m_cmdInProgress = null;
                        if(m_responseWaitHandle?.TrySetResult(cmd) != true)
                        {
                            Debug.WriteLine($"Recieved Command {cmd} is not handlet...");
                        }
                        else { }
                    }
                    else
                    {
                    }
                }
                else
                {
                    Debug.WriteLine("Recieved unknown command");
                }
            }
        }

        public async Task<bool> Send(SendBaseCommand cmd)
        {
            m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            if (m_recieveQueue.Count > 0)
                m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();
            var writer = new DataWriter();
            writer.WriteBytes(cmd.Serialize().ToArray());
            var result = await m_GattCharacteristic.WriteValueAsync(writer.DetachBuffer());

            return result == GattCommunicationStatus.Success;
        }

        public async Task<RecieveBaseCommand> Recieve(int nTimeout)
        {
            RecieveBaseCommand retCommand = null;
            if (m_recieveQueue.Count <= 0)
            {
                Task completedTask = await Task.WhenAny(m_responseWaitHandle.Task, Task.Delay(nTimeout));

                if (completedTask == m_responseWaitHandle.Task)
                {
                    retCommand = m_responseWaitHandle.Task.Result;
                }
                else
                {
                    //Timeout
                }
            }
            else
            {
                m_recieveQueue.TryDequeue(out retCommand);
            }
            return retCommand;
        }
    }
}