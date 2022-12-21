using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafePriorityQueue;

public class ThreadSafePriorityQueue<TElement>
{
    private SortedList<int, Queue<TElement>> priorityQueue;
    private int size;
    private object lockObject;

    public ThreadSafePriorityQueue()
    {
        this.priorityQueue = new SortedList<int, Queue<TElement>>();
        this.lockObject = new object();
    }

    public int Size => this.size;
    
    public void Enqueue(TElement value, int priority)
    {
        lock(lockObject)
        {
            if (!this.priorityQueue.ContainsKey(priority))
            {
                this.priorityQueue.Add(priority, new Queue<TElement>());
            }

            this.priorityQueue[priority].Enqueue(value);
            this.size++;
            Monitor.PulseAll(lockObject);
        }
    }

    public TElement Dequeue()
    {
        lock(this.lockObject)
        {
            while (this.priorityQueue.Count == 0)
            {
                Monitor.Wait(this.lockObject);
            }

            var result = this.priorityQueue.Last().Value.Dequeue();
            if(this.priorityQueue.Last().Value.Count == 0)
            {
                this.priorityQueue.Remove(this.priorityQueue.Last().Key);
            }

            this.size--;
            return result;
        }
    }

}
