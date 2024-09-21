using System;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    CardNameType Name { get; } //Tên bài

    CardOriginType Origin { get; } //Tộc bài

    CardClassType Class { get; } //Hệ bài

    int RuneStoneCost { get; } //Giá tiền mua bài

    int EnergyPoint { get; } //Năng lượng

    int EnergyCurrent { get; } //Năng lượng hiện tại

    bool EnergyFull { get; }

    int Attack { get; } //Sát thương cơ bản

    int Grow { get; } //Sức mạnh tăng trưởng

    int AttackCombine { get; } //Tổng sát thương khi tấn công thường

    IPlayer Player { get; }

    Image Renderer { get; }

    bool Avaible { get; }


    void Init(CardData Data); //Khởi tạo bài

    void Fill(Transform Point); //Thêm bài vào khu vực chung


    void Ready();

    void Pointer(Transform Point);


    void FlipOpen(Action OnComplete);

    void FlipClose(Action OnComplete);


    void MoveTop(Action OnComplete);

    void MoveBack(Action OnComplete);


    void Rumble(Action OnComplete);


    void EffectAlpha(Action OnComplete);

    void EffectOutlineNormal(Action OnComplete);

    void EffectOutlineEnergy(Action OnComplete);


    void InfoShow(bool Show);

    void InfoGrowUpdate(int Value, bool Effect = false);

    void InfoManaUpdate(int Value, int Max, bool Effect = false);

    void InfoDamageUpdate(int Value, bool Effect = false);


    void DoCollectActive(IPlayer Player, Action OnComplete); //Kích hoạt khi lên sân

    void DoOriginActive(Action OnComplete); //Kích hoạt kĩ năng tộc khi lên sân

    void DoEnterActive(); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)


    void DoPassiveActive(); //Kích hoạt kĩ năng bị động


    void DostaffActive(Action OnComplete); //Kích hoạt khi đặt trượng phép

    void DoAttackActive(Action OnComplete); //Tấn công thường khi đặt trượng phép

    void DoEnergyFill(int Value, Action OnComplete); //Nhận năng lượng khi đặt trượng phép


    void DoEnergyActive(Action OnComplete); //Kích hoạt kĩ năng khi đủ năng lượng

    void DoClassActive(Action OnComplete); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void DoSpellActive(Action OnComplete); //Kích hoạt kĩ năng phép khi đủ năng lượng
}