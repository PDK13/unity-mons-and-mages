public interface ICardStep
{
    bool QuickAbilityActive(); //Kích hoạt khi đặt bài lên sân

    //

    bool WandActive(); //Kích hoạt khi trượng phép đến nơi

    bool OriginActive(); //Trượng phép kích hoạt kĩ năng tộc

    bool AttackActive(); //Trượng phép kích hoạt tấn công thường

    bool EnergyFill(int Value = 1); //Trượng phép mặc định cung cấp 1 năng lượng

    bool ClassActive(); //Kích hoạt kĩ năng hệ khi đủ năng lượng

    bool SkillActive(); //Kích hoạt kĩ năng độc nhất sau khi kích hoạt kĩ năng hệ
}