
/// <summary>
/// 
/// </summary>
namespace WebAppInventory
{
    /// <summary>
    /// Notification message class
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="item">Inventory item, for which the message is about.</param>
        /// <param name="message">InventoryError message about the inventory item</param>
        public NotificationMessage(InventoryItem item, InventoryError message)
        {
            InventoryItem = item;
            Message = message;
        }

        /// <summary>
        /// Get propoerty for the Inventory item.
        /// </summary>
        public InventoryItem InventoryItem { get; private set; }

        /// <summary>
        /// Get property for the InventoryError message.
        /// </summary>
        public InventoryError Message { get; private set; }
    }
}
