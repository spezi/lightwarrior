const WebSocket = require('ws');
const osc = require('osc');

const wss = new WebSocket.Server({ port: 9998 });
const udpPort = new osc.UDPPort({
    localAddress: "0.0.0.0",
    localPort: 57121,
    remoteAddress: "127.0.0.1",
    remotePort: 9997// ossia score port - adjust if needed
});

udpPort.open();

wss.on('connection', (ws) => {
    console.log('✓ Client connected from browser');
    
    ws.on('message', (data) => {
        try {
            const msg = JSON.parse(data);
            udpPort.send({
                address: msg.address,
                args: msg.args.map(v => ({ type: 'f', value: v }))
            });
            console.log('Forwarded:', msg.address, msg.args);
        } catch (err) {
            console.error('Error:', err);
        }
    });
});

console.log('🚀 Bridge running on ws://localhost:9998');
console.log('📡 Forwarding OSC to 127.0.0.1:9997');

//{"address": "/corner/tl", "args": [400, 300]}