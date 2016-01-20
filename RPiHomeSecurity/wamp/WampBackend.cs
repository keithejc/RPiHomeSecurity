using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using WampSharp.Core.Listener;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Realm;
using WampSharp.V2.Rpc;

namespace RPiHomeSecurity.wamp
{
    public struct RPiHomeSecurityStatus
    {
        public bool InAlarm;
    }

    public class WampBackend
    {
        private static readonly WampBackend instance = new WampBackend();

        public static WampBackend Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsConnected { get; set; }

        public WampBackend()
        {
            IsConnected = false;
        }

        private DefaultWampChannelFactory factory;
        private IWampChannel channel;
        private IWampClientConnectionMonitor monitor;
        private IWampRealmServiceProvider services;


        public void Close()
        {
            channel.Close();
        }

        public void Init(string wsuri, string realm)
        {
            factory = new DefaultWampChannelFactory();

            channel = factory.CreateJsonChannel(wsuri, realm);

            try
            {
                channel.Open().Wait();

                services = channel.RealmProxy.Services;

                // register procedures for remote calling
                services.RegisterCallee(this).Wait();

                // publishing
                onPublishSystemStatusSubject = services.GetSubject<RPiHomeSecurityStatus>("com.rpihomesecurity.onstatus");

                monitor = channel.RealmProxy.Monitor;
                monitor.ConnectionBroken += OnClose;
                monitor.ConnectionError += OnError;

                IsConnected = true;
                Console.WriteLine("WAMP connection success");

            }
            catch (Exception e)
            {
                Console.WriteLine("Failure to Initialise WAMP connection. Is crossbar started?" + e.Message);
            }
        }


        private ISubject<RPiHomeSecurityStatus> onPublishSystemStatusSubject;

        //send out the status of the system
        public void PublishRpiSystemStatus(RPiHomeSecurityStatus status)
        {
            try
            {
                if (IsConnected)
                {
                    onPublishSystemStatusSubject.OnNext(status);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed sending status: " + e.Message);
            }
        }


        public delegate void RunActionListEvent(String actionList);

        public event RunActionListEvent RunActionListHandler;

        [WampProcedure("com.rpihomesecurity.runactionlist")]
        public void RunActionList(String actionList)
        {
            if (RunActionListHandler != null)
            {
                RunActionListHandler.Invoke(actionList);
            }
        }


        private static void OnClose(object sender, WampSessionCloseEventArgs e)
        {
            WampBackend.Instance.IsConnected = false;
            Console.WriteLine("WAMP connection broken.");
        }

        private static void OnError(object sender, WampConnectionErrorEventArgs e)
        {
            WampBackend.Instance.IsConnected = false;
            Console.WriteLine("WAMP connection error. ");
        }



    }
}
