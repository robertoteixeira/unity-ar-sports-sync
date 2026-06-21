# Unity AR Sports Sync

Unity 6 AR Foundation prototype with WebSocket pose streaming, sequence handling, RTT metrics, and quaternion-based smoothing.

## Overview

This project is a compact Unity-first AR demo for practicing mobile AR architecture and realtime object synchronization.

It simulates a sports training scenario where a realtime server streams pose updates for an AR target. The Unity client receives those updates, rejects stale snapshots, and renders smooth movement using vector and quaternion interpolation.

## Goals

- Build a small AR Foundation app for iPhone-first testing.
- Stream realtime pose snapshots from a local server.
- Keep networking separate from Unity transform smoothing.
- Show sequence handling and stale message rejection.
- Measure WebSocket round-trip time with ping/pong.
- Practice the same patterns used in multiplayer AR training apps.

## Tech Stack

- Unity 6
- AR Foundation
- Apple ARKit XR Plugin
- C#
- Node.js
- WebSocket

## Repository Structure

```text
server/   Node.js WebSocket pose simulator
unity/    Unity 6 AR Foundation project
docs/     Setup notes and architecture docs
```

## Realtime Flow

```text
Node WebSocket server
  -> JSON pose snapshots
    -> RealtimePoseClient
      -> PoseSnapshot
        -> RealtimePoseTarget
          -> smoothed AR object transform
```

The Unity client does not directly move objects from the network receive callback. It queues incoming messages and applies them on Unity's main thread.

## Message Example

```json
{
  "type": "pose",
  "id": "ball_1",
  "seq": 42,
  "serverTimeMs": 123456.7,
  "position": { "x": 0.1, "y": 0.0, "z": 1.4 },
  "rotation": { "x": 0.0, "y": 35.0, "z": 0.0 }
}
```

## Engineering Focus

This prototype focuses on the runtime architecture behind realtime mobile AR experiences:

- authoritative pose updates from a server
- sequence-based stale message rejection
- main-thread handoff for Unity scene changes
- smoothing between network snapshots
- quaternion-based rotation interpolation
- simple RTT measurement with ping/pong
- local AR placement with AR Foundation raycasts

## Local Server

From the repo root:

```bash
cd server
npm install
npm run dev
```

Expected output:

```text
Unity AR Sports Sync server listening on ws://0.0.0.0:8080
broadcasting pose snapshots at 30 Hz
```

## iPhone Testing

The iPhone must connect to the Mac using the Mac's local network IP.

Get the Mac IP:

```bash
ipconfig getifaddr en0
```

Then configure Unity with:

```text
ws://YOUR_MAC_IP:8080
```

Do not use `localhost` from the iPhone.

## Docs

- [Unity Scene Setup](docs/unity-scene-setup.md)
- [iOS Build Notes](docs/ios-build-notes.md)