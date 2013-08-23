Priority Task Queue
===================

C# TPL Priority Queue

## The Need
I needed a way to post work items into a queue and have them processed one at a time, yet have the ability to post high priority items and get them done first.

## The Solution
I've use a simple implementation of PriorityQueue to store the work items.  
I've use TPL Task to handle the execution and continouation and thus eliminating the need for hard-to-test multi-threading code.

## The Use
      int handledItems = 0;
      
      var subject = new PriorityTaskQueue<string>(
        item => handledItems++);

      for (int i = 0; i < 10; i++) {
        subject.Post("A" + i, 0);
      }

## License
This project is under MIT license.

## Limitations
PriorityTaskQueue is currently designed to process one item at a time.
