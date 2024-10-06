using System;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    IPlayer Player { get; } //Người chơi sở hữu

    CardNameType Name { get; } //Tên bài

    CardOriginType Origin { get; } //Tộc bài

    CardClassType Class { get; } //Hệ bài

    CardType Type { get; } //Loại bài

    int RuneStoneCost { get; } //Giá tiền mua bài

    int ManaPoint { get; } //Năng lượng

    int ManaCurrent { get; } //Năng lượng hiện tại

    bool ManaFull { get; }

    int Attack { get; } //Sát thương cơ bản

    int Growth { get; } //Sức mạnh tăng trưởng

    int AttackCombine { get; } //Tổng sát thương khi tấn công thường


    RectTransform Pointer { get; set; }

    RectTransform Centre { get; set; }

    RectTransform Body { get; }

    Image Renderer { get; }

    int Index { get; }


    void Init(CardData Data); //Khởi tạo bài

    void DoFill(RectTransform Point, RectTransform Centre); //Thêm bài vào khu vực chung

    void DoFixed();


    void DoChoiceReady();

    void DoChoiceUnReady();

    void DoChoiceOnce(bool Stage);


    void DoFlipOpen(Action OnComplete);

    void DoFlipClose(Action OnComplete);


    void DoMoveTop(Action OnComplete);

    void DoMoveBack(Action OnComplete);

    void DoMoveCentreLinear(RectTransform Centre, Action OnComplete);

    void DoMoveCentreJump(RectTransform Centre, Action OnComplete);


    void DoRumble(Action OnComplete);


    void DoEffectAlpha(Action OnComplete);

    void DoEffectOutlineNormal(Action OnComplete);

    void DoEffectOutlineMana(Action OnComplete);

    void DoEffectOutlineChoice(Action OnComplete);


    void DoEffectOrigin(Action OnComplete);

    void DoEffectClass(Action OnComplete);


    void InfoShow(bool Show);


    void DoCollectActive(IPlayer Player, Action OnComplete); //Kích hoạt khi lên sân

    void DoCollectProgess(); //Tiếp tục tiến trình khi lên sân


    void DoOriginActive(Action OnComplete); //Kích hoạt kĩ năng tộc khi lên sân


    void DoEnterActive(Action OnComplete); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)

    void DoPassiveActive(Action OnComplete); //Kích hoạt kĩ năng bị động


    void DostaffActive(Action OnComplete); //Kích hoạt khi đặt trượng phép

    void DoAttackActive(Action OnComplete); //Tấn công thường khi đặt trượng phép

    void DoManaFill(int Value, Action OnComplete); //Nhận năng lượng khi đặt trượng phép


    void DoManaActive(Action OnComplete); //Kích hoạt kĩ năng khi đủ năng lượng

    void DoManaProgess(); //Tiếp tục tiến trình khi đủ năng lượng


    void DoClassActive(Action OnComplete); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void DoSpellActive(Action OnComplete); //Kích hoạt kĩ năng phép khi đủ năng lượng
}