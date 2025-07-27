namespace SkillBallBingo.Application.Models;

public class CountdownTimer
{
    public string DisplayTime => TimeSpan.FromSeconds(RemainingSeconds).ToString(@"mm\:ss");
    
    public event Action? OnTick;
    
    public int RemainingSeconds { get; set; } = 60;
    
    private Timer? _timer;

    public void Start()
    {
        _timer = new Timer(_ =>
        {
            if (RemainingSeconds > 0)
            {
                RemainingSeconds--;
                OnTick?.Invoke();
            }
            else
            {
                Stop();
            }
        }, null, 0, 1000);
    }

    private void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }
}