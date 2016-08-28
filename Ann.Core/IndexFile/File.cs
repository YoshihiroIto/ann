// automatically generated, do not modify

namespace IndexFile
{

using System;
using FlatBuffers;

public sealed class File : Table {
  public static File GetRootAsFile(ByteBuffer _bb) { return GetRootAsFile(_bb, new File()); }
  public static File GetRootAsFile(ByteBuffer _bb, File obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public File __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Versions Version { get { int o = __offset(4); return o != 0 ? (Versions)bb.GetInt(o + bb_pos) : Versions.Version0; } }
  public ExecutableFile GetRows(int j) { return GetRows(new ExecutableFile(), j); }
  public ExecutableFile GetRows(ExecutableFile obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int RowsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<File> CreateFile(FlatBufferBuilder builder,
      Versions Version = Versions.Version0,
      VectorOffset RowsOffset = default(VectorOffset)) {
    builder.StartObject(2);
    File.AddRows(builder, RowsOffset);
    File.AddVersion(builder, Version);
    return File.EndFile(builder);
  }

  public static void StartFile(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddVersion(FlatBufferBuilder builder, Versions Version) { builder.AddInt(0, (int)Version, 0); }
  public static void AddRows(FlatBufferBuilder builder, VectorOffset RowsOffset) { builder.AddOffset(1, RowsOffset.Value, 0); }
  public static VectorOffset CreateRowsVector(FlatBufferBuilder builder, Offset<ExecutableFile>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRowsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<File> EndFile(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<File>(o);
  }
  public static void FinishFileBuffer(FlatBufferBuilder builder, Offset<File> offset) { builder.Finish(offset.Value); }
};


}
