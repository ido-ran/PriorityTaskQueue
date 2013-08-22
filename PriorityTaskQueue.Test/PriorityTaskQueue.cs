using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityTaskQueue.Test {
  [TestClass]
  public class PriorityTaskQueue {
    [TestMethod]
    public void EnqueueOneItem() {
      string handledItem = null;

      var subject = new PriorityTaskQueue<string>(
        item => handledItem = item);
      subject.Post("hello", 0);

      subject.BusyTask.Wait();

      Assert.AreEqual("hello", handledItem);
    }

    [TestMethod]
    public void Enqueue10ItemsSamePriority() {
      int handledItems = 0;

      var subject = new PriorityTaskQueue<string>(
        item => handledItems++);

      for (int i = 0; i < 10; i++) {
        subject.Post("A" + i, 0);
      }

      subject.BusyTask.Wait();

      Assert.AreEqual(10, handledItems);
    }

    [TestMethod]
    public void EnqueueSeveralSlowLowPriorityItemsAndThenHighPriorityItem() {
      List<string> handledItemsOrder = new List<string>();

      var subject = new PriorityTaskQueue<string>(
        item =>
        {
          handledItemsOrder.Add(item);
          if (item.StartsWith("A")) {
            Thread.Sleep(50);
          }
        });

      for (int i = 0; i < 4; i++) {
        subject.Post("A" + i, 22);
      }

      subject.Post("B", 0);

      subject.BusyTask.Wait();

      CollectionAssert.AreEqual(new[] { "A0", "B", "A1", "A2", "A3" }, handledItemsOrder);
    }
  }
}
