// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct GameInitInfoS : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static GameInitInfoS GetRootAsGameInitInfoS(ByteBuffer _bb) { return GetRootAsGameInitInfoS(_bb, new GameInitInfoS()); }
  public static GameInitInfoS GetRootAsGameInitInfoS(ByteBuffer _bb, GameInitInfoS obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public GameInitInfoS __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Mode { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Map { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int GameModeValue { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public PlayerInitInfoS? Players(int j) { int o = __p.__offset(10); return o != 0 ? (PlayerInitInfoS?)(new PlayerInitInfoS()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int PlayersLength { get { int o = __p.__offset(10); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<GameInitInfoS> CreateGameInitInfoS(FlatBufferBuilder builder,
      int mode = 0,
      int map = 0,
      int gameModeValue = 0,
      VectorOffset playersOffset = default(VectorOffset)) {
    builder.StartTable(4);
    GameInitInfoS.AddPlayers(builder, playersOffset);
    GameInitInfoS.AddGameModeValue(builder, gameModeValue);
    GameInitInfoS.AddMap(builder, map);
    GameInitInfoS.AddMode(builder, mode);
    return GameInitInfoS.EndGameInitInfoS(builder);
  }

  public static void StartGameInitInfoS(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddMode(FlatBufferBuilder builder, int mode) { builder.AddInt(0, mode, 0); }
  public static void AddMap(FlatBufferBuilder builder, int map) { builder.AddInt(1, map, 0); }
  public static void AddGameModeValue(FlatBufferBuilder builder, int gameModeValue) { builder.AddInt(2, gameModeValue, 0); }
  public static void AddPlayers(FlatBufferBuilder builder, VectorOffset playersOffset) { builder.AddOffset(3, playersOffset.Value, 0); }
  public static VectorOffset CreatePlayersVector(FlatBufferBuilder builder, Offset<PlayerInitInfoS>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreatePlayersVectorBlock(FlatBufferBuilder builder, Offset<PlayerInitInfoS>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartPlayersVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<GameInitInfoS> EndGameInitInfoS(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<GameInitInfoS>(o);
  }
};

public struct PlayerInitInfoS : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static PlayerInitInfoS GetRootAsPlayerInitInfoS(ByteBuffer _bb) { return GetRootAsPlayerInitInfoS(_bb, new PlayerInitInfoS()); }
  public static PlayerInitInfoS GetRootAsPlayerInitInfoS(ByteBuffer _bb, PlayerInitInfoS obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public PlayerInitInfoS __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Name { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetNameBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetNameArray() { return __p.__vector_as_array<byte>(4); }
  public int Number { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Hero { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int Color { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int PlayerType { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int PhotonPlayerNumber { get { int o = __p.__offset(14); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool IsReady { get { int o = __p.__offset(16); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }

  public static Offset<PlayerInitInfoS> CreatePlayerInitInfoS(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      int number = 0,
      int hero = 0,
      int color = 0,
      int playerType = 0,
      int photonPlayerNumber = 0,
      bool isReady = false) {
    builder.StartTable(7);
    PlayerInitInfoS.AddPhotonPlayerNumber(builder, photonPlayerNumber);
    PlayerInitInfoS.AddPlayerType(builder, playerType);
    PlayerInitInfoS.AddColor(builder, color);
    PlayerInitInfoS.AddHero(builder, hero);
    PlayerInitInfoS.AddNumber(builder, number);
    PlayerInitInfoS.AddName(builder, nameOffset);
    PlayerInitInfoS.AddIsReady(builder, isReady);
    return PlayerInitInfoS.EndPlayerInitInfoS(builder);
  }

  public static void StartPlayerInitInfoS(FlatBufferBuilder builder) { builder.StartTable(7); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddNumber(FlatBufferBuilder builder, int number) { builder.AddInt(1, number, 0); }
  public static void AddHero(FlatBufferBuilder builder, int hero) { builder.AddInt(2, hero, 0); }
  public static void AddColor(FlatBufferBuilder builder, int color) { builder.AddInt(3, color, 0); }
  public static void AddPlayerType(FlatBufferBuilder builder, int playerType) { builder.AddInt(4, playerType, 0); }
  public static void AddPhotonPlayerNumber(FlatBufferBuilder builder, int photonPlayerNumber) { builder.AddInt(5, photonPlayerNumber, 0); }
  public static void AddIsReady(FlatBufferBuilder builder, bool isReady) { builder.AddBool(6, isReady, false); }
  public static Offset<PlayerInitInfoS> EndPlayerInitInfoS(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PlayerInitInfoS>(o);
  }
};

