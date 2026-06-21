import { WebSocketServer } from "ws"

const port = Number(process.env.PORT ?? 8080);
const hz = Number(process.env.HZ ?? 30);
const intervalMs = Math.max(1, Math.round(1000 / hz));

const wss = new WebSocketServer({ port });

let seq = 0;
const startedAt = performance.now();

function nowMs() {
    return performance.now();
}

function createPose() {
    const t = (nowMs() - startedAt) / 1000;
    const radius = 0.55;

    const x = Math.cos(t * 1.3) * radius;
    const z = 1.1 + Math.sin(t * 1.3) * radius;
    const y = 0.05 + Math.abs(Math.sin(t * 3.0)) * 0.18;
    const yaw = (t * 90) % 360;

    return {
        type: "pose",
        id: "ball_1",
        seq: ++seq,
        serverTimeMs: nowMs(),
        position: { x, y, z },
        rotation: { x: 0, y: yaw, z: 0}
    };
}

function broadcast(message) {
    const payload = JSON.stringify(message);
    
    for (const client of wss.clients) {
        if (client.readyState == client.OPEN) {
            client.send(payload);
        }
    }
}

wss.on("connection", (socket, request) => {
    const address = request.socket.remoteAddress;
    console.log(`client connected: ${address}`);

    socket.send(JSON.stringify({
        type: "hello",
        serverTimeMs: nowMs(),
        message: "Unity AR Sports Sync server connected"
    }));

    socket.on("message", (raw) => {
        try {
            const message = JSON.parse(raw.toString());

            if (message === "ping") {
                socket.send(JSON.stringify({
                    type: "pong",
                    clientTimeMs: message.clientTimeMs,
                    serverTimeMs: nowMs()
                }));
            }
        } catch {
            socket.send(JSON.stringify({
                type: "error",
                message: "Invalid JSON message"
            }));
        }
    });

    socket.on("close", () => {
        console.log(`client disconnected: ${address}`);
    });
});

setInterval(() => {
    broadcast(createPose());
}, intervalMs);

console.log(`Unity AR Sports Sync server listening on ws://0.0.0.0:${port}`);
console.log(`broadcast post snapshots at ${hz} HZ`);