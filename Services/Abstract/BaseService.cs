using WeebReader.Data.Contexts.Abstract;

namespace Services.Abstract
{
    public class BaseService
    {
        protected readonly BaseContext Context;

        protected BaseService(BaseContext context) => Context = context;
    }
}