using Philosophers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

int numberOfPhilosophers = 2;

var forks = new object[numberOfPhilosophers];

for (var i = 0; i < forks.Length; ++i)
{
    forks[i] = new object();
}

var threads = new Thread[numberOfPhilosophers];
for (var i = 0; i < numberOfPhilosophers; ++i)
{
    var locali = i;
    threads[i] = new Thread(() =>
    {
        var philosopher = new Philosopher(locali, forks, numberOfPhilosophers);
        while (true)
        {
            philosopher.Live();
        }
    });
}

foreach (var thread in threads)
{
    thread.Start();
}