using System;

namespace WebAppInventory
{
    /// <summary>
    /// Inventory items class
    /// </summary>
    public class InventoryItem
    {
        private string m_label;
        private DateTime m_expirationDate;
        private InventoryItemType m_inventoryItemType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">unique label of inventory item.</param>
        /// <param name="expirationDate">expiration date</param>
        /// <param name="itemType">type of inventory item.</param>
        public InventoryItem(string label, DateTime expirationDate, InventoryItemType itemType)
        {
            Label = label;
            ExpirationDate = expirationDate;
            ItemType = itemType;
        }

        /// <summary>
        /// Get property for item label. 
        /// </summary>
        public string Label
        {
            get
            {
                return m_label;
            }
            private set
            { 
                if (ValidLabelStatic(value))
                {
                    m_label = value;
                }
                else
                {
                    throw new Exception("Invalid parameter label.");
                }
            }
        }

        /// <summary>
        /// Get/set property for expiration date .
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return m_expirationDate; }
            set
            {
                m_expirationDate = value;
            }
        }

        /// <summary>
        /// Get/set property for item type.
        /// </summary>
        public InventoryItemType ItemType
        {
            get { return m_inventoryItemType; }
            set
            {
                m_inventoryItemType = value;
            }
        }


        /// <summary>
        /// returns true if the label is valid (not null or empty). Returns false othervise. 
        /// </summary>
        /// <returns></returns>
        public bool ValidLabel()
        {
            return ValidLabelStatic(Label);
        }

        /// <summary>
        /// returns true if the label is valid (not null or empty). Returns false othervise. 
        /// </summary>
        /// <param name="label">label to validate.</param>
        /// <returns></returns>
        public static bool ValidLabelStatic(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  returns true if the item is expired. Returns false othervise. 
        /// </summary>
        /// <returns></returns>
        public bool Expired()
        {
            return ExpiredStatic(ExpirationDate, Label);
        }

        /// <summary>
        /// Returns true if the date is expired. Returns false othervise. 
        /// </summary>
        /// <param name="expirationDate"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool ExpiredStatic(DateTime expirationDate, string label = null)
        {
            if (expirationDate <= DateTime.Now)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Inventory item type.
    /// </summary>
    public enum InventoryItemType
    {
        Solid = 0,
        Liquid,
        Gas,
    }
}