using System.ServiceModel.Security;
using CommonBase.Exceptions;
using CommonBase.Utils;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;

namespace StoreKeeper.Common
{
    public sealed class Infrastructure
    {
        #region Constants

        public static readonly Version Version = new Version(1, 1, 0, 0);

        #endregion

        static Infrastructure()
        {
            // 30 minutes by default
            ConnectionTimeout = 30 * 60;

            // 25 minutes by default
            LongOperationTimeout = 120 * 60;
        }

        #region Properties

        /// <summary>Connection timeout in seconds used to create bindings. The default value is 30 minutes.</summary>
        public static int ConnectionTimeout { get; private set; }

        /// <summary>Timeout for long operations in seconds used to create bindings. The default value is 25 minutes.</summary>
        public static int LongOperationTimeout { get; private set; }

        /// <summary>
        /// Verify inputs and changes timeout values.
        /// </summary>
        /// <param name="connectionTimeout">Connection timeout in seconds. This value must be greater than <paramref name="longOperationTimeout"/>.</param>
        /// <param name="longOperationTimeout">Timeout for long operations in seconds.</param>
        public static void ChangeTimeout(int connectionTimeout, int longOperationTimeout)
        {
            ArgumentValidator.IsGreaterThan("connectionTimeout", connectionTimeout, 0);
            ArgumentValidator.IsGreaterThan("longOperationTimeout", longOperationTimeout, 0);
            ArgumentValidator.IsGreaterThan("connectionTimeout", connectionTimeout, longOperationTimeout);

            ConnectionTimeout = connectionTimeout;
            LongOperationTimeout = longOperationTimeout;
        }

        #endregion

        #region URL building

        public static string GetUrlScheme(bool secured)
        {
            return secured ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        }

        public static string GetEndpointUrl(IServiceDescriptor serviceDescriptor)
        {
            return GetEndpointUrl(
                serviceDescriptor.Secured,
                serviceDescriptor.Server,
                serviceDescriptor.Port,
                serviceDescriptor.ServiceName,
                string.Empty
            );
        }

        public static string GetEndpointUrl(IServiceDescriptor serviceDescriptor, string endpoint)
        {
            return GetEndpointUrl(
                serviceDescriptor.Secured,
                serviceDescriptor.Server,
                serviceDescriptor.Port,
                serviceDescriptor.ServiceName,
                endpoint
            );
        }

        public static string GetEndpointUrl(bool secured, string server, int port, string service, string endpoint)
        {
            StringBuilder builder = GetUriBuilder(GetUrlScheme(secured), server, port);
            builder.Append(service);
            if (builder[builder.Length - 1] != '/')
            {
                builder.Append("/");
            }
            builder.Append(endpoint);
            return builder.ToString();
        }

        public static string GetEndpointUrl(string scheme, string server, int port, string path)
        {
            StringBuilder builder = GetUriBuilder(scheme, server, port);
            if (!string.IsNullOrEmpty(path))
            {
                builder.Append(path);

                if (builder[builder.Length - 1] != '/')
                {
                    builder.Append("/");
                }
            }

            return builder.ToString();
        }

        public static string GetListenerUri(bool secured, string server, int port)
        {
            return GetUriBuilder(GetUrlScheme(secured), server, port).ToString();
        }

        #endregion

        #region Bindings

        public static Binding CreateApplicationBinding(bool allowSessions)
        {
            return CreateWsBinding(allowSessions);
        }

        #endregion

        #region Behaviors

