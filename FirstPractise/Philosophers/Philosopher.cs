using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Philosophers;

public class Philosopher
{
    private int number;
    public object[] forks;
    public int numberOfPhilosophers;
    public Philosopher(int number, object[] forks, int numberOfPhilosophers)
    {
        this.number = number;
        this.forks = forks;
        this.numberOfPhilosophers = numberOfPhilosophers;
    }

    private Random random = new Random();

    public void Live()
    {
        var nextFork = this.number == numberOfPhilosophers - 1 ? 0 : this.number + 1;
        var firstForkNumber = this.number;
        if (firstForkNumber < nextFork)
        {
            (firstForkNumber, nextFork) = (nextFork, firstForkNumber);
        }

        this.Think();
        Console.WriteLine($"Philosopher number {this.number} try to get {firstForkNumber} fork");
        lock (forks[this.number])
        {
            Console.WriteLine($"Philosopher number {this.number} try to get {nextFork} fork");
            lock (forks[nextFork])
            {
                this.Eat();
            }
        }
    }
    public void Think()
    {
        Console.WriteLine($"Philosopher number {this.number} thinks");
        Thread.Sleep(random.Next(100));
    }

    public void Eat()
    {
        Console.WriteLine($"Philosopher number {this.number} eats");
        Thread.Sleep(random.Next(100));
    }
}
