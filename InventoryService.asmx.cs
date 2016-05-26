using System;
using System.Collections.Generic;
using System.Web.Services;

namespace WebAppInventory
{
    /// <summary>
    /// Web service to manage an inventory.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class InventoryService : System.Web.Services.WebService, IObservable<NotificationMessage>
    {
        private class DictionaryValue
        {
            public DictionaryValue(InventoryItem item, bool expirationReported)
            {
                InventoryItem = item;
                ExpirationReported = expirationReported;
            }

            public InventoryItem InventoryItem { get; private set; }
            public bool ExpirationReported { get; set; }
        }

        private Object m_lock = new object();
        private static Dictionary<string, DictionaryValue> m_inventory;
        private static TimeSpan m_monitoringTimeInterval = new TimeSpan(TimeSpan.TicksPerMinute); // 1 minute by default for easy testing in this example. In production this could be set to a longer time such as 1 day.
        System.Timers.Timer m_timer;
        private static List<IObserver<NotificationMessage>> m_observers;

        public InventoryService()
        {
            m_inventory = new Dictionary<string, DictionaryValue>();
            m_observers = new List<IObserver<NotificationMessage>>();
        }

        /// <summary>
        /// Get property to retrive number of items in inventory
        /// </summary>
        public int ItemCount
        {
            get
            {
                return m_inventory == null ? 0 : m_inventory.Count;
            }
        }


        /// <summary>
        /// Add a new item to the inventory. 
        /// </summary>
        /// <param name="label">unique label of new item.</param>
        /// <param name="expirationDate">Expiration date.</param>
        /// <param name="itemType">Type of item: InventoryItemType.Solid, InventoryItemType.Gas, InventoryItemType.Liquid</param>
        /// <returns>InventoryError with ErrorCode.Success if successfull and  InventoryError with an error ErrorCode if failed.</returns>
        [WebMethod]
        public InventoryError  AddItem(string label, DateTime expirationDate, InventoryItemType itemType)
        {
            lock (m_lock)
            {
                if (!InventoryItem.ValidLabelStatic(label))
                {
                    return new InventoryError(Severity.Error, ErrorCode.EmptyLabelError, null);
                }

                InventoryItem item = new InventoryItem(label, expirationDate, itemType);
                if (m_inventory.ContainsKey(label))
                {
                    return new InventoryError(Severity.Error, ErrorCode.ItemWithLabelExistsError, new string[] { label });
                }

                DictionaryValue value = new DictionaryValue(item, false);
                m_inventory.Add(item.Label,value );

                if (item.Expired())
                {
                    value.ExpirationReported = true;
                    SendNotification(new NotificationMessage(item, new InventoryError(Severity.Error, ErrorCode.ExpiredItemWarning, new string[] { label })));
                }

                return new InventoryError(Severity.Info, ErrorCode.Success, null);
            }
        }


        /// <summary>
        /// Remove item from the inventory by label.
        /// </summary>
        /// <param name="label">unique label of item to be removed</param>
        /// <returns>InventoryError with ErrorCode.Success if successfull. InventoryError with an error ErrorCode if failed.</returns>
        [WebMethod]
        public InventoryError RemoveItem(string label)
        {
            lock (m_lock)
            {
                InventoryItem item = GetItem(label);
                if (item == null)
                {
                    if (label == null)
                    {
                        label = string.Empty;
                    }
                    return new InventoryError(Severity.Error, ErrorCode.LabelNotFoundError, new string[] { label });
                }

                m_inventory.Remove(label);

                SendNotification(new NotificationMessage(item, new InventoryError(Severity.Info, ErrorCode.ItemRemovedInfo, new string[] { label })));
                return new InventoryError(Severity.Info, ErrorCode.Success, null);
            }
        }

        /// <summary>
        /// Find item in the inventory by label.
        /// </summary>
        /// <param name="label">unique label of item to be found</param>
        /// <returns>Inventory item if found, null otherwise.</returns>
        public InventoryItem GetItem(string label)
        {
            if (!InventoryItem.ValidLabelStatic(label))
            {
                return null;
            }

            if (!m_inventory.ContainsKey(label))
            {
                return null;
            }

            return m_inventory[label].InventoryItem;
        }

        /// <summary>
        /// Subsribe to notifications. 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        [WebMethod]
        public IDisposable Subscribe(IObserver<NotificationMessage> observer)
        {
            StartMonitoring();
            if (!m_observers.Contains(observer))
            {
                m_observers.Add(observer);
                // provide observer with existing data
                foreach (DictionaryValue value in m_inventory.Values)
                {
                    if (value.InventoryItem.Expired())
                    {
                        observer.OnNext(new NotificationMessage(value.InventoryItem, new InventoryError(Severity.Error, ErrorCode.ExpiredItemWarning, new string[] { value.InventoryItem.Label })));
                    }
                }
            }

            return new Unsubscriber(m_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<NotificationMessage>> _observers;
            private IObserver<NotificationMessage> _observer;

            public Unsubscriber(List<IObserver<NotificationMessage>> observers, IObserver<NotificationMessage> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        private void StartMonitoring()
        {
            if (m_monitoringTimeInterval > TimeSpan.Zero)
            {
                if (m_timer == null)
                {
                    m_timer = new System.Timers.Timer(m_monitoringTimeInterval.TotalMilliseconds);
                }

                m_timer.Elapsed += Timer_Elapsed;
                m_timer.Start();
            }
            else
            {
                StopMonitoring();
            }
        }

        private void StopMonitoring()
        {
            if (m_timer != null)
            {
                m_timer.Elapsed -= Timer_Elapsed;
                m_timer.Stop();
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendNotificationForNewlyExpiredItems();
        }

        private static void SendNotificationForNewlyExpiredItems() 
        {
            foreach (DictionaryValue value in m_inventory.Values)
            {
                if (!value.ExpirationReported && value.InventoryItem.Expired())
                {
                    value.ExpirationReported = true;
                    SendNotification(new NotificationMessage(value.InventoryItem, new InventoryError(Severity.Error, ErrorCode.ExpiredItemWarning, new string[] { value.InventoryItem.Label })));
                }
            }
        }

        private static void SendNotification(NotificationMessage message)
        {
            foreach (var observer in m_observers)
                observer.OnNext(message);
        }

    }




}