        public static void AddServiceBehaviors(ServiceDescription descriptor)
        {
            if (!descriptor.Behaviors.Contains(typeof(ServiceDebugBehavior)))
            {
                descriptor.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            }
            if (!descriptor.Behaviors.Contains(typeof(ServiceThrottlingBehavior)))
            {
                descriptor.Behaviors.Add(new ServiceThrottlingBehavior());
                /*behavior.MaxConcurrentCalls = 100;
                behavior.MaxConcurrentInstances = 100;
                behavior.MaxConcurrentSessions = 100;*/
            }

            ServiceBehaviorAttribute sba = descriptor.Behaviors.Find<ServiceBehaviorAttribute>();
            if (sba == null)
            {
                sba = new ServiceBehaviorAttribute();
                descriptor.Behaviors.Add(sba);
            }

            sba.MaxItemsInObjectGraph = int.MaxValue;

#if DEBUG
            if (!descriptor.Behaviors.Contains(typeof(ServiceSecurityAuditBehavior)))
            {
                descriptor.Behaviors.Add(new ServiceSecurityAuditBehavior
                    {
                        AuditLogLocation = AuditLogLocation.Application,
                        ServiceAuthorizationAuditLevel = AuditLevel.Failure,
                        MessageAuthenticationAuditLevel = AuditLevel.Failure,
                        SuppressAuditFailure = true
                    });
            }
#endif
        }

        #endregion

        #region Helpers

        private static StringBuilder GetUriBuilder(string scheme, string server, int port)
        {
            // 'server' is expected to be in form of 'servername/applicationname', e.g: 'MyServer/Infor.BI.WebFramework.Web',
            // i.e. NO SCHEME, NO PORT
            // see also Infor.BI.ConfigService/PendingRegistrations.cs method TryGetApproval(..)

            StringBuilder builder = new StringBuilder();

            string serverName = server;
            int pos = server.IndexOf("://", StringComparison.InvariantCultureIgnoreCase);
            if (pos > 0)
            {
                serverName = server.Substring(pos + 3);
            }

            string appName = null;
            pos = serverName.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (pos > 0)
            {
                appName = serverName.Substring(pos + 1);
                serverName = serverName.Substring(0, pos);
            }

            builder.Append(scheme);
            builder.Append("://");
            builder.Append(serverName);
            if (port > 0)
            {
                builder.Append(":");
                builder.Append(port);
            }
            if (!String.IsNullOrEmpty(appName))
            {
                builder.Append("/");
                builder.Append(appName);
            }
            if (builder[builder.Length - 1] != '/')
            {
                builder.Append("/");
            }

            return builder;
        }

        private static Binding CreateWsBinding(bool allowSession, MessageCredentialType security = MessageCredentialType.None)
        {
            WSHttpBinding binding = new WSHttpBinding
                {
                    AllowCookies = true,
                    BypassProxyOnLocal = false,
                    TransactionFlow = false,
                    MessageEncoding = WSMessageEncoding.Text,
                    TextEncoding = Encoding.UTF8,
                    UseDefaultWebProxy = true,
                    HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                    MaxReceivedMessageSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 0, ConnectionTimeout)
                };

            binding.ReliableSession.Enabled = allowSession;
            binding.ReliableSession.Ordered = false;
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 0, ConnectionTimeout);

            binding.Security.Mode = security == MessageCredentialType.None ? SecurityMode.None : SecurityMode.Message;
            binding.Security.Transport.Realm = String.Empty;
            binding.Security.Message.NegotiateServiceCredential = true;
            binding.Security.Message.ClientCredentialType = security;

            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxStringContentLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                    MaxBytesPerRead = Int32.MaxValue,
                    MaxDepth = Int32.MaxValue,
                    MaxNameTableCharCount = Int32.MaxValue
                };

            BindingElementCollection elements = binding.CreateBindingElements();
            EnableMultithreadingForReliableSessions(elements);
            return new CustomBinding(elements) { ReceiveTimeout = new TimeSpan(0, 0, ConnectionTimeout) };
        }

        private static void EnableMultithreadingForReliableSessions(BindingElementCollection elements)
        {
            ReliableSessionBindingElement sessionBindingElement = elements.Find<ReliableSessionBindingElement>();
            if (sessionBindingElement != null)
            {
                sessionBindingElement.FlowControlEnabled = true;
                sessionBindingElement.Ordered = false;
                sessionBindingElement.MaxPendingChannels = 16384;
                sessionBindingElement.MaxTransferWindowSize = 4096;
                sessionBindingElement.AcknowledgementInterval = TimeSpan.FromMinutes(1);
                sessionBindingElement.MaxRetryCount = 64;
            }
        }

        #endregion
    }
}