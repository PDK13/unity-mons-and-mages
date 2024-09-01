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

    void DoCardAddActive(IPlayer Player); //Kích hoạt khi lên sân

    void AbilityOriginActive(IPlayer Player); //Kích hoạt kĩ năng tộc khi lên sân

    void AbilityEnterActive(IPlayer Player); //Kích hoạt kĩ năng khi lên sân (Biểu tượng sấm sét)

    void AbilityPassiveActive(IPlayer Player); //Kích hoạt kĩ năng bị động

    //

    void DoWandActive(IPlayer Player); //Kích hoạt khi đặt trượng phép

    void DoAttackActive(IPlayer Player); //Tấn công thường khi đặt trượng phép

    void DoEnergyFill(IPlayer Player, int Value); //Nhận năng lượng khi đặt trượng phép

    //

    void DoEnergyActive(IPlayer Player); //Kích hoạt kĩ năng khi đủ năng lượng

    void AbilityClassActive(IPlayer Player); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    void AbiltySpellActive(IPlayer Player); //Kích hoạt kĩ năng phép khi đủ năng lượng
}