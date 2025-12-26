using System.Threading.Tasks;

namespace ClipFlow.Interfaces;

public interface IClipboardService
{
    Task SetTextAsync(string text);
    Task<string?> GetTextAsync();
}