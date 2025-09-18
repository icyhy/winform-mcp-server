const { chromium } = require('playwright');

async function previewChatPage() {
    console.log('启动浏览器预览聊天页面...');
    
    const browser = await chromium.launch({ 
        headless: false,
        slowMo: 1000 // 慢速模式，便于观察
    });
    
    const context = await browser.newContext({
        viewport: { width: 1200, height: 800 }
    });
    
    const page = await context.newPage();
    
    try {
        console.log('访问聊天页面: http://localhost:3000/index');
        await page.goto('http://localhost:3000/index', { 
            waitUntil: 'networkidle',
            timeout: 10000 
        });
        
        console.log('页面加载成功！');
        
        // 等待页面元素加载
        await page.waitForSelector('.chat-container', { timeout: 5000 });
        console.log('聊天容器已加载');
        
        // 模拟发送一条消息来测试样式
        const messageInput = await page.locator('#messageInput');
        await messageInput.fill('这是一条测试消息，用来预览新的聊天界面样式');
        
        const sendButton = await page.locator('.send-button');
        await sendButton.click();
        
        console.log('已发送测试消息');
        
        // 等待一段时间让用户观察界面
        console.log('请观察新的聊天界面样式...');
        console.log('- 配色方案已从紫色改为蓝灰色调');
        console.log('- 头部样式更加简洁现代');
        console.log('- 消息气泡采用现代设计');
        console.log('- 输入区域更加专业');
        
        // 测试响应式设计
        console.log('测试响应式设计...');
        await page.setViewportSize({ width: 600, height: 800 });
        await page.waitForTimeout(2000);
        
        await page.setViewportSize({ width: 1200, height: 800 });
        await page.waitForTimeout(2000);
        
        console.log('响应式设计测试完成');
        
        // 保持浏览器打开30秒供用户观察
        console.log('浏览器将保持打开30秒供您观察...');
        await page.waitForTimeout(30000);
        
    } catch (error) {
        console.error('预览过程中出现错误:', error.message);
        
        if (error.message.includes('net::ERR_CONNECTION_REFUSED')) {
            console.log('提示: 请确保WinForm应用程序已启动并且服务器正在运行在端口3000');
        }
    } finally {
        await browser.close();
        console.log('预览完成');
    }
}

// 运行预览
previewChatPage().catch(console.error);