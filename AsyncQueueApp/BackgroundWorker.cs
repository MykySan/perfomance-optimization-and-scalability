public class BackgroundWorker
{
    private readonly TaskQueue _taskQueue;

    public BackgroundWorker(TaskQueue taskQueue)
    {
        _taskQueue = taskQueue;
    }

    public async Task StartProcessing()
    {
        while (await _taskQueue.GetReader().WaitToReadAsync())
        {
            var task = await _taskQueue.DequeueTask();
            await task();
        }
    }
}