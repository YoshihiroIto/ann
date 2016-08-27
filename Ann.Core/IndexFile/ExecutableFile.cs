// automatically generated, do not modify

namespace IndexFile
{

using System;
using FlatBuffers;

public sealed class ExecutableFile : Table {
  public static ExecutableFile GetRootAsExecutableFile(ByteBuffer _bb) { return GetRootAsExecutableFile(_bb, new ExecutableFile()); }
  public static ExecutableFile GetRootAsExecutableFile(ByteBuffer _bb, ExecutableFile obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ExecutableFile __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Path { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPathBytes() { return __vector_as_arraysegment(4); }

  public static Offset<ExecutableFile> CreateExecutableFile(FlatBufferBuilder builder,
      StringOffset PathOffset = default(StringOffset)) {
    builder.StartObject(1);
    ExecutableFile.AddPath(builder, PathOffset);
    return ExecutableFile.EndExecutableFile(builder);
  }

  public static void StartExecutableFile(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset PathOffset) { builder.AddOffset(0, PathOffset.Value, 0); }
  public static Offset<ExecutableFile> EndExecutableFile(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExecutableFile>(o);
  }
};


}
