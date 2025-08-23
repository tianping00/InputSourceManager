using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InputSourceManager.Models;

namespace InputSourceManager.Windows
{
	public class UrlReceivedEventArgs : EventArgs
	{
		public string Url { get; set; } = string.Empty;
		public string Domain { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.Now;
	}

	public sealed class UrlReceiverService
	{
		private readonly HttpListener _listener = new HttpListener();
		private readonly CancellationTokenSource _cts = new();
		
		// 事件：当收到新的 URL 时触发
		public event EventHandler<UrlReceivedEventArgs>? UrlReceived;
		
		public UrlReceiverService(string prefix = "http://127.0.0.1:43219/")
		{
			_listener.Prefixes.Add(prefix);
		}

		public async Task StartAsync(CancellationToken token)
		{
			try
			{
				_listener.Start();
				var prefix = _listener.Prefixes.Count > 0 ? _listener.Prefixes.FirstOrDefault() : "http://127.0.0.1:43219/";
				Console.WriteLine($"URL 接收服务已启动: {prefix}");
				
				while (!token.IsCancellationRequested)
				{
					var ctx = await _listener.GetContextAsync();
					_ = Task.Run(async () => await HandleRequestAsync(ctx), token);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"URL 接收服务错误: {ex.Message}");
			}
			finally
			{
				Stop();
			}
		}

		private async Task HandleRequestAsync(HttpListenerContext ctx)
		{
			try
			{
				if (ctx.Request.HttpMethod == "POST" && ctx.Request.Url!.AbsolutePath == "/tab")
				{
					using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
					var body = await reader.ReadToEndAsync();
					
					// 解析 JSON 请求体
					var urlData = JsonSerializer.Deserialize<UrlData>(body);
					if (urlData?.Url != null)
					{
						var domain = ExtractDomain(urlData.Url);
						var args = new UrlReceivedEventArgs
						{
							Url = urlData.Url,
							Domain = domain
						};
						
						// 触发事件
						UrlReceived?.Invoke(this, args);
						
						// 返回成功响应
						ctx.Response.StatusCode = 200;
						ctx.Response.ContentType = "application/json";
						var response = JsonSerializer.Serialize(new { success = true, domain = domain });
						await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(response));
					}
					else
					{
						ctx.Response.StatusCode = 400;
						await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid URL data"));
					}
				}
				else
				{
					ctx.Response.StatusCode = 404;
					await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Not found"));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"处理请求时出错: {ex.Message}");
				ctx.Response.StatusCode = 500;
				await ctx.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
			}
			finally
			{
				ctx.Response.Close();
			}
		}

		private string ExtractDomain(string url)
		{
			try
			{
				if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
				{
					return uri.Host.ToLower();
				}
			}
			catch
			{
				// 忽略解析错误
			}
			return url;
		}

		public void Stop()
		{
			if (_listener.IsListening)
			{
				_listener.Stop();
				_cts.Cancel();
			}
		}

		public void Dispose()
		{
			Stop();
			_listener.Close();
			_cts.Dispose();
		}

		private class UrlData
		{
			public string? Url { get; set; }
		}
	}
}
