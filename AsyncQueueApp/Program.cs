using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        TaskQueue queue = new TaskQueue();
        BackgroundWorker backgroundWorker = new BackgroundWorker(queue);

        Task workerTask = Task.Run(() => backgroundWorker.StartProcessing());

        for (int i = 1; i <= 5; i++)
        {
            int taskId = i;
            await queue.EnqueueTask(async () =>
            {
                Console.WriteLine($"Task {taskId} started...");
                await Task.Delay(7000);
                Console.WriteLine($"Task {taskId} completed.");
            });
        }

        Console.WriteLine("You can type messages while tasks are running. Type 'exit' to stop.");

        while (true)
        {
            Console.WriteLine("[Main Thread] Type a message and press enter:");
            string input = Console.ReadLine();
            if (input?.ToLower() == "exit")
                break;
            Console.WriteLine($"[Main Thread] You typed: {input.ToUpper()}");
        }

        Console.WriteLine("Main thread finished. Waiting for background tasks...");

        await workerTask;
    }
}