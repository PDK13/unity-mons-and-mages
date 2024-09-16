﻿using System;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    CardNameType Name { get; } //Tên bài

    CardOriginType Origin { get; } //Tộc bài

    CardClassType Class { get; } //Hệ bài

    int RuneStoneCost { get; } //Giá tiền mua bài

    int Energy { get; } //Năng lượng

    int EnergyCurrent { get; } //Năng lượng hiện tại

    bool EnergyFull { get; }

    int Attack { get; } //Sát thương cơ bản

    int Grow { get; } //Sức mạnh tăng trưởng

    int AttackCombine { get; } //Tổng sát thương khi tấn công thường

    IPlayer Player { get; }

    Image Renderer { get; }

    bool Avaible { get; }


    void Init(CardData Data); //Khởi tạo bài


    void Ready();

    void Point(Transform Point);


    void Open(float Duration, Action OnComplete);

    void Close(float Duration, Action OnComplete);


    void MoveTop(float Duration, Action OnComplete);

    void MoveBack(float Duration, Action OnComplete);


    void Rumble(Action OnComplete);


    void Effect(CardEffectType Type, float Duration, Action OnComplete);

    void EffectAlpha(float Duration, Action OnComplete);


    void InfoShow(bool Show);

    void InfoGrowUpdate(int Value, bool Effect = false);

    void InfoManaUpdate(int Value, int Max, bool Effect = false);

    void InfoDamageUpdate(int Value, bool Effect = false);


    void DoCollectActive(IPlayer Player); //Kích hoạt khi lên sân

    void DoOriginActive(Action OnComplete); //Kích hoạt kĩ năng tộc khi lên sân

    void DoEnterActive(); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)


    void DoPassiveActive(); //Kích hoạt kĩ năng bị động


    void DoWandActive(); //Kích hoạt khi đặt trượng phép

    void DoAttackActive(); //Tấn công thường khi đặt trượng phép

    void DoEnergyFill(int Value); //Nhận năng lượng khi đặt trượng phép

    void DoEnergyCheck();


    void DoEnergyActive(); //Kích hoạt kĩ năng khi đủ năng lượng

    void DoClassActive(); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void DoSpellActive(); //Kích hoạt kĩ năng phép khi đủ năng lượng
}