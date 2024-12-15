# DVTAppSpecification
This repo contains descriptions for various formats, protocols and systems I've created for my VTuber setup.

---
The main setup consists of the following components:
- DVTTempApp (Demkeys VTuber Temp App. It's not temp anymore, but the 'temp' name is stuck.)
- Virtual Stream Deck
- Twitch Middleware App
---
# Protocols/Formats
These are the protocols used for communication between DVTTempApp and various other components. Some of these protocols are considered legacy and are not used anymore, but have been included anyway.

### RawMessage Protocol:
- Protocol to use when sending messages between DTVTempApp and various other apps. This is a byte array. The RawMessage contains the payload. The magic number mentions the kind of payload it is. Based on the magic number, the payload will be decoded accordingly.
- Format
    - Byte 0: Magic number
    - Byte 1-4: Payload Size (int32)
    - Byte 5-n: Payload Data
- Magic number:
    - 0: DVTMPMessage
    - 1: BodyPoseMessage
    - 2: DVTMPMessagePacked
    - 3: MDLChatMessage
    - 4: MDLChannelPointRedeemMessage
- All apps communicating with DVT App must implement the RawMessage protocol and the payload's message protocol.

### BodyPoseMessage Protocol:
- Protocol fo BodyPoseMessage. This is body pose landmark data that has been captured remotely via Mediapipe and Python, and sent over the network to DVT App. To avoid having to deal with JSON, we are manually packing and unpacking the data to and from a byte array.
- Format:
    -Byte 0: Landmark Count
    -Byte 1-n: Landmarks. Each Landmark is 20 bytes (5 floats).
- Note: Byte-order is Little Endian

### DVTMPMessage Protocol:
- Protocol to use when sending messages between DTVTempApp and various other apps. Currently, the remotes apps and the tracking app will be sending messages to the main app, but in the future, more apps could be added.
- A DVTMPMessage will usually be contained in the payload of a RawMessage and will be presented as JSON data.
- Below is the description of the protocol, mentioning all the fields, types of fields and their descriptions.
{
    MessageAppType (int),
    MessageBodyName (int),
    MessageBodyValueType (int),
    MessageBodyValue (byte[])
}

All apps using DVTMP should implement these enums:
- NOTE: When making protocols try to implement these as ints instead of strings. Since all involved apps are implementing these enums, knowing the exact name isn't a necessity. Just sending the right int should be enough. This is also more performance friendly.
- MessageAppType: RUCA, RVCA, RHTCA, BTA
- MessageBodyValueType: Bool, Int32, Single, Char, String, Vector3, Vector4

### DVTMPMessagePacked Protocol:
- Packed version of DVTMPMessage protocol, but all the data is packed into bytes instead of being serialized into JSON.
- Format:
  - byte 0: messageAppType (byte)
  - byte 1: messageBodyName (byte)
  - byte 2: messageBodyValueType (byte)
  - byte 3-6: messageBodyValueSize (int32)
  - byte 7-n: messageBodyValue (byte[])
- Based on messageBodyValueType, the messageBodyValue can be unpacked. Blob type can be used for special cases where multiple data needs to be bundled. The method receiving the blob should know how to unpack it.

NOTES:
- To decode a message byte array into a DVTMPMessageUnpacked object, call 'DVTMPMessageUnpacked.UnpackMessage()'.
- DVTMPMessageUnpacked has instance helper methods that can help you extract the value from messageBodyValue, eg. GetBool, GetInt32, GetSingle, etc. To extract a specific type of value from the message object, call the respective Get method on the object instance.
---
### Events:
- SwitchToFlyCam01: Make FlyCam01 the active cam
- SwitchToOrbitCam01: Make OrbitCam01 the active cam
- ChangeCamera
- ToggleFlyMode: Toggle fly mode on/off for FlyCam01
- RandomDVTMPMessage: Random message
- RecvBodyPoseMessage: Receive body pose landmark tracking message.
    - Message Type: BodyPoseMessage
- RandomDVTMPMessagePacked: Random message.
    - Message Type: DVTMPMessagePacked
- RecvMicAudioRMS: Receive mic audio RMS data
    - Message Type: DVTMPMessagePacked | Params: float
- ToggleGreenScreen: Toggle green screen on/off
- PrideHeartsReward
- OnChatMessageReceived
- OnChannelPointRewardRedeemed
- SetMicSysDeviceToMic
- SetMicSysDeviceToStereoMix
- SetMicRMSMul:
    - Message Type: DVTMPMessagePacked | Params: float
- SetSmoothDropMicRMSMul:
    - Message Type: DVTMPMessagePacked | Params: float
- SetAvCube01ScaleMul:
    - Message Type: DVTMPMessagePacked | Params: float
- SetMicNoiseGateThreshold:
    - Message Type: DVTMPMessagePacked | Params: float
- SetAvCube01ScaleMul
    - Message Type: DVTMPMessagePacked | Params: float

NOTES: 
- Each event mentions params and message type.
- If no params are mentioned for an event, it doesn't use params. 
- For the sake of uniform design, messages that don't require parameters will contain a MsgValue of int type. Ignore it.
- Each time the Events List is updated in DVTTempApp, update it in Virtual Stream Deck app as well.

Events List (for programming convenience):

SwitchToFlyCam01, SwitchToOrbitCam01, ChangeCamera, ToggleFlyMode, RandomDVTMPMessage, RecvBodyPoseMessage, RandomDVTMPMessagePacked, RecvMicAudioRMS, ToggleGreenScreen, PrideHeartsReward, OnChatMessageReceived, OnChannelPointRewardRedeemed, SetMicSysDeviceToMic, SetMicSysDeviceToStereoMix, SetMicRMSMul, SetSmoothDropMicRMSMul, SetMicNoiseGateThreshold, SetAvCube01ScaleMul
