// automatically generated, do not modify

namespace IndexFile
{

using System;
using FlatBuffers;

public sealed class ExecutableUnit : Table {
  public static ExecutableUnit GetRootAsExecutableUnit(ByteBuffer _bb) { return GetRootAsExecutableUnit(_bb, new ExecutableUnit()); }
  public static ExecutableUnit GetRootAsExecutableUnit(ByteBuffer _bb, ExecutableUnit obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ExecutableUnit __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Path { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPathBytes() { return __vector_as_arraysegment(6); }

  public static Offset<ExecutableUnit> CreateExecutableUnit(FlatBufferBuilder builder,
      int Id = 0,
      StringOffset PathOffset = default(StringOffset)) {
    builder.StartObject(2);
    ExecutableUnit.AddPath(builder, PathOffset);
    ExecutableUnit.AddId(builder, Id);
    return ExecutableUnit.EndExecutableUnit(builder);
  }

  public static void StartExecutableUnit(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddId(FlatBufferBuilder builder, int Id) { builder.AddInt(0, Id, 0); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset PathOffset) { builder.AddOffset(1, PathOffset.Value, 0); }
  public static Offset<ExecutableUnit> EndExecutableUnit(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExecutableUnit>(o);
  }
};


}
