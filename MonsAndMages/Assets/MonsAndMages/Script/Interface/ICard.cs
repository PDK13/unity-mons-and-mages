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

    void Init(CardData Data); //Khởi tạo bài

    //

    void DoCollectActive(IPlayer Player); //Kích hoạt khi lên sân

    void OriginActive(IPlayer Player); //Kích hoạt kĩ năng tộc khi lên sân

    void EnterActive(IPlayer Player); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)

    //

    void PassiveActive(IPlayer Player); //Kích hoạt kĩ năng bị động

    //

    void WandActive(IPlayer Player); //Kích hoạt khi đặt trượng phép

    void AttackActive(IPlayer Player); //Tấn công thường khi đặt trượng phép

    void EnergyFill(IPlayer Player, int Value); //Nhận năng lượng khi đặt trượng phép

    void EnergyCheck();

    //

    void EnergyActive(IPlayer Player); //Kích hoạt kĩ năng khi đủ năng lượng

    void ClassActive(IPlayer Player); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void SpellActive(IPlayer Player); //Kích hoạt kĩ năng phép khi đủ năng lượng
}