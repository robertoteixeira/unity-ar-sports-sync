# Unity Scene Setup

This guide wires the first AR demo scene for iPhone XR.

## Required Unity Packages

Install these packages in Unity 6:

- AR Foundation
- Apple ARKit XR Plugin
- XR Plug-in Management

Then enable ARKit:

1. Open `Edit > Project Settings`.
2. Go to `XR Plug-in Management`.
3. Select the iOS tab.
4. Enable `ARKit`.

## Scene Objects

Create or verify these objects in the scene:

```text
AR Session
XR Origin
DemoRoot
Canvas
```

Depending on the Unity template, `XR Origin` may already include the AR Camera.

## XR Origin Components

On `XR Origin`, add:

- `AR Plane Manager`
- `AR Raycast Manager`

Optional but useful:

- `AR Point Cloud Manager`

## Plane Visualization

For a quick first test, use Unity's default AR plane prefab if available.

Later we can replace it with a subtle transparent grid.

## Target Prefab

Create a simple target prefab:

1. Create a small sphere.
2. Name it `RealtimeBallTarget`.
3. Scale it to around `0.12, 0.12, 0.12`.
4. Add `RealtimePoseTarget`.
5. Save it as a prefab.

Suggested material:

- bright green or orange
- unlit if you want strong visibility outdoors

## DemoRoot

Create an empty object named `DemoRoot`.

Add:

- `ARTapToPlace`
- `RealtimePoseClient`

Configure `ARTapToPlace`:

- `Raycast Manager`: drag `XR Origin`
- `Target Prefab`: drag `RealtimeBallTarget`
- `Content Root`: optional

Configure `RealtimePoseClient`:

- `Server Url`: `ws://YOUR_MAC_IP:8080`
- `Target`: after first pass, drag the placed target manually if it exists in scene

For a later commit, we can automatically connect `ARTapToPlace.SpawnedPoseTarget` to `RealtimePoseClient`.

## Debug HUD

Create a Canvas:

1. Add UI Text.
2. Anchor it top-left.
3. Add `RealtimeDebugHud`.
4. Drag `DemoRoot` into the `client` field.
5. Drag the target object into the `target` field when available.

## Local Network Notes

The iPhone cannot connect to `localhost` on your Mac.

Use your Mac's LAN IP:

```bash
ipconfig getifaddr en0
```

Example:

```text
ws://192.168.1.42:8080
```

The Mac and iPhone must be on the same Wi-Fi network.

## Server

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