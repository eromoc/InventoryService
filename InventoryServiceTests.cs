using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace WebAppInventory.Tests
{
    [TestClass()]
    public class InventoryServiceTests
    {
        [TestMethod()]
        public void InventoryServiceTest()
        {
            InventoryService target = new InventoryService();
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void AddItemTest()
        {
            InventoryService target = new InventoryService();
            Assert.AreEqual(target.ItemCount, 0);

            // try to add new item to inventory
            string label = "Reagent1";
            DateTime expiration = new DateTime(2020, 12, 31);
            InventoryItemType type = InventoryItemType.Liquid;
            InventoryError error = target.AddItem(label, expiration, type);
            Assert.AreEqual(error.ErrorCode, ErrorCode.Success);
            Assert.AreEqual(target.ItemCount, 1);
            InventoryItem item = target.GetItem(label);
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Label, label);
            Assert.AreEqual(item.ExpirationDate, expiration);
            Assert.AreEqual(item.ItemType, type);

            // try to add item with existing label to inventory
            DateTime expiration2 = new DateTime(2022, 12, 31);
            InventoryItemType type2 = InventoryItemType.Gas;
            error = target.AddItem(label, expiration2, type2);
            Assert.AreEqual(error.ErrorCode, ErrorCode.ItemWithLabelExistsError);
            Assert.AreEqual(target.ItemCount, 1);

            // try to add new item to inventory with new label
            string label3 = "Reagent3";
            DateTime expiration3 = new DateTime(2023, 12, 31);
            InventoryItemType type3 = InventoryItemType.Gas;
            error = target.AddItem(label3, expiration3, type3);
            Assert.AreEqual(error.ErrorCode, ErrorCode.Success);
            Assert.AreEqual(target.ItemCount, 2);
            item = target.GetItem(label3);
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Label, label3);
            Assert.AreEqual(item.ExpirationDate, expiration3);
            Assert.AreEqual(item.ItemType, type3);

            // Verify that no other data was overwritten in inventory
            item = target.GetItem(label);
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Label, label);
            Assert.AreEqual(item.ExpirationDate, expiration);
            Assert.AreEqual(item.ItemType, type);
        }

        [TestMethod()]
        public void RemoveItemTest()
        {
            InventoryService target = new InventoryService();
            Assert.AreEqual(target.ItemCount, 0);

            // add new items to inventory
            string label = "Reagent1";
            DateTime expiration = new DateTime(2020, 12, 31);
            InventoryItemType type = InventoryItemType.Liquid;
            target.AddItem(label, expiration, type);
            Assert.AreEqual(target.ItemCount, 1);

            string label2 = "Reagent2";
            DateTime expiration2 = new DateTime(2023, 12, 31);
            InventoryItemType type2 = InventoryItemType.Gas;
            target.AddItem(label2, expiration2, type2);
            Assert.AreEqual(target.ItemCount, 2);

            // try removing item not in the inventory
            string label3 = "Reagent3";
            InventoryError error = target.RemoveItem(label3);
            Assert.AreEqual(error.ErrorCode, ErrorCode.LabelNotFoundError);
            Assert.AreEqual(target.ItemCount, 2);

            // try removing existing item from inventory
            error = target.RemoveItem(label2);
            Assert.AreEqual(error.ErrorCode, ErrorCode.Success);
            Assert.AreEqual(target.ItemCount, 1);
            InventoryItem item = target.GetItem(label2);
            Assert.IsNull(item);
            item = target.GetItem(label);
            Assert.IsNotNull(item);

            // try removing existing item from inventory
            error = target.RemoveItem(label);
            Assert.AreEqual(error.ErrorCode, ErrorCode.Success);
            Assert.AreEqual(target.ItemCount, 0);
            item = target.GetItem(label);
            Assert.IsNull(item);

            // try removing item from empty inventory
            error = target.RemoveItem(label);
            Assert.AreEqual(error.ErrorCode, ErrorCode.LabelNotFoundError);
            Assert.AreEqual(target.ItemCount, 0);
        }


        [TestMethod()]
        public void GetItemTest()
        {
            InventoryService target = new InventoryService();
            Assert.AreEqual(target.ItemCount, 0);

            // add new item to inventory
            string label = "Reagent1";
            DateTime expiration = new DateTime(2020, 12, 31);
            InventoryItemType type = InventoryItemType.Liquid;
            InventoryError error = target.AddItem(label, expiration, type);
            Assert.AreEqual(error.ErrorCode, ErrorCode.Success);
            Assert.AreEqual(target.ItemCount, 1);

            // get existing item
            InventoryItem item = target.GetItem(label);
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Label, label);
            Assert.AreEqual(item.ExpirationDate, expiration);
            Assert.AreEqual(item.ItemType, type);

            // get non existing item
            item = target.GetItem("Reagent2");
            Assert.IsNull(item);
        }

    }
}