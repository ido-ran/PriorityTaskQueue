using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityTaskQueue {
  public class PriorityTaskQueue<T> {

    private readonly Action<T> handler;
    private readonly object locker;
    private readonly PriorityQueue<T> queue;

    private bool running;
    internal TaskCompletionSource<int> tcs;

    public PriorityTaskQueue(Action<T> handler) {
      this.handler = handler;
      locker = new object();
      queue = new PriorityQueue<T>();
    }

    public void Post(T item, int priority) {
      lock (locker) {
        queue.Enqueue(item, priority);
        StartTaskIfNeed();
      }
    }

    internal Task BusyTask {
      get { return tcs.Task; }
    }

    /// <summary>
    /// Start task to handle the next item if needed.
    /// This method always called inside the lock.
    /// </summary>
    private void StartTaskIfNeed() {
      if (!running) {
        RunNextItem();
        tcs = new TaskCompletionSource<int>();
        running = true;
      }
    }

    private void RunNextItem() {
      T nextItem = queue.Dequeue();

      Task task = new Task(() => handler(nextItem));
      task.ContinueWith(OnTaskFinished);
      task.Start();
    }

    private void OnTaskFinished(Task finishedTask) {
      lock (locker) {
        if (0 == queue.Count) {
          running = false;
          tcs.TrySetResult(0);
          tcs = null;
        }
        else {
          RunNextItem();
        }
      }
    }
  }
}
