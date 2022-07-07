using System;
using System.Reflection;

namespace HEIRS.HOLDING.INTERVIEW.TEST.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}