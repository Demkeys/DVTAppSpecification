/*
Contains definitions for DVTMPMessage and DVTMPMessageUnpacked classes.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DVTApp
{
public enum MessageAppType { RUCA, RVCA, RHTCA, BTA }
public enum MessageBodyValueType { Bool, Int32, Single, Char, String, Vector3, Vector4, Blob }
public class DVTMPMessage
{
    public MessageAppType messageAppType;
    public MessagingSystem.MsgEventName messageBodyName;
    public MessageBodyValueType messageBodyValueType;
    public byte[] messageBodyValue;

}

public struct DVTMPMessageUpacked
{
    public MessageAppType messageAppType;
    public MessagingSystem.MsgEventName messageBodyName;
    public MessageBodyValueType messageBodyValueType;
    public int messageBodyValueSize;
    public byte[] messageBodyValue;

    public static bool UnpackMessage(
        byte[] raw_message_bytes, out DVTMPMessageUpacked message)
    {
        message = new();
        if(raw_message_bytes[0] != 2) return false;

        // Offset to determine where the message starts in the raw_message_bytes array
        int message_start_offset = 5;
        message.messageAppType = 
            (MessageAppType)raw_message_bytes[message_start_offset+0];
        message.messageBodyName = 
            (MessagingSystem.MsgEventName)raw_message_bytes[
                message_start_offset+1];
        message.messageBodyValueType =
            (MessageBodyValueType)raw_message_bytes[message_start_offset+2];
        message.messageBodyValueSize = BitConverter.ToInt32(
            raw_message_bytes, message_start_offset+3
        );
        message.messageBodyValue = new byte[message.messageBodyValueSize];
        Buffer.BlockCopy(
            raw_message_bytes, message_start_offset+7, 
            message.messageBodyValue, 0, message.messageBodyValueSize
        );

        return true;
    }


    public bool GetBool() => BitConverter.ToBoolean(messageBodyValue, 0);
    public int GetInt32() => BitConverter.ToInt32(messageBodyValue, 0);
    public float GetSingle() => BitConverter.ToSingle(messageBodyValue, 0);
    public char GetChar() => BitConverter.ToChar(messageBodyValue, 0);
    public string GetString() => ASCIIEncoding.ASCII.GetString(messageBodyValue);
    public Vector3 GetVector3() => 
        new Vector3(
            BitConverter.ToSingle(messageBodyValue, 0),
            BitConverter.ToSingle(messageBodyValue, 4),
            BitConverter.ToSingle(messageBodyValue, 8)
        );

    public Vector4 GetVector4() => 
        new Vector4(
            BitConverter.ToSingle(messageBodyValue, 0),
            BitConverter.ToSingle(messageBodyValue, 4),
            BitConverter.ToSingle(messageBodyValue, 8),
            BitConverter.ToSingle(messageBodyValue, 12)
        );

    // DEBUG ONLY!!!
    public string ToString()
    {
        return $"";
    }
}
}
