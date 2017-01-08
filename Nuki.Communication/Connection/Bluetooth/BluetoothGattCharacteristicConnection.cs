using MetroLog;
using Nuki.Communication.Connection.Bluetooth.Commands;
using Nuki.Communication.Connection.Bluetooth.Commands.Request;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection.Bluetooth
{
    internal abstract class BluetoothGattCharacteristicConnection
    {
        protected ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<BluetoothGattCharacteristicConnection>();
        private object syncroot = new object();
        private GattCharacteristic m_GattCharacteristic = null;
        private TaskCompletionSource<RecieveBaseCommand> m_responseWaitHandle = null;
        private ConcurrentQueue<RecieveBaseCommand> m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();
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
                    while (reader.UnconsumedBufferLength >= 2)
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

                                bool blnWarnIfNotHandlet = false;
                                if (cmd is RecieveErrorReportCommand)
                                    Connection.Update(cmd as RecieveErrorReportCommand);
                                else if (cmd is RecieveNukiStatesCommand)
                                    Connection.Update(cmd as RecieveNukiStatesCommand);
                                else if (cmd is RecieveChallengeCommand)
                                    Connection.Update(cmd as RecieveChallengeCommand);
                                else
                                    blnWarnIfNotHandlet = true;
                                m_recieveQueue.Enqueue(cmd);
                                if (m_responseWaitHandle?.TrySetResult(cmd) == true)
                                {
                                    Log.Debug("m_responseWaitHandle set");
                                }
                                else
                                {
                                    if (blnWarnIfNotHandlet)
                                        Log.Warn($"Recieved Command {cmd} is not handlet...");
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

        public async Task<T> Send<T>(SendBaseCommand cmd,int nTimeout = 2000)
            where T:RecieveBaseCommand
        {
            T ret = default(T);
            if(await Send(cmd))
            {
                ret = await Recieve<T>(nTimeout);
                if (ret == null)
                {
                    Connection.Update(new NukiCommandTimeout(cmd, nTimeout));
                }
                else if (ret is RecieveStatusCommand)
                {
                    var status = ((RecieveStatusCommand)(object)ret).StatusCode;
                    if(status != API.NukiErrorCode.COMPLETE && status != API.NukiErrorCode.ACCEPTED)
                    {
                        this.Connection.Update(new NukiCommandFailed(cmd.CommandType, status));
                    }
                    else { } //OK
                }
                else { }
            }
            else
            {
                Connection.Update(new NukiCommandTimeout(cmd, nTimeout));
            }
            return ret;
        }

        public virtual void Reset()
        {
            m_cmdInProgress = null;
            m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();
        }


        public async Task<bool> Send(SendBaseCommand cmd, int nTimeout)
        {
            bool blnRet = false;
            Reset();
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

        public Task<RecieveBaseCommand> Recieve(int nTimeout)
        {
            return Recieve<RecieveBaseCommand>(nTimeout);
        }

        public async Task<T> Recieve<T>(int nTimeout)
            where T : RecieveBaseCommand
        {
            T retCommand = null;
            RecieveBaseCommand cmd = null;
            Log.Debug("Await response...");
            if (!m_recieveQueue.TryDequeue(out cmd))
            {
                Task completedTask = await Task.WhenAny(m_responseWaitHandle.Task, Task.Delay(nTimeout));

                if (m_recieveQueue.TryDequeue(out cmd))
                {
                    
                    retCommand = cmd as T;
                    if (retCommand == null && cmd != null)
                    {
                        Log.Debug($"Discarding response {retCommand}...");
                        m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
                        retCommand = await Recieve<T>(nTimeout);
                    }
                    else
                    {
                        Log.Debug($"Returning response {retCommand}...");
                        m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
                    }
                }
                else
                {
                    Log.Warn("Recieve timed out...");
                    //Timeout
                }
            }
            else
            {
                retCommand = cmd as T;
                m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            }

            return retCommand;
        }
    }
}
