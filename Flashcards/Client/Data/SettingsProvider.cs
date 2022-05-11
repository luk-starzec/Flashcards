using Microsoft.JSInterop;
using System.Text.Json;

namespace Flashcards.Client.Data;

public class SettingsProvider : ISettingsProvider
{
    private readonly IJSRuntime _js;

    public SettingsProvider(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<T?> GetValueAsync<T>(string key) where T : class
    {
        var value = await GetValueAsync(key);
        return value is not null ? JsonSerializer.Deserialize<T>(value) : null;
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/settingsStorage.js");

        var value = await module.InvokeAsync<string>("getLocalValue", key);

        return string.IsNullOrEmpty(value) || value == "undefined" ? null : value;
    }

    public async Task SetValueAsync<T>(string key, T value) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        await SetValueAsync(key, json);
    }

    public async Task SetValueAsync(string key, string value)
    {
        var module = await _js.InvokeAsync<IJSObjectReference>("import", "./scripts/settingsStorage.js");

        await module.InvokeVoidAsync("setLocalValue", key, value);
    }
}
