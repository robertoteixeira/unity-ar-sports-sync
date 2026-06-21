# iOS Build Notes

Target test device: iPhone XR.

## Unity Version

Recommended:

- Unity 6
- AR Foundation 6
- Apple ARKit XR Plugin 6
- XR Plug-in Management

## Player Settings

Open:

`File > Build Profiles > iOS > Player Settings`

Check:

- Bundle Identifier: use your own reverse-DNS identifier, for example `com.yourname.unityarsportssync`
- Camera Usage Description: `Camera access is required for AR training visualization.`
- Target minimum iOS version: keep Unity's ARKit-supported default unless the project requires otherwise.

## XR Plug-in Management

Open:

`Edit > Project Settings > XR Plug-in Management`

Then:

1. Select the iOS tab.
2. Enable ARKit.

## Build Flow

1. Connect the iPhone XR.
2. Select iOS Build Profile.
3. Build the Xcode project.
4. Open the generated Xcode project.
5. Select your Team under Signing & Capabilities.
6. Build and run on device.

## Local Server

The iPhone must reach the Mac over Wi-Fi.

Get the Mac IP:

```bash
ipconfig getifaddr en0
```

Use that IP in Unity:

```text
ws://YOUR_MAC_IP:8080
```

Example:

```text
ws://192.168.1.42:8080
```

## Common Issues

### WebSocket does not connect

Check:

- Mac and iPhone are on the same Wi-Fi.
- Server is running with `npm run dev`.
- Firewall allows incoming connections to Node.
- Unity URL uses the Mac LAN IP, not `localhost`.

### AR camera opens but no placement works

Check:

- `AR Raycast Manager` is on the XR Origin.
- `AR Plane Manager` is enabled.
- Device is moving enough to detect surfaces.
- Target prefab is assigned in `ARTapToPlace`.

### Build fails on signing

Check:

- Xcode Team is selected.
- Bundle Identifier is unique.
- iPhone is trusted by the Mac.