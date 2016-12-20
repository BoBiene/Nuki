using Nuki.Communication.Connection.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Nuki.Communication.Connection
{
    public static class NukiConnectionFactory
    {
        private readonly static LinkedList<INukiConnectionFactory> s_Factories = new LinkedList<INukiConnectionFactory>();

        static NukiConnectionFactory()
        {
            s_Factories.AddFirst(new BluetoothConnectionFactory());
        }

        public static async Task<NukiConnectResult> TryConnect(NukiConnectionBinding connectionInfo, Func<Action, IAsyncAction> dispatch,int nTimeout = 3000)
        {
            NukiConnectResult returnValue = null;
            var current = s_Factories.First;
            while (returnValue == null && current != null)
            {
                returnValue = await current.Value.TryConnect(connectionInfo, dispatch, nTimeout);
            }
            return returnValue;
        }
    }
    internal interface INukiConnectionFactory
    {
        Task<NukiConnectResult> TryConnect(NukiConnectionBinding connectionInfo, Func<Action, IAsyncAction> dispatch, int nTimeout = 3000);
    }

    public class NukiConnectResult
    {
        public INukiConnection Connection { get; private set; }
        public bool Successfull { get; private set; }

        public NukiConnectResult(bool blnSuccessfull, INukiConnection nukiConnection)
        {
            Connection = nukiConnection;
            Successfull = blnSuccessfull;
        }
    }
}
