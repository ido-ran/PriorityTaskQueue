using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityTaskQueue.Test {
  [TestClass]
  public class PriorityQueueTest {
    [TestMethod]
    public void TwoItemsSamePriority() {
      PriorityQueue<int> q = new PriorityQueue<int>();
      q.Enqueue(0, 1);
      q.Enqueue(1, 1);

      int first = q.Dequeue();
      int second = q.Dequeue();

      Assert.AreEqual(0, first);
      Assert.AreEqual(1, second);
    }

    [TestMethod]
    public void TwoItemsDifferentPriorities() {
      var q = new PriorityQueue<string>();
      q.Enqueue("A", 10);
      q.Enqueue("B", 0);

      string first = q.Dequeue();
      string second = q.Dequeue();

      Assert.AreEqual("B", first);
      Assert.AreEqual("A", second);
    }

    [TestMethod]
    public void ManyLowPriorityItemsAndOneHigh() {
      var q = new PriorityQueue<string>();

      for (int i = 0; i < 100; i++) {
        q.Enqueue("A" + i, 10);
      }

      q.Enqueue("B", 0);

      string first = q.Dequeue();

      Assert.AreEqual("B", first);
    }
  }
}
