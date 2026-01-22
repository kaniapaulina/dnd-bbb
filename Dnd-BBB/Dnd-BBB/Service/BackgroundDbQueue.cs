using Dnd_BBB.Core;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Dnd_BBB.Service
{
    // Prosty worker queue — singleton uruchamiany raz; bezpieczny dla w¹tków.
    public sealed class BackgroundDbQueue : IDisposable
    {
        private readonly Channel<Func<Task>> _queue = Channel.CreateUnbounded<Func<Task>>();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;

        public static BackgroundDbQueue Instance { get; } = new BackgroundDbQueue();

        private BackgroundDbQueue()
        {
            _worker = Task.Run(WorkerLoop);
        }

        public ValueTask Enqueue(Func<Task> work) => _queue.Writer.WriteAsync(work);

        public ValueTask EnqueueSaveCharacterAsync(Character c)
            => Enqueue(() => Task.Run(() => PartyRepository.SaveCharacter(c)));

        public ValueTask EnqueueSavePartyAsync(Party p)
            => Enqueue(() => Task.Run(() => PartyRepository.SaveParty(p)));

        private async Task WorkerLoop()
        {
            try
            {
                while (await _queue.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (_queue.Reader.TryRead(out var work))
                    {
                        try
                        {
                            await work();
                        }
                        catch (Exception ex)
                        {
                            // TODO: logowaæ (Debug/plik) — nie blokujemy dalszych zadañ
                            System.Diagnostics.Debug.WriteLine($"BackgroundDbQueue task failed: {ex}");
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _queue.Writer.TryComplete();
            try { _worker.Wait(TimeSpan.FromSeconds(5)); } catch { }
            _cts.Dispose();
        }
    }
}