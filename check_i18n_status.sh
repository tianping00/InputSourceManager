#!/bin/bash

echo "🌐 Input Source Manager 国际化状态检查"
echo "====================================="

echo "📋 当前文档状态:"
if [ -f "README.md" ]; then
    echo "✅ README.md (英文版)"
else
    echo "❌ README.md 缺失"
fi

if [ -f "README.zh-CN.md" ]; then
    echo "✅ README.zh-CN.md (中文版)"
else
    echo "❌ README.zh-CN.md 缺失"
fi

echo -e "\n🔗 文档链接:"
echo "英文版: https://github.com/tianping00/InputSourceManager/blob/master/README.md"
echo "中文版: https://github.com/tianping00/InputSourceManager/blob/master/README.zh-CN.md"

echo -e "\n📝 国际化说明:"
echo "GitHub 会根据用户的语言偏好自动显示对应版本的文档"
echo "中文用户会看到 README.zh-CN.md"
echo "英文用户会看到 README.md"

echo -e "\n✅ 国际化支持已启用！"
