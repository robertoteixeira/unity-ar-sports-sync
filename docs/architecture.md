# Architecture

## Runtime Model

```text
Server
  -> broadcasts pose snapshots over WebSocket
    -> Unity receives JSON messages
      -> Unity queues messages from network thread
        -> Unity parses snapshots on main thread
          -> RealtimePoseTarget smooths object transform
```

## Components

### Node WebSocket Server

The server is an authoritative pose source for the demo.

It broadcasts one `pose` message at a fixed rate. The first version uses generated motion instead of live tracking so the Unity client can be tested without external hardware.

Responsibilities:

- accept WebSocket clients
- broadcast pose snapshots
- increment sequence numbers
- respond to ping messages with pong
- expose configurable update frequency through `HZ`

### RealtimePoseClient

`RealtimePoseClient` owns network transport.

Responsibilities:

- connect to the WebSocket server
- receive messages on a background task
- queue raw JSON into a thread-safe queue
- process queued messages on Unity's main thread
- parse pose snapshots
- measure RTT with ping/pong
- forward accepted snapshots to `RealtimePoseTarget`

It does not directly move scene objects.

### PoseSnapshot

`PoseSnapshot` is the network data contract.

It converts JSON-friendly DTOs into Unity runtime types:

- `{ x, y, z }` to `UnityEngine.Vector3`
- Euler degrees to `Quaternion`

### RealtimePoseTarget

`RealtimePoseTarget` owns visual smoothing.

Responsibilities:

- reject stale snapshots using `seq`
- store the latest target position and rotation
- interpolate position with `Vector3.Lerp`
- interpolate rotation with `Quaternion.Slerp`
- snap to the target if the error is too large

### ARTapToPlace

`ARTapToPlace` owns local AR placement.

Responsibilities:

- read touch input
- raycast against detected AR planes
- instantiate the target prefab
- bind the placed target to `RealtimePoseClient`

## Threading Model

Network receive happens outside Unity's main thread.

Unity scene changes happen on the main thread.

```text
ReceiveLoop task
  -> ConcurrentQueue<string>
    -> Unity Update()
      -> parse and apply snapshot
```

This avoids blocking AR tracking or rendering with network I/O.

## Snapshot Rules

Each pose has a sequence number.

```json
{
  "seq": 42
}
```

The client keeps the last applied sequence number.

If a snapshot arrives with a sequence number less than or equal to the last applied value, it is rejected as stale.

## Smoothing Rules

The smoothing target applies the following policy:

```text
if distance to target > snap threshold:
    snap to target
else:
    interpolate toward target
```

Position uses framerate-independent `Lerp`:

```csharp
float t = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
```

Rotation uses quaternion interpolation:

```csharp
Quaternion.Slerp(currentRotation, targetRotation, t);
```

## AR Coordinate Space

The first version uses local AR content space.

The placed target acts as a local reference. Server positions are applied relative to that target's parent/root, which keeps the demo simple and makes the streamed motion appear around the placed AR object.

Later versions can introduce shared anchors or cloud anchors for multi-device alignment.