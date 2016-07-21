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
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string LowerName { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLowerNameBytes() { return __vector_as_arraysegment(8); }
  public string LowerDirectory { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLowerDirectoryBytes() { return __vector_as_arraysegment(10); }
  public string LowerFileName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLowerFileNameBytes() { return __vector_as_arraysegment(12); }
  public string SearchKey { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSearchKeyBytes() { return __vector_as_arraysegment(14); }

  public static Offset<ExecutableUnit> CreateExecutableUnit(FlatBufferBuilder builder,
      StringOffset PathOffset = default(StringOffset),
      StringOffset NameOffset = default(StringOffset),
      StringOffset LowerNameOffset = default(StringOffset),
      StringOffset LowerDirectoryOffset = default(StringOffset),
      StringOffset LowerFileNameOffset = default(StringOffset),
      StringOffset SearchKeyOffset = default(StringOffset)) {
    builder.StartObject(6);
    ExecutableUnit.AddSearchKey(builder, SearchKeyOffset);
    ExecutableUnit.AddLowerFileName(builder, LowerFileNameOffset);
    ExecutableUnit.AddLowerDirectory(builder, LowerDirectoryOffset);
    ExecutableUnit.AddLowerName(builder, LowerNameOffset);
    ExecutableUnit.AddName(builder, NameOffset);
    ExecutableUnit.AddPath(builder, PathOffset);
    return ExecutableUnit.EndExecutableUnit(builder);
  }

  public static void StartExecutableUnit(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset PathOffset) { builder.AddOffset(0, PathOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddLowerName(FlatBufferBuilder builder, StringOffset LowerNameOffset) { builder.AddOffset(2, LowerNameOffset.Value, 0); }
  public static void AddLowerDirectory(FlatBufferBuilder builder, StringOffset LowerDirectoryOffset) { builder.AddOffset(3, LowerDirectoryOffset.Value, 0); }
  public static void AddLowerFileName(FlatBufferBuilder builder, StringOffset LowerFileNameOffset) { builder.AddOffset(4, LowerFileNameOffset.Value, 0); }
  public static void AddSearchKey(FlatBufferBuilder builder, StringOffset SearchKeyOffset) { builder.AddOffset(5, SearchKeyOffset.Value, 0); }
  public static Offset<ExecutableUnit> EndExecutableUnit(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExecutableUnit>(o);
  }
};


}
