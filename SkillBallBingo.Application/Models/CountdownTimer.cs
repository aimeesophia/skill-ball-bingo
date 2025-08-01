namespace SkillBallBingo.Application.Models;

public class CountdownTimer
{
    public string DisplayTime => TimeSpan.FromSeconds(RemainingSeconds).ToString(@"mm\:ss");
    
    public event Action? OnTick;
    
    public event Action? OnTimeUp;

    public event Action? OnTimeAdded;
    
    public event Action? OnTimeRemoved;
    
    public event Action? OnLastSeconds;

    private int RemainingSeconds { get; set; } = 60;

    public int LastSeconds { get; set; } = 10;
    
    private Timer? _timer;

    public void Start()
    {
        _timer = new Timer(_ =>
        {
            if (RemainingSeconds > 0)
            {
                OnTick?.Invoke();

                if (RemainingSeconds < LastSeconds)
                {
                    OnLastSeconds?.Invoke();
                }
                
                RemainingSeconds--;
            }
            else
            {
                OnTimeUp?.Invoke();
                Stop();
            }
        }, null, 0, 1000);
    }

    public void AddTime()
    {
        RemainingSeconds += 5;
        OnTimeAdded?.Invoke();
    }

    public void RemoveTime()
    {
        RemainingSeconds -= 5;
        OnTimeRemoved?.Invoke();
    }

    private void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }
}