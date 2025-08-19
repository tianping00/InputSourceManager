# URL Receiver for Input Source Manager

## Overview

The URL Receiver is a local HTTP service component of Input Source Manager that receives URL information from browser extensions to enable automatic input method switching based on website content.

## 🚀 Features

### Real-time URL Monitoring
- **Local HTTP Service**: Runs on localhost to receive browser data
- **Secure Communication**: Only accepts connections from localhost
- **Automatic Processing**: Processes received URLs and triggers input method switching

### Browser Extension Integration
- **Chrome Extension Support**: Compatible with Chrome/Edge extensions
- **Firefox Extension Support**: Compatible with Firefox WebExtensions
- **Cross-browser Compatibility**: Works with all major browsers

### Smart URL Processing
- **Pattern Matching**: Supports wildcard patterns like `*.github.com`
- **Priority Handling**: Processes URLs based on rule priority
- **Error Handling**: Graceful handling of malformed URLs

## 🔧 Technical Details

### Service Configuration
- **Port**: Default 8080 (configurable)
- **Protocol**: HTTP
- **Security**: Localhost only, no external access
- **Timeout**: 5 seconds for request processing

### API Endpoints

#### POST /url
Receives URL information from browser extensions.

**Request Body:**
```json
{
  "url": "https://github.com/tianping00/InputSourceManager",
  "title": "Input Source Manager - GitHub",
  "timestamp": "2025-01-18T10:30:00Z"
}
```

**Response:**
```json
{
  "status": "success",
  "message": "URL processed successfully",
  "inputMethod": "en-US"
}
```

### Error Handling
- **Invalid URL**: Returns 400 Bad Request
- **Server Error**: Returns 500 Internal Server Error
- **Timeout**: Returns 408 Request Timeout

## 🛠️ Setup and Configuration

### 1. Enable URL Receiver
In your configuration file (`config.json`):
```json
{
  "Settings": {
    "EnableUrlReceiver": true,
    "UrlReceiverPort": 8080,
    "UrlReceiverTimeout": 5000
  }
}
```

### 2. Browser Extension Setup
Install the Input Source Manager browser extension and configure it to send URLs to:
```
http://localhost:8080/url
```

### 3. Firewall Configuration
Ensure that port 8080 (or your configured port) is allowed for localhost connections.

## 📱 Browser Extension Development

### Chrome Extension Example
```javascript
// content.js
function sendUrlToManager() {
  const url = window.location.href;
  const title = document.title;
  
  fetch('http://localhost:8080/url', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      url: url,
      title: title,
      timestamp: new Date().toISOString()
    })
  })
  .then(response => response.json())
  .then(data => console.log('Success:', data))
  .catch(error => console.error('Error:', error));
}

// Send URL when page loads
document.addEventListener('DOMContentLoaded', sendUrlToManager);
```

### Firefox Extension Example
```javascript
// content.js
function sendUrlToManager() {
  const url = window.location.href;
  const title = document.title;
  
  fetch('http://localhost:8080/url', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      url: url,
      title: title,
      timestamp: new Date().toISOString()
    })
  })
  .then(response => response.json())
  .then(data => console.log('Success:', data))
  .catch(error => console.error('Error:', error));
}

// Send URL when page loads
document.addEventListener('DOMContentLoaded', sendUrlToManager);
```

## 🔍 Troubleshooting

### Common Issues

#### Service Not Starting
- Check if port 8080 is already in use
- Verify firewall settings
- Check configuration file syntax

#### Extension Connection Failed
- Ensure Input Source Manager is running
- Check if URL Receiver is enabled
- Verify port configuration matches

#### Input Method Not Switching
- Check rule configuration
- Verify URL pattern matching
- Check system input method availability

### Debug Mode
Enable debug logging in configuration:
```json
{
  "Settings": {
    "EnableDebugLogging": true,
    "LogLevel": "Debug"
  }
}
```

## 📊 Performance

### Response Times
- **URL Processing**: < 10ms
- **Rule Matching**: < 1ms
- **Input Method Switching**: < 100ms

### Resource Usage
- **Memory**: < 5MB
- **CPU**: < 1% (idle)
- **Network**: Minimal (localhost only)

## 🔒 Security Considerations

### Local Access Only
- Service only accepts connections from localhost
- No external network access
- No sensitive data transmission

### Data Privacy
- URLs are processed locally
- No data is sent to external servers
- Temporary storage only for processing

## 📚 Related Documentation

- **[中文版本](URL_RECEIVER_README.zh-CN.md)** - 中文版 URL 接收器说明
- **[English Version](URL_RECEIVER_README.en.md)** - English URL receiver documentation
- **[Project Status Report](PROJECT_STATUS_REPORT.en.md)** - Complete project documentation

---

**Note**: The URL Receiver is an optional component. Input Source Manager works without it for application and process-based switching.
