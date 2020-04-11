using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Web.Localization.Utilities
{
    public static class TitleStatusTranslator
    {
        public static string FromStatus(Title.Statuses status) => status switch
        {
            Title.Statuses.Ongoing => Labels.Ongoing,
            Title.Statuses.Hiatus => Labels.Hiatus,
            Title.Statuses.Completed => Labels.Completed,
            Title.Statuses.Dropped => Labels.Dropped,
            _ => string.Empty
        };
    }
}