using System;


namespace InventoryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebAppInventory.InventoryService service = new WebAppInventory.InventoryService();
            InventoryServiceMonitor monitor = new InventoryServiceMonitor();
            monitor.Subscribe(service);
            service.AddItem("orange", DateTime.Now.AddSeconds(30), WebAppInventory.InventoryItemType.Solid);
            service.AddItem("apple", DateTime.Now, WebAppInventory.InventoryItemType.Solid); // expect getting item expired notification
            System.Threading.Thread.Sleep(2000);
            service.RemoveItem("apple"); // expect getting item removed notification
            System.Threading.Thread.Sleep(65000); // after sleep expect getting item expired notification for orange
            monitor.Unsubscribe();
            service.RemoveItem("orange"); // expect not getting item removed notification, because the monitor already unsubscribed
            System.Threading.Thread.Sleep(65000);
        }
    }

    public class InventoryServiceMonitor : IObserver<WebAppInventory.NotificationMessage>
    {
        private IDisposable cancellation;
        public virtual void Subscribe(WebAppInventory.InventoryService provider)
        {
            cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            cancellation.Dispose();
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception e)
        {
            // No implementation.
        }

        // Update information.
        public virtual void OnNext(WebAppInventory.NotificationMessage message)
        {
            Console.WriteLine(message.Message.Message);
        }
    }
}
