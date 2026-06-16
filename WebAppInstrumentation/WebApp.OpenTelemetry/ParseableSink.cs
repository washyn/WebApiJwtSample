using Serilog;
using Serilog.Core;
using Serilog.Events;

using System.Text;
using System.Text.Json;

using Serilog.Configuration;

public class ParseableSink : ILogEventSink, IDisposable
{
    private readonly HttpClient _client;
    private readonly string _stream;
    private readonly List<object> _buffer = new();
    private readonly object _lock = new();
    private readonly Timer _flushTimer;

    public ParseableSink(string url, string dataset, string username, string password)
    {
        _stream = dataset;
        _client = new HttpClient { BaseAddress = new Uri(url) };
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

        _flushTimer = new Timer(_ => Flush(), null,
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    public void Emit(LogEvent logEvent)
    {
        var entry = new
        {
            timestamp = logEvent.Timestamp.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            level = logEvent.Level.ToString().ToLower(),
            message = logEvent.RenderMessage(),
            exception = logEvent.Exception?.ToString(),
            properties = logEvent.Properties.ToDictionary(
                p => p.Key,
                p => p.Value.ToString().Trim('"'))
        };

        lock (_lock)
        {
            _buffer.Add(entry);
            if (_buffer.Count >= 100)
            {
                FlushInternal();
            }
        }
    }

    public void Flush()
    {
        lock (_lock) { FlushInternal(); }
    }

    private void FlushInternal()
    {
        if (_buffer.Count == 0) return;

        var entries = _buffer.ToList();
        _buffer.Clear();

        Task.Run(async () =>
        {
            var json = JsonSerializer.Serialize(entries);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("X-P-Stream", _stream);
            await _client.PostAsync("/api/v1/ingest", content);
        });
    }

    public void Dispose()
    {
        _flushTimer.Dispose();
        Flush();
        _client.Dispose();
    }
}

// Extension method
public static class ParseableSinkExtensions
{
    public static LoggerConfiguration Parseable(
        this LoggerSinkConfiguration config,
        string url,
        string dataset,
        string username,
        string password)
    {
        return config.Sink(new ParseableSink(url, dataset, username, password));
    }
}