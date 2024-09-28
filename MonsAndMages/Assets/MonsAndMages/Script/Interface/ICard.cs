using System;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    CardNameType Name { get; } //Tên bài

    CardOriginType Origin { get; } //Tộc bài

    CardClassType Class { get; } //Hệ bài

    int RuneStoneCost { get; } //Giá tiền mua bài

    int ManaPoint { get; } //Năng lượng

    int Mana { get; } //Năng lượng hiện tại

    bool ManaFull { get; }

    int Attack { get; } //Sát thương cơ bản

    int Growth { get; } //Sức mạnh tăng trưởng

    int AttackCombine { get; } //Tổng sát thương khi tấn công thường

    IPlayer Player { get; }

    Image Renderer { get; }

    Vector2 CentreInPointer { get; }

    bool Avaible { get; }


    void Init(CardData Data); //Khởi tạo bài

    void Fill(RectTransform Point, RectTransform Centre); //Thêm bài vào khu vực chung


    void Ready();

    public void Pointer(RectTransform Point, RectTransform Centre, bool PointChange, bool CentreChange);


    void FlipOpen(Action OnComplete);

    void FlipClose(Action OnComplete);


    void MoveTop(Action OnComplete);

    void MoveBack(Action OnComplete);

    void MoveHorizontal(Action OnComplete);


    void Rumble(Action OnComplete);


    void EffectAlpha(Action OnComplete);

    void EffectOutlineNormal(Action OnComplete);

    void EffectOutlineMana(Action OnComplete);

    void EffectOrigin(Action OnComplete);

    void EffectClass(Action OnComplete);


    void InfoShow(bool Show);


    void DoCollectActive(IPlayer Player, Action OnComplete); //Kích hoạt khi lên sân


    void DoOriginActive(Action OnComplete); //Kích hoạt kĩ năng tộc khi lên sân

    void DoOriginDragonActive(int DragonLeft, Action OnComplete);

    void DoOriginGhostActive(int GhostCount);

    void DoOriginGhostReady();

    void DoOriginGhostStart();

    void DoOriginGhostUnReady();

    void DoOriginInsectActive(Action OnComplete);

    void DoOriginSirenActive(Action OnComplete);

    void DoOriginWoodlandActive(int WoodlandCount, Action OnComplete);


    void DoEnterActive(Action OnComplete); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)

    void DoPassiveActive(Action OnComplete); //Kích hoạt kĩ năng bị động


    void DostaffActive(Action OnComplete); //Kích hoạt khi đặt trượng phép

    void DoAttackActive(Action OnComplete); //Tấn công thường khi đặt trượng phép

    void DoManaFill(int Value, Action OnComplete); //Nhận năng lượng khi đặt trượng phép


    void DoManaActive(Action OnComplete); //Kích hoạt kĩ năng khi đủ năng lượng


    void DoClassActive(Action OnComplete); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void DoClassFighterActive(int AttackCombineLeft, int DiceDotSumRolled, Action OnComplete);

    void DoClassMagicAddictActive(Action OnComplete);

    void DoClassMagicAddictReady();

    void DoClassMagicAddictStart();

    void DoClassMagicAddictUnReady();

    void DoClassSingerActive(int SingerCount, Action OnComplete);

    void DoClassCareTakerActive(Action OnComplete);

    void DoClassDiffuserActive(Action OnComplete);

    void DoClassFlyingActive(Action OnComplete);

    void DoClassFlyingReady();

    void DoClassFlyingStart();

    void DoClassFlyingUnReady();


    void DoSpellActive(Action OnComplete); //Kích hoạt kĩ năng phép khi đủ năng lượng
}