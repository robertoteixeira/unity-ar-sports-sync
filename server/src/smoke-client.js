import WebSocket from "ws";

const url = process.env.URL ?? "ws://localhost:8080";
const maxMessages = Number(process.env.MAX_MESSAGES ?? 5);
const timeoutMs = Number(process.env.TIMEOUT_MS ?? 5000);

const socket = new WebSocket(url);

let receivedPoses = 0;
let receivedPong = false;

const timeout = setTimeout(() => {
  console.log(`timeout reached; received poses: ${receivedPoses}; received pong: ${receivedPong}`);
  socket.close();
}, timeoutMs);

function maybeClose() {
  if (receivedPong && receivedPoses >= maxMessages) {
    clearTimeout(timeout);
    socket.close();
  }
}

function sendPing() {
  socket.send(JSON.stringify({
    type: "ping",
    clientTimeMs: performance.now()
  }));
}

socket.on("open", () => {
  console.log(`connected to ${url}`);
});

socket.on("message", (raw) => {
  const message = JSON.parse(raw.toString());

  console.log(message);

  if (message.type === "hello") {
    sendPing();
  }

  if (message.type === "pong") {
    receivedPong = true;
  }

  if (message.type === "pose") {
    receivedPoses += 1;
  }

  maybeClose();
});

socket.on("close", () => {
  clearTimeout(timeout);
  console.log("closed");
});

socket.on("error", (error) => {
  clearTimeout(timeout);
  console.error(error);
  process.exitCode = 1;
});