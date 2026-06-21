# Realtime Pose Server

Node.js WebSocket server that simulates realtime pose updates for the Unity AR client.

## Responsibilities

- accept WebSocket clients
- broadcast pose snapshots at a fixed frequency
- increment sequence numbers
- respond to ping/pong RTT checks
- provide deterministic generated motion for local testing

## Install

From this folder:

```bash
npm install
```

## Run

```bash
npm run dev
```

Expected output:

```text
Unity AR Sports Sync server listening on ws://0.0.0.0:8080
broadcasting pose snapshots at 30 Hz
```

## Configuration

Use environment variables:

```bash
PORT=8080 HZ=30 npm run dev
```

Options:

- `PORT`: WebSocket port. Default: `8080`.
- `HZ`: pose broadcast frequency. Default: `30`.

## Message: pose

The server broadcasts:

```json
{
  "type": "pose",
  "id": "ball_1",
  "seq": 1,
  "serverTimeMs": 123.4,
  "position": { "x": 0.2, "y": 0.1, "z": 1.3 },
  "rotation": { "x": 0, "y": 90, "z": 0 }
}
```

Fields:

- `type`: message type, always `pose`
- `id`: tracked object identifier
- `seq`: monotonically increasing sequence number
- `serverTimeMs`: server runtime timestamp
- `position`: local AR-space target position
- `rotation`: Euler rotation in degrees

## Message: ping

Client sends:

```json
{
  "type": "ping",
  "clientTimeMs": 12345.6
}
```

Server replies:

```json
{
  "type": "pong",
  "clientTimeMs": 12345.6,
  "serverTimeMs": 67890.1
}
```

The client uses this to estimate round-trip time.

## Local Network

When testing on iPhone, use the Mac's LAN IP in Unity:

```text
ws://YOUR_MAC_IP:8080
```

Do not use `localhost` on the phone.