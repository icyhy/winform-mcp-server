class ChatApp {
    constructor() {
        this.messageInput = document.getElementById('messageInput');
        this.sendButton = document.getElementById('sendButton');
        this.chatMessages = document.getElementById('chatMessages');
        this.loadingOverlay = document.getElementById('loadingOverlay');
        this.statusDot = document.getElementById('statusDot');
        this.statusText = document.getElementById('statusText');
        this.charCount = document.getElementById('charCount');
        
        this.isLoading = false;
        this.apiBaseUrl = window.location.origin;
        
        this.initializeEventListeners();
        this.checkServerStatus();
    }
    
    initializeEventListeners() {
        // 发送按钮点击事件
        this.sendButton.addEventListener('click', () => this.sendMessage());
        
        // 输入框事件
        this.messageInput.addEventListener('input', () => this.handleInputChange());
        this.messageInput.addEventListener('keydown', (e) => this.handleKeyDown(e));
        
        // 自动调整输入框高度
        this.messageInput.addEventListener('input', () => this.autoResizeTextarea());
    }
    
    handleInputChange() {
        const text = this.messageInput.value;
        const length = text.length;
        
        // 更新字符计数
        this.charCount.textContent = `${length}/2000`;
        
        // 更新发送按钮状态
        this.sendButton.disabled = length === 0 || this.isLoading;
        
        // 字符数超限提示
        if (length > 1900) {
            this.charCount.style.color = length > 2000 ? '#ef4444' : '#f59e0b';
        } else {
            this.charCount.style.color = '#64748b';
        }
    }
    
    handleKeyDown(e) {
        if (e.key === 'Enter') {
            if (e.ctrlKey || e.metaKey) {
                e.preventDefault();
                this.sendMessage();
            } else if (!e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        }
    }
    
    autoResizeTextarea() {
        this.messageInput.style.height = 'auto';
        const scrollHeight = this.messageInput.scrollHeight;
        const maxHeight = 120;
        this.messageInput.style.height = Math.min(scrollHeight, maxHeight) + 'px';
    }
    
    async checkServerStatus() {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/health`);
            if (response.ok) {
                this.updateStatus('connected', '已连接');
            } else {
                this.updateStatus('error', '服务器错误');
            }
        } catch (error) {
            this.updateStatus('error', '连接失败');
            console.error('Server status check failed:', error);
        }
    }
    
    updateStatus(status, text) {
        this.statusDot.className = `status-dot ${status}`;
        this.statusText.textContent = text;
    }
    
    async sendMessage() {
        const message = this.messageInput.value.trim();
        if (!message || this.isLoading) return;
        
        // 添加用户消息到界面
        this.addMessage('user', message);
        
        // 清空输入框
        this.messageInput.value = '';
        this.handleInputChange();
        this.autoResizeTextarea();
        
        // 显示加载状态
        this.setLoading(true);
        
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chat`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    message: message,
                    timestamp: new Date().toISOString()
                })
            });
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const data = await response.json();
            
            // 添加助手回复
            this.addMessage('assistant', data.response);
            
            // 如果有工具调用，显示工具调用信息
            if (data.toolCalls && data.toolCalls.length > 0) {
                data.toolCalls.forEach(toolCall => {
                    this.addToolCall(toolCall);
                });
            }
            
        } catch (error) {
            console.error('Send message failed:', error);
            this.addMessage('system', `发送消息失败: ${error.message}`);
            this.updateStatus('error', '发送失败');
        } finally {
            this.setLoading(false);
        }
    }
    
    addMessage(type, content, timestamp = null) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${type}-message`;
        
        const contentDiv = document.createElement('div');
        contentDiv.className = 'message-content';
        
        // 处理内容格式化
        if (typeof content === 'string') {
            // 简单的 Markdown 支持
            content = content
                .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
                .replace(/\*(.*?)\*/g, '<em>$1</em>')
                .replace(/`(.*?)`/g, '<code>$1</code>')
                .replace(/\n/g, '<br>');
        }
        
        contentDiv.innerHTML = content;
        
        const timeDiv = document.createElement('div');
        timeDiv.className = 'message-time';
        timeDiv.textContent = timestamp || this.formatTime(new Date());
        
        messageDiv.appendChild(contentDiv);
        messageDiv.appendChild(timeDiv);
        
        this.chatMessages.appendChild(messageDiv);
        this.scrollToBottom();
    }
    
    addToolCall(toolCall) {
        const toolDiv = document.createElement('div');
        toolDiv.className = 'tool-call';
        toolDiv.innerHTML = `
            <strong>🔧 工具调用:</strong> ${toolCall.name}<br>
            <strong>参数:</strong> ${JSON.stringify(toolCall.arguments, null, 2)}
        `;
        
        const lastMessage = this.chatMessages.lastElementChild;
        if (lastMessage && lastMessage.classList.contains('assistant-message')) {
            lastMessage.querySelector('.message-content').appendChild(toolDiv);
        }
        
        // 如果有工具结果，也显示出来
        if (toolCall.result) {
            const resultDiv = document.createElement('div');
            resultDiv.className = 'tool-result';
            resultDiv.innerHTML = `
                <strong>✅ 工具结果:</strong><br>
                ${JSON.stringify(toolCall.result, null, 2)}
            `;
            lastMessage.querySelector('.message-content').appendChild(resultDiv);
        }
        
        this.scrollToBottom();
    }
    
    setLoading(loading) {
        this.isLoading = loading;
        this.sendButton.disabled = loading || this.messageInput.value.trim() === '';
        
        if (loading) {
            this.loadingOverlay.classList.add('show');
            this.updateStatus('connecting', 'AI 思考中...');
        } else {
            this.loadingOverlay.classList.remove('show');
            this.updateStatus('connected', '已连接');
        }
    }
    
    scrollToBottom() {
        setTimeout(() => {
            this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
        }, 100);
    }
    
    formatTime(date) {
        return date.toLocaleTimeString('zh-CN', {
            hour: '2-digit',
            minute: '2-digit'
        });
    }
}

// 页面加载完成后初始化聊天应用
document.addEventListener('DOMContentLoaded', () => {
    new ChatApp();
});