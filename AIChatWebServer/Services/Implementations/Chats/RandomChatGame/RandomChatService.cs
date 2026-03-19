using AIChatWebServer.Models.Chats;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public class RandomChatService : IRandomChatService
    {
        public event Action<Chat>? OnChatEnded;

        private readonly PriorityQueue<Chat, DateTime> _chats = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _roundTime;
        private readonly Lock _lock = new();

        private CancellationTokenSource? _timerCts;

        public RandomChatService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;

            _roundTime = int.Parse(configuration["RandomChatSettings:RoundTimeMinutes"]
                ?? throw new ArgumentException("Random Chat Round Time is not configured."));
        }

        public async Task Create(Guid first, Guid second, Guid chatId, CancellationToken ct)
        {
            Chat chat;

            using (var scope = _scopeFactory.CreateScope())
            {
                var chatRepository = scope.ServiceProvider.GetRequiredService<IChatRepository>();

                DateTime endTime = DateTime.UtcNow.AddMinutes(_roundTime);

                await chatRepository.End(chatId, endTime, ct);

                chat = await chatRepository.GetById(chatId, ct)
                    ?? throw new ArgumentException();
            }

            bool shouldReschedule = false;

            lock (_lock)
            {
                bool isFirst = _chats.Count == 0;

                DateTime? currentNext = null;
                if (!isFirst)
                {
                    currentNext = _chats.Peek().EndTime;
                }

                _chats.Enqueue(chat, chat.EndTime
                    ?? throw new ArgumentException(nameof(chat.EndTime)));

                if (isFirst || chat.EndTime < currentNext)
                {
                    shouldReschedule = true;
                }
            }

            if (shouldReschedule)
            {
                RescheduleTimer();
            }
        }

        private void RescheduleTimer()
        {
            CancellationTokenSource? oldCts;

            lock (_lock)
            {
                oldCts = _timerCts;
                _timerCts = new CancellationTokenSource();
            }

            oldCts?.Cancel();

            _ = RunTimerAsync(_timerCts.Token);
        }

        private async Task RunTimerAsync(CancellationToken ct)
        {
            while (true)
            {
                Chat? nextChat;

                lock (_lock)
                {
                    if (_chats.Count == 0)
                        return;

                    nextChat = _chats.Peek();
                }

                if (!nextChat.EndTime.HasValue)
                {
                    throw new ArgumentNullException(nameof(nextChat.EndTime));
                }

                TimeSpan delay = nextChat.EndTime.Value - DateTime.UtcNow;

                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero;

                try
                {
                    await Task.Delay(delay, ct);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                List<Chat> endedChats = [];

                lock (_lock)
                {
                    DateTime now = DateTime.UtcNow;

                    while (_chats.Count > 0)
                    {
                        Chat chat = _chats.Peek();

                        if (chat.EndTime > now)
                            break;

                        endedChats.Add(_chats.Dequeue());
                    }
                }

                foreach (var chat in endedChats)
                {
                    OnChatEnded?.Invoke(chat);
                }
            }
        }
    }
}