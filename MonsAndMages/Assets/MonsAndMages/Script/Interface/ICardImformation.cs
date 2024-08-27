public interface ICardImformation
{
    CardNameType CardName { get; } //Tên bài

    CardOriginType CardOrigin { get; } //Tộc bài

    CardClassType CardClass { get; } //Hệ bài

    int CardCost { get; } //Giá tiền mua bài

    int CardEnergy { get; } //Năng lượng

    int CardEnergyCurrent { get; } //Năng lượng hiện tại

    int CardAttack { get; } //Sát thương cơ bản

    int CardAttackCombine { get; } //Tổng sát thương khi tấn công thường

    void CardInit(CardData Data); //Khởi tạo bài
}