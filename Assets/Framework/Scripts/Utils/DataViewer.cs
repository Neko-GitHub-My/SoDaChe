using System;

public class DataViewer {
    public static void WriteUShortLE(byte[] buf, int offset, ushort value) { 
        // value ---> byte[];
        byte[] byte_value = BitConverter.GetBytes(value);
        // 小尾，还是大尾？BitConvert 系统是小尾还是大尾;
        if (!BitConverter.IsLittleEndian) {
            Array.Reverse(byte_value);
        }

        Array.Copy(byte_value, 0, buf, offset, byte_value.Length);
    }

    public static void WriteUIntLE(byte[] buf, int offset, uint value) {
        // value ---> byte[];
        byte[] byte_value = BitConverter.GetBytes(value);
        // 小尾，还是大尾？BitConvert 系统是小尾还是大尾;
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(byte_value);
        }

        Array.Copy(byte_value, 0, buf, offset, byte_value.Length);
    }

    public static void WriteBytes(byte[] dst, int offset, byte[] value) {
        Array.Copy(value, 0, dst, offset, value.Length);
    }

    public static ushort ReadUShortLE(byte[] data, int offset) {
        int ret = (data[offset] | (data[offset + 1] << 8));

        return (ushort)ret;
    }

    public static short ReadShortLE(byte[] data, int offset)
    {
        short value = (short)((data[offset + 1] << 8) | (data[offset + 0]));
        return value;
    }

    public static void WriteShortLE(byte[] data, int offset, short value)
    {
        data[offset + 0] = (byte)((value & 0x00ff));
        data[offset + 1] = (byte)((value & 0xff00) >> 8);
    }

    public static void WriteULongLE(byte[] data, int offset, ulong value)
    {
        data[offset + 0] = (byte)((value & 0x00000000000000ff));
        data[offset + 1] = (byte)((value & 0x000000000000ff00) >> 8);

        data[offset + 2] = (byte)((value & 0x0000000000ff0000) >> 16);
        data[offset + 3] = (byte)((value & 0x00000000ff000000) >> 24);

        data[offset + 4] = (byte)((value & 0x000000ff00000000) >> 32);
        data[offset + 5] = (byte)((value & 0x0000ff0000000000) >> 40);

        data[offset + 6] = (byte)((value & 0x00ff000000000000) >> 48);
        data[offset + 7] = (byte)((value & 0xff00000000000000) >> 56);
    }
}
