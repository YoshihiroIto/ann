// automatically generated, do not modify

namespace IndexFile
{

using System;
using FlatBuffers;

public sealed class ExecutableUnit : Table {
  public static ExecutableUnit GetRootAsExecutableUnit(ByteBuffer _bb) { return GetRootAsExecutableUnit(_bb, new ExecutableUnit()); }
  public static ExecutableUnit GetRootAsExecutableUnit(ByteBuffer _bb, ExecutableUnit obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ExecutableUnit __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Path { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPathBytes() { return __vector_as_arraysegment(4); }

  public static Offset<ExecutableUnit> CreateExecutableUnit(FlatBufferBuilder builder,
      StringOffset PathOffset = default(StringOffset)) {
    builder.StartObject(1);
    ExecutableUnit.AddPath(builder, PathOffset);
    return ExecutableUnit.EndExecutableUnit(builder);
  }

  public static void StartExecutableUnit(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset PathOffset) { builder.AddOffset(0, PathOffset.Value, 0); }
  public static Offset<ExecutableUnit> EndExecutableUnit(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExecutableUnit>(o);
  }
};


}
