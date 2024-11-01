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

    int ClassFighterDiceDot { get; set; }

    int ClassFlyingMoveDir { get; set; }

    bool ClassFlyingManaFull { get; set; }

    void Init(CardData Data); //Khởi tạo bài

    void DoFill(RectTransform Point, RectTransform Centre); //Thêm bài vào khu vực chung

    void DoFixed();

    void DoChoiceReady(); //Lựa chọn sẵn sàng

    void DoChoiceUnReady();

    void DoChoiceOnce(bool Stage);

    void DoTutorialReady();

    void DoTutorialUnReady();

    void DoFlipOpen(Action OnComplete); //Lật thẻ bài

    void DoFlipClose(Action OnComplete);

    void DoMoveTop(Action OnComplete); //Di chuyển thẻ bài

    void DoMoveBack(Action OnComplete);

    void DoMoveCentreLinear(RectTransform Centre, Action OnComplete);

    void DoMoveCentreJump(RectTransform Centre, Action OnComplete);

    void DoRumble(Action OnComplete); //Giậm đất

    void DoEffectAlpha(Action OnComplete); //Hiệu ứng

    void DoEffectOutlineNormal(Action OnComplete);

    void DoEffectOutlineMana(Action OnComplete);

    void DoEffectOutlineChoice(Action OnComplete);

    void DoEffectOutlineProgess(Action OnComplete);

    void DoEffectOrigin(Action OnComplete);

    void DoEffectClass(Action OnComplete);

    void DoTextAttack();

    void InfoShow(bool Show); //Thông tin hiển thị

    void DoCollectActive(IPlayer Player, Action OnComplete); //Kích hoạt khi lên sân

    bool DoCollectProgess(); //Tiếp tục tiến trình khi lên sân

    void DoOriginActive(Action OnComplete); //Kích hoạt kĩ năng tộc khi lên sân

    void DoOriginDragon(Action OnComplete); //Origin Dragon Event

    void DoOriginWoodlandReady(Action OnComplete); //Origin Woodland Event

    void DoOriginWoodlandStart();

    void DoOriginGhostReady(Action OnComplete); //Origin Ghost Event

    void DoOriginGhostStart();

    void DoOriginInsect(Action OnComplete); //Origin Insect Event

    void DoOriginSiren(Action OnComplete); //Origin Siren Event

    void DoOriginNeutral(Action OnComplete); //Origin Neutral Event

    void DoEnterActive(Action OnComplete); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)

    void DoEnterStart();

    void DoStaffActive(Action OnComplete); //Kích hoạt khi đặt trượng phép

    void DoStaffActiveSerenity(Action OnComplete); //Kích hoạt khi đặt trượng phép trên Landmask

    void DoAttackActive(Action OnComplete); //Tấn công thường khi đặt trượng phép

    void DoManaChange(int Value, Action OnComplete); //Nhận năng lượng khi đặt trượng phép

    void DoManaActive(Action OnComplete); //Kích hoạt kĩ năng khi đủ năng lượng

    bool DoManaProgess(); //Tiếp tục tiến trình khi đủ năng lượng

    void DoClassActive(Action OnComplete); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void DoClassFighter(Action OnComplete); //Class Fighter Event

    void DoClassMagicAddictReady(Action OnComplete); //Class Magic Addict Event

    void DoClassMagicAddictStart();

    void DoClassSinger(Action OnComplete); //Class Singer Event

    void DoClassCareTaker(Action OnComplete); //Class Care Taker Event

    void DoClassDiffuser(Action OnComplete); //Class Diffuser Event

    void DoClassFlyingReady(Action OnComplete); //Class Flying Event

    void DoClassFlyingStart();

    void DoSpellActive(Action OnComplete); //Kích hoạt kĩ năng phép khi đủ năng lượng

    void DoSpellStart();
}