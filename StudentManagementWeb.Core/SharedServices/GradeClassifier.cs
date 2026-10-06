namespace StudentManagementWeb.Core.SharedServices;

public static class GradeClassifier
{
    /// <summary>Xếp loại học lực theo điểm trung bình.</summary>
    public static string Classify(decimal average) => average switch
    {
        >= 8m => "Giỏi",
        >= 6.5m => "Khá",
        >= 5m => "Trung bình",
        _ => "Yếu"
    };
}