using System.Threading.Channels;

public class TaskQueue
{
    private readonly Channel<Func<Task>> _queue;

    public TaskQueue()
    {
        _queue = Channel.CreateUnbounded<Func<Task>>();
    }

    public async Task EnqueueTask(Func<Task> task)
    {
        await _queue.Writer.WriteAsync(task);
    }

    public async Task<Func<Task>> DequeueTask()
    {
        return await _queue.Reader.ReadAsync();
    }

    public ChannelReader<Func<Task>> GetReader()
    {
        return _queue.Reader;
    }
}