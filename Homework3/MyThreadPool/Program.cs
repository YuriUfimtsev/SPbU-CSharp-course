using MyThreadPool;

var pull = new MyThreadPool.MyThreadPool(8);
var newTask = pull.Submit(() => 2 * 2).ContinueWith(x => x.ToString());
var p = newTask.Result;
pull.ShutDown();