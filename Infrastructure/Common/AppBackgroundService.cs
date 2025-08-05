using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace Infrastructure.Common;

public abstract class AppBackgroundService : BackgroundService, INotifyPropertyChanged
{
    private string _taskId = Guid.NewGuid().ToString("N") + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
    private bool _isRunning = true;
    private bool _isInterval = false;
    private string _message = string.Empty;


    public string TaskId
    {
        get => _taskId;
        set => SetProperty(nameof(TaskId), ref _taskId, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        set => SetProperty(nameof(IsRunning), ref _isRunning, value);
    }

    public bool IsInterval
    {
        get => _isInterval;
        set => SetProperty(nameof(IsInterval), ref _isInterval, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(nameof(Message), ref _message, value);
    }

    public abstract string Name { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChangedEventHandler? handler = PropertyChanged;

        if (handler != null)
        {
            handler(this, args);
        }
    }

    private void SetProperty<T>(string propertyName, ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await AppExecuteAsync(stoppingToken).ConfigureAwait(false);
    }

    public abstract Task AppExecuteAsync(CancellationToken stoppingToken);
}
