TO run the file:

1. Create new C# ASP .NET Web application in Visual Studio 2015 and name it "WebAppInventory".  Import these files from Github
- InventoryError.cs
- InventoryItem.cs
- InventoryService.asmx
- InventoryService.asmx.cs
- NotificationMessage.cs
- Resource1.resx
- Resource1.Designer.cs
2. Build solution.
3. Right click on InventoryService class and create unit tets. Replace unit test file with "InventoryServiceTest.cs" from Github. Add reference to "System.Web.Services" to fix build error. Run unit tests. 
2. Create a new C# console application in Visual Studio 2015 and name it "InventoryClient". Import files from "InventoryClient" fodler on GitHub.  Add reference to "SYstem.Web.Services" upon bild error. Run client application and make note of messages displayed regarding expired and removed inventory items. 



Design docuemntation:

I have 10 years .NET developemnt experience, but new to web services. My design therefore may have some flows. 

InventoryService.asmx: implements a web service in C# and exposes the following web methods:

		/// <summary>
        /// Add a new item to the inventory. 
        /// </summary>
        /// <param name="label">unique label of new item.</param>
        /// <param name="expirationDate">Expiration date.</param>
        /// <param name="itemType">Type of item: InventoryItemType.Solid, InventoryItemType.Gas, InventoryItemType.Liquid</param>
        /// <returns>InventoryError with ErrorCode.Success if successfull and  InventoryError with an error ErrorCode if failed.</returns>
        [WebMethod]
        public InventoryError  AddItem(string label, DateTime expirationDate, InventoryItemType itemType)


		/// <summary>
        /// Remove item from the inventory by label.
        /// </summary>
        /// <param name="label">unique label of item to be removed</param>
        /// <returns>InventoryError with ErrorCode.Success if successfull. InventoryError with an error ErrorCode if failed.</returns>
        [WebMethod]
        public InventoryError RemoveItem(string label)


		/// <summary>
        /// Subsribe to notifications. 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        [WebMethod]
        public IDisposable Subscribe(IObserver<NotificationMessage> observer)

	InventoryService service stores inventory items in memory using a hashtable for fast lookup of items by label.
	We assume that each item has a unique label. AddItem fails and returns an error if an item with the same label already exists.  
	The service implements the IObservable interface in order to notify clients who subscribe when an item expires or it's deleted. 
	Expired items: 
	1.  When a new item is added to the inventory the service checks if it's expired and notifies all subscribers. 
	2. Using a timer, the service periodicaly checks for newly expired items and notifies all subscribers. For testing pourposes the timeout value for the timer is set to 1 minute. In production it may be acceptable to set it to a much bigger value, like 24 hours. We should consider making the timeout value a setting or a public property, so it can be changed. 
	 
IventoryItem.cs: Implements InventoryItem class.
