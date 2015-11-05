using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHitStats
{
    internal static class SearchTermProvider
    {
        public static string GetSearchTerms(Technology technology)
        {
            switch(technology)
            {
                case Technology.Abap:
                    return "Abab";

                case Technology.AngularJS:
                    return "Angular";

                case Technology.Assembler:
                    return "Assembler";

                case Technology.BackboneJS:
                    return "Backbone";

                case Technology.Bootstrap:
                    return "Bootstrap";

                case Technology.Cobol:
                    return "Cobol";

                case Technology.CPlusPlus:
                    return "C++";

                case Technology.CSharp:
                    return "C#";

                case Technology.Css:
                    return "CSS";

                case Technology.Fortran:
                    return "Fortran";

                case Technology.Html:
                    return "HTML";

                case Technology.Java:
                    return "Java";

                case Technology.JavaScript:
                    return "JavaScript";

                case Technology.Json:
                    return "JSON";

                case Technology.Matlab:
                    return "Matlab";

                case Technology.ObjectiveC:
                    return "Objective-C";

                case Technology.Pascal:
                    return "Pascal";

                case Technology.Perl:
                    return "Perl";

                case Technology.Php:
                    return "PHP";

                case Technology.PowerShell:
                    return "PowerShell";

                case Technology.Python:
                    return "Python";

                case Technology.Ruby:
                    return "Ruby";

                case Technology.Scala:
                    return "Scala";

                case Technology.SharePoint:
                    return "SharePoint";

                case Technology.Sitecore:
                    return "Sitecore";

                case Technology.Sql:
                    return "SQL";

                //case Technology.VisualBasic:
                //    return "Visual Basic";

                case Technology.Xml:
                    return "XML";

                default:
                    return null;
            }
        }
    }
}
