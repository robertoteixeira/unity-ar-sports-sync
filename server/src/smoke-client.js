import WebSocket from "ws";

const url = process.env.URL ?? "ws://localhost:8080";
const maxMessages = Number(process.env.MAX_MESSAGES ?? 5);

const socket = new WebSocket(url);

let received = 0;

socket.on("open", () => {
  console.log(`connected to ${url}`);

  socket.send(JSON.stringify({
    type: "ping",
    clientTimeMs: performance.now()
  }));
});

socket.on("message", (raw) => {
  const message = JSON.parse(raw.toString());

  console.log(message);

  if (message.type === "pose") {
    received += 1;
  }

  if (received >= maxMessages) {
    socket.close();
  }
});

socket.on("close", () => {
  console.log("closed");
});

socket.on("error", (error) => {
  console.error(error);
  process.exitCode = 1;
});