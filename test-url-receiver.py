#!/usr/bin/env python3
"""
测试 URL 接收服务的脚本
模拟浏览器扩展向本地服务发送当前标签页 URL
"""

import requests
import json
import time
import sys

def test_url_receiver(url, domain):
    """测试发送 URL 到本地接收服务"""
    try:
        data = {
            "url": url
        }
        
        response = requests.post(
            "http://127.0.0.1:43219/tab",
            json=data,
            headers={"Content-Type": "application/json"},
            timeout=5
        )
        
        if response.status_code == 200:
            result = response.json()
            print(f"✅ 成功发送 URL: {domain}")
            print(f"   响应: {result}")
            return True
        else:
            print(f"❌ 发送失败: HTTP {response.status_code}")
            print(f"   响应: {response.text}")
            return False
            
    except requests.exceptions.ConnectionError:
        print("❌ 连接失败: 请确保 Input Source Manager 正在运行")
        return False
    except Exception as e:
        print(f"❌ 发送出错: {e}")
        return False

def main():
    print("=== Input Source Manager URL 接收测试 ===\n")
    
    # 测试用例
    test_cases = [
        ("https://zhihu.com/question/123", "zhihu.com"),
        ("https://stackoverflow.com/questions/456", "stackoverflow.com"),
        ("https://github.com/runjuu/InputSourcePro", "github.com"),
        ("https://www.baidu.com", "baidu.com"),
        ("https://www.google.com", "google.com")
    ]
    
    print("开始测试...\n")
    
    for i, (url, domain) in enumerate(test_cases, 1):
        print(f"测试 {i}: {domain}")
        success = test_url_receiver(url, domain)
        
        if success:
            print(f"   等待 2 秒...")
            time.sleep(2)
        
        print()
    
    print("测试完成！")
    print("\n说明:")
    print("- 如果看到 '连接失败'，请先运行 Input Source Manager")
    print("- 成功发送后，程序会自动根据网站规则切换输入法")
    print("- 可以在主窗口的 '规则配置' 页面查看和管理规则")

if __name__ == "__main__":
    main()


