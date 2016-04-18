using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using CommonBase.Exceptions;

namespace CommonBase.Application
{
    public abstract class ApplicationConfiguration
    {
        private readonly Dictionary<string, string> _properties;
        private bool _loaded;
        private readonly ConfigLoadType _loadType;

        protected ApplicationConfiguration(ConfigLoadType loadType)
        {
            if (Instance != null)
            {
                throw new SingletonAlreadyInitializedException(typeof(ApplicationConfiguration));
            }
            Instance = this;

            _properties = new Dictionary<string, string>();
            _loadType = loadType;
            if (_loadType == ConfigLoadType.Immediate)
            {
                Load();
            }
        }

        #region Properties

        protected virtual string RootElementName
        {
            get
            {
                return "AppConfig";
            }
        }

        protected virtual string ConfigFile
        {
            get
            {
                string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                return Path.Combine(directory, Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + ".appConfig");
            }
        }

        protected static ApplicationConfiguration Instance { get; private set; }

        #endregion

        #region Public Methods

        public void Load()
        {
            _properties.Clear();

            XmlDocument document = new XmlDocument();
            document.Load(ConfigFile);

            XmlNodeList list = document.SelectNodes("//Property");
            for (int i = 0; i < list.Count; ++i)
            {
                XmlNode node = list[i];
                string property = node.Attributes.GetNamedItem("Name").Value;
                string value = node.Attributes.GetNamedItem("Value").Value;
                _properties.Add(property, value);
            }

            _loaded = true;
        }

        protected void Set<T>(string property, T value)
        {
            _properties[property] = value.ToString();
        }

        protected T Get<T>(string property, Func<string, T> convert)
        {
            OnAccess();
            string value;
            if (_properties.TryGetValue(property, out value))
            {
                return convert(value);
            }
            return default(T);
        }

        protected T Get<T>(string property, Func<string, T> convert, Func<T> factory)
        {
            OnAccess();
            string value;
            if (_properties.TryGetValue(property, out value))
            {
                return convert(value);
            }
            else
            {
                T newValue = factory();
                Set(property, newValue);
                return newValue;
            }
        }

        protected string Get(string property)
        {
            return Get(property, v => v);
        }

        protected string Get(string property, Func<string> factory)
        {
            return Get(property, v => v, factory);
        }

        public void Close()
        {
            Instance.OnClosing();
            Instance.SaveConfig();
            Instance = null;
        }

        protected virtual void OnClosing()
        {
        }

        #endregion

        #region Internals and Helpers

        private void SaveConfig()
        {
            XmlDocument document = new XmlDocument();

            XmlDeclaration decl = document.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
            document.AppendChild(decl);

            XmlElement rootElem = document.CreateElement(RootElementName);
            document.AppendChild(rootElem);

            foreach (KeyValuePair<string, string> prop in _properties)
            {
                XmlElement propElem = document.CreateElement("Property");

                XmlAttribute attr = document.CreateAttribute("Name");
                attr.Value = prop.Key;
                propElem.Attributes.Append(attr);

                attr = document.CreateAttribute("Value");
                attr.Value = prop.Value;
                propElem.Attributes.Append(attr);

                rootElem.AppendChild(propElem);
            }

            document.Save(ConfigFile);
        }

        private void OnAccess()
        {
            if (!_loaded && _loadType == ConfigLoadType.OnDemand)
            {
                Load();
            }
        }

        #endregion
    }
}