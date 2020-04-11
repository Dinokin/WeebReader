using System;
using System.Collections.Generic;
using WeebReader.Web.Models.Controllers.ChaptersManager;
using WeebReader.Web.Models.Controllers.TitlesManager;

namespace WeebReader.Web.Portal.Others
{
    public class ReleaseComparer : IEqualityComparer<Tuple<TitleModel, ChapterModel>>
    {
        public bool Equals(Tuple<TitleModel, ChapterModel> x, Tuple<TitleModel, ChapterModel> y) => x?.Item2?.Number == y?.Item2.Number && x?.Item2?.TitleId == y?.Item2?.TitleId;

        public int GetHashCode(Tuple<TitleModel, ChapterModel> obj) => base.GetHashCode();
    }
}