using System;
using System.Threading.Tasks;

namespace PriorityTaskQueue {
  /// <summary>
  /// Priority FIFO task processing queue.
  /// </summary>
  /// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// Post item to be processed later in the queue.
    /// </summary>
    /// <param name="item">Item information</param>
    /// <param name="priority">Lower value indicate higher priority.</param>
    public void Post(T item, int priority) {
      lock (locker) {
        queue.Enqueue(item, priority);
        StartTaskIfNeed();
      }
    }

    /// <summary>
    /// Internal Task that signal when all the items
    /// in the queue were processed.
    /// </summary>
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
