namespace ProjectNest.Domain.Surveys;

public class SurveyPersonnel
{
    public string Name { get; }
    public string Phone { get; }

    public SurveyPersonnel(string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Survey personnel name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Survey personnel phone is required.", nameof(phone));
        }

        Name = name.Trim();
        Phone = phone.Trim();
    }
}
